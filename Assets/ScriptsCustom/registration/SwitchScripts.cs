using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class SwitchScripts : MonoBehaviour
{
    public string mainControllerEvent;

    public GameObject testControllerObject;
    public GameObject coordinateSystem;

    // https://answers.unity.com/questions/207441/how-do-i-disable-a-script-from-a-different-script.html

    private bool moveControllerScriptEnabeld = true;
    private bool lastStateMenuButton = false; 


    void toggleActiveScript()
    {
        moveControllerScriptEnabeld = !moveControllerScriptEnabeld;
        testControllerObject.GetComponent<MoveViaButtonsBetter>().enabled = !moveControllerScriptEnabeld;
        coordinateSystem.SetActive(!moveControllerScriptEnabeld);
        testControllerObject.GetComponent<MoveController>().enabled = moveControllerScriptEnabeld;
    }
    void OnEnable()
    {
        EventManager.StartListening(mainControllerEvent, readMenuButtonAndToggle);


        testControllerObject.GetComponent<MoveViaButtonsBetter>().enabled = false;
        coordinateSystem.SetActive(false);
        testControllerObject.GetComponent<MoveController>().enabled = true;
        moveControllerScriptEnabeld = true;

    }
    void readMenuButtonAndToggle(EventParam mainControllerState)
    {
        bool menuButton = Convert.ToBoolean(mainControllerState.buttonState["menuButton"]);

        if (menuButton == true && lastStateMenuButton == false)
        {
            toggleActiveScript();
        }
            lastStateMenuButton = menuButton;
    }

    void OnDisable()
    {
        EventManager.StopListening(mainControllerEvent, readMenuButtonAndToggle);
    }
}
