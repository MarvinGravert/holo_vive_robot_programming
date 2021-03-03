using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System;

// I have adapted this script from the source found below:
// https://stackoverflow.com/questions/42034245/unity-eventmanager-with-delegate-instead-of-unityevent/42034899#42034899
/*
 * This script implements the EventManager who triggers the callback function of the listener when the associated event is triggerd 
 * This eventManager is implemented using the Singleton pattern thus only one can exist at the same time it is initialised on startup
 * 
 * Events can register and deregister itself using the "StartListening" "StopListening"- Methods 
 * Events are triggerd via TriggerEvent
 * 
 * Events are referenced via a stringname 
 * 
 * All events pass the EventParam structure which is defined below:
 * It includes the parameters which are set for the individual streams
 * Not all parameters have to be set and it should be clear for every event which parameters are expected to be set
 * This list can be extended fairly easily
 */

public struct EventParam
{
    //this strcuture
    public string tcpIPMessage;
    public string status;
    public Vector3 controllerPosition;
    public Quaternion controllerRotation;
    public Dictionary<string, float> buttonState; // boolean Values are encoded as 0.0 and 1.0
}
//Main EventManager who keeps a dictionary of all events and their registered listeners. Other functions cann call its public methods to register themselves inside
public class EventManager : MonoBehaviour
{
    // Action is a generic function type. The way i understand it, it represents all function that take EventParam as their argument and return Nothing.
    // the no return value is important
    // here we just declare a dictionary that has the event as a key and (a collection) methods as the value
    private Dictionary<string, Action<EventParam>> eventDictionary;

    // we are defining an eventManager (the class itself) but as a Singleton pattern
    // int he get method if it isnt defined we are looking for it in the system and only if int also defined there we instantiate another one
    private static EventManager eventManager;

    public static EventManager instance {
        get {
            if (!eventManager)
            {
                eventManager = FindObjectOfType(typeof(EventManager)) as EventManager;

                if (!eventManager)
                {
                    Debug.LogError("There needs to be one active EventManger script on a GameObject in your scene.");
                }
                else
                {
                    eventManager.Init();
                }
            }
            return eventManager;
        }
    }

    // init the eventManager object well basically just create the dictionary as declared above
    void Init()
    {
        if (eventDictionary == null)
        {
            eventDictionary = new Dictionary<string, Action<EventParam>>();
        }
    }
    // given an eventname and a function (as a callback) that takes EventParam, add it to the eventDictionary. If the event does not exist create a new entry in dict
    public static void StartListening(string eventName, Action<EventParam> listener)
    {
        Action<EventParam> thisEvent;
        // TryGetValue versucht für den Key (hier eventName) den Value aus dem Dict zu holen (und speichert den in thisEvent )
        // Dazu gibt diese Methode True zurück falls erfolgreich oder False falls der Key nicht exisitiert
        // hence the if clause.
        if (instance.eventDictionary.TryGetValue(eventName, out thisEvent))
        {
            //Add more event to the existing one
            // remember that thisEvent it actually a list of other event (name is confusing a bit)
            thisEvent += listener;

            //Update the Dictionary
            // now just write the prolonged list of listeners back into the dict
            //instance is refereing to this instance of eventManager but as there can only be one instance (singleton pattern) everythign is referring to the same manager
            instance.eventDictionary[eventName] = thisEvent;
        }
        else
        {
            //Add event to the Dictionary for the first time
            // If the key did not exist we add it to the Dictionary
            // The next line is maybe necessary as the listener is not ncessary of type Action<eventParam> but more likly of a derived type
            thisEvent += listener;
            instance.eventDictionary.Add(eventName, thisEvent);
        }
    }

    //unsubscribe from an event. Hand over the eventname and the listener which is supposed to be removed
    public static void StopListening(string eventName, Action<EventParam> listener)
    {
        //obviously if the eventManager doesnt exist nothing to remove from it
        if (eventManager == null) return;
        // pretty straight forward again. Try to get all callback functions that are keyed by the eventName
        Action<EventParam> thisEvent;
        if (instance.eventDictionary.TryGetValue(eventName, out thisEvent))
        {
            //Remove event from the existing one
            // remove the desired listener from the events
            // dunno what happends if the listener is not inside of it? Exception?
            thisEvent -= listener;

            //Update the Dictionary
            instance.eventDictionary[eventName] = thisEvent;
        }
    }
    //Also obviously necessary to have a function that can trigger event
    //so basically call this function based give the name of the event to trigger
    // and hand over the eventParam to it
    public static void TriggerEvent(string eventName, EventParam eventParam)
    {
        // find all the callback function in the diciontary for that event, save reference into thisEvent
        Action<EventParam> thisEvent = null;
        if (instance.eventDictionary.TryGetValue(eventName, out thisEvent))
        {
            //then invoke all function with the eventParam handed to trigger
            thisEvent.Invoke(eventParam);
            // the next line is easier to understand if coming from non-unity oh well
            // OR USE  instance.eventDictionary[eventName](eventParam);
        }
    }
}

