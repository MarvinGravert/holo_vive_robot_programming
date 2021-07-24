using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Globalization;

public class processTCPMessage : MonoBehaviour
{
    //private Action<EventParam> incomingMessageListener;

    
    public string newTCPMessageEventName;

    private Dictionary<string, float> buttonState;
    private Vector3 position;
    private Quaternion rotation;


    //void Awake()
    //{
    //    incomingMessageListener = new Action<EventParam>(ParseControllerInformation);

    //}



    void OnEnable()
    {

        //EventManager.StartListening(newTcpControllerState, ParseControllerInformation);
        EventManager.StartListening(newTCPMessageEventName, ParseTCPMessage);
       

    }
    void OnDisable()
    {

        //EventManager.StopListening(newTcpControllerState, ParseControllerInformation);
        EventManager.StopListening(newTCPMessageEventName, ParseTCPMessage);
       
    }

    void ParseTCPMessage(EventParam eventParam)
    {
        //Debug.Log(eventParam.tcpIPMessage);
        /*
         * PARSE DATA
         * strucuture is as follows
         * Tracker1|Tracker2|..|TrackerN$Controller1|Controller2|...|ControllerN$Command
         * Tracker structed as x,y,z:i,j,k,w
         * Controller structured x,y,z:w,i,j,k:x_trackpad,y_trackpad:trigger,trackpad_pressed, menuButton,grip_button
         * Command is a string
         * 
         */
        var messageObjects = eventParam.tcpIPMessage.Split('$');
        var trackerData = messageObjects[0].Split('|');
        var controllerData = messageObjects[1].Split('|');
        var command = messageObjects[2]; // if no commadn is sent this is just "\n" 
        //Debug.Log(command);
        // publish tracker information depending on the name of the tracker. Tracker defined as pos+rot+name
        foreach (string tracker in trackerData)
        {
            var parts = tracker.Split(':');
            var name = parts[0];
            var positionData = parts[1].Split(',');
            var rotationData = parts[2].Split(',');
            //CultureInfo.InvariantCulture necessary because it was not parsing "." correctly on my german system so maybe you have to adapt this line
            float x = float.Parse(positionData[0], CultureInfo.InvariantCulture);
            float y = float.Parse(positionData[1], CultureInfo.InvariantCulture);
            float z = float.Parse(positionData[2], CultureInfo.InvariantCulture);
            float qx = float.Parse(rotationData[0], CultureInfo.InvariantCulture);
            float qy = float.Parse(rotationData[1], CultureInfo.InvariantCulture);
            float qz = float.Parse(rotationData[2], CultureInfo.InvariantCulture);
            float w = float.Parse(rotationData[3], CultureInfo.InvariantCulture);

            position = new Vector3(x, y, z);//RealWorld object in holoWorld
            rotation = new Quaternion(qx, qy, qz, w);
            EventParam newTracker = new EventParam();
            newTracker.name = name;
            newTracker.position = position;
            newTracker.rotation = rotation;
            EventManager.TriggerEvent(name, newTracker);
        }
        foreach (string controller in controllerData)
        {
            var parts = controller.Split(':');
            var name = parts[0];
            var positionData = parts[1].Split(',');
            var rotationData = parts[2].Split(',');
            var buttonStates = parts[3].Split(',');
            /*
             * Build Position Data
             */
            //CultureInfo.InvariantCulture necessary because it was not parsing "." correctly on my german system so maybe you have to adapt this line
            float x = float.Parse(positionData[0], CultureInfo.InvariantCulture);
            float y = float.Parse(positionData[1], CultureInfo.InvariantCulture);
            float z = float.Parse(positionData[2], CultureInfo.InvariantCulture);
            float qx = float.Parse(rotationData[0], CultureInfo.InvariantCulture);
            float qy = float.Parse(rotationData[1], CultureInfo.InvariantCulture);
            float qz = float.Parse(rotationData[2], CultureInfo.InvariantCulture);
            float w = float.Parse(rotationData[3], CultureInfo.InvariantCulture);

            position = new Vector3(x, y, z);//RealWorld object in holoWorld
            rotation = new Quaternion(qx, qy, qz, w);

            /*
             * Build a Dictionary (map) of button_names and their state 
             */
            var x_trackpad = buttonStates[0];
            var y_trackpad = buttonStates[1];
            var trackpad_pressed = buttonStates[2];
            var trigger = buttonStates[3];
            var menu_button = buttonStates[4];
            var grip_button = buttonStates[5];

            buttonState = new Dictionary<string, float>();
            buttonState["x_trackpad"] = float.Parse(x_trackpad, CultureInfo.InvariantCulture);
            buttonState["y_trackpad"] = float.Parse(y_trackpad, CultureInfo.InvariantCulture);
            buttonState["trackpadPressed"] = Convert.ToSingle(bool.Parse(trackpad_pressed));
            buttonState["triggerButton"] = Convert.ToSingle(bool.Parse(trigger));
            buttonState["menuButton"] = Convert.ToSingle(bool.Parse(menu_button));
            buttonState["gripButton"] = Convert.ToSingle(bool.Parse(grip_button));


            EventParam newcontroller = new EventParam();
            newcontroller.name = name;
            newcontroller.position = position;
            newcontroller.rotation = rotation;
            newcontroller.buttonState = buttonState;
            EventManager.TriggerEvent(name, newcontroller);
        }
        // publih event if it isnt just \n
        if (command.Length > 1)
        {
            EventParam newCommand = new EventParam();
            newCommand.command = command;
            EventManager.TriggerEvent("command", newCommand);
        }
        


    }

    //https://www.codeproject.com/Tips/1240454/How-to-Convert-Right-Handed-to-Left-Handed-Coordin
    private Vector3 ConvertRightHandedToLeftHandedVector(Vector3 rightHandedVector)
    {
        return new Vector3(rightHandedVector.x, rightHandedVector.z, rightHandedVector.y);
    }
    private Quaternion ConvertRightHandedToLeftHandedQuaternion(Quaternion rightHandedQuaternion)
    {
        return new Quaternion(-rightHandedQuaternion.x,
                               -rightHandedQuaternion.z,
                               -rightHandedQuaternion.y,
                                 rightHandedQuaternion.w);
    }
}
