using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using UnityEngine;

using System.Globalization;


#if !UNITY_EDITOR
using System.Threading.Tasks;
#endif

public class tcpScript : MonoBehaviour
{
    public String ipTCPHost;
    public String portTCPHost;

    public GameObject StatusTextManager;
   

    private Vector3 position;
    private Quaternion rotation;
    private bool moveObjectFlag = false;
    private int currentAxisNum = 0;

    private bool fineIncrementMode = false;//2 modes for increment input one for fine second one for rough
                                           // Start is called before the first frame update

#if !UNITY_EDITOR
    private bool _useUWP = true;
    private Windows.Networking.Sockets.StreamSocket socket;
    private Task exchangeTask;
#endif

#if UNITY_EDITOR
    private bool _useUWP = false;
    System.Net.Sockets.TcpClient client;
    System.Net.Sockets.NetworkStream stream;
    private Thread exchangeThread;
#endif

    private Byte[] bytes = new Byte[256];
    private StreamWriter writer;
    private StreamReader reader;

    public void Connect(string host, string port)
    {
        if (_useUWP)
        {
            ConnectUWP(host, port);
        }
        else
        {
            ConnectUnity(host, port);
        }
    }



#if UNITY_EDITOR
    private void ConnectUWP(string host, string port)
#else
    private async void ConnectUWP(string host, string port)
#endif
    {
#if UNITY_EDITOR
        StatusTextManager.GetComponent<TextMesh>().text = "UWP TCP client used in Unity!";
#else
        try
        {
            if (exchangeTask != null) StopExchange();
        
            socket = new Windows.Networking.Sockets.StreamSocket();
            Windows.Networking.HostName serverHost = new Windows.Networking.HostName(host);
            await socket.ConnectAsync(serverHost, port);
        
            Stream streamOut = socket.OutputStream.AsStreamForWrite();
            writer = new StreamWriter(streamOut) { AutoFlush = true };
        
            Stream streamIn = socket.InputStream.AsStreamForRead();
            reader = new StreamReader(streamIn);

            RestartExchange();
            StatusTextManager.GetComponent<TextMesh>().text = "Connected!";
        }
        catch (Exception e)
        {
            errorStatus = e.ToString();
        }
#endif
    }

    private void ConnectUnity(string host, string port)
    {
#if !UNITY_EDITOR
        errorStatus = "Unity TCP client used in UWP!";
#else
        try
        {
            if (exchangeThread != null) StopExchange();

            client = new System.Net.Sockets.TcpClient(host, Int32.Parse(port));
            stream = client.GetStream();
            reader = new StreamReader(stream);
            writer = new StreamWriter(stream) { AutoFlush = true };

            RestartExchange();
            StatusTextManager.GetComponent<TextMesh>().text = "Connected!";
            connected = true;
        }
        catch (Exception e)
        {
            errorStatus = e.ToString();
        }
#endif
    }

    private bool exchanging = false;
    private bool exchangeStopRequested = false;
    private string lastPacket = null;
    private bool connected = false;
    private string errorStatus = null;

    public void RestartExchange()
    {
#if UNITY_EDITOR
        if (exchangeThread != null) StopExchange();
        exchangeStopRequested = false;
        exchangeThread = new System.Threading.Thread(ExchangePackets);
        exchangeThread.Start();
#else
        if (exchangeTask != null) StopExchange();
        exchangeStopRequested = false;
        exchangeTask = Task.Run(() => ExchangePackets());
#endif
    }

   

    void Start()
    {
        StatusTextManager.GetComponent<TextMesh>().text = "asdf";
        Connect(ipTCPHost, portTCPHost);
        //Connect("192.168.178.44", "5005");//Laptop at home
        //Connect("192.168.43.152", "5005");//Hotspot
        //Connect("192.168.43.138", "5005");//Alienware
        //axisList = new List<GameObject>() { xAxis, yAxis, zAxis, xRotation, yRotation, zRotation };
        StatusTextManager.GetComponent<TextMesh>().text = "sdf";
    }

    // Update is called once per frame
    void Update()
    {
        //if (connected == false){
        //    Connect("192.168.178.44", "5005");//makes the programm impossible to run if there is no connection:/
        //}
        if (connected == false)
        {
            StatusTextManager.GetComponent<TextMesh>().text = "not connected";
        }
        if (connected == true)
        {
            StatusTextManager.GetComponent<TextMesh>().text = "connected";
        }
        //if (lastPacket != null)
        //{
        //    ReportDataToTrackingManager(lastPacket);
        //    StatusTextManager.GetComponent<TextMesh>().text = position.ToString("F4") + "\n" + rotation.ToString("F4");
        //}


    }
    private void ReportDataToTrackingManager(string data)
    {
        if (data == null)
        {
            Debug.Log("Received a frame but data was null");
            return;
        }
        Debug.Log("data to process");
        Debug.Log(data);
        //print(data);
        //var parts = data.Split(';');
        //foreach (var part in parts)
        //{
        //    ReportStringToTrackingManager(part);
        //}
        //Debug.Log(data);
        //ReportStringToTrackingManager(data);
    }

    public void ExchangePackets()
    {
        while (!exchangeStopRequested)
        {
            if (writer == null || reader == null) continue;
            exchanging = true;

            writer.Write("X\n");
            //Debug.Log("Sent data!");
            string received = null;

#if UNITY_EDITOR
            byte[] bytes = new byte[client.SendBufferSize];
            int recv = 0;
            while (true)
            {
                recv = stream.Read(bytes, 0, client.SendBufferSize);
                received += Encoding.UTF8.GetString(bytes, 0, recv);
                if (received.EndsWith("\n")) break;
            }
#else
            received = reader.ReadLine();
#endif

            lastPacket = received;
            //Debug.Log("Read data: " + received);

            exchanging = false;
        }
    }
    public void StopExchange()
    {
        exchangeStopRequested = true;

#if UNITY_EDITOR
        if (exchangeThread != null)
        {
            exchangeThread.Abort();
            stream.Close();
            client.Close();
            writer.Close();
            reader.Close();

            stream = null;
            exchangeThread = null;
        }
#else
        if (exchangeTask != null) {
            exchangeTask.Wait();
            socket.Dispose();
            writer.Dispose();
            reader.Dispose();

            socket = null;
            exchangeTask = null;
        }
#endif
        writer = null;
        reader = null;
    }

    public void OnDestroy()
    {
        StopExchange();
    }

}

