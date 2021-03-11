using System;
using System.Collections;
using System.IO;
using System.Text;
using System.Threading;
using UnityEngine;
using System.Globalization;
using UnityEngine.Events;
#if !UNITY_EDITOR
using System.Threading.Tasks;
#endif
/*
* This script runs a thread to continously query the backend server for the controller position
* The received information is progated using an EventManager 
* No information processing is done in this module
* 
* The connection is initially etablished and maintained throughout the runtime of the program
* As unity and hololens(UWP) implement threads in different manners there are essentially two setup procedures included in this file
* 
* To allow easier setup the following parameters can be set in unity:
* - IP and Port of the TCP/IP Backend server from whom to retrieve the position
* - the name of the event that is to be triggered 
* 
* A "EventParam" struct is used to pass the information see the eventManager for its specifications
* 
* 
* In Start() a new connection is etablished
* in Update() we check if a new controller position has arrived and trigger the event
* in ExchangePackeg() we communicate information with the backend service
*   - we send single byte to signal that we want new information
*   - backend server responds with new information
*   - new information is put into a variable which is checked in the update method
*  
*  Problems as of now:
*  - we are always one cycle behind in regards to the controller infromation->send request->and on next check read the response to that request
*   => next implementation implement contiounous server side streaming
*  - received message needs to conform to agreed Standard. as of now we consider the message fully read when we receive "\n" 
*  - There is a maximum message size dictacted by the bytes object maximum accepted as of now is 256 bytes which is large enough to handle the current request scope
*  - potential race condition as we are not locking lastPacket
 * */



public class receiveTCPMessageClient : MonoBehaviour
{
    public String ipTCPHost;
    public String portTCPHost;
    public String controllerInformationEventName;

    private EventParam eventParam; // structure which will be send to the EventManager to propagate as argument to callback functions

    public int updateInterval;
    
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

    public void Update()
    {
        // In every update we check if we have received new information and propagate the message to the event stream if necessary
        if (lastPacket != null)
        {
            // build EventParam 
            eventParam = new EventParam();
            eventParam.tcpIPMessage = lastPacket;
            // send it based on eventname specified in unity interface
            EventManager.TriggerEvent(controllerInformationEventName, eventParam);
            // clear lastPacket 
            lastPacket = null;
            
        }



    }
    public void Start()
    {        
        Connect(ipTCPHost, portTCPHost);
    }

    public void ExchangePackets()
    {

        while (!exchangeStopRequested)
        {
            if (writer == null || reader == null) continue;
            exchanging = true;

            writer.Write("X\n"); //to signify to server that we want new information
            string received = null;
            string latestInfo = null;

#if UNITY_EDITOR
            // read into buffer until we receive a "\n"
            byte[] bytes = new byte[client.SendBufferSize];
            int recv = 0;
            while (true)
            {
                recv = stream.Read(bytes, 0, client.SendBufferSize);
                received += Encoding.UTF8.GetString(bytes, 0, recv);
                if (received.EndsWith("\n")) 
                {
                    latestInfo=received;
                    break;

                }
                
            }
            //the following will work in UWP as there is already a function that reads until we receive a "\n"
#else
            received = reader.ReadLine();
            latestInfo=received;
#endif

            lastPacket = latestInfo;
            exchanging = false; // to be replaced as not used atm
            Thread.Sleep(updateInterval);
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
