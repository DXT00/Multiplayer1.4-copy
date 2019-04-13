using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using System.Timers;

public class Room : MonoBehaviour
{

    public InGameGUI InGameGUI;
    GameObject InGameMgr;

    Timer timer;
    List<int> ConnectionID;
    Dictionary<int, int> PlayerID_Map_ConnectID;
    Dictionary<int, int> ConnectID_Map_PlayerID;

    int sendingRate =10;
    int boardcastFrameNum = 0;

    List<Player> players;
    List<List<Frame>> TotalFrames;//存储所有的帧

    Queue<Frame> FrameBuffer;//缓冲帧数famecount大于TotalFrame.Count的帧
    int PlayerID = 0;
    int maxplayer = 1000;
    NetworkManager networkManager;
    public bool isGameStart = false;
    public Room()
    {
        timer = new Timer();
        ConnectionID = new List<int>();
        players = new List<Player>();
        TotalFrames = new List<List<Frame>>();
        PlayerID_Map_ConnectID = new Dictionary<int, int>();
        ConnectID_Map_PlayerID = new Dictionary<int, int>();
        FrameBuffer = new Queue<Frame>();
       // InGameGUI = InGameMgr.GetComponent<InGameGUI>();

    }

    public void Tick()
    {
        Debug.Log("connectID size = " + ConnectionID.Count);
        InGameGUI.show_text("framecount = " + TotalFrames.Count.ToString());
        if (isGameStart && TotalFrames.Count < 45000)
        {
            //向每个player broadcast 第0帧

            //加入空帧 第1 帧


            if (TotalFrames.Count != 0 && TotalFrames.Count % sendingRate == 0)
            {

                BroadcastFrameUpdate(boardcastFrameNum, boardcastFrameNum + sendingRate);

                boardcastFrameNum += sendingRate;
            }

            AddEmptyFrames();
            RecordFastFrame();
            Debug.Log("framecount = " + TotalFrames.Count);

        }
    }


    public void BroadcastFrameUpdate(int startframe, int end)
    {
        for(int i = startframe; i < end; i++)
        {
            Tool.Print("Room Broadcasting frame :" + i.ToString());
            Dictionary<int,List<Frame>> updateFrames = new Dictionary<int, List<Frame>>();
            updateFrames.Add(i, TotalFrames[i]);
            ReplyAskFrame replyAskFrame = new ReplyAskFrame(updateFrames);
            roomBroadCast(replyAskFrame);
        }
    }

    public void RecordFastFrame()//从player.frameBuffer中取出已经发送过来的帧
    {
        foreach (Player player in players)
        {

            while (player.frameBuffer.Count > 0 && player.frameBuffer.Peek().syncFrame.frame_count < TotalFrames.Count)
            {
                TotalFrames[player.frameBuffer.Peek().syncFrame.frame_count].Add(player.frameBuffer.Peek());
                player.frameBuffer.Dequeue();
            }

            //SyncFrame syncFrame = new SyncFrame(TotalFrames.Count, 0);//area id 先设为0
            //Frame frame = new Frame(player.getConnectionID(), syncFrame);
            //frames_list.Add(frame);
        }
    }

    public void AddEmptyFrames()
    {
        List<Frame> frames_list = new List<Frame>();
        foreach (Player player in players)
        {
            SyncFrame syncFrame = new SyncFrame(TotalFrames.Count, 0);//area id 先设为0
            Frame frame = new Frame(player.getConnectionID(), syncFrame);
            frames_list.Add(frame);
        }
        TotalFrames.Add(frames_list);
    }


    public void RecordFrame(Frame frame, int clientID)
    {
      //  FrameBuffer.Enqueue(frame);
      //  while (FrameBuffer.Count > 0)
        //{
           // Frame curFrame = FrameBuffer.Peek();
            if (frame.syncFrame.frame_count < TotalFrames.Count)
            {
                TotalFrames[frame.syncFrame.frame_count].Add(frame);
              //  FrameBuffer.Dequeue();
            }
            else
            {
                int playerID = ConnectID_Map_PlayerID[clientID];
                players[playerID].frameBuffer.Enqueue(frame);
             //   FrameBuffer.Dequeue();
            }
                
       // }

        //try
        //{
        //    if (frame.syncFrame.frame_count < TotalFrames.Count)
        //        TotalFrames[frame.syncFrame.frame_count].Add(frame);
        //    else
        //    {

        //    }

        //}
        //catch
        //{
        //    Debug.Log("frame.syncFrame.frame_count=" + frame.syncFrame.frame_count.ToString() + "TotalFrames.Count" + TotalFrames.Count);
        //}
    }

    public Dictionary<int, List<Frame>> GetFrame(int startFrame)
    {
        Dictionary<int, List<Frame>> replyFrames = new Dictionary<int, List<Frame>>();

        for (int i = startFrame; i < TotalFrames.Count; i++)
        {
            List<Frame> frames = TotalFrames[i];
            replyFrames.Add(i, frames);


            foreach (Frame frame in frames)
            {
                Tool.printFrameMsgList("On Reply Frame", frame);

            }
        }

        return replyFrames;

    }

    public int JoinRoom(int clientID)
    {
        if (!ConnectionID.Contains(clientID) && players.Count() < maxplayer)
        {
            ConnectionID.Add(clientID);
            Player player = new Player(PlayerID, clientID);

            PlayerID_Map_ConnectID.Add(PlayerID, clientID);
            ConnectID_Map_PlayerID.Add(clientID, PlayerID);

            players.Add(player);
            PlayerID++;
            return PlayerID - 1;
        }
        else//已经包含该玩家connectID
            return -1;
    }

    public void StartGame()
    {
        isGameStart = true;

    }

    public List<string> GetPlayerID()
    {
        List<string> ids = new List<string>();
        foreach (Player player in players)
        {
            ids.Add(player.getID().ToString());
        }
        return ids;

    }


    public void roomBroadCast(NetworkMsg msg)
    {
        foreach (int clientID in ConnectionID)
        {
            networkManager.SendDataTo(clientID, msg);
        }
    }

    public void RegisterNetworkMgr(NetworkManager networkManager)
    {
        this.networkManager = networkManager;
    }

    public void bind_InGameMgr(GameObject InGameMgr)
    {
        this.InGameMgr = InGameMgr;
        InGameGUI = InGameMgr.GetComponent<InGameGUI>();
    }
}

