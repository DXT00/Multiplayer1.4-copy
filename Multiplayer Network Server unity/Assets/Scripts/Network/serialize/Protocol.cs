using ProtoBuf;
using System.Collections.Generic;
using System.Collections;

[ProtoContract]
public class p_AllMsg
{
    //Server接收的类
    [ProtoMember(1)]
    public p_BaseProtocol BaseProtocol { get; set; }
    [ProtoMember(2)]
    public p_SyncFrame SyncFrameMsg { get; set; }
    [ProtoMember(3)]
    public p_NetworkMsg NetworkMsg { get; set; }
    [ProtoMember(4)]
    public p_GetRooms GetRooms { get; set; }
    [ProtoMember(5)]
    public p_Join Join { get; set; }
    [ProtoMember(6)]
    public p_AskFrame AskFrame { get; set; }
    [ProtoMember(7)]
    public p_StartGame StartGame { get; set; }
    [ProtoMember(8)]
    public p_Frame Frame { get; set; }

    [ProtoMember(9)]
    public p_ReplyGetRooms ReplyGetRooms { get; set; }
    [ProtoMember(10)]
    public p_ReplyJoin ReplyJoin { get; set; }
    [ProtoMember(11)]
    public p_ReplyAskFrame ReplyAskFrame { get; set; }
    [ProtoMember(12)]
    public p_ReplyStart ReplyStart { get; set; }
    [ProtoMember(13)]
    public p_Tick Tick { get; set; }

    [ProtoMember(14)]
    public p_ReplyAskChaseFrame ReplyAskChaseFrame { get; set; }
    [ProtoMember(15)]
    public p_AskChaseFrame AskChaseFranme { get; set; }
    [ProtoMember(16)]
    public p_EnterArea EnterArea { get; set; }
    [ProtoMember(17)]
    public p_LeaveArea LeaveArea { get; set; }
    [ProtoMember(18)]
    public p_ReplyID ReplyID { get; set; }

    public p_AllMsg()
    {

        BaseProtocol = new p_BaseProtocol();
        SyncFrameMsg = new p_SyncFrame();
        NetworkMsg = new p_NetworkMsg();
        GetRooms = new p_GetRooms();
        Join = new p_Join();
        AskFrame = new p_AskFrame();
        AskFrame.frame = new List<int>();
        StartGame = new p_StartGame();
        Frame = new p_Frame();
        Frame.syncFrame = new p_SyncFrame();
        Frame.syncFrame.msg_list = new List<p_CustomSyncMsg>();

        ReplyGetRooms = new p_ReplyGetRooms();
        ReplyJoin = new p_ReplyJoin();

        ReplyID = new p_ReplyID();


        ReplyAskFrame = new p_ReplyAskFrame();
        ReplyAskFrame.replyFrames = new Dictionary<int, List<p_Frame>>();

        ReplyAskChaseFrame = new p_ReplyAskChaseFrame();
        ReplyAskChaseFrame.areas_id = new List<int>();
        ReplyAskChaseFrame.areas_to_frames = new Dictionary<int, List<p_SyncFrame>>();

        AskChaseFranme = new p_AskChaseFrame();
        AskChaseFranme.areas_id = new List<int>();
        AskChaseFranme.area_to_frame = new Dictionary<int, List<int>>();

        EnterArea = new p_EnterArea();
        EnterArea.msg = new p_EnterAreaMessage();
        LeaveArea = new p_LeaveArea();
        LeaveArea.msg = new p_LeaveAreaMessage();


        Tick = new p_Tick();
        ReplyStart = new p_ReplyStart();
    }

    [ProtoContract]
    [ProtoInclude(6, typeof(p_NetworkMsg))]
    public class p_BaseProtocol
    {



        [ProtoMember(1)]
        public int seq { get; set; }
        [ProtoMember(2)]
        public int cmd { get; set; }
        [ProtoMember(3)]
        public int uid { get; set; }
        [ProtoMember(4)]
        public int ack { get; set; }
        [ProtoMember(5)]
        public int flag { get; set; }

    }

    [ProtoContract]
    [ProtoInclude(3, typeof(p_GetRooms))]
    [ProtoInclude(4, typeof(p_Join))]
    [ProtoInclude(5, typeof(p_AskFrame))]
    [ProtoInclude(6, typeof(p_StartGame))]
    [ProtoInclude(7, typeof(p_Frame))]

    [ProtoInclude(8, typeof(p_ReplyGetRooms))]
    [ProtoInclude(9, typeof(p_ReplyJoin))]
    [ProtoInclude(10, typeof(p_ReplyAskFrame))]
    [ProtoInclude(11, typeof(p_ReplyStart))]
    [ProtoInclude(12, typeof(p_Tick))]
    [ProtoInclude(13, typeof(p_ReplyAskChaseFrame))]
    [ProtoInclude(14, typeof(p_AskChaseFrame))]
    [ProtoInclude(15, typeof(p_EnterArea))]
    [ProtoInclude(16, typeof(p_LeaveArea))]
    [ProtoInclude(17, typeof(p_ReplyID))]
    public class p_NetworkMsg : p_BaseProtocol
    {
        [ProtoMember(1)]
        public int player_id { get; set; }
        [ProtoMember(2)]
        public int type { get; set; }


    }

    [ProtoContract]
    public class p_GetRooms : p_NetworkMsg
    {

    }

    [ProtoContract]
    public class p_Join : p_NetworkMsg
    {

        [ProtoMember(1)]
        public int room_id;
    }
    [ProtoContract]
    public class p_AskFrame : p_NetworkMsg
    {
        [ProtoMember(1)]
        public List<int> frame;
    }

    [ProtoContract]
    public class p_StartGame : p_NetworkMsg
    {

    }

    [ProtoContract]
    public class p_Frame : p_NetworkMsg
    {

        [ProtoMember(1)]
        public p_SyncFrame syncFrame;   //------------
    }



    [ProtoContract]
    public class p_SyncFrame
    {
        [ProtoMember(1)]
        public int frame_count;
        [ProtoMember(2)]
        public List<p_CustomSyncMsg> msg_list;
    }

    // Server发送的类
    [ProtoContract]
    public class p_ReplyGetRooms : p_NetworkMsg
    {
        [ProtoMember(1)]
        public List<int> rooms;

    }
    [ProtoContract]
    public class p_ReplyJoin : p_NetworkMsg
    {
        [ProtoMember(1)]
        public int real_player_id;
        [ProtoMember(2)]
        public List<string> player_names;

    }
    [ProtoContract]
    public class p_ReplyID : p_NetworkMsg
    {
        [ProtoMember(1)]
        public int your_connectID;
    }
    [ProtoContract]
    public class p_ReplyAskFrame : p_NetworkMsg
    {

        [ProtoMember(1)]
        public Dictionary<int, List<p_Frame>> replyFrames;

    }
    [ProtoContract]
    public class p_ReplyStart : p_NetworkMsg
    {
        [ProtoMember(1)]
        public bool start;
    }

    [ProtoContract]
    public class p_Tick : p_NetworkMsg
    {
        [ProtoMember(1)]
        public int frame_count;

    }

    [ProtoContract]
    public class p_ReplyAskChaseFrame : p_NetworkMsg
    {

        [ProtoMember(1)]
        public List<int> areas_id;
        [ProtoMember(2)]
        public Dictionary<int, List<p_SyncFrame>> areas_to_frames;

    }

    [ProtoContract]
    public class p_AskChaseFrame : p_NetworkMsg
    {

        [ProtoMember(1)]
        public List<int> areas_id;
        [ProtoMember(2)]
        public Dictionary<int, List<int>> area_to_frame;

    }

    [ProtoContract]
    public class p_EnterArea : p_NetworkMsg
    {
        [ProtoMember(1)]
        public p_EnterAreaMessage msg;

    }
    [ProtoContract]
    public class p_LeaveArea : p_NetworkMsg
    {
        [ProtoMember(1)]
        public p_LeaveAreaMessage msg;

    }

    [ProtoContract]
    [ProtoInclude(4, typeof(p_RotateMessage))]
    [ProtoInclude(5, typeof(p_SpawnMessage))]
    [ProtoInclude(6, typeof(p_InputMessage))]
    [ProtoInclude(7, typeof(p_EnterAreaMessage))]
    [ProtoInclude(8, typeof(p_LeaveAreaMessage))]
    [ProtoInclude(9, typeof(p_ShootMessage))]
    public class p_CustomSyncMsg
    {
        [ProtoMember(1)]
        public int player_id;
        [ProtoMember(2)]
        public int area_id;
        [ProtoMember(3)]
        public int msg_type;
        // Marshal.Sizeof(Custom)

    }
    [ProtoContract]
    public class p_RotateMessage : p_CustomSyncMsg
    {
        [ProtoMember(1)]
        public int id;


        [ProtoMember(2)]
        public float delta_x;
        [ProtoMember(3)]
        public float delta_y;

    }
    [ProtoContract]
    public class p_SpawnMessage : p_CustomSyncMsg
    {
        [ProtoMember(1)]
        public int id;



        [ProtoMember(2)]
        public float position_x;
        [ProtoMember(3)]
        public float position_y;
        [ProtoMember(4)]
        public float position_z;

    }
    [ProtoContract]
    public class p_InputMessage : p_CustomSyncMsg
    {
        [ProtoMember(1)]
        public int id;

        [ProtoMember(2)]
        public float moving_x;
        [ProtoMember(3)]
        public float moving_y;
        [ProtoMember(4)]
        public float moving_z;
    }
    [ProtoContract]
    public class p_EnterAreaMessage : p_CustomSyncMsg
    {
        [ProtoMember(1)]
        public int id;
        [ProtoMember(2)]
        public int health;


        [ProtoMember(3)]
        public float position_x;
        [ProtoMember(4)]
        public float position_y;
        [ProtoMember(5)]
        public float position_z;

        [ProtoMember(6)]
        public float direction_x;
        [ProtoMember(7)]
        public float direction_y;
        [ProtoMember(8)]
        public float direction_z;

        [ProtoMember(9)]
        public float rotation_x;
        [ProtoMember(10)]
        public float rotation_y;


    }
    [ProtoContract]
    public class p_LeaveAreaMessage : p_CustomSyncMsg
    {
        [ProtoMember(1)]
        public int id;


    }
    [ProtoContract]
    public class p_ShootMessage : p_CustomSyncMsg
    {
        [ProtoMember(1)]
        public float origin_x;
        [ProtoMember(2)]
        public float origin_y;
        [ProtoMember(3)]
        public float origin_z;
        [ProtoMember(4)]
        public float direction_x;
        [ProtoMember(5)]
        public float direction_y;
        [ProtoMember(6)]
        public float direction_z;

    }

}
