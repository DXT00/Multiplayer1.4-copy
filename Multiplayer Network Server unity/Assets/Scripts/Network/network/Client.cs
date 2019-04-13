using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Net;
using System.Net.Sockets;

public class Client
{
    public Socket acceptSocket;
    public IPAddress clientIP;
    public int clientPort;
    public int connectionID;

    List<byte> ReceiveCache;
    Queue<byte[]> SendCache;

   
    public Client()
    {
        
        SendCache = new Queue<byte[]>();
       
    }
    //这个data可能是几个msg的集合，需要递归decode
    public void Receive_HandleData(byte[] data)
    {
        if (data != null)
        {
            ReceiveCache = new List<byte>(data);
            lock (Serialization.SerializationLock)
            {
                DecodData();
            }

        }

    }

    public void SendData()
    {
        byte[] bytes;
        if (SendCache.Count > 0)
        {
            bytes = SendCache.Dequeue();
            if (bytes != null)
            {
                acceptSocket.Send(bytes);
                SendData();
            }

        }
    }
    public void AddSendCache(NetworkMsg msg)
    {
        byte[] bytes = Serialization.SerializeData(msg,msg);
        if (bytes != null)
        {
            bytes = NetCommon.Encode(bytes);
            SendCache.Enqueue(bytes);
        }
      
    }

    void DecodData()
    {
        byte[] bytes = NetCommon.Decode(ref ReceiveCache);
        if (bytes == null) return;

        p_AllMsg p_AllMsg = new p_AllMsg();
        p_AllMsg = Serialization.DeserializeData(bytes);
        NetCommon.HandleData(connectionID, p_AllMsg);
        DecodData();



    }
}

