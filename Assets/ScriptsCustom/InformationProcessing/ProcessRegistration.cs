using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Globalization;
public class ProcessRegistration : MonoBehaviour
{
    // events to receive calibration data to send to server
    public string startRegisteringEventName;
    public string sendCalibrationEventName;
    // events to process server response and publish it
    public string receivedRegistrationEventName;
    public string publishRegistrationEventName;

    void ProcessPositionDataToString(EventParam registrationEvent)
    {
        int numPointPairs=registrationEvent.trackerPoses.Count;
        string pointPairsAsStrings = "";
        for (int i = 0; i < numPointPairs; i++)
        {
            CustomPose trackerPose = registrationEvent.trackerPoses[i];
            CustomPose calibObjectPose = registrationEvent.calibObjectPoses[i];

            string trackerAsString = TurnPoseIntoString(trackerPose);
            string calibAsString = TurnPoseIntoString(calibObjectPose);

            pointPairsAsStrings += trackerAsString + "|" + calibAsString+"|";
        }

        pointPairsAsStrings = pointPairsAsStrings.Remove(pointPairsAsStrings.Length - 1);

        
        EventParam poseString = new EventParam();
        poseString.tcpIPMessage = pointPairsAsStrings;
        EventManager.TriggerEvent(sendCalibrationEventName, poseString);
    }
    string TurnPoseIntoString(CustomPose pose)
    {
       
        char[] charsToTrim = { '(', ' ', ')' };//hack because im too lazy to find the proper method to parse into string without bracket
        string positionAsString = string.Join(",", pose.position.ToString("f6")).Trim(charsToTrim);//without trim this results in (x,y,z). 
        string rotationAsString = string.Join(",", pose.rotation.ToString("f6")).Trim(charsToTrim);
        string poseAsString = positionAsString + ":" + rotationAsString;
        return poseAsString;
    }
    void ProcessTCPStringToRegistrationTransformation(EventParam newRegistration)
    {
        
        var parts = newRegistration.tcpIPMessage.Split(':');
        var positionData = parts[0].Split(',');
        var rotationData = parts[1].Split(',');
        //CultureInfo.InvariantCulture necessary because it was not parsing "." correctly on my german system so maybe you have to adapt this line
        float x = float.Parse(positionData[0], CultureInfo.InvariantCulture);
        float y = float.Parse(positionData[1], CultureInfo.InvariantCulture);
        float z = float.Parse(positionData[2], CultureInfo.InvariantCulture);
        float qx = float.Parse(rotationData[0], CultureInfo.InvariantCulture);
        float qy = float.Parse(rotationData[1], CultureInfo.InvariantCulture);
        float qz = float.Parse(rotationData[2], CultureInfo.InvariantCulture);
        float w = float.Parse(rotationData[3], CultureInfo.InvariantCulture);

        Vector3 position = new Vector3(x, y, z);//RealWorld object in holoWorld
        Quaternion rotation = new Quaternion(qx, qy, qz, w);
        EventParam registration = new EventParam();

        registration.position = position;
        registration.rotation = rotation;
        EventManager.TriggerEvent(publishRegistrationEventName, registration);
    }
    void OnEnable()
    {

        EventManager.StartListening(startRegisteringEventName, ProcessPositionDataToString);
        EventManager.StartListening(receivedRegistrationEventName, ProcessTCPStringToRegistrationTransformation);

    }
    void OnDisable()
    {

        EventManager.StopListening(startRegisteringEventName, ProcessPositionDataToString);
        EventManager.StopListening(receivedRegistrationEventName, ProcessTCPStringToRegistrationTransformation);
    }

}
