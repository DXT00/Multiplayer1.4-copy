using System;
using System.IO;

using ProtoBuf;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Serialization
{
    public static p_AllMsg p_AllMsg;



    public static byte[] Serialize(BaseProtocol baseProtocol, NetworkMsg networkMsg, 
       StartGame startGame = null, Frame frame = null, SyncFrame syncFrame = null,AskFrame askFrame =null
        )//在这可以继续添加要发送的Msg
    {
        p_AllMsg = new p_AllMsg();
        if (baseProtocol != null)
            Buffer_BaseProtocol(baseProtocol);
        if (networkMsg != null)
            Buffer_NetworkMsg(networkMsg);
   
         if (networkMsg.type == (int)CmdType.START)
        {
            startGame = networkMsg as StartGame;
            Buffer_StartGame(startGame);
        }
         if (networkMsg.type == (int)CmdType.FRAME)
        {
            frame = networkMsg as Frame;
            Buffer_Frame(frame);
        }

         if (networkMsg.type == (int)CmdType.SYNC_FRAME)
        {
        
            Buffer_SyncFrame(syncFrame);
        }
         if(networkMsg.type == (int)CmdType.ASKFRAME)
        {
            askFrame = networkMsg as AskFrame;
            Buffer_AskFrame(askFrame);
        }
        return Serialize<p_AllMsg>(p_AllMsg);
    }

    private static void Buffer_AskFrame(AskFrame askFrame)
    {
      // p_AllMsg.AskFrame.frame = askFrame.frames;
        p_AllMsg.AskFrame.type = askFrame.type;
        foreach(int val in askFrame.frames)
        {
            p_AllMsg.AskFrame.frame.Add(val);
        }

    }

    public static void Buffer_BaseProtocol(BaseProtocol baseProtocol)
    {
        p_AllMsg.BaseProtocol.seq = baseProtocol.get_seq();
        p_AllMsg.BaseProtocol.cmd = baseProtocol.get_cmd();
        p_AllMsg.BaseProtocol.uid = baseProtocol.get_uid();
        p_AllMsg.BaseProtocol.ack = baseProtocol.get_ack();
        p_AllMsg.BaseProtocol.flag = baseProtocol.get_flag();

    }
    public static void Buffer_NetworkMsg(NetworkMsg networkMsg)
    {
        p_AllMsg.NetworkMsg.player_id = networkMsg.player_id;
        p_AllMsg.NetworkMsg.type = networkMsg.type;
    }
 
    
    public static void Buffer_StartGame(StartGame startGame)
    {
        p_AllMsg.StartGame.player_id = startGame.player_id;
        p_AllMsg.StartGame.type = startGame.type;

    }

    public static void Buffer_Frame(Frame frame)
    {
        p_AllMsg.Frame = new p_AllMsg.p_Frame();
        p_AllMsg.Frame.syncFrame = new p_AllMsg.p_SyncFrame();
        p_AllMsg.Frame.player_id = frame.player_id;
        p_AllMsg.Frame.syncFrame.frame_count = frame.syncFrame.get_frame_count();
        
        p_AllMsg.Frame.syncFrame.msg_list = Buffer_SyncFrame_msg_list(frame.syncFrame.get_msg());

    }



    public static void Buffer_SyncFrame(SyncFrame syncFrame)
    {
        p_AllMsg.SyncFrameMsg.frame_count = syncFrame.get_frame_count();
        if (syncFrame.msg_list != null)
            p_AllMsg.SyncFrameMsg.msg_list = Buffer_SyncFrame_msg_list(syncFrame.get_msg());
        else
            p_AllMsg.SyncFrameMsg.msg_list = null;
    }

    public static List<p_AllMsg.p_CustomSyncMsg> Buffer_SyncFrame_msg_list(List<CustomSyncMsg> msg_list)
    {
        List<p_AllMsg.p_CustomSyncMsg> p_msg_list = new List<p_AllMsg.p_CustomSyncMsg>();
        if (msg_list == null) return null;
        foreach (CustomSyncMsg msg in msg_list)
        {

            //if (msg.msg_type == (int)RequestType.ENTERAREA)
            //{
            //    EnterAreaMessage enterArea = msg as EnterAreaMessage;
            //    p_AllMsg.p_EnterAreaMessage p_msg = new p_AllMsg.p_EnterAreaMessage();
            //    p_msg.id = enterArea.id;
            //    p_msg.health = enterArea.health; ;
            //    p_msg.position_x = enterArea.position.x;
            //    p_msg.position_y = enterArea.position.y;
            //    p_msg.position_z = enterArea.position.z;

            //    p_msg.direction_x = enterArea.direction.x;
            //    p_msg.direction_y = enterArea.direction.y;
            //    p_msg.direction_z = enterArea.direction.z;

            //    p_msg.rotation_x = enterArea.rotation.x;
            //    p_msg.rotation_y = enterArea.rotation.y;

            //    p_msg.msg_type = enterArea.msg_type;
            //    p_msg.player_id = enterArea.player_id;
            //    p_msg.area_id = enterArea.area_id;
            //    p_msg_list.Add(p_msg);
            //}
            if (msg.msg_type == (int)RequestType.INPUT)
            {
                InputMessage input = msg as InputMessage;
                p_AllMsg.p_InputMessage p_msg = new p_AllMsg.p_InputMessage();
                p_msg.id = input.id;
                p_msg.moving_x = input.moving.x;
                p_msg.moving_y = input.moving.y;
                p_msg.moving_z = input.moving.z;

                p_msg.msg_type = input.msg_type;
                p_msg.player_id = input.player_id;
                p_msg.area_id = input.area_id;

                p_msg_list.Add(p_msg);

            }
            //if (msg.msg_type == (int)RequestType.LEAVEAREA)
            //{
            //    LeaveAreaMessage leaveArea = msg as LeaveAreaMessage;
            //    p_AllMsg.p_LeaveAreaMessage p_msg = new p_AllMsg.p_LeaveAreaMessage();

            //    p_msg.id = leaveArea.id;
            //    p_msg.msg_type = leaveArea.msg_type;
            //    p_msg.player_id = leaveArea.player_id;
            //    p_msg.area_id = leaveArea.area_id;
            //    p_msg_list.Add(p_msg);

            //}
            if (msg.msg_type == (int)RequestType.ROTATE)
            {
                RotateMessage rotate = msg as RotateMessage;
                p_AllMsg.p_RotateMessage p_msg = new p_AllMsg.p_RotateMessage();

                p_msg.id = rotate.id;
                p_msg.delta_x = rotate.delta.x;
                p_msg.delta_y = rotate.delta.y;
                p_msg.msg_type = rotate.msg_type;
                p_msg.player_id = rotate.player_id;
                p_msg.area_id = rotate.area_id;

                p_msg_list.Add(p_msg);

            }

            if (msg.msg_type == (int)RequestType.SHOOT)
            {
                ShootMessage shoot = msg as ShootMessage;
                p_AllMsg.p_ShootMessage p_msg = new p_AllMsg.p_ShootMessage();


                p_msg.direction_x = shoot.direction_x;
                p_msg.direction_y = shoot.direction_y;
                p_msg.direction_z = shoot.direction_z;

                p_msg.origin_x = shoot.origin_x;
                p_msg.origin_y = shoot.origin_y;
                p_msg.origin_z = shoot.origin_z;

                p_msg.msg_type = shoot.msg_type;
                p_msg.player_id = shoot.player_id;
                p_msg_list.Add(p_msg);

            }
            //if (msg.msg_type == (int)RequestType.SPAWN)
            //{
            //    SpawnMessage spawn = msg as SpawnMessage;
            //    p_AllMsg.p_SpawnMessage p_msg = new p_AllMsg.p_SpawnMessage();

            //    p_msg.id = spawn.id;
            //    p_msg.position_x = spawn.position.x;
            //    p_msg.position_y = spawn.position.y;
            //    p_msg.position_z = spawn.position.z;

            //    p_msg.msg_type = spawn.msg_type;
            //    p_msg.player_id = spawn.player_id;
            //    p_msg.area_id = spawn.area_id;

            //    p_msg_list.Add(p_msg);

            //}
        }

        return p_msg_list;
    }

    public static p_AllMsg DeserializeData(byte[] data)
    {

        using (MemoryStream stream = new MemoryStream(data, 0, data.Length))
        {
            // int len = ProtoReader.DirectReadVarintInt32(stream);
            stream.Write(data, 0, data.Length);
            stream.Position = 0;
            p_AllMsg = Serializer.Deserialize<p_AllMsg>(stream);


        }
        return p_AllMsg;

    }
    public static Byte[] Serialize<T>(T obj)
    {
        using (MemoryStream memory = new MemoryStream())
        {
            Serializer.Serialize(memory, obj);
            return memory.ToArray();
        }
    }

}



