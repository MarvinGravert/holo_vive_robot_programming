using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PlaceWaypoints : MonoBehaviour
{
    public GameObject controllerWaypointMarker;
    public GameObject referenceWaypoint;
    public GameObject referenceLineRenderer;

    public List<Waypoint> waypointObjectList = new List<Waypoint>();
    public List<GameObject> lineRendererList = new List<GameObject>();


    public string mainControllerEvent;
    public string pathWaypointsEvent;

    private bool linearTypeActive = true;



    private bool triggerLastState = false;
    private bool trackpadPressedLastState = false;
    private bool menuButtonPressedLastState = false;
    private bool gripButtonLastState = false;

    void readMainControllerState(EventParam mainController)
    {
        //float x_trackpad = mainController.buttonState["x_trackpad"];
        //float y_trackpad = mainController.buttonState["y_trackpad"];
        bool triggerButton = Convert.ToBoolean(mainController.buttonState["triggerButton"]);
        bool trackpadPressed = Convert.ToBoolean(mainController.buttonState["trackpadPressed"]);
        bool menuButton = Convert.ToBoolean(mainController.buttonState["menuButton"]);
        bool gripButton = Convert.ToBoolean(mainController.buttonState["gripButton"]);

        /* Check if the state of any button changed from false->true which triggers different behaviors
         * trigger: placeway point
         * menuButton: delete last waypoint
         * trackpadPressed: change waypoitn type
         * gripButton: send current Waypoints path to server
         */
        if (triggerButton == true && triggerLastState == false)
        {
            PlaceWaypoint(mainController.position,mainController.rotation);

        }
        triggerLastState = triggerButton;
        if (trackpadPressed == true && trackpadPressedLastState == false)
        {
            ChangeInterpolation();

        }
        trackpadPressedLastState = trackpadPressed;
        if (menuButton==true && menuButtonPressedLastState == false)
        {
            DeleteWaypoint();
        }
        menuButtonPressedLastState = menuButton;
        if (gripButton==true && gripButtonLastState == false)
        {
            if (waypointObjectList.Count > 0)
            {
                var path = new EventParam();
                path.waypoints = waypointObjectList;
                EventManager.TriggerEvent(pathWaypointsEvent, path);
            }
           
        }
    }

    public void PlaceWaypoint(Vector3 realPosition, Quaternion realRotation)
    {
        //triggerd when button pressed on controller
        /* Create referenceObject at location 
         * 
         */

        var waypointModel = Instantiate(referenceWaypoint);
        waypointModel.transform.position = controllerWaypointMarker.transform.position;
        waypointModel.transform.rotation = controllerWaypointMarker.transform.rotation;
        waypointModel.SetActive(true);
        Waypoint newWaypoint = new Waypoint(waypointModel, InterpolationType.Linear,realPosition,realRotation);
        if (!linearTypeActive)
        {
            newWaypoint.type = InterpolationType.PTP;
        }
        
        
        waypointObjectList.Add(newWaypoint);
        if (waypointObjectList.Count > 1)
        {
            Debug.Log("added line");
            var newRenderer = Instantiate(referenceLineRenderer);
            newRenderer.SetActive(true);
            ControlLineRenderer rendLineController = newRenderer.GetComponent<ControlLineRenderer>();
            var lastElement = waypointObjectList[waypointObjectList.Count - 1];
            var secondLastElement= waypointObjectList[waypointObjectList.Count - 2];
            if (newWaypoint.type == InterpolationType.Linear)
            {
                rendLineController.lineColor = Color.green;
            }
            if (newWaypoint.type == InterpolationType.PTP)
            {
                rendLineController.lineColor = Color.red;
            }
            rendLineController.SetupLineRenderer(new List<Waypoint> { secondLastElement, lastElement });
            lineRendererList.Add(newRenderer);
            //SetupLine();
        }
        
       
        
    }
   
    void DeleteWaypoint()
    {
        // remove renderer and waypoint from their lists and then destroy them
        if (lineRendererList.Count > 0)
        {
            var lastRenderer = lineRendererList[lineRendererList.Count - 1];
            lineRendererList.Remove(lastRenderer);
            Destroy(lastRenderer);
        }
        // line has to be removed first as 
        if (waypointObjectList.Count > 0)
        {
            var lastWaypoint = waypointObjectList[waypointObjectList.Count - 1];
            waypointObjectList.Remove(lastWaypoint);
            Destroy(lastWaypoint.obj);
        }
       

    }
    void ChangeInterpolation()
    {
        linearTypeActive = !linearTypeActive;
    }

    public void SendPathToServer()
    {
        if (waypointObjectList.Count > 0)
        {
            var path = new EventParam();
            path.waypoints = waypointObjectList;
            EventManager.TriggerEvent(pathWaypointsEvent, path);
        }
    }


    void OnEnable()
    {

        EventManager.StartListening(mainControllerEvent, readMainControllerState);
        foreach (var waypoint in waypointObjectList)
        {
            waypoint.obj.SetActive(true);
        }
        foreach (var line in lineRendererList)
        {
            line.SetActive(true);
        }
    }
    void OnDisable()
    {
        EventManager.StopListening(mainControllerEvent, readMainControllerState);
        foreach (var waypoint in waypointObjectList)
        {
            waypoint.obj.SetActive(false);
        }
        foreach(var line in lineRendererList)
        {
            line.SetActive(false);
        }
    }
}
public class Waypoint
{
    public GameObject obj;
    public InterpolationType type;
    public Vector3 realPosition;// Position in tracker space (untransformed)
    public Quaternion realRotation;

    public Waypoint(GameObject gObj, InterpolationType mtype,Vector3 mPos, Quaternion mRot)
    {
        obj = gObj;
       
        type = mtype;
        realPosition = mPos;
        realRotation = mRot;
    }
}
public enum InterpolationType
{
    Linear,
    PTP
}