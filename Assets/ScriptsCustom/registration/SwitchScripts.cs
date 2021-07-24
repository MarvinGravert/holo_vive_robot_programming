using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchScripts : MonoBehaviour
{
    public string testControllerEvent;

    public GameObject testControllerObject;

    // https://answers.unity.com/questions/207441/how-do-i-disable-a-script-from-a-different-script.html

    private bool moveControllerScriptEnabeld = true;

    void toggleActiveScript()
    {
        moveControllerScriptEnabeld = !moveControllerScriptEnabeld;
        testControllerObject.GetComponent<MoveViaButtonsBetter>().enabled = !moveControllerScriptEnabeld;
        testControllerObject.GetComponent<MoveController>().enabled = moveControllerScriptEnabeld;
    }
    void OnEnable()
    {
        testControllerObject.GetComponent<MoveViaButtonsBetter>().enabled = false;
        testControllerObject.GetComponent<MoveController>().enabled = true;
        moveControllerScriptEnabeld = true;

    }
}
