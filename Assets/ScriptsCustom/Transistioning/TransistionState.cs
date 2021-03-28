using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransistionState : MonoBehaviour
{
    public List<GameObject> objectsToUnload;
    public List<GameObject> objectsToLoad;
    
    public void transistion()
    {
        foreach (GameObject obj in objectsToLoad)
        {
            obj.SetActive(true);
        }
        foreach (GameObject obj in objectsToUnload)
        {
            obj.SetActive(false);
        }

    }
}
