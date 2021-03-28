using System.IO;
using System.Text;
using System.Threading;
using UnityEngine;
using System;
#if !UNITY_EDITOR
using System.Threading.Tasks;
#endif
public class sendTcpIpToServer : MonoBehaviour
{
    public string sendCalibrationEventName;

    private string messageToSend;

    public String ipTCPHost;
    public String portTCPHost;

    private string errorStatus = null;
    private bool exchangeStopRequested = false;

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
    private void SendToServer(EventParam calibrationData)
    {
        messageToSend = calibrationData.tcpIPMessage + "X";//end signifies that its the last string and we cancel communication
        Connect(ipTCPHost, portTCPHost);
        ExchangePackets();
    }
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

        }
        catch (Exception e)
        {
            errorStatus = e.ToString();
        }
#endif
    }

    public void ExchangePackets()
    {

        //while (!exchangeStopRequested)
        //{
        //    if (writer == null || reader == null) continue;

        //messageToSend = "X";
        writer.Write(messageToSend); //to signify to server that we want new information
        string received = null;

#if UNITY_EDITOR
        // read into buffer until we receive a "\n"
        //byte[] bytes = new byte[client.SendBufferSize];
        //int recv = 0;
        //recv = stream.Read(bytes, 0, client.SendBufferSize);
        //received = Encoding.UTF8.GetString(bytes, 0, recv);
#else
        //received = reader.ReadLine();
#endif
        //Debug.Log(received);
        //Thread.Sleep(300);
        //exchangeStopRequested = true;
        //break;



        //}
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


    public void OnDestroy()
    {
        StopExchange();
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
