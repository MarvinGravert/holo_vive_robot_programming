using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class NotifyPoseEvent : MonoBehaviour
{
    public string objectPoseInformationEventName;
    public GameObject referenceObject;
    public GameObject calibObject;
    private EventParam pose;
    public void notifyPose()
    {
        Vector3 calibObjectPositionInWorld= calibObject.transform.position;
        Quaternion calibObjectRotation2World = calibObject.transform.rotation;
        //
        Vector3 refPositionInWorld = referenceObject.transform.position; // position of reference in world
        Quaternion reference2World = referenceObject.transform.rotation; //reference to world
        // we need to calculate the the position in regards to the referenceObject
        Quaternion world2Reference = Quaternion.Inverse(reference2World);
        Vector3 worldPosInRef = -(world2Reference *  refPositionInWorld);
        Quaternion calibObject2Reference = calibObjectRotation2World * world2Reference;
        Vector3 calibObjectPositionInReference = worldPosInRef+ world2Reference* refPositionInWorld;
        // Build the event reponse
        pose = new EventParam();
        pose.position = calibObjectPositionInReference;
        pose.rotation = calibObject2Reference;
        pose.position = calibObjectPositionInWorld;
        pose.rotation = calibObjectRotation2World;
        EventManager.TriggerEvent(objectPoseInformationEventName, pose);
    }
}
