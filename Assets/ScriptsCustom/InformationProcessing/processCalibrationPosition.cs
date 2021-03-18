using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class processCalibrationPosition : MonoBehaviour
{
    public string calibrationPositionEventName;
    public string sendCalibrationEventName;

    void ProcessPositionDataToString(EventParam pose)
    {
        char[] charsToTrim = { '(', ' ', ')' };//hack because im too lazy to find the proper method to parse into string without bracket
        string positionAsString= string.Join(",", pose.position.ToString("f6")).Trim(charsToTrim);//without trim this results in (x,y,z). 
        string rotationAsString = string.Join(",", pose.rotation.ToString("f6")).Trim(charsToTrim);
        string poseAsString = positionAsString + ":" + rotationAsString;
        EventParam poseString = new EventParam();
        poseString.tcpIPMessage = poseAsString;
        EventManager.TriggerEvent(sendCalibrationEventName, poseString);
    }
    void OnEnable()
    {

        EventManager.StartListening(calibrationPositionEventName, ProcessPositionDataToString);

    }
    void OnDisable()
    {

        EventManager.StopListening(calibrationPositionEventName, ProcessPositionDataToString);
    }

}
