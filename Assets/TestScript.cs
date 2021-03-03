using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using System;

//https://stackoverflow.com/questions/42034245/unity-eventmanager-with-delegate-instead-of-unityevent/42034899#42034899
public class TestScript : MonoBehaviour
{

    private Action<EventParam> incomingMessageListener;


    void Awake()
    {
        incomingMessageListener = new Action<EventParam>(ParseControllerPosition);


        //StartCoroutine(invokeTest());
    }

    //IEnumerator invokeTest()
    //{
    //    WaitForSeconds waitTime = new WaitForSeconds(0.5f);

    //    //Create parameter to pass to the event
    //    EventParam eventParam = new EventParam();
    //    eventParam.param1 = "Hello";
    //    eventParam.param2 = 99;
    //    eventParam.param3 = 43.4f;
    //    eventParam.param4 = true;

    //    while (true)
    //    {
    //        yield return waitTime;
    //        EventManager.TriggerEvent("test", eventParam);
    //        yield return waitTime;
    //        EventManager.TriggerEvent("Spawn", eventParam);
    //        yield return waitTime;
    //        EventManager.TriggerEvent("Destroy", eventParam);
    //    }
    //}

    void OnEnable()
    {
        //Register With Action variable
        //EventManager.StartListening("test", someListener1);
        //EventManager.StartListening("Spawn", someListener2);
        //EventManager.StartListening("Destroy", someListener3);

        //OR Register Directly to function
        EventManager.StartListening("test", ParseControllerPosition);

    }
    void OnDisable()
    {
        //Un-Register With Action variable
        //EventManager.StopListening("test", someListener1);
        //EventManager.StopListening("Spawn", someListener2);
        //EventManager.StopListening("Destroy", someListener3);

        //OR Un-Register Directly to function
        EventManager.StopListening("test", ParseControllerPosition);
      
    }

    void ParseControllerPosition(EventParam eventParam)
    {
        Debug.Log("Some Function was called!");
        Debug.Log(eventParam.tcpIPMessage);
    }


}