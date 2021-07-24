using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrientObject : MonoBehaviour
{
    public GameObject referenceObject;

    void OnEnable()
    {
        // object is positioned in 1.5m infront of the reference object. Used to spawn elements at relevant position
        this.transform.position = referenceObject.transform.position + referenceObject.transform.rotation*( new Vector3(0, 0, 1.5f));
    }

    
}
