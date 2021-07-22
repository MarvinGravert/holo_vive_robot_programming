using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

// attach this script to TextMeshpro object and put a reference to the object that you are interested displaying their pose
public class ReferencePositon : MonoBehaviour
{
    private TextMeshPro StatusTextManager;
    public GameObject referenceObject;
    public string prependMessage;
    void Start()
    {
        StatusTextManager = this.gameObject.GetComponent<TextMeshPro>();
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 position = referenceObject.transform.position;
        Quaternion rotation = referenceObject.transform.rotation;
        //Debug.Log(Matrix4x4.TRS(position, rotation, new Vector3(1, 1, 1)));
        StatusTextManager.text =prependMessage+": "+ position.ToString("F4") + "\n" + rotation.ToString("F4");
    }
}
