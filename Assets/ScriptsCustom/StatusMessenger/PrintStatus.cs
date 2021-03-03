using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

// print the status that was sent by the server 
// 
public class PrintStatus : MonoBehaviour
{
    public string statusEventName;
    private TextMeshPro statusTextManager;
    void Awake()
    {
        statusTextManager = this.gameObject.GetComponent<TextMeshPro>();
        
    }

    void OnEnable()
    {

        EventManager.StartListening(statusEventName, setNewStatus);

    }
    void OnDisable()
    {

        EventManager.StartListening(statusEventName, setNewStatus);
    }
    
    void setNewStatus(EventParam newStatus)
    {
        Debug.Log("new status received");
        statusTextManager.text = newStatus.status;
    }
}
