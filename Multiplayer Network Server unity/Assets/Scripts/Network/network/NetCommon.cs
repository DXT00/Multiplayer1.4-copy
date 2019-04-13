using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using System.IO;

public class NetCommon
{
    public static int PortNum = 5000;
    public static string IP = "127.0.0.1";
    public static GameServer gameServer;
    public static byte[] Encode(byte[] data)
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

    public static byte[] Decode(ref List<byte> cache)
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


    public static List<CustomSyncMsg> extract_msg(List<p_AllMsg.p_CustomSyncMsg> pmsg_list)
    {
        List<CustomSyncMsg> customSyncMsgs = new List<CustomSyncMsg>();

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
            else if (pmsg.msg_type == (int)RequestType.SHOOT)
            {
                p_AllMsg.p_ShootMessage pshoot = pmsg as p_AllMsg.p_ShootMessage;
                ShootMessage shoot_msg = new ShootMessage(pmsg.player_id, new Vector3(pshoot.origin_x, pshoot.origin_y, pshoot.origin_z), new Vector3(pshoot.direction_x, pshoot.direction_y, pshoot.direction_z));
                customSyncMsgs.Add(shoot_msg);

            }

        }



        return customSyncMsgs;
    }


    public static void HandleData(int clientID, p_AllMsg p_allmsg)
    {
        //p_AllMsg ReceiveMsg = new p_AllMsg();
        int msg_type = p_allmsg.NetworkMsg.type;
        int seq = p_allmsg.BaseProtocol.seq;
        //gameServer.hanldeSeq(client_id, seq);

        //Console.WriteLine("get client: " + client_id + "seq pkg : " + seq);
        if (msg_type == (int)CmdType.START)
        {
            Debug.Log("get start info\n");
            gameServer.OnGameStart(clientID);
        }
        else if (msg_type == (int)CmdType.FRAME)
        {
            Debug.Log("get frame info "+clientID.ToString()+"\n");
            int frame_count = p_allmsg.Frame.syncFrame.frame_count;
            int player_id = p_allmsg.Frame.player_id;
            List<CustomSyncMsg> msg_list = extract_msg(p_allmsg.Frame.syncFrame.msg_list);
            SyncFrame syncFrame = new SyncFrame(frame_count, 0);
            syncFrame.msg_list = msg_list;

            Frame frame = new Frame(player_id, syncFrame);
            gameServer.OnGetFrame(frame, clientID);
        }

        else if (msg_type == (int)CmdType.ASKFRAME) 
        {
            Debug.Log("get askframe info from"+clientID.ToString()+"\n");
            List<int> frames = new List<int>();
            foreach (int val in p_allmsg.AskFrame.frame)
            {
                frames.Add(val);
            }
            gameServer.OnAskFrame(frames, clientID);

        }
        else if (msg_type == (int)CmdType.JOIN)
        {
            Debug.Log("get join info, it is from :" + clientID);
            gameServer.OnPlayerJoin(clientID);
        }
        //else if (msg_type == (int)CmdType.ASKCHASEFRAME)
        //{
        //    //Console.WriteLine("get asked chase frame request");
        //    List<int> areas_id = p_allmsg.AskChaseFranme.areas_id;
        //    Dictionary<int, List<int>> areas_to_frame = p_allmsg.AskChaseFranme.area_to_frame;
        //    int player_Id = p_allmsg.AskChaseFranme.player_id;
        //    if (player_Id == -1)
        //    {
        //        areas_id = null;
        //        areas_to_frame = null;
        //    }
        //    //List<int> miss_frame = allMsg.AskFrame.frame_list;
        //    //int area_id = allMsg.AskFrame.area_id;
        //    gameServer.OnAskedChaseFrame(clientID, areas_id, areas_to_frame);
        //}
        //else if (msg_type == (int)CmdType.END)
        //{
        //    //Console.WriteLine("get End");
        //    gameServer.OnGetEnd(clientID);
        //}
    }

}

