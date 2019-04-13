using System;
using System.Collections.Generic;
using System.IO;

using ProtoBuf;


public class Serialization
{
    public static object SerializationLock = new object();

    static p_AllMsg p_AllMsg;
    public static byte[] SerializeData(BaseProtocol baseProtocol, NetworkMsg networkMsg, ReplyGetRooms replyGetRooms = null, ReplyJoin replyJoin = null,
       ReplyAskFrame replyAskFrame = null,ReplyID replyID=null, ReplyStart replyStart = null)
    {
        p_AllMsg = new p_AllMsg();
        if (baseProtocol != null)
            Buffer_BaseProtocol(baseProtocol);
        if (networkMsg != null)
            Buffer_NetworkMsg(networkMsg);
        if (networkMsg.type == (int)CmdType.REPLYGETROOMS)
        {
            replyGetRooms = networkMsg as ReplyGetRooms;
            Buffer_ReplyGetRooms(replyGetRooms);
        }
        else if (networkMsg.type == (int)CmdType.REPLYJOIN)
        {
            replyJoin = networkMsg as ReplyJoin;
            Buffer_ReplyJoin(replyJoin);
        }
        else if (networkMsg.type == (int)CmdType.REPLYASKFRAME)
        {
            replyAskFrame = networkMsg as ReplyAskFrame;
            Buffer_ReplyAskFrame(replyAskFrame);
        }
        else if (networkMsg.type == (int)CmdType.REPLYSTART)
        {
            replyStart = networkMsg as ReplyStart;
            BufferReplyStart(replyStart);
        }
        else if (networkMsg.type == (int)CmdType.REPLYID)
        {
            replyID = networkMsg as ReplyID;
            Buffer_ReplyID(replyID);

        }
  
        return Serialize<p_AllMsg>(p_AllMsg);
    }

    public static void Buffer_ReplyID(ReplyID replyID)
    {

        p_AllMsg.ReplyID.your_connectID = replyID.your_connectID;
        p_AllMsg.ReplyID.type = replyID.type;

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

    public static void Buffer_ReplyGetRooms(ReplyGetRooms replyGetRooms)
    {
        p_AllMsg.ReplyGetRooms.rooms = replyGetRooms.rooms;
        p_AllMsg.ReplyGetRooms.type = replyGetRooms.type;

    }
    public static void Buffer_ReplyJoin(ReplyJoin replyJoin)
    {
        p_AllMsg.ReplyJoin.real_player_id = replyJoin.real_player_id;
        p_AllMsg.ReplyJoin.player_names = replyJoin.player_names;
        p_AllMsg.ReplyJoin.type = replyJoin.type;


    }
    public static void Buffer_ReplyAskFrame(ReplyAskFrame replyAskFrame)
    {
        p_AllMsg.ReplyAskFrame.type = replyAskFrame.type;
     
        p_AllMsg.ReplyAskFrame.replyFrames = new Dictionary<int, List<p_AllMsg.p_Frame>>();
        foreach (KeyValuePair<int, List<Frame>> replyframe in replyAskFrame.replyFrames)
        {
            int frame_count = replyframe.Key;
            
            if (!p_AllMsg.ReplyAskFrame.replyFrames.ContainsKey(frame_count))
            {

                List<p_AllMsg.p_Frame> p_frame_list = new List<p_AllMsg.p_Frame>();

                foreach (Frame frame in replyframe.Value)
                {
                    
                    SyncFrame syncFrame = frame.syncFrame;
                    List<p_AllMsg.p_CustomSyncMsg> p_msg_list = Buffer_SyncFrame_msg_list(syncFrame.msg_list);

                    p_AllMsg.p_SyncFrame p_syncFrame = new p_AllMsg.p_SyncFrame();
                    p_syncFrame.frame_count = syncFrame.frame_count;
                    p_syncFrame.msg_list = p_msg_list;


                    p_AllMsg.p_Frame p_frame = new p_AllMsg.p_Frame();
                    p_frame.player_id = frame.player_id;

                    p_frame.syncFrame = new p_AllMsg.p_SyncFrame();
                    p_frame.syncFrame=p_syncFrame;

                    p_frame_list.Add(p_frame);


                    
                }
                p_AllMsg.ReplyAskFrame.replyFrames.Add(frame_count, p_frame_list);
            }
        }
    }

   public static void BufferReplyStart(ReplyStart replyStart)
    {
        p_AllMsg.ReplyStart.start = replyStart.start;
        p_AllMsg.ReplyStart.type = replyStart.type;


    }
    
    public static List<p_AllMsg.p_CustomSyncMsg> Buffer_SyncFrame_msg_list(List<CustomSyncMsg> msg_list)
    {
        List<p_AllMsg.p_CustomSyncMsg> p_msg_list = new List<p_AllMsg.p_CustomSyncMsg>();
        foreach (CustomSyncMsg msg in msg_list)
        {

            if (msg.msg_type == (int)RequestType.ENTERAREA)
            {
                EnterAreaMessage enterArea = msg as EnterAreaMessage;
                p_AllMsg.p_EnterAreaMessage p_msg = new p_AllMsg.p_EnterAreaMessage();
                p_msg.id = enterArea.id;
                p_msg.health = enterArea.health; ;
                p_msg.position_x = enterArea.position.x;
                p_msg.position_y = enterArea.position.y;
                p_msg.position_z = enterArea.position.z;

                p_msg.direction_x = enterArea.direction.x;
                p_msg.direction_y = enterArea.direction.y;
                p_msg.direction_z = enterArea.direction.z;

                p_msg.rotation_x = enterArea.rotation.x;
                p_msg.rotation_y = enterArea.rotation.y;

                p_msg.msg_type = enterArea.msg_type;
                p_msg.player_id = enterArea.player_id;
                p_msg.area_id = enterArea.area_id;
                p_msg_list.Add(p_msg);
            }
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
            if (msg.msg_type == (int)RequestType.LEAVEAREA)
            {
                LeaveAreaMessage leaveArea = msg as LeaveAreaMessage;
                p_AllMsg.p_LeaveAreaMessage p_msg = new p_AllMsg.p_LeaveAreaMessage();

                p_msg.id = leaveArea.id;

                p_msg.msg_type = leaveArea.msg_type;
                p_msg.player_id = leaveArea.player_id;
                p_msg.area_id = leaveArea.area_id;
                p_msg_list.Add(p_msg);

            }
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
            if (msg.msg_type == (int)RequestType.SPAWN)
            {
                SpawnMessage spawn = msg as SpawnMessage;
                p_AllMsg.p_SpawnMessage p_msg = new p_AllMsg.p_SpawnMessage();

                p_msg.id = spawn.id;
                p_msg.position_x = spawn.position.x;
                p_msg.position_y = spawn.position.y;
                p_msg.position_z = spawn.position.z;

                p_msg.msg_type = spawn.msg_type;
                p_msg.player_id = spawn.player_id;
                p_msg.area_id = spawn.area_id;
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
                p_msg.player_id = shoot.player_id;
                p_msg.msg_type = shoot.msg_type;

                p_msg_list.Add(p_msg);

            }
        }

        return p_msg_list;
    }





    public static p_AllMsg DeserializeData(byte[] data)
    {
        using (MemoryStream stream = new MemoryStream(data, 0, data.Length))
        {
            stream.Write(data, 0, data.Length);
            stream.Position = 0;
            try
            {
                p_AllMsg = Serializer.Deserialize<p_AllMsg>(stream);
            }
            catch (Exception ex)
            {
                throw new Exception("DeserializeData error" + ex);
            }


        }
        return p_AllMsg;
    }

    public static Byte[] Serialize<T>(T obj)
    {
        using (MemoryStream memory = new MemoryStream())
        {
            //bug: 集合已修改，可能无法执行枚举操作
            //可能是由于收发同时多线程访问冲突导致
            Serializer.Serialize(memory, obj);
            return memory.ToArray();
        }
    }
}