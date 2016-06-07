using UnityEngine;
using System.Collections;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System;
using System.Threading;
using System.Linq;

[ExecuteInEditMode()]
public class SimpleTCPServer : MonoBehaviour
{

    TcpListener Listener;
    public int Port = 1234;
    Socket Server;
    byte[] result;
    Socket connectedSocket;
    bool connected = false;
    string textToSend = "";
    string ReceivedText = "";
    bool received = false;
    [Compact]
    public Rect Window = new Rect(500, 100, 300, 300);
    public int WindowID = 1;
    public string Status = "";
    void Awake()
    {
        //UdpClient Server = new UdpClient(Port);
        Listener = new TcpListener(IPAddress.Any, Port);


        //Socket Server = Listener.AcceptSocket();
        //Socket Server = Listener.AcceptSocket();

        IPEndPoint RemoteIpEndPoint = new IPEndPoint(IPAddress.Any, 0);
        int count = 0;
        result = new byte[90000];
    }

    void StartAccepting(IAsyncResult acceptResult)
    {
        connectedSocket = Listener.EndAcceptSocket(acceptResult);
        connected = true;
        Interpolations.MyLog("Listener connected.");
        Status = "Listener connected.";
        connectedSocket.BeginReceive(result, 0, result.Length, SocketFlags.None, new AsyncCallback(ProcessReceivedPackets), null);
    }
    void ProcessReceivedPackets(IAsyncResult acceptResult)
    {
        int NoOfBytes = connectedSocket.EndReceive(acceptResult);
        var bytes = result.Take(NoOfBytes).ToArray();
        ReceivedText = System.Text.Encoding.UTF8.GetString(bytes);
        received = true;

        connectedSocket.BeginReceive(result, 0, result.Length, SocketFlags.None, new AsyncCallback(ProcessReceivedPackets), null);
    }
    void OnGUI()
    {
        Window = GUILayout.Window(WindowID, Window, ShowWindow, "Server");

    }

    private void ShowWindow(int WindowID)
    {
        if (WindowID == this.WindowID)
        {
            try
            {
                if (!connected)
                {
                    GUILayout.Label("Server IP: " + "");
                    GUILayout.BeginHorizontal();
                    GUILayout.Label("Port:");
                    Port = int.Parse(GUILayout.TextField(Port + ""));
                    GUILayout.EndHorizontal();
                    if (GUILayout.Button("Start listening"))
                    {
                        //Thread t = new Thread(new ThreadStart(StartListener));
                        //t.Start();

                        Listener.Start();
                        Listener.BeginAcceptSocket(new AsyncCallback(StartAccepting), null);
                        Interpolations.MyLog("Listener started.");
                        Status = "Listener started.";
                    }
                }
                if (received)
                {
                    GUILayout.Label("Message:\n" + ReceivedText);
                }
            }
            catch (Exception e)
            {
                Debug.LogError(e);
                Status = e.Message;
            }
            GUIStyle statusStyle = new GUIStyle(GUI.skin.label);
            statusStyle.fontStyle = FontStyle.BoldAndItalic;
            GUILayout.Label(Status);
            GUI.DragWindow();
        }
    }

}
