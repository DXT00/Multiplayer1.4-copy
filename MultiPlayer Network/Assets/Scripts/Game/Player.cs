using UnityEngine;
using System;
using System.Collections.Generic;

public class Player
{

    public ViewPlayer viewPlayer;
    ViewManager viewManager;
    NetworkManager networkManager;
    public int connectID;
    //int playerID;
    public int maxExe_FrameCount = -1;//已经执行的最大的帧号
    int maxSend_FrameCount = -1;//已经发送的最大的帧号
    int sending_t = 0;
    int max_sendingNum = 0;
   
    Queue<List<SyncFrame>> executeQue;



    public Player(int connectID)
    {
        this.connectID = connectID;
      
        executeQue = new Queue<List<SyncFrame>>();//从server中获取 replyframes 并存储在executeList中

    }


    public void bind_viewManager(ViewManager viewManager)
    {
        this.viewManager = viewManager;
    }
    public void bind_networkManager(NetworkManager networkManager)
    {
        this.networkManager = networkManager;
    }

    public void bind_view_player(ViewPlayer viewPlayer)
    {
        this.viewPlayer = viewPlayer;
        this.viewPlayer.connectID = connectID;
    }



    public void Tick()
    {
        //viewManager.viewPlayers_y_update();
        if (maxSend_FrameCount < 0)//一帧都没发送也没执行
            get_local_input_send();

        execute_frames();
   
        Debug.Log("framecount = " + maxExe_FrameCount.ToString());//已经执行了第maxRecv_FrameCount帧


    }

    public void get_replayframes(Dictionary<int, List<SyncFrame>> replyframes)//frameCount到 这一帧中多个SyncFrame的映射
    {
        Debug.Log("replyframes size = " + replyframes.Count.ToString());

        foreach (KeyValuePair<int, List<SyncFrame>> replyframe in replyframes)
        {
            executeQue.Enqueue(replyframe.Value);
            Tool.printExecueQue_MsgList("ExecueQue", replyframe.Value);
        }
    }

    public Frame EmptyFrame(int frame_count)
    {
        return new Frame(connectID, new SyncFrame(frame_count));
    }
    public void execute_frames()
    {
        Debug.Log("que size = " + executeQue.Count.ToString());
        //sending_t++;
        if (executeQue.Count != 0)
        {
            max_sendingNum = executeQue.Count;
        }
        while (executeQue.Count != 0)
        {
            List<SyncFrame> replyframe = executeQue.Peek();
            executeQue.Dequeue();

            int frame_count = replyframe[0].frame_count;
            maxExe_FrameCount = Math.Max(maxExe_FrameCount, frame_count);

          

            //执行第maxRecv_FrameCount帧
            viewManager.execute_frames(replyframe);//一般第一帧是空帧，第二帧才是有消息的帧

            if (executeQue.Count == 0&&maxSend_FrameCount<maxExe_FrameCount)
            {
                maxSend_FrameCount = maxExe_FrameCount;
            }
        }
         if (max_sendingNum > 0)//控制发送的速率// if(sending_t>5)
       
        {
            
            get_local_input_send();
            //sending_t = 0;
            max_sendingNum--;
        }


    }

    public void get_local_input_send()//把所有这一帧的输入收集起来并发送给server
    {
        maxSend_FrameCount++;
        SyncFrame syncFrame = new SyncFrame(maxSend_FrameCount);//发送第maxSend_FrameCount帧
        syncFrame.msg_list = viewPlayer.get_local_input();

        Tool.printMsgList(" 还可以发送" + max_sendingNum + "帧"+" ， localInput + 正在发送 framecount = " + syncFrame.frame_count, syncFrame.msg_list);



        NetworkMsg frame_msg = new Frame(connectID, syncFrame);//给server 发送第0帧
        //networkMgr 发送
        networkManager.SendData(frame_msg);
      
    }

    public void send_askframe()
    {


        //List<int> frames = new List<int>();//List只有1帧
        //frames.Add(TotalFrame.Count);//要server的第0帧

        //AskFrame askFrame = new AskFrame(connectID, frames);
        //networkManager.SendData(askFrame);
    }



}

