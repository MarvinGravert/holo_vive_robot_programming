using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class test_rotation_script : MonoBehaviour
{
    public GameObject testpoint;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log(this.transform.position.ToString("F4"));
        Debug.Log(this.transform.rotation.ToString("F6"));
        Debug.Log($"Point: {testpoint.transform.position.ToString("F4")}");
    }
}
