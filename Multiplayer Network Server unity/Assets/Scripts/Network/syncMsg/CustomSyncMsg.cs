using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using ProtoBuf;
using UnityEngine;

[ProtoContract]
[ProtoInclude(4, typeof(RotateMessage))]
[ProtoInclude(5, typeof(SpawnMessage))]
[ProtoInclude(6, typeof(InputMessage))]
[ProtoInclude(7, typeof(EnterAreaMessage))]
[ProtoInclude(8, typeof(LeaveAreaMessage))]
public class CustomSyncMsg
{
    [ProtoMember(1)]
    public int player_id { get; set; }
    [ProtoMember(2)]
    public int area_id { get; set; }
    [ProtoMember(3)]
    public int msg_type { get; set; }
    // Marshal.Sizeof(Custom)
    public virtual void printInfo() { }
}
[ProtoContract]
public class RotateMessage : CustomSyncMsg
{
    [ProtoMember(1)]
    public int id;

    [ProtoIgnore]
    public Vector2 delta;
    public RotateMessage(int player_id, Vector2 delta)
    {
        this.delta = delta;
        this.player_id = player_id;
        msg_type = (int)RequestType.ROTATE;
    }
    [ProtoMember(2)]
    public float delta_x { get { return delta.x; } set { delta.x = value; } }
    [ProtoMember(3)]
    public float delta_y { get { return delta.y; } set { delta.y = value; } }

    public override void printInfo()
    {
        Debug.Log("rotate msg:  player id--" + player_id.ToString() + "delta_x,y --" + delta_x.ToString() + "  " + delta_y.ToString());
    }
}
[ProtoContract]
public class SpawnMessage : CustomSyncMsg
{
    [ProtoMember(1)]
    public int id;

    [ProtoIgnore]
    public Vector3 position;
    public SpawnMessage(int player_id, Vector3 pos)
    {
        this.position = pos;
        this.player_id = player_id;
        msg_type = (int)RequestType.SPAWN;
    }

    [ProtoMember(2)]
    public float position_x { get { return position.x; } set { position.x = value; } }
    [ProtoMember(3)]
    public float position_y { get { return position.y; } set { position.y = value; } }
    [ProtoMember(4)]
    public float position_z { get { return position.z; } set { position.z = value; } }

    public override void printInfo()
    {
        Console.WriteLine("spawn msg:  player id--" + player_id + "posx,y,z --" + position_x + "  " + position_y + " " + position_z);
    }
}
[ProtoContract]
public class InputMessage : CustomSyncMsg
{
    [ProtoMember(1)]
    public int id;
    [ProtoMember(2)]
    public float moving_x { get; set; }
    [ProtoMember(3)]
    public float moving_y { get; set; }
    [ProtoMember(4)]
    public float moving_z { get; set; }
    public Vector3 moving;
    public InputMessage(int player_id, Vector3 moving)
    {
        this.moving = moving;
        this.player_id = player_id;
        moving_x = moving.x;
        moving_y = moving.y;
        moving_z = moving.z;
        msg_type = (int)RequestType.INPUT;
    }

    public override void printInfo()
    {
        Debug.Log("input msg:  player id--" + player_id.ToString() + "moving x,y,z --" + moving_x.ToString() + "  " + moving_y.ToString() + " " + moving_z.ToString());
    }
    /*
    [ProtoMember(2)]
    public float moving_x { get { return moving.x; } set { moving.x = value; } }
    [ProtoMember(3)]
    public float moving_y { get { return moving.y; } set { moving.y = value; } }
    [ProtoMember(4)]
    public float moving_z { get { return moving.z; } set { moving.z = value; } }*/
}
[ProtoContract]
public class EnterAreaMessage : CustomSyncMsg
{
    [ProtoMember(1)]
    public int id;
    [ProtoMember(2)]
    public int health;

    [ProtoIgnore]
    public Vector2 rotation;
    public Vector3 direction;
    public Vector3 position;
    public EnterAreaMessage(int player_id, int health, Vector2 rotation, Vector3 direction, Vector3 position)
    {
        this.player_id = player_id;
        this.health = health;
        this.rotation = rotation;
        this.direction = direction;
        this.position = position;
        msg_type = (int)RequestType.ENTERAREA;
    }
    [ProtoMember(3)]
    public float position_x { get { return position.x; } set { position.x = value; } }
    [ProtoMember(4)]
    public float position_y { get { return position.y; } set { position.y = value; } }
    [ProtoMember(5)]
    public float position_z { get { return position.z; } set { position.z = value; } }

    [ProtoMember(5)]
    public float direction_x { get { return direction.x; } set { direction.x = value; } }
    [ProtoMember(6)]
    public float direction_y { get { return direction.y; } set { direction.y = value; } }
    [ProtoMember(7)]
    public float direction_z { get { return direction.z; } set { direction.z = value; } }

    [ProtoMember(8)]
    public float rotation_x { get { return rotation.x; } set { rotation.x = value; } }
    [ProtoMember(9)]
    public float rotation_y { get { return rotation.y; } set { rotation.y = value; } }

    public override void printInfo()
    {
       Debug.Log("Enter msg:  player id--" + player_id.ToString() + "posx,y,z --" + position_x.ToString() + "  " + position_y.ToString() + " " + direction_z.ToString());
    }
}
[ProtoContract]
public class LeaveAreaMessage : CustomSyncMsg
{
    [ProtoMember(1)]
    public int id;

    public LeaveAreaMessage(int player_id)
    {
        this.player_id = player_id;
        msg_type = (int)RequestType.LEAVEAREA;
    }

    public override void printInfo()
    {
        Debug.Log("LeaveAreaMessage msg:  player id--" + player_id.ToString());
    }
}


public class ShootMessage : CustomSyncMsg
{
    public Vector3 origin;
    public Vector3 direction;
    public float origin_x { get; set; }
    public float origin_y { get; set; }
    public float origin_z { get; set; }

    public float direction_x { get; set; }
    public float direction_y { get; set; }
    public float direction_z { get; set; }

    public ShootMessage(int player_id, Vector3 origin, Vector3 direction)
    {
        this.player_id = player_id;
        this.direction = direction;
        this.origin = origin;
        origin_x = origin.x;
        origin_y = origin.y;
        origin_z = origin.z;

        direction_x = direction.x;
        direction_y = direction.y;
        direction_z = direction.z;

        msg_type = (int)RequestType.SHOOT;

    }




}







