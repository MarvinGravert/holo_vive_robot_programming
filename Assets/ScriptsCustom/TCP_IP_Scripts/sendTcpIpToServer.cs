using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class sendTcpIpToServer : MonoBehaviour
{
    public string sendCalibrationEventName;

    private void SendToServer(EventParam calibrationData)
    {
        Debug.Log(calibrationData.tcpIPMessage);
    }
    void OnEnable()
    {

        EventManager.StartListening(sendCalibrationEventName, SendToServer);

    }
    void OnDisable()
    {

        EventManager.StopListening(sendCalibrationEventName, SendToServer);
    }
}
