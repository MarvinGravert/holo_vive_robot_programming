using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectAndSend : MonoBehaviour
{
    public string startRegisteringEventName;
    public string caliTrackerEventName;

    private List<CustomPose> trackerPoses=new List<CustomPose>();
    private List<CustomPose> calibObjectPoses = new List<CustomPose>();

    private CustomPose caliTracker= new CustomPose(new Vector3(0,0,0),new Quaternion(0,0,0,1));

    public GameObject calibObjectKOS;
   
    public void Register()
    {
        if (this.gameObject.activeSelf)
        {
            // take the lists and send them off to be processed
            EventParam poses = new EventParam();
            poses.trackerPoses = trackerPoses;
            poses.calibObjectPoses = calibObjectPoses;
            EventManager.TriggerEvent(startRegisteringEventName, poses);
            //clear past poses afterwards=>new registering
            trackerPoses = new List<CustomPose>();
            calibObjectPoses = new List<CustomPose>();
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
        }
    }
    void ChangeTrackerPose(EventParam newCaliTrackerPose)
    {
        caliTracker.position = newCaliTrackerPose.position;
        caliTracker.rotation = newCaliTrackerPose.rotation;
    }
    void OnEnable()
    {

        EventManager.StartListening(caliTrackerEventName, ChangeTrackerPose);

    }
    void OnDisable()
    {
        EventManager.StopListening(caliTrackerEventName, ChangeTrackerPose);
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
       