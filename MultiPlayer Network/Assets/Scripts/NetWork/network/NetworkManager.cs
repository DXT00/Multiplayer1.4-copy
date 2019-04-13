using System.Threading;
using System.IO;
using System.Collections.Generic;
using UnityEngine;

public class NetworkManager
{
    public static TCPSocket tcpSocket;
    public List<byte> cache = new List<byte>();
    public bool isReceiving = false;

    private bool isConnect = false;
    private Game game;
    private static p_AllMsg Server_msg;

    public void init(Game game)//register game
    {
        this.game = game;
    }

    public bool connect()//通过按键connect
    {

        if (TCPSocket.Connect())
        {
            isConnect = true;

        }
        return isConnect;
    }
    public bool get_isConnect()
    {
        return isConnect;
    }

    public void Receive_HandleData()
    {


        byte[] data = TCPSocket.ReceiveData();
        lock (cache)
        {
            if (data != null)
            {
                cache.AddRange(data);
                if (!isReceiving)
                {
                    isReceiving = true;
                    ReadData();
                }
            }
        }


    }
    void ReadData()
    {
        byte[] bytes = Decode(ref cache);
        if (bytes != null)
        {
            //Debug.Log("once decode at times : " + times + " bytes length is: " + bytes.Length);
            Server_msg = new p_AllMsg();
            Server_msg = Serialization.DeserializeData(bytes);
            //Debug.Log("Receive Server_msg.NetworkMsg.type Finished!!" + ((CmdType)Server_msg.NetworkMsg.type).ToString());
            HandleData(Server_msg);
            ReadData();
        }
        else
        {
            isReceiving = false;
        }
    }
    public p_AllMsg ReceiveData()
    {
        byte[] bytes = null;
        bytes = TCPSocket.ReceiveData();
        cache.AddRange(bytes);
        byte[] data = Decode(ref cache);
        p_AllMsg p_allmsg = null;
        if (bytes != null)
        {
            p_allmsg = new p_AllMsg();
            p_allmsg = Serialization.DeserializeData(data);

        }

        return p_allmsg;
    }

    public static List<CustomSyncMsg> extract_msg(List<p_AllMsg.p_CustomSyncMsg> pmsg_list)
    {
        
        List<CustomSyncMsg> customSyncMsgs = new List<CustomSyncMsg>();

        if (pmsg_list == null) return null;
        foreach (p_AllMsg.p_CustomSyncMsg pmsg in pmsg_list)
        {
            if (pmsg.msg_type == (int)RequestType.ROTATE)
            {
                p_AllMsg.p_RotateMessage pRotate = pmsg as p_AllMsg.p_RotateMessage;
                RotateMessage rotate_msg = new RotateMessage(pmsg.player_id, new Vector2(pRotate.delta_x, pRotate.delta_y));
                customSyncMsgs.Add(rotate_msg);
            }
            else if (pmsg.msg_type == (int)RequestType.SPAWN)
            {

            }
            else if (pmsg.msg_type == (int)RequestType.INPUT)
            {
                p_AllMsg.p_InputMessage pInput = pmsg as p_AllMsg.p_InputMessage;
                InputMessage input_msg = new InputMessage(pmsg.player_id, new Vector3(pInput.moving_x, pInput.moving_y, pInput.moving_z));
                customSyncMsgs.Add(input_msg);
            }
            else if(pmsg.msg_type == (int)RequestType.SHOOT)
            {
                p_AllMsg.p_ShootMessage pshoot = pmsg as p_AllMsg.p_ShootMessage;
                ShootMessage shoot_msg = new ShootMessage(pmsg.player_id, new Vector3(pshoot.origin_x, pshoot.origin_y, pshoot.origin_z), new Vector3(pshoot.direction_x, pshoot.direction_y, pshoot.direction_z));
                customSyncMsgs.Add(shoot_msg);

            }


        }



        return customSyncMsgs;
    }


    Dictionary<int, List<SyncFrame>> extract_replyFrames(Dictionary<int, List<p_AllMsg.p_Frame>> p_replyFrames)
    {
        Dictionary<int, List<SyncFrame>> replyFrames = new Dictionary<int, List<SyncFrame>>();


        foreach(KeyValuePair<int,List<p_AllMsg.p_Frame>>dpframe in p_replyFrames)
        {
           
            int frame_count = dpframe.Key;
            List<p_AllMsg.p_Frame> pframe_list = dpframe.Value;

            List<SyncFrame> syncFrames_list = new List<SyncFrame>();

            foreach(p_AllMsg.p_Frame pframe in pframe_list)
            {
                
               // p_AllMsg.p_SyncFrame psyncFrame = pframe.syncFrame;
                List<p_AllMsg.p_CustomSyncMsg> p_msg_list = pframe.syncFrame.msg_list;
                List<CustomSyncMsg> msg_list = extract_msg(p_msg_list);
               
                SyncFrame syncFrame = new SyncFrame(frame_count);
                syncFrame.frame_count = pframe.syncFrame.frame_count;
                syncFrame.msg_list = msg_list;
                syncFrames_list.Add(syncFrame);
            }

            replyFrames.Add(frame_count,syncFrames_list);
        }

        return replyFrames;


    }


    public void HandleData(p_AllMsg p_allmsg)
    {
        if (p_allmsg.NetworkMsg.type == (int)CmdType.REPLYJOIN)
        {
            List<string> ids = p_allmsg.ReplyJoin.player_names;
            int connectID = p_allmsg.ReplyJoin.real_player_id;
            game.on_reply_join(connectID, ids);
            Debug.Log("replyJoin connectID=" + connectID);
        }

        else if (p_allmsg.NetworkMsg.type == (int)CmdType.REPLYSTART)
        {
            bool isGameStart = p_allmsg.ReplyStart.start;
            game.on_reply_start(isGameStart);
        }
        else if(p_allmsg.NetworkMsg.type == (int)CmdType.REPLYASKFRAME)
        {

            Dictionary<int, List<SyncFrame>> replyframes = extract_replyFrames(p_allmsg.ReplyAskFrame.replyFrames);
            game.on_reply_frames(replyframes);


        }
        else if(p_allmsg.NetworkMsg.type == (int)CmdType.REPLYID)
        {
            int your_connectID = p_allmsg.ReplyID.your_connectID;
            
            game.on_reply_ID(your_connectID);
        }


    }
    public void SendData(NetworkMsg Msg)
    {
        byte[] bytes = null;
        bytes = Serialization.Serialize(Msg, Msg);
        bytes = Encode(bytes);
        TCPSocket.SendData(bytes);
    }


    //编码数据： 长度+内容
    //一个整形占4个子节
    public byte[] Encode(byte[] data)
    {
        byte[] result = new byte[data.Length + 4];

        //使用流将编码写二进制
        MemoryStream ms = new MemoryStream();
        BinaryWriter br = new BinaryWriter(ms);
        br.Write(data.Length);
        br.Write(data);
        //将流中的内容复制到数组中
        System.Buffer.BlockCopy(ms.ToArray(), 0, result, 0, (int)ms.Length);
        br.Close();
        ms.Close();
        return result;
    }
    public byte[] Decode(ref List<byte> cache)
    {
        //首先要获取长度，整形4个字节，如果字节数不足4个字节
        if (cache.Count < 4)
        {
            return null;
        }
        //读取数据
        MemoryStream ms = new MemoryStream(cache.ToArray());
        BinaryReader br = new BinaryReader(ms);
        int len = br.ReadInt32();//从当前流中读取 4 字节有符号整数，并使流的当前位置提升 4 个字节。
        //根据长度，判断内容是否传递完毕, ms.Length - ms.Position为剩余流长度
        if (ms.Length - ms.Position < len || len < 0)
        {
            return null;
        }
        //获取数据
        byte[] result = br.ReadBytes(len);
        //清空消息池
        cache.Clear();
        //讲剩余没处理的消息存入消息池
        cache.AddRange(br.ReadBytes((int)ms.Length - (int)ms.Position));
        return result;
    }
}

