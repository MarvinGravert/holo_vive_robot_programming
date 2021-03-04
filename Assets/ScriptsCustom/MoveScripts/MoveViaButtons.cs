using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class MoveViaButtons : MonoBehaviour
{
    public string buttonStateEventName;

    public GameObject referenceObject;

    public GameObject xAxis;
    public GameObject yAxis;
    public GameObject zAxis;
    public GameObject xRotation;
    public GameObject yRotation;
    public GameObject zRotation;

    private List<GameObject> axisList;
    public float incrementStepSize;

    private bool moveObjectFlag = false;
    private int currentAxisNum = 0;

    private bool fineIncrementMode = false;//2 modes for increment input one for fine second one for rough

    void Awake()
    {
        axisList = new List<GameObject>() { xAxis, yAxis, zAxis, xRotation, yRotation, zRotation };
    }


    void readButtonStateAndMove(EventParam buttonState)
    {
        float x_trackpad = buttonState.buttonState["x_trackpad"];
        bool triggerButton = Convert.ToBoolean(buttonState.buttonState["triggerButton"]);
        bool trackpadPressed = Convert.ToBoolean(buttonState.buttonState["trackpadPressed"]);
        bool menuButton = Convert.ToBoolean(buttonState.buttonState["menuButton"]);
        bool gripButton = Convert.ToBoolean(buttonState.buttonState["gripButton"]);

        MoveObject(x_trackpad, triggerButton, trackpadPressed, menuButton, gripButton);
    }

    private void MoveObject(float xTrackpadPosition, bool triggerButton, bool trackpadPressed, bool menuButton, bool gripButton)
    {
        // change between rough and fine increment
        if (menuButton == true)
        {
            fineIncrementMode = !fineIncrementMode;
        }
        if (triggerButton == true)
        {
            //change target Axis
            //first current highlight disable if moveObjectFlag is set
            Debug.Log("here we go");
            if (moveObjectFlag == false)
            {
                //if not true no axis is highlighed thus just increase counter
                currentAxisNum++;
                if (currentAxisNum > 5)
                {
                    currentAxisNum = 0;
                }
            }
            else
            {

                if (currentAxisNum < 3)//the translationelements have subelemnents
                {
                    
                    foreach (Renderer r in axisList[currentAxisNum].GetComponentsInChildren<Renderer>())
                    {
                        r.material.color = Color.white;
                    }
                }
                else
                {
                    axisList[currentAxisNum].GetComponent<Renderer>().material.color = Color.white;
                }
                currentAxisNum++;
                //now highlight the current one
                if (currentAxisNum > 5)
                {
                    currentAxisNum = 0;
                }
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
        }

        if (gripButton == true && moveObjectFlag == true)
        {
            //disable moving
            moveObjectFlag = false;
            //remove highlighting
            if (currentAxisNum < 3)
            {
                foreach (Renderer r in axisList[currentAxisNum].GetComponentsInChildren<Renderer>())
                {
                    r.material.color = Color.white;
                }
            }
            else
            {
                axisList[currentAxisNum].GetComponent<Renderer>().material.color = Color.white;
            }


        }


        if (gripButton == true && moveObjectFlag == false)
        {
            //enable moving
            moveObjectFlag = true;
            //highlight axis
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
        if (moveObjectFlag == true)
        {
            if (fineIncrementMode == false)
            {
                //move object in rough mode
                if (currentAxisNum < 3)
                {
                    var tempRotation = referenceObject.transform.rotation;//we need to transform the unit vectors in the coordinante system of the object before
                                                                          //adding them to the current position of the object
                                                                          //change position
                    switch (currentAxisNum)
                    {
                        case 0:
                            referenceObject.transform.position += tempRotation * new Vector3(0.05f * xTrackpadPosition, 0, 0);
                            break;
                        case 1:
                            referenceObject.transform.position += tempRotation * new Vector3(0, 0.05f * xTrackpadPosition, 0);
                            break;
                        case 2:
                            referenceObject.transform.position += tempRotation * new Vector3(0, 0, 0.05f * xTrackpadPosition);
                            break;
                    }

                }
                else
                {
                    //change rotation
                    switch (currentAxisNum)
                    {
                        case 3:
                            referenceObject.transform.eulerAngles += new Vector3(0.2f * xTrackpadPosition, 0, 0);
                            break;
                        case 4:
                            referenceObject.transform.eulerAngles += new Vector3(0, 0.2f * xTrackpadPosition, 0);
                            break;
                        case 5:
                            referenceObject.transform.eulerAngles += new Vector3(0, 0, 0.2f * xTrackpadPosition);
                            break;
                    }
                }
            }
            else
            {
                //move object in fine mode
                //move object only if trackpad is pressed and depending on the xposition
                if (trackpadPressed == true)
                {
                    var inputDirection = Math.Sign(xTrackpadPosition);

                    if (currentAxisNum < 3)
                    {
                        var tempRotation = referenceObject.transform.rotation;//we need to transform the unit vectors in the coordinante system of the object before
                                                                              //adding them to the current position of the object
                                                                              //change position
                        switch (currentAxisNum)
                        {
                            case 0:
                                referenceObject.transform.position += tempRotation * new Vector3(0.001f * inputDirection, 0, 0);
                                break;
                            case 1:
                                referenceObject.transform.position += tempRotation * new Vector3(0, 0.001f * inputDirection, 0);
                                break;
                            case 2:
                                referenceObject.transform.position += tempRotation * new Vector3(0, 0, 0.001f * inputDirection);
                                break;
                        }

                    }
                    else
                    {
                        //change rotation
                        switch (currentAxisNum)
                        {
                            case 3:
                                referenceObject.transform.eulerAngles += new Vector3(0.1f * inputDirection, 0, 0);
                                break;
                            case 4:
                                referenceObject.transform.eulerAngles += new Vector3(0, 0.1f * inputDirection, 0);
                                break;
                            case 5:
                                referenceObject.transform.eulerAngles += new Vector3(0, 0, 0.1f * inputDirection);
                                break;
                        }
                    }
                }

            }

        }
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
