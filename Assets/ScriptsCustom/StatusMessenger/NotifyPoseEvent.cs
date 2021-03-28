using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class NotifyPoseEvent : MonoBehaviour
{
    public string objectPoseInformationEventName;
    public GameObject referenceKOS;
    public GameObject calibKOS;
    private EventParam pose;
    public void notifyPose()
    {
        Vector3 calibObjectPositionInWorld= calibKOS.transform.position;
        Quaternion calibObjectRotation2World = calibKOS.transform.rotation;
        //
        Vector3 refPositionInWorld = referenceKOS.transform.position; // position of reference in world
        Quaternion reference2World = referenceKOS.transform.rotation; //reference to world
        // we need to calculate the the position in regards to the referenceObject
        Quaternion world2Reference = Quaternion.Inverse(reference2World);
        Vector3 worldPosInRef = -(world2Reference *  refPositionInWorld);
        Quaternion calibObject2Reference = calibObjectRotation2World * world2Reference;
        Vector3 calibObjectPositionInReference = worldPosInRef+ world2Reference* refPositionInWorld;
        // Build the event reponse
        pose = new EventParam();
        //pose.position = calibObjectPositionInReference;
        //pose.rotation = calibObject2Reference;



        pose.position = calibKOS.transform.position;
        pose.rotation = calibKOS.transform.rotation;
        EventManager.TriggerEvent(objectPoseInformationEventName, pose);
    }
}
