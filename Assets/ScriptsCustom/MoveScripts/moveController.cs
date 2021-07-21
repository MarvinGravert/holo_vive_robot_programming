using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveController : MonoBehaviour
{
    private GameObject controller;
    public GameObject referenceObject;

    // initiliase the registration transformation with a 
    private Quaternion registrationRotation=new Quaternion(0,0,0,1);
    private Vector3 registrationPosition =new Vector3(0,0,0);
    private Matrix4x4 registrationMatrix = new Matrix4x4(new Vector4(1,0,0,0),new Vector4(0,1,0,0),new Vector4(0,0,1,0),new Vector4(0,0,0,1));
    public string registrationTransformationEventName;
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
    void MoveToNewPose(EventParam newController)
    {
        Debug.Log(newController.name);
        //// the reference object the controller is located to. (either world or camera)
        //Vector3 refPositionInWorld = referenceObject.transform.position; // position of reference in world
        //Quaternion reference2World = referenceObject.transform.rotation; //reference to world
        //// we get the transformation  controller->virtual center (aka the camera) as we need to locate the controller prefab
        //// in regards to the user. 
        //Quaternion reference2Controller = Quaternion.Inverse(newPose.rotation);
        //Quaternion controller2Reference = newPose.rotation;//this should be the rotation (controller2reference)
        //Vector3 controllerPosInReference = newPose.position;
        //this.transform.position = refPositionInWorld + reference2World*controllerPosInReference;
        //// as per this https://docs.unity3d.com/ScriptReference/Quaternion-operator_multiply.html
        //// the left most quaternion represents the fist rotation so first apply the rotation (controller->reference)
        //// then the rotation (reference to 
        //this.transform.rotation = controller2Reference * reference2World;
        //// for debugging purposes just simply place the controller into the world aka use the world as reference
        //this.transform.position = controllerPosInReference;
        //this.transform.rotation = controller2Reference ;
    }
}
