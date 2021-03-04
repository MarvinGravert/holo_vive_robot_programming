using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class processCalibrationPosition : MonoBehaviour
{
    public string calibrationPositionEventName;

    void ProcessPositionDataToString(EventParam pose)
    {
        string positionAsString= string.Join(",", pose.position);
        Debug.Log(positionAsString);
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
