using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    Game game;
    bool flag = true;
    ViewPlayer currentViewPlayer;

    public Vector3 offset;
    private float smoothing = 5f;

    public void bind_currentViewPlayer(ViewPlayer currentViewPlayer)
    {
        this.currentViewPlayer = currentViewPlayer;
    }

    public void bind_Game(Game game)
    {
        this.game = game;
    }
   

    public void CameraUpdate()
    {
        //if (game.start_flag && flag == true)
        //{
        //    flag = false;

        //    Vector3 cur_pos = new Vector3(currentViewPlayer.transform.position.x, currentViewPlayer.transform.position.y, currentViewPlayer.transform.position.z - 8f);

        //    transform.position = cur_pos;
        //    offset = transform.position - currentViewPlayer.transform.position;

        //}
        if (game.start_flag)
        {


            transform.position = currentViewPlayer.transform.position;
            Vector3 playerForward = currentViewPlayer.transform.TransformDirection(Vector3.forward);//把player的x方向转换为世界坐标系 

            transform.rotation = Quaternion.LookRotation(playerForward);
            transform.Translate(Vector3.back * 15f);
            transform.Translate(Vector3.up*4f);

    
        }

    }




}

