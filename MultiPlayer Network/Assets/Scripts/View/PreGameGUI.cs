using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class PreGameGUI : MonoBehaviour
{
    public GameObject PreGameMgr;
    public static Game Game;
    public Button ConnectButton;
    public Button JoinButton;
    public Button startBtn;
    public GameObject StartButtonPrefab;
    //public GameObject playerText;
    public GameObject OnePlayer;
    int textPosX = 0;
    int textPosY = 0;
    GameObject playerText;
    GameObject canvas;
    GameObject tmpBtn;
   


    public Font Arial;

    // Use this for initialization
    void Start()
    {
        Game = PreGameMgr.GetComponent<Game>();
        canvas = GameObject.Find("Canvas");
        playerText = GameObject.Find("Canvas/playerText");
        tmpBtn = GameObject.Find("StartButton");
       
        startBtn = tmpBtn.GetComponent<Button>();
        tmpBtn.SetActive(false);
        //startBtn.onClick.AddListener(() => Game.start_click());
        //startBtn.interactable = false;


    }

    

    public void show_room_info(int connectID, List<string> player_names)
    {

        foreach (Transform childText in playerText.transform)
        {
            Destroy(childText.gameObject);
        }

        foreach (string name in player_names)
        {

            GameObject textGO = new GameObject();
            textGO.transform.parent = playerText.transform;

            textGO.AddComponent<Text>();
            Text text = textGO.GetComponent<Text>();

            text.text = "player id" + name;
            text.font = Arial;
            text.fontSize = 18;
            text.color = Color.red;

            RectTransform rectTransform;
            rectTransform = textGO.GetComponent<RectTransform>();
            rectTransform.localPosition = new Vector3(textPosX, textPosY, 0);

            textPosY -= 30;
        }
        tmpBtn.SetActive(true);
        startBtn.interactable = true;
    }
}
