using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


public class NetworkMsg:BaseProtocol
{
    public int player_id;
    public int type;

}

public class StartGame : NetworkMsg
{
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

public class ReplyAskFrame : NetworkMsg
{
    public Dictionary<int, List<Frame>> replyFrames;
    public ReplyAskFrame(Dictionary<int, List<Frame>> replyFrames)
    {
        this.replyFrames = replyFrames;
        this.type = (int)CmdType.REPLYASKFRAME;
    }
}

public class Join : NetworkMsg
{
    public Join(int player_id)
    {
        this.player_id = player_id;
        this.type = (int)CmdType.JOIN;
    }
}

public class AskFrame : NetworkMsg
{
    // public CmdType type;
    public List<int> frames;

    public AskFrame(int player_id, List<int> frame)//这里可以不用List,因为只有一个area
    {
        this.type = (int)CmdType.ASKFRAME;
        this.player_id = player_id;

        this.frames = frame;

    }
}
