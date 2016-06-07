using UnityEngine;
using System.Collections;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System;

[ExecuteInEditMode()]
public class SimpleTCPClient : MonoBehaviour
{

    void Awake()
    {
        client = new TcpClient();
    }
    TcpClient client;
    public string ip = "127.0.0.1";
    public int Port = 1234;
    bool connected = false;
    string textToSend = "";
    [Compact]
    public Rect Window = new Rect(100, 100, 300, 300);
    public int WindowID = 2;
    public string Status = "";
    void OnGUI()
    {
        Window = GUILayout.Window(WindowID, Window, ShowWindow, "Client");

    }
    private void ShowWindow(int WindowID)
    {
        if (WindowID == this.WindowID)
        {
            try
            {
                if (!connected)
                {
                    GUILayout.BeginHorizontal();
                    GUILayout.Label("IP:");
                    ip = GUILayout.TextField(ip);
                    GUILayout.Label("Port:");
                    Port = int.Parse(GUILayout.TextField(Port + ""));
                    GUILayout.EndHorizontal();
                    if (GUILayout.Button("Connect"))
                    {
                        client.BeginConnect(ip, Port, new AsyncCallback(OnConnect), null);
                    }
                }
                if (connected)
                {
                    textToSend = GUILayout.TextField(textToSend);
                    if (GUILayout.Button("Send"))
                    {
                        Status = "Sending..";
                        SendTextMessage(textToSend);

                    }
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

    public void SendTextMessage(string message)
    {
        try
        {
            NetworkStream stream = client.GetStream();
            ASCIIEncoding asen = new ASCIIEncoding();
            byte[] buff = asen.GetBytes(textToSend);
            stream.BeginWrite(buff, 0, buff.Length, new AsyncCallback(OnWrite), null);
        }
        catch (Exception e)
        {
            Debug.LogError(e);
            Status = e.Message;
        }
    }

    private void OnWrite(IAsyncResult result)
    {
        client.GetStream().EndWrite(result);
        Status = "Message was sent.";
    }
    private void OnConnect(IAsyncResult result)
    {
        client.EndConnect(result);
        connected = true;
        Status = "Connected.";
    }

}
