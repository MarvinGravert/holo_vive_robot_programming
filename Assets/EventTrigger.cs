using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class EventTrigger : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown("q"))
        {
            EventParam info = new EventParam();
            info.tcpIPMessage = "hello";
            EventManager.TriggerEvent("test", info);
        }

    }
    //    if (Input.GetKeyDown("q"))
    //    {
    //        EventManager.TriggerEvent("test");
    //    }

    //    if (Input.GetKeyDown("o"))
    //    {
    //        EventManager.TriggerEvent("Spawn");
    //    }

    //    if (Input.GetKeyDown("p"))
    //    {
    //        EventManager.TriggerEvent("Destroy");
    //    }

    //    if (Input.GetKeyDown("x"))
    //    {
    //        EventManager.TriggerEvent("Junk");
    //    }
    //}
}
