using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProcessRegistration : MonoBehaviour
{
    public string startRegisteringEventName;
    public string sendCalibrationEventName;

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
    void OnEnable()
    {

        EventManager.StartListening(startRegisteringEventName, ProcessPositionDataToString);

    }
    void OnDisable()
    {

        EventManager.StopListening(startRegisteringEventName, ProcessPositionDataToString);
    }

}
