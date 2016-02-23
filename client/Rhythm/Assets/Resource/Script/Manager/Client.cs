using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using MiniJSON;
using System;
using System.Text;
using System.Net;
using System.Net.Sockets;

public class Client
{

    private Socket _clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
    private byte[] _recieveBuffer = new byte[2048];
    public Command command;

    public Client(Command input)
    {
        command = input;
    }

    string host = "127.0.0.1";
    Int32 port = 12345;
    public void connectToServer()
    {
        SetupServer(host, port);
        SendData("login [1]");
    }
    
    public void connectToInputServer(string ipHost)
    {
        Debug.Log(ipHost+":"+port);
        SetupServer(ipHost, port);
        SendData("login [1]");
    }

    private void SetupServer(string ip, int port)
    {
        try
        {
            _clientSocket.Connect(new IPEndPoint(IPAddress.Parse(ip), port));
        }
        catch (SocketException ex)
        {
            Debug.Log(ex.Message);
        }

        _clientSocket.BeginReceive(_recieveBuffer, 0, _recieveBuffer.Length, SocketFlags.None, new AsyncCallback(ReceiveCallback), null);

    }

    private void ReceiveCallback(IAsyncResult AR)
    {
        //Check how much bytes are recieved and call EndRecieve to finalize handshake
        int recieved = _clientSocket.EndReceive(AR);

        if (recieved <= 0)
            return;

        //Copy the recieved data into new buffer , to avoid null bytes
        byte[] recData = new byte[recieved];
        Buffer.BlockCopy(_recieveBuffer, 0, recData, 0, recieved);

        //Process data here the way you want , all your bytes will be stored in recData
        string recvStr = Encoding.UTF8.GetString(recData, 0, recieved);
        // Debug.Log("GetLine:"+recvStr);
        // command.getSocket(recvStr);
        string[] strs = recvStr.Split('\n');
        foreach (string s in strs)
        {
            if( s != string.Empty )
                command.socketQueue.Add(s);
        }


        //Start receiving again
        _clientSocket.BeginReceive(_recieveBuffer, 0, _recieveBuffer.Length, SocketFlags.None, new AsyncCallback(ReceiveCallback), null);
    }

    public void SendData(string line)
    {
        byte[] data = System.Text.Encoding.UTF8.GetBytes(line + "\n");
        SocketAsyncEventArgs socketAsyncData = new SocketAsyncEventArgs();
        socketAsyncData.SetBuffer(data, 0, data.Length);
        bool res = _clientSocket.SendAsync(socketAsyncData);
        Debug.Log("Send line:" + line + " res:" + res);
    }
}