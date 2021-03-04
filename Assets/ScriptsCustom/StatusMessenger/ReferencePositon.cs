using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

// attach this script to TextMeshpro object and put a reference to the object that you are interested displaying their pose
public class ReferencePositon : MonoBehaviour
{
    private TextMeshPro StatusTextManager;
    public GameObject referenceObject;
    void Start()
    {
        StatusTextManager = this.gameObject.GetComponent<TextMeshPro>();
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 position = referenceObject.transform.position;
        Quaternion rotation = referenceObject.transform.rotation;
        StatusTextManager.text = position.ToString("F4") + "\n" + rotation.ToString("F4");
    }
}
