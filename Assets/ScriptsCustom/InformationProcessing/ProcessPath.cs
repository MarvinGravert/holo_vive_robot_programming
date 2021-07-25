using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProcessPath : MonoBehaviour
{
    public string pathPointsEventName;
    public string pathPointsAsStringEvent;

    void ProcessPathIntoString(EventParam waypoints)
    {
        string s = "";
        foreach (var waypoint in waypoints.waypoints)
        {
            var pos = waypoint.obj.transform.position;
            var rot = waypoint.obj.transform.rotation;
            var type = waypoint.type;

            s += PositionAndRotationAsString(pos, rot);
            s += "|"+type.ToString()+"$";
        }
        Debug.Log(s);
        EventParam param = new EventParam();
        param.tcpIPMessage = s;
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
