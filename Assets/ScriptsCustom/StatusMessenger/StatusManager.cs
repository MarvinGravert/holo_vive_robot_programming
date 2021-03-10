using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class StatusManager : MonoBehaviour
{
    public string statusEventName;
    public GameObject DebugLogger;
   

    void OnEnable()
    {

        EventManager.StartListening(statusEventName, ManageStatus);

    }
    //private void Update()
    //{
    //    DebugLogger.SetActive(false);
    //}
    void OnDisable()
    {

        EventManager.StartListening(statusEventName, ManageStatus);
    }

    void ManageStatus(EventParam newStatus)
    {
        Debug.Log("so its go wit");
        string status = newStatus.status;
        if (status.Contains("cmd")){//cmd_order_setting
            var parts=status.Split('_');
            string order = parts[1];
            var setting = parts[2];
            Debug.Log(setting);
            switch (order)
            {
                case "debug":
                    if (setting.Contains("on"))
                    {
                        Debug.Log("turning on");
                        DebugLogger.SetActive(true);
                    }
                    if (setting.Contains("off"))
                    {
                        Debug.Log("turning off");
                        DebugLogger.SetActive(false);
                    }
                    break;
            }

        }
            
    }
}
