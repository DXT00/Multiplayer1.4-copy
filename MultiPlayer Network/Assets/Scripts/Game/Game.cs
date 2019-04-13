using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game : MonoBehaviour
{

    public static PreGameGUI preGameGUI;
    public InGameGUI InGameGUI;
    public GameObject mainCamera;
    CameraFollow CameraFollow;

    public GameObject u_PreGameMgr;
    public GameObject u_ViewManager;
    public GameObject u_InGameMgr;
    
    NetworkManager networkManager;
    ViewManager viewManager;

    Player currentPlayer;

    int currentPlayer_connectID;
    public bool start_flag = false;



    private void Start()
    {
        networkManager = new NetworkManager();
        networkManager.init(this);

        viewManager = u_ViewManager.GetComponent<ViewManager>();

        preGameGUI = u_PreGameMgr.GetComponent<PreGameGUI>();
        InGameGUI = u_InGameMgr.GetComponent<InGameGUI>();
        InGameGUI.Start();
        u_InGameMgr.SetActive(false);

        CameraFollow = mainCamera.GetComponent<CameraFollow>();
        CameraFollow.bind_Game(this);
    }


    private void FixedUpdate()
    {

        if (networkManager.get_isConnect())
        {
            networkManager.Receive_HandleData();
        }
        if (start_flag)
            logic_tick();

    }


    public void logic_tick()
    {


        if (currentPlayer != null)
        {
            currentPlayer.Tick();

            InGameGUI.show_text("framecount :" + currentPlayer.maxExe_FrameCount.ToString());
            InGameGUI.show_textID("clientID :" + currentPlayer.connectID.ToString());
          //  InGameGUI.show_infotext("ss");
        }

    }



    public void connect_click()
    {
        bool flag = networkManager.connect();
        if (!flag)
        {
            Debug.Log("failed to play online game");
            return;
        }
        preGameGUI.ConnectButton.interactable = false;

    }
    public void join_click()//ask players' list
    {
        NetworkMsg Msg = new Join(115);
        networkManager.SendData(Msg);
        preGameGUI.JoinButton.interactable = false;
    }
    public void start_click()
    {
        Debug.Log("Game is starting....");
        NetworkMsg Msg = new StartGame(115);
        networkManager.SendData(Msg);
        preGameGUI.startBtn.interactable = false;
    }

    public void on_reply_join(int connectID, List<string> player_names)
    {
        Debug.Log("receive replyJoin info....");
        //currentPlayer_connectID = connectID;
        // currentPlayer.connectID = connectID;
        preGameGUI.show_room_info(connectID, player_names);
    }
    public void on_reply_ID(int your_connectID)
    {
        Debug.Log("receive on_reply_ID info...."+ your_connectID.ToString());
        currentPlayer_connectID = your_connectID;
        // currentPlayer.connectID = connectID;
        //preGameGUI.show_room_info(connectID, player_names);
    }
    public void on_reply_start(bool start)
    {
        start_flag = start;
        generate_currentPlayer();
        u_PreGameMgr.SetActive(false);
        u_InGameMgr.SetActive(true);
    }

    public void on_reply_frames(Dictionary<int, List<SyncFrame>> replyframes)
    {
        currentPlayer.get_replayframes(replyframes);
    }





    public void generate_currentPlayer()
    {
        currentPlayer = new Player(currentPlayer_connectID);
        currentPlayer.bind_networkManager(networkManager);
        GameObject instance=viewManager.spawn_view_player(currentPlayer_connectID);

        //CameraFollow.bind_currentViewPlayer(instance);
        
        currentPlayer.viewPlayer = instance.GetComponent<ViewPlayer>();
        CameraFollow.bind_currentViewPlayer(currentPlayer.viewPlayer);//MY
        currentPlayer.viewPlayer.Start();
        currentPlayer.viewPlayer.connectID = currentPlayer_connectID;
        currentPlayer.viewPlayer.bind_cameraFollow(CameraFollow);//MY
        currentPlayer.viewPlayer.bind_playerInstance(instance);
        viewManager.bind_InGameGUI(InGameGUI);

        currentPlayer.bind_viewManager(viewManager);
        viewManager.viewPlayers.Add(currentPlayer_connectID, currentPlayer.viewPlayer);

        viewManager.bind_currentPlayer(currentPlayer);//MY

    }




}
