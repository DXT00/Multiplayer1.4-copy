using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public enum RequestType
{
    SPAWN,
    ROTATE,
    INPUT,
    SHOOT,
    ENTERAREA,
    LEAVEAREA
}


public enum CmdType
{
    NULL,
    SPAWNINFO,
    GETROOMS,
    JOIN,
    ASKFRAME,
    ASKCHASEFRAME,
    START,
    FRAME,
    GETFRAME,
    ENTERAREA,
    LEAVEAREA,

    SYNC_FRAME,

    REPLYGETROOMS,
    REPLYJOIN,
    REPLYASKFRAME,
    REPLYASKCHASEFRAME,
    REPLYSTART,
    REPLYID,

    TICK,
    END,
    HEART
}

