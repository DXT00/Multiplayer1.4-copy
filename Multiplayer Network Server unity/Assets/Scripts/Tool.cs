using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class Tool
{


    static public void printFrameMsgList(string text, Frame frame)
    {
        string str =" Tool "+text+" frame_count = " + frame.syncFrame.frame_count;
         foreach(CustomSyncMsg msg in frame.syncFrame.msg_list){

            if (msg.msg_type == (int)RequestType.INPUT)
            {
                InputMessage input = msg as InputMessage;


               str+="Tool msg_type = INPUT"+ "input.moving_x = " + input.moving_x + "input.moving_z = " + input.moving_z;
            }
            else if ((msg.msg_type == (int)RequestType.ROTATE))
            {
                RotateMessage rot = msg as RotateMessage;


                str += " msg_type = ROTATE" + "rot.moving_x = " + rot.delta_x + "rot.moving_z = " + rot.delta_y;

            }

        }

        Debug.Log(str);
    }
    static public void Print(string text)
    {
        Debug.Log("Tool"+text);
    }




}

