using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnLoadOnStartup : MonoBehaviour
{
    public List<GameObject> objectsToUnload;
    void Start()
    {
     foreach (GameObject obj in objectsToUnload)
        {
            obj.SetActive(false);
        }   
    }
}
