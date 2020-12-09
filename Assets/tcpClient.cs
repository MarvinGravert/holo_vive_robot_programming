using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using UnityEngine;
using System;
//use as hololens requires this
#if !UNITY_EDITOR
using System.Threading.Tasks;
#endif

/*
 * NOTES: 
 * This is a client
 * It requires the server to be already up and running when this
 * script executes
 * Write the server ont eh correct port and ip
 * 
 * 
 * 
 * 
 */
public class tcpClient : MonoBehaviour
{
    //String Variable that will be sent over to the server
    [SerializeField]
    public string clientMessage;
#if !UNITY_EDITOR
    //Used to determine whether we'll be using the Unity editor or a Hololens
    private bool _useUWP = true;
    //Create a socket to be used as a pipeline to communicate with our server
    private Windows.Networking.Sockets.StreamSocket socket;
    //Allows the program to know when to start, dispose, or close a task
    private Task exchangeTask;
#endif
#if UNITY_EDITOR
    //Use the Unity editor
    private bool _useUWP = false;    //Pipeline used to communicate with our server
    System.Net.Sockets.TcpClient client;
    System.Net.Sockets.NetworkStream stream;    //Allows the program to know when to start, dispose, or close a thread
    private Thread exchangeThread; //keeps track of exchanges
#endif
    private Byte[] bytes = new Byte[256]; //storage for stuff to be send to server
    private StreamWriter writer; //write on the stream socket
    private StreamReader reader; //read of the socket
    // Start is called before the first frame update
    public void Start()
    {
        Connect("#.#.#.#", "####"); //connect to address and port specified
        ExchangePackets();//send message to the server probably use the byte array
    }
    public void Connect(string host, string port)
    // check if connected to unity or to hololens=>how tasks are handeld differs
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
        errorStatus = "UWP TCP client used in Unity!";
#else
        try {
            if (exchangeTask != null) StopExchange();
            socket = new Windows.Networking.Sockets.StreamSocket();
            Windows.Networking.HostName serverHost = new
                Windows.Networking.HostName(host);
            await socket.ConnectAsync(serverHost, port);            
            Stream streamOut=socket.OutputStream.AsStreamForWrite();
            writer = new StreamWriter(streamOut) {AutoFlush=true};
            
            Stream streamIn = socket.InputStream.AsStreamForRead();
            reader = new StreamReader(streamIn);
   
            successStatus = "Connected!";
        } catch (Exception e) {
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
            if (exchangeThread != null)
            {
                StopExchange();
            }
            client = new System.Net.Sockets.TcpClient(host, Int32.Parse(port));
            stream = client.GetStream();
            reader = new StreamReader(stream);
            writer = new StreamWriter(stream) { AutoFlush = true };
            successStatus = "Connected!";
        }
        catch (Exception e)
        {
            errorStatus = e.ToString();
        }
#endif
    }
    //variables to handle information exchange between server and client
    private bool exchanging = false;
    private bool exchangeStopRequested = false;
    private string lastPacket = null;
    private string errorStatus = null;
    private string successStatus = null;

    // Update is called once per frame
    public void Update() {
        /*if(errorStatus != null){
            Debug.Log(errorStatus);
            errorStatus = null;
        }
        if (successStatus != null){
            Debug.Log(successStatus);
            successStatus = null;
        }*/
    }
    public void ExchangePackets()
    {
        try
        {
            if (clientMessage == "Ping")
            {
                writer.Write("Ping");
            }
            else if (clientMessage == "Start Game")
            {
                writer.Write("Start Game");
            }
        }
        catch (Exception e)
        {
            Debug.Log(e.ToString());
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
            reader.Close(); stream = null;
            exchangeThread = null;
        }
#else
        if(exchangeTask != null) {
            exchangeTask.Wait();
            socket.Dispose();
            writer.Dispose();
            reader.Dispose();            socket = null;
            exchangeTask = null;
        }
#endif
        writer = null;
        reader = null;
    }
    public void OnDestroy()
        //end connection and dispose of socket if object this script is attached to is
        //destroyed
    {
        StopExchange();
    }
}