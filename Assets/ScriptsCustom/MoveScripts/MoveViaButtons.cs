using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Microsoft.MixedReality.Toolkit.UI;//for button slider
public class MoveViaButtons : MonoBehaviour
{
    public string buttonStateEventName;
    public float lowerIncrementStep;
    public float upperIncrementStep;

    public GameObject referenceObject;

    public GameObject xAxis;
    public GameObject yAxis;
    public GameObject zAxis;
    public GameObject xRotation;
    public GameObject yRotation;
    public GameObject zRotation;

    private List<GameObject> axisList;


    private int currentAxisNum = 0;
    private int lastAxisNum = 0;

    private bool lastTriggerButtonState=false; //assume that the button is not pressed 
    private float increment = 0.02f;//values will be changed between 0.05f and 0.001f via a slider
    private bool changeAxis = false;

    /*
     *  This script is implemnting the logic to move an object in 6D depending on the input from the Vive Controller
     *  The basic idea is as follows:
     *  Object is only moved when the trackpad is pressed and for a value set via a slider. Direction depends on the trackpad position
    * An axis is choosen via the trigger button. Initially its set to the x axis. The cycle is x-y-z-Rx-Ry-Rz
    * Direction is done via checking the angle on the circle that is the trackpad. If the user is pushing in the upper diagonal half its positive direction otherwise negative
    * The increment is set via a slider
    * The current axis is highlighted
    * 
    * The change is only done on a change of the trigger when its going from not pressed to pressed.
    */

    private void Start()
    {
        ChangeAxisHighlight();//To color in the first axis 
    }
    void readButtonStateAndMove(EventParam buttonState)
    {
        float x_trackpad = buttonState.buttonState["x_trackpad"];
        float y_trackpad = buttonState.buttonState["y_trackpad"];
        bool triggerButton = Convert.ToBoolean(buttonState.buttonState["triggerButton"]);
        bool trackpadPressed = Convert.ToBoolean(buttonState.buttonState["trackpadPressed"]);
        bool menuButton = Convert.ToBoolean(buttonState.buttonState["menuButton"]);
        bool gripButton = Convert.ToBoolean(buttonState.buttonState["gripButton"]);

        //detect a change from not pressed (false) to pressed (True)
        if (triggerButton==true && lastTriggerButtonState ==false )
        {
            Debug.Log("Change in TriggerButton");
            lastAxisNum = currentAxisNum;
            currentAxisNum += 1;
            if (currentAxisNum > 5)//only 6 axis hence wrap to first axis again
            {
                currentAxisNum = 0;
            }
            Debug.Log($"The current Axis is {currentAxisNum}");
            changeAxis = true;

        }
        lastTriggerButtonState = triggerButton;
        if (changeAxis == true)
        {
            ChangeAxisHighlight();
            changeAxis = false;
        }
        if (trackpadPressed == true)
        {
            MoveObjectNew(x_trackpad,y_trackpad);
        }
        

        //MoveObject(x_trackpad, triggerButton, trackpadPressed, menuButton, gripButton);
    }

    private void ChangeAxisHighlight()
    {
        //0 ->x,... ,5->Rz
        /*
         * Remove highlighting. Not used atm but kept around
         */
        if (lastAxisNum<3 )
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

    private void MoveObjectNew(float xTrackpadPosition, float yTrackpadPosition)
    {
        float angle = Mathf.Rad2Deg*Mathf.Atan2(yTrackpadPosition, xTrackpadPosition);//conversion via multiplicatoin with 360/(2*pi)
        Debug.Log($"The angle is: {angle}");
        int direction = 1;
        float rotationScaler = 16.0f; //rotation are done slower than translation hence scaling!
        if (angle <=135 && angle > -45) //atan retuns 0->360 
        {
            direction = 1;
        }
        else
        {
            direction = -1;
        }

        if (currentAxisNum < 3)
        {
            var tempRotation = referenceObject.transform.rotation;//we need to transform the unit vectors in the coordinante system of the object before
                                                                  //adding them to the current position of the object
                                                                  //change position
            switch (currentAxisNum)
            {
                case 0:
                    referenceObject.transform.position += tempRotation * new Vector3(increment * direction, 0, 0);
                    break;
                case 1:
                    referenceObject.transform.position += tempRotation * new Vector3(0, increment * direction, 0);
                    break;
                case 2:
                    referenceObject.transform.position += tempRotation * new Vector3(0, 0, increment * direction);
                    break;
            }

        }
        else
        {
            //change rotation
            switch (currentAxisNum)
            {
                case 3:
                    referenceObject.transform.eulerAngles += new Vector3(increment * rotationScaler* direction, 0, 0);
                    break;
                case 4:
                    referenceObject.transform.eulerAngles += new Vector3(0, increment * rotationScaler*direction, 0);
                    break;
                case 5:
                    referenceObject.transform.eulerAngles += new Vector3(0, 0, increment * rotationScaler*direction);
                    break;
            }
        }
    }
    void Awake()
    {
        axisList = new List<GameObject>() { xAxis, yAxis, zAxis, xRotation, yRotation, zRotation };
    }
    public void OnSliderUpdated(SliderEventData eventData)
    {
        //map the slider value onto range from lower to upper value
        increment = lowerIncrementStep + eventData.NewValue*(upperIncrementStep- lowerIncrementStep);
        Debug.Log($"Changing Value to {increment:F4} ");
    }

    void OnEnable()
    {

        EventManager.StartListening(buttonStateEventName, readButtonStateAndMove);

    }
    void OnDisable()
    { 
        EventManager.StopListening(buttonStateEventName, readButtonStateAndMove);
    }
}
