using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToggleObject : MonoBehaviour
{
    public GameObject toggleObject;
    private bool objectState=true;
    
    public void toggle()
    {
        objectState = !objectState;
        toggleObject.SetActive(objectState);
    }
}
