using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Net;
using UnityEngine;
public class TCPSocket
{

    static Socket connectingSocket;
    static readonly string IP_ADDRESS = "127.0.0.1";// "192.168.137.1";
    static readonly short Port = 5000;

    public static bool Connect()
    {
        Debug.Log("Trying to connect to the server.");
        connectingSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        while (!connectingSocket.Connected)
        {
            try
            {
                connectingSocket.Connect(new IPEndPoint(IPAddress.Parse(IP_ADDRESS), Port));
            }
            catch
            {
                Debug.Log("Unable to connect to server.");
                return false;
            }
        }
        Debug.Log("Sucessfully connect to the server.");

        return true;
    }
    public static void SendData(byte[] data)
    {
        try
        {
            connectingSocket.Send(data, data.Length, SocketFlags.None);
        }
        catch (Exception ex)
        {
            throw new Exception("msg sending error" + ex);
        }

    }
    public static void SendCallback(IAsyncResult ar)
    {
        try
        {
            // Retrieve the socket from the state object.  
            Socket handler = (Socket)ar.AsyncState;

            // Complete sending the data to the remote device.  
            int bytesSent = handler.EndSend(ar);
            Console.WriteLine("Sent {0} bytes to client.", bytesSent);

            handler.Shutdown(SocketShutdown.Both);
            handler.Close();

        }
        catch (Exception e)
        {
            Console.WriteLine(e.ToString());
        }
    }
    public static byte[] ReceiveData()
    {

        byte[] bytes = null;
        int len = connectingSocket.Available;

        if (len > 0)
        {
            bytes = new byte[len];
            int receiveNumber = connectingSocket.Receive(bytes);

        }
        return bytes;
    }
}

