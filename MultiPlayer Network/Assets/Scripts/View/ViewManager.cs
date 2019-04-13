using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
/*
 
viewManager 负责执行frame，并在客户端进行游戏模拟     
     
 */
public class ViewManager : MonoBehaviour
{
    InGameGUI InGameGUI;
    Player currentPlayer;
    public GameObject player_prefabs;//ViewManager从prefabs中spawn players!!
    public Dictionary<int, ViewPlayer> viewPlayers = new Dictionary<int, ViewPlayer>();//存储localPlayer在本地模拟的其他viewPlayers  (clientID--->ViewPlayer)
    Vector3 init = new Vector3(-74, 1, 37);
    public float distBetweenViewplayers = 0.1f;
    int shootableMask;
    public Slider currentPlayerHealthSlider;


    public GameObject spawn_view_player(int clientID)
    {
        GameObject instance = null;

        if (player_prefabs != null)
        {
            Vector3 initPos = new Vector3(init.x + clientID * distBetweenViewplayers, init.y, init.z);
            transform.eulerAngles = new Vector3(0, 0, 0);//注意不能直接设置transform.rotation！！rotation是四元数

            instance = Instantiate(player_prefabs, initPos, transform.rotation) as GameObject;


        }
        else
            Debug.Log("player_prefabs is null !");
        return instance;
    }
    public void bind_currentPlayer(Player currentPlayer)
    {
        this.currentPlayer = currentPlayer;
    }
    public ViewPlayer generate_other_viewPlayer(int clientID)
    {
        GameObject instance = spawn_view_player(clientID);
        ViewPlayer v_player = instance.GetComponent<ViewPlayer>();
        viewPlayers.Add(clientID, v_player);

        return v_player;
    }


    public void Start()
    {
        shootableMask = LayerMask.GetMask("shootable");
        currentPlayerHealthSlider.value = 1000;
    }



    public void execute_frames(List<SyncFrame> syncFrames)
    {

        foreach (SyncFrame syncFrame in syncFrames)
        {
            if (syncFrame.msg_list == null)
            {
                Debug.Log("executing frame----" + syncFrame.frame_count + "no msg_list");
                continue;
            }
            foreach (CustomSyncMsg msg in syncFrame.msg_list)
            {

                int clientID = msg.player_id;
                //Debug.Log("executing frame----" + syncFrame.frame_count + "clientID = " + clientID + "msg.type=" + msg.msg_type);
                ViewPlayer viewPlayer;
                if (viewPlayers.ContainsKey(clientID))
                {
                    viewPlayer = viewPlayers[clientID];
                }
                else
                {
                    viewPlayer = generate_other_viewPlayer(clientID);
                    viewPlayer.Start();
                    viewPlayer.connectID = clientID;
                }


                if (msg.msg_type == (int)RequestType.INPUT)
                {
                    InputMessage Input_msg = msg as InputMessage;
                    viewPlayer.Move(Input_msg.moving_x, Input_msg.moving_z);//接收到的都是非零的x,z


                    Debug.Log("executing frame----" + syncFrame.frame_count + " clientID " + clientID.ToString() + "..........is moving..."
                        + " dist_x =  " + Input_msg.moving_x * viewPlayer.get_speed() * Time.deltaTime + " dist_z= " + Input_msg.moving_z * viewPlayer.get_speed() * Time.deltaTime
                        + "y = " + Input_msg.moving_y);

                }
                else if (msg.msg_type == (int)RequestType.ROTATE)
                {
                    RotateMessage rot_msg = msg as RotateMessage;
                    viewPlayer.Rotate(rot_msg.delta_x, rot_msg.delta_y);
                }
                else if (msg.msg_type == (int)RequestType.SHOOT)
                {
                    ShootMessage shoot_msg = msg as ShootMessage;
                    Ray shootRay = new Ray(new Vector3(shoot_msg.origin_x, shoot_msg.origin_y, shoot_msg.origin_z), new Vector3(shoot_msg.direction_x, shoot_msg.direction_y, shoot_msg.direction_z));


                    OnShootJudging(viewPlayer, shootRay);

                }

                else if (msg.msg_type == (int)RequestType.SPAWN)
                {

                }
                //如果是currentPlayer -->cameraFolllow
                if (viewPlayer.connectID == currentPlayer.connectID)
                {
                    viewPlayer.camera.CameraUpdate();
                    InGameGUI.show_infotext("currentHealth:" + viewPlayer.currentHealth);
                    currentPlayerHealthSlider.value = viewPlayer.currentHealth;//只有currentPlayer才显示healthSlider
                }


            }



        }



    }


    public void OnShootJudging(ViewPlayer origin_viewPlayer, Ray shootRay)
    {


        RaycastHit hit;
        //   origin_viewPlayer.Animator.SetBool("isShooting", true);
        origin_viewPlayer.Shoot();
        if (Physics.Raycast(shootRay, out hit, 500f, shootableMask))
        {

            ViewPlayer hit_viewPlayer = hit.collider.GetComponent<ViewPlayer>();
          
            if (hit_viewPlayer != null)
            {

                Debug.Log("&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&& hit_viewPlayer.connectID = " + hit_viewPlayer.connectID + "origin_viewPlayer.connectID =  " + origin_viewPlayer.connectID);
                if (hit_viewPlayer.connectID == origin_viewPlayer.connectID)//原来的Player发出的打到了自己身上
                {

                }
                else
                {
                    hit_viewPlayer.TakeDamage();
                    
                }




            }





        }
     //    origin_viewPlayer.Animator.SetBool("isShooting", false);
    }

    public void bind_InGameGUI(InGameGUI inGameGUI)
    {
        this.InGameGUI = inGameGUI;
    }
}

