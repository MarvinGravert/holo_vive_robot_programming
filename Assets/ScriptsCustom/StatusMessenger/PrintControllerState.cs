using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Linq;
public class PrintControllerState : MonoBehaviour
{
    public string fullControllerStateEventName;

    private TextMeshPro statusTextManager;
    void Awake()
    {
        statusTextManager = this.gameObject.GetComponent<TextMeshPro>();

    }

    void OnEnable()
    {

        EventManager.StartListening(fullControllerStateEventName, displayFullControllerState);


    }
    void OnDisable()
    {

        EventManager.StartListening(fullControllerStateEventName, displayFullControllerState);

    }

    void displayFullControllerState(EventParam newState)
    {
        //https://stackoverflow.com/questions/3871760/convert-dictionarystring-string-to-semicolon-separated-string-in-c-sharp
        string s = string.Join("\n", newState.buttonState.Select(x => x.Key + "=" + x.Value).ToArray());
        string state = newState.position.ToString("F4") + "\n" + newState.rotation.ToString("F4") + "\n" + s;
        statusTextManager.text = state;
    }
}
