using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToggleObject : MonoBehaviour
{
    public GameObject toggleObject;
    public List<GameObject> objectsToToggle;
    public  bool initialObjectsState=true;

    private void Awake()
    {
        foreach (var obj in objectsToToggle)
        {
            obj.SetActive(initialObjectsState);
        }
    }
    public void toggle()
    {
        Debug.Log(initialObjectsState);
        initialObjectsState = !initialObjectsState;
        foreach (var obj in objectsToToggle)
        {
            obj.SetActive(initialObjectsState);
        }
        
       
    }
    
}
