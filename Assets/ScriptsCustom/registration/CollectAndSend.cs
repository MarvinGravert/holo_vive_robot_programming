using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;

public class CollectAndSend : MonoBehaviour
{
    public string FinishedCollectingPointPairs;
    public string caliTrackerEventName;
    public string controllerEventName; //to use button input

    private List<CustomPose> trackerPoses=new List<CustomPose>();
    private List<CustomPose> calibObjectPoses = new List<CustomPose>();

    private CustomPose caliTracker= new CustomPose(new Vector3(0,0,0),new Quaternion(0,0,0,1));

    public GameObject calibObjectKOS;
    public TextMeshPro pointPairCounterText;
    private int pointPairCounter = 0;


    private bool lastStateMenuButton = false;//use for collection
    private bool lastStateGripButton = false;//use to send to server
   
    public void Register()
    {
        if (this.gameObject.activeSelf)
        {
            // take the lists and send them off to be processed
            EventParam poses = new EventParam();
            poses.trackerPoses = trackerPoses;
            poses.calibObjectPoses = calibObjectPoses;
            //clear past poses afterwards=>new registering
            trackerPoses = new List<CustomPose>();
            calibObjectPoses = new List<CustomPose>();
            pointPairCounter = 0;
            pointPairCounterText.text = "Pairs:" + pointPairCounter.ToString();
            // now send so it doesnt interferee
            EventManager.TriggerEvent(FinishedCollectingPointPairs, poses);
           
            
        }

    }
    public void CollectPosePairs()
    {
        //take the calib and the tracker
        if (this.gameObject.activeSelf)
        {
            Vector3 calibObjectPositionInWorld = calibObjectKOS.transform.position;
            Quaternion calibObjectRotation2World = calibObjectKOS.transform.rotation;
            CustomPose test = new CustomPose(calibObjectPositionInWorld, calibObjectRotation2World);
            calibObjectPoses.Add(test);
            // create new object as adding to list is just adding a reference and as we are changing the tracker oncstantly...
            trackerPoses.Add(caliTracker.CopyPose());
            pointPairCounter++;
            pointPairCounterText.text = "Pairs:" + pointPairCounter.ToString();
        }
    }
    void ChangeTrackerPose(EventParam newCaliTrackerPose)
    {
        caliTracker.position = newCaliTrackerPose.position;
        caliTracker.rotation = newCaliTrackerPose.rotation;
    }

    void CheckControllerButton(EventParam newController)
    {
        bool menuButton = Convert.ToBoolean(newController.buttonState["menuButton"]);
        bool gripButton = Convert.ToBoolean(newController.buttonState["gripButton"]);

        if (lastStateMenuButton==false && menuButton == true)
        {
            CollectPosePairs();
        }
        if (lastStateGripButton==false && gripButton == true){
            Register();
        }
        lastStateGripButton = gripButton;
        lastStateMenuButton = menuButton;
    }
    void OnEnable()
    {

        EventManager.StartListening(caliTrackerEventName, ChangeTrackerPose);
        EventManager.StartListening(controllerEventName, CheckControllerButton);

    }
    void OnDisable()
    {
        EventManager.StopListening(caliTrackerEventName, ChangeTrackerPose);
        EventManager.StopListening(controllerEventName, CheckControllerButton);
    }
}
public class CustomPose
{
    public Vector3 position;
    public Quaternion rotation;
    public CustomPose(Vector3 newPosition, Quaternion newRotation)
    {
        position = newPosition;
        rotation = newRotation;
    }
    public CustomPose CopyPose()
    {
        return new CustomPose(position, rotation);
    }
}
       