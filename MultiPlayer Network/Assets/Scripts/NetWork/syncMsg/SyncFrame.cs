using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


public class SyncFrame
{
    //帧号基本信息
    public int frame_count;
    public List<CustomSyncMsg> msg_list;

    public SyncFrame(int frame_count)
    {
        this.frame_count = frame_count;
        msg_list = new List<CustomSyncMsg>();
    }


    //存放这一帧做了什么动作
    public void dump_action(CustomSyncMsg msg)
    {
        msg_list.Add(msg);
    }

    public List<CustomSyncMsg> get_msg()
    {
        return this.msg_list;
    }
    public int get_frame_count()
    {
        return this.frame_count;
    }

    public void printFrameInfo()
    {
        foreach (CustomSyncMsg msg in this.msg_list)
        {
            msg.printInfo();
        }
    }
}

