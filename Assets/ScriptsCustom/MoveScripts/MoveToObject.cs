using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveToObject : MonoBehaviour

    
{
    public GameObject targetObject;
    public string controllerWaypointEventName;

    void movetoObject(EventParam eventParam)
    {
        this.transform.position = targetObject.transform.position;
        this.transform.rotation = targetObject.transform.rotation;
        Debug.Log("Moved to controller");
    }
    void OnEnable()
    {

        EventManager.StartListening(controllerWaypointEventName, movetoObject);

    }
    void OnDisable()
    {

        EventManager.StopListening(controllerWaypointEventName, movetoObject);
    }

}
