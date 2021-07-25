using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProcessPath : MonoBehaviour
{
    public string pathPointsEventName;
    public string pathPointsAsStringEvent;

    void ProcessPathIntoString(EventParam waypoints)
    {
        string pathAsString = "";
        foreach (var waypoint in waypoints.waypoints)
        {
            var pos = waypoint.realPosition;
            var rot = waypoint.realRotation;
            var type = waypoint.type;

            pathAsString += PositionAndRotationAsString(pos, rot);
            pathAsString += "|"+type.ToString()+"$";
        }
        pathAsString = pathAsString.Remove(pathAsString.Length - 1);
        Debug.Log(pathAsString);
        EventParam param = new EventParam();
        param.tcpIPMessage = pathAsString;
        EventManager.TriggerEvent(pathPointsAsStringEvent, param);
    }

    string PositionAndRotationAsString(Vector3 posi, Quaternion quat)
    {
        char[] charsToTrim = { '(', ' ', ')' };//hack because im too lazy to find the proper method to parse into string without bracket
        string positionAsString = string.Join(",", posi.ToString("f6")).Trim(charsToTrim);//without trim this results in (x,y,z). 
        string rotationAsString = string.Join(",", quat.ToString("f6")).Trim(charsToTrim);
        string poseAsString = positionAsString + ":" + rotationAsString;
        return poseAsString;
    }
    void OnEnable()
    {

        EventManager.StartListening(pathPointsEventName, ProcessPathIntoString);


    }
    void OnDisable()
    {

        EventManager.StopListening(pathPointsEventName, ProcessPathIntoString);

    }
}
