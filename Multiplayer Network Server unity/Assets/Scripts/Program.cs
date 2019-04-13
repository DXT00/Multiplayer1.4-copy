using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;


class Program : MonoBehaviour
{
    GameServer gameServer;
    //static public GameObject serverManager;
    public GameObject InGameMgr;
    private void Start()
    {

        gameServer = new GameServer();
        gameServer.bind_InGameMgr(InGameMgr);
      
        gameServer.init();
        NetCommon.gameServer = gameServer;
     

    }

    private void Update()
    {
        gameServer.Update();
    }



}

