using System.Threading;
using UnityEngine;
public class NetworkManager
{
    ServerSocket serverSocket;
    Thread ReceiveThread;
    Thread SendThread;

    


    public NetworkManager()
    {
        serverSocket = new ServerSocket();
    }
    public void init()
    {
        serverSocket.StartListen();
       // ThreadInit();
    }


   


    public void ThreadInit()
    {
        ReceiveThread = new Thread(new ThreadStart(ReceiveData));//相当于new Thread(ReceiveData)
        ReceiveThread.Start();

        SendThread = new Thread(new ThreadStart(SendData));
        SendThread.Start();

    }

    public void ReceiveData()
    {

       
            lock(serverSocket.clients)
            {
                foreach(Client client in serverSocket.clients.Values)
                {
                    byte[] data=serverSocket.ReceiveData(client.acceptSocket);
                    client.Receive_HandleData(data);
                Debug.Log("client connectID" + client.connectionID);
                }

            }
        
    }

    public void SendData()
    {
        
            lock (serverSocket.clients)
            {
                foreach (Client client in serverSocket.clients.Values)
                {
                    client.SendData();
                }
            }
      
    }

    public void SendDataTo(int clientID,NetworkMsg msg)
    {
        if (msg != null)
        {
            Client client = serverSocket.clients[clientID];
            client.AddSendCache(msg);
        }
    }

}

