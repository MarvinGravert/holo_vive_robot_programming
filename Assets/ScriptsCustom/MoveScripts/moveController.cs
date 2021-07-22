using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveController : MonoBehaviour
{
    private GameObject controller;

    // initiliase the registration transformation with a identy matrix
    private Quaternion registrationRotation=new Quaternion(0,0,0,1);
    private Vector3 registrationPosition =new Vector3(0,0,0);
    private Matrix4x4 registrationMatrix = new Matrix4x4(new Vector4(1,0,0,0),new Vector4(0,1,0,0),new Vector4(0,0,1,0),new Vector4(0,0,0,1));
    public string registrationTransformationEventName;
    public string newControllerPoseEventName;

    void OnEnable()
    {
        EventManager.StartListening(newControllerPoseEventName, MoveToNewPose);
        EventManager.StartListening(registrationTransformationEventName, ChangeRegistration);
        controller = this.gameObject;
    }

    // Update is called once per frame
    void OnDisable()
    {
        EventManager.StopListening(newControllerPoseEventName, MoveToNewPose);
        EventManager.StopListening(registrationTransformationEventName, ChangeRegistration);
    }
    void ChangeRegistration(EventParam newRegistration)
    {
        registrationMatrix = Matrix4x4.TRS(newRegistration.position, newRegistration.rotation, new Vector3(1, 1, 1));
        Debug.Log(registrationMatrix);
    }
    void MoveToNewPose(EventParam newController)
    {
        //Debug.Log(newController.name);
        // so now lets create a 4x4 matrix of the controller 
        Matrix4x4 controllerTrackerKOS = Matrix4x4.TRS(newController.position, newController.rotation, new Vector3(1, 1, 1));
        // Apply the transformatino matrix to 
        Matrix4x4 controllerHoloWorld = registrationMatrix * controllerTrackerKOS;

        // turn matrix to left handed by negating third row and third column
        Matrix4x4 controllerUnityWorld = ChangeHandedness(controllerHoloWorld);

        MoveController.FromMatrix(controller, controllerTrackerKOS);//TODO CHANGE THIS
    }

    public static Matrix4x4 ChangeHandedness(Matrix4x4 matrix)
    {
        Vector4 thirdRow = matrix.GetRow(2);//0->3
        matrix.SetRow(2, thirdRow);
        Vector4 thirdColumn = matrix.GetColumn(2);
        matrix.SetColumn(2, thirdColumn);
        return matrix;
    }
    public static void FromMatrix(GameObject totransfrom, Matrix4x4 matrix)
    {
        totransfrom.transform.rotation = matrix.rotation;
        totransfrom.transform.position = new Vector3(matrix.m03,matrix.m13,matrix.m23);
    }

}
