using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Microsoft.MixedReality.Toolkit.UI;//for button slider
public class MoveViaButtonsBetter : MonoBehaviour
{
    public string newControllerEvent;
    public float lowerIncrementStep;
    public float upperIncrementStep;

    public GameObject objectToMove;

    public GameObject xAxis;
    public GameObject yAxis;
    public GameObject zAxis;
    public GameObject xRotation;
    public GameObject yRotation;
    public GameObject zRotation;

    private List<GameObject> axisList;


    private int currentAxisNum = 0;
    private int lastAxisNum = 0;

    private bool lastTriggerButtonState = false; //assume that the button is not pressed 
    private float increment = 0.02f;//values will be changed between 0.05f and 0.001f via a slider

    /*
     * This script is implementing the logic to move an object in 6D depending on the input from the Vive Controller
     *  The basic idea is as follows:
     *  The object is shown with its local coordinate axis. One of these axes is highlighted. The object may be moved in negative or positive direction 
     *  in respect to the axis. The user may cycle through all 6 axes (translative and rotational). 
     *  
     *  The cycle is local x y z Rx Ry Rz and may be switched by pulling the "trigger" button (one switch per trigger pull)
     *  The direction (negative or positive) depends on where the trackpad is pressed. The upper diagonal half its positive in the lower its negative)
     *  
     *  The degree or strength of movement is influenced by a scaling factor which may be set using a slider and the trackpad press position. The further outword the stronger. 
     *  
     *  Object is only moved when the trackpad is pressed and for a value set via a slider. Direction depends on the trackpad position
    * 
    */
    void Awake()
    {
        axisList = new List<GameObject>() { xAxis, yAxis, zAxis, xRotation, yRotation, zRotation };
    }
    private void Start()
    {
        ChangeAxisHighlight();//To color in the first axis 
        Vector3 currentEulerAngles = this.transform.eulerAngles;
    }
    void readButtonStateAndMove(EventParam buttonState)
    {   
        /*We are looking for one of two things:
         * 1. Change in trigger button  =>  change the hlighted axis
         * 2. Change in Trackpad_pressed => move the object in its highlighted
         */

        float x_trackpad = buttonState.buttonState["x_trackpad"];
        float y_trackpad = buttonState.buttonState["y_trackpad"];
        bool triggerButton = Convert.ToBoolean(buttonState.buttonState["triggerButton"]);
        bool trackpadPressed = Convert.ToBoolean(buttonState.buttonState["trackpadPressed"]);
        bool menuButton = Convert.ToBoolean(buttonState.buttonState["menuButton"]);
        bool gripButton = Convert.ToBoolean(buttonState.buttonState["gripButton"]);

        // detect a change from not pressed (false) to pressed (True) and cycle to the next axis (increment axis count)
        // if trigger has been pulled, start the function to change the highlight (and thus the active axis)
        if (triggerButton == true && lastTriggerButtonState == false)
        {
            //Debug.Log("Change in TriggerButton");
            lastAxisNum = currentAxisNum;
            currentAxisNum += 1;
            if (currentAxisNum > 5)//only 6 axis hence wrap to first axis again
            {
                currentAxisNum = 0;
            }
            //Debug.Log($"The current Axis is {currentAxisNum}");
            ChangeAxisHighlight();

        }
        lastTriggerButtonState = triggerButton;
         
        if (trackpadPressed == true)
        {
            MoveObject(x_trackpad, y_trackpad);
        }

    }

    private void ChangeAxisHighlight()
    {
        // The axis are numbered 0->x,..., 5->Rz
        // The translative and rotational parts are different models thus need to be changed differently
        // The active axis is red the other ones are white
  
        if (lastAxisNum < 3)
        {
            foreach (Renderer r in axisList[lastAxisNum].GetComponentsInChildren<Renderer>())
            {
                r.material.color = Color.white;
            }
        }
        else
        {
            axisList[lastAxisNum].GetComponent<Renderer>().material.color = Color.white;
        }
        // the axis are different objects hence their colo has to be changed differently
        if (currentAxisNum < 3)
        {
            foreach (Renderer r in axisList[currentAxisNum].GetComponentsInChildren<Renderer>())
            {
                r.material.color = Color.red;
            }
        }
        else
        {
            axisList[currentAxisNum].GetComponent<Renderer>().material.color = Color.red;
        }

    }

    private void MoveObject(float xTrackpadPosition, float yTrackpadPosition)
    {
        /* The 
         * 1. Calculate angular position of where to trackpad was pressed to determine the direction
         * 2. Move the object in its local kos
         */
        float angle = Mathf.Rad2Deg * Mathf.Atan2(yTrackpadPosition, xTrackpadPosition);//conversion via multiplicatoin with 360/(2*pi)
     
        int direction = 1;
        if (angle <= 135 && angle > -45) //atan retuns 0->360 
        {
            direction = 1;
        }
        else
        {
            direction = -1;
        }

        /*  
         * ITS MOVING TIME
         */
        float trackpad_amp = 1; //Mathf.Sqrt(xTrackpadPosition * xTrackpadPosition + yTrackpadPosition * yTrackpadPosition);
        float amp = increment;// trackpad_amp * increment;


        switch (currentAxisNum)
        {
            case 0:
                objectToMove.transform.Translate(direction * Vector3.right*Time.deltaTime * amp);
                break;
            case 1:
                objectToMove.transform.Translate(direction * Vector3.up * Time.deltaTime * amp);
                break;
            case 2:
                objectToMove.transform.Translate(direction * Vector3.forward * Time.deltaTime * amp);
               
                break;
            case 3:
                objectToMove.transform.RotateAround(transform.position, transform.right, amp * direction *Time.deltaTime * 90f);
                break;
            case 4:
                objectToMove.transform.RotateAround(transform.position, transform.up, amp * direction * Time.deltaTime * 90f);
                break;
            case 5:
                objectToMove.transform.RotateAround(transform.position, transform.forward, amp * direction * Time.deltaTime * 90f);
                break;

        }
       
    }

    public void OnSliderUpdated(SliderEventData eventData)
    {
        // slider returns between 0->1
        increment = lowerIncrementStep + eventData.NewValue *upperIncrementStep;
        //Debug.Log($"Changing Value to {increment:F4} ");
    }

    void OnEnable()
    {

        EventManager.StartListening(newControllerEvent, readButtonStateAndMove);

    }
    void OnDisable()
    {
        EventManager.StopListening(newControllerEvent, readButtonStateAndMove);
    }
}
