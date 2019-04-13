using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;



public class Player
{
    int playerID;
    int connectionID;
    public Queue<Frame> frameBuffer;//缓存那些发送过快的帧，Room.Tick()时从这取  clientID--->PlayerID
    public Player(int playerID,int connectionID)
    {
        this.playerID = playerID;
        this.connectionID = connectionID;
        frameBuffer = new Queue<Frame>();
    }

    public int getID()
    {
        return playerID;
    }

    public int getConnectionID()
    {
        return connectionID;
    }
}

