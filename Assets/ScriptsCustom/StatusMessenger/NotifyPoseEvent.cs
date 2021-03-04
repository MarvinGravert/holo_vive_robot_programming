using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class NotifyPoseEvent : MonoBehaviour
{
    public string objectPoseInformationEventName;
    private EventParam pose;
    public void notifyPose()
    {
        Vector3 position=this.gameObject.transform.position;
        Quaternion rotation = this.gameObject.transform.rotation;

        pose = new EventParam();
        pose.position = position;
        pose.rotation = rotation;
        EventManager.TriggerEvent(objectPoseInformationEventName, pose);
    }
}
