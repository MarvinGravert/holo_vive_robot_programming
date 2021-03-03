using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveController : MonoBehaviour
{
    private GameObject controller;
    public GameObject referenceObject;

    public string newControllerPoseEventName;

    void OnEnable()
    {
        EventManager.StartListening(newControllerPoseEventName, MoveToNewPose);
        controller = this.gameObject;
    }

    // Update is called once per frame
    void OnDisable()
    {
        EventManager.StopListening(newControllerPoseEventName, MoveToNewPose);
    }
    void MoveToNewPose(EventParam newPose)
    {
        Vector3 refPos = referenceObject.transform.position;
        Quaternion refRot = referenceObject.transform.rotation;
        this.transform.position = refPos + newPose.controllerPosition;
        this.transform.rotation = newPose.controllerRotation * refRot;
    }
}
