using System.Collections;
using System.Collections.Generic;
using ProtoBuf;



public class NetworkMsg : BaseProtocol
{

    public int player_id;
    public int type;

}

//public class GetRooms : NetworkMsg
//{
//    // public CmdType type;
//    public GetRooms()
//    {
//        this.type = (int)CmdType.GETROOMS;
//    }
//}

public class Join : NetworkMsg
{
    //  public CmdType type;
    public int room_id;
    public Join(int player_id, int room_id)
    {
        this.player_id = player_id;
        this.room_id = room_id;
        this.type = (int)CmdType.JOIN;
    }
}

public class AskFrame : NetworkMsg
{
    // public CmdType type;
    public List<int> frame_list;
    public int area_id;//忽略Area即可！

    public AskFrame(int player_id, int area_id, List<int> frame_list)
    {
        this.type = (int)CmdType.ASKFRAME;
        this.player_id = player_id;
        this.frame_list = frame_list;
        this.area_id = area_id;
    }
}

public class StartGame : NetworkMsg
{
    //public CmdType type;
    public StartGame(int player_id)
    {
        this.player_id = player_id;
        this.type = (int)CmdType.START;

    }
}

public class Frame : NetworkMsg
{
    // public CmdType type;
    public SyncFrame syncFrame;
    public Frame(int player_id, SyncFrame syncFrame)
    {
        this.player_id = player_id;
        this.syncFrame = syncFrame;
        this.type = (int)CmdType.FRAME;
    }
}



public class ReplyGetRooms : NetworkMsg
{
    public List<int> rooms;

    public ReplyGetRooms(List<int> rooms)
    {
        this.rooms = rooms;
        this.type = (int)CmdType.REPLYGETROOMS;
    }
}

public class ReplyJoin : NetworkMsg
{
    public int real_player_id;
    public List<string> player_names;
    public ReplyJoin(int id, List<string> player_names)
    {
        real_player_id = id;
        this.player_names = player_names;
        this.type = (int)CmdType.REPLYJOIN;
    }
}


public class ReplyID : NetworkMsg
{
    public int your_connectID;
    public ReplyID(int your_connectID)
    {
        this.your_connectID = your_connectID;
        this.type = (int)CmdType.REPLYID;
    }
}
/*
public class ReplyAskFrame : NetworkMsg
{
    public List<SyncFrame> syncFrames;

    public ReplyAskFrame(List<SyncFrame> syncFrames)
    {
        this.syncFrames = syncFrames;
        this.type = (int)CmdType.REPLYASKFRAME;
    }
}*/

public class ReplyAskFrame : NetworkMsg
{
    public Dictionary<int, List<Frame>> replyFrames;
    public ReplyAskFrame(Dictionary<int, List<Frame>> replyFrames)
    {
        this.replyFrames = replyFrames;
        this.type = (int)CmdType.REPLYASKFRAME;
    }
}






public class ReplyStart : NetworkMsg
{
    public bool start;
    public ReplyStart(bool start)
    {
        this.start = start;
        this.type = (int)CmdType.REPLYSTART;
    }
}


