using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Net;
using UnityEngine;
using System.Collections;
public class ServerSocket {

    public Socket socket;
    public int portNum = NetCommon.PortNum;
    public string IP = NetCommon.IP;

    public Dictionary<int,Client> clients;

    public ServerSocket()
    {
        clients = new Dictionary<int, Client>();
        IPAddress ip = IPAddress.Any;
        socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        socket.Bind(new IPEndPoint(ip, portNum));
    }

    public void StartListen()
    {
        try
        {
           Debug.Log("Listening started port"+portNum.ToString() +"protocol type:"+ ProtocolType.Tcp.ToString());
            
            socket.Listen(128);//排队等待的数量
            socket.BeginAccept(AcceptCallback, socket);//when a client connects we can accept them by AcceptCallback
        }
        catch (Exception ex)
        {
            throw new Exception("listerning error" + ex);
        }
    }

    void AcceptCallback(IAsyncResult ar)
    {
        lock (clients)
        {

            try
            {
                Socket acceptSocket = socket.EndAccept(ar);//return a Socket object to handle communication with the remote host.
                IPAddress clientIP = (acceptSocket.RemoteEndPoint as IPEndPoint).Address;
                int clientPort = (acceptSocket.RemoteEndPoint as IPEndPoint).Port;

                Client client = new Client();

                client.acceptSocket = acceptSocket;
                client.clientIP = clientIP;
                client.clientPort = clientPort;
                client.connectionID = clients.Count;

                Debug.Log("" +"client Id: " + clients.Count.ToString() + "connected");
                clients.Add(clients.Count, client);

                socket.BeginAccept(AcceptCallback, socket);//再次开始监听。。这条语句很重要不要落了，不然只能connect一个player
            }
            catch (Exception ex)
            {
                throw new Exception("Socket Accept error" + ex);
            }

        }
    }

    public byte[] ReceiveData(Socket client_socket)//从client_socket接收数据
    {
        try
        {
            byte[] bytes = null;
            int len = client_socket.Available;//获取已经从网络接收且可供读取的数据量

            if (len > 0)
            {
                bytes = new byte[len];
                client_socket.Receive(bytes);
            }
            return bytes;
        }catch(Exception ex)
        {
            throw new Exception("msg sending error" + ex);
        }  
    }
}

