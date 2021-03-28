using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Globalization;

/*
 * this script listens to the event setup from tcpManager in regards to the controllerState and parses that information
 * into the correct data structure and then propagates it using events 
 * 
 * Important:
 * This script includes the logic for transforming from righthanded KOS to lefthanded
 * 
 * Problem:
 * we may need to not invert the transformation. This may cause significant problems 
 */
public class processReceivedControllerState : MonoBehaviour
{
    //private Action<EventParam> incomingMessageListener;

    public string newTcpControllerState;//event name for new data from tcp client
    public string statusEventName;
    public string buttonStateEventName;
    public string controllerPoseEventName;
    public string fullControllerStateEventName;

    private Dictionary<string, float> buttonState;
    private Vector3 position;
    private Quaternion rotation;

    private EventParam newPose;
    private EventParam newButtonState;
    private EventParam newStatus;

    private string oldStatus = "not init";//just init with random string to check against Status send by server
    //void Awake()
    //{
    //    incomingMessageListener = new Action<EventParam>(ParseControllerInformation);

    //}



    void OnEnable()
    {

        EventManager.StartListening(newTcpControllerState, ParseControllerInformation);

    }
    void OnDisable()
    {

        EventManager.StopListening(newTcpControllerState, ParseControllerInformation);
    }

    void ParseControllerInformation(EventParam eventParam)
    {
        //Debug.Log("New Controller State Received");
        //Debug.Log(eventParam.tcpIPMessage);
        /*
         * PARSE DATA
         * strucuture is as follows
         * x,y,z:w,i,j,k:x_trackpad,y_trackpad:trigger,trackpad_pressed, menuButton,grip_button:status
         * 
         */
        var parts = eventParam.tcpIPMessage.Split(':');
        var positionData = parts[0].Split(',');
        var rotationData = parts[1].Split(',');
        var x_trackpad = parts[2].Split(',')[0];
        var y_trackpad = parts[2].Split(',')[1];
        var listButtonChanged = parts[3].Split(',');
        var status = parts[4];

        //CultureInfo.InvariantCulture necessary because it was not parsing "." correctly
        float x = float.Parse(positionData[0], CultureInfo.InvariantCulture);
        float y = float.Parse(positionData[1], CultureInfo.InvariantCulture);
        float z = float.Parse(positionData[2], CultureInfo.InvariantCulture);
        float qw = float.Parse(rotationData[0], CultureInfo.InvariantCulture);
        float qx = float.Parse(rotationData[1], CultureInfo.InvariantCulture);
        float qy = float.Parse(rotationData[2], CultureInfo.InvariantCulture);
        float qz = float.Parse(rotationData[3], CultureInfo.InvariantCulture);



        position = new Vector3(x, y, z);//RealWorld object in holoWorld
        rotation = new Quaternion(qx, qy, qz, qw);
        // Pre Calibration this is the position of the controller in LH world, post claibration this is the position of the controller
        // either to the center of projection of the hololens aka the user or in reference to the world system
        // all of the logic is already implemented server side so we can act dumb on unity 


        /*
         * CONVERT TO LEFT HANDED KOS
         * unity uses a left handed KOS. In the following the position has to be changed to reflect that
         * but as of writint this we will do these transformation on server side to easier allow 
         * Prototypeing
         * 
         */

        //position = ConvertRightHandedToLeftHandedVector(position);
        //rotation = ConvertRightHandedToLeftHandedQuaternion(rotation);
        /*
         * BUILD BUTTON DICT
         */

        buttonState = new Dictionary<string, float>();
        buttonState["x_trackpad"] = float.Parse(x_trackpad, CultureInfo.InvariantCulture);
        buttonState["y_trackpad"] = float.Parse(y_trackpad, CultureInfo.InvariantCulture);
        buttonState["triggerButton"] = Convert.ToSingle(bool.Parse(listButtonChanged[0]));     
        buttonState["trackpadPressed"] = Convert.ToSingle(bool.Parse(listButtonChanged[1]));     
        buttonState["menuButton"] = Convert.ToSingle(bool.Parse(listButtonChanged[2]));
        buttonState["gripButton"] = Convert.ToSingle(bool.Parse(listButtonChanged[3]));
        /*
         * TRIGGER EVENTS
         */
        // Build eventParam
        newPose = new EventParam();
        newButtonState = new EventParam();
        newButtonState.buttonState = buttonState;
        newPose.position = position;
        newPose.rotation = rotation;
        //controller Pose
        EventManager.TriggerEvent(controllerPoseEventName, newPose);
        //buttonState        
        EventManager.TriggerEvent(buttonStateEventName, newButtonState);
        // Status
        if (status.Length != 0)//check if the string is actually set
        {
            if (oldStatus != status) //change if there has actually been a change in status
            {
                
                newStatus = new EventParam();
                newStatus.status = status;
                oldStatus = status;
                EventManager.TriggerEvent(statusEventName, newStatus);
            }
        }
        /*
         * FULL CONTROLLER STATE DEBUGGING PURPOSES ONLY
         */
        //just adding the buttonstate to the newPose
        newPose.buttonState = buttonState;
        EventManager.TriggerEvent(fullControllerStateEventName, newPose);
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
