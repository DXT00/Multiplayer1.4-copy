using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class InGameGUI : MonoBehaviour
{
    public GameObject InGameMgr;
    GameObject frameText;
    GameObject clientIDText;
    GameObject infoText;
    Canvas canvas;
    
    public Font Arial;
    //public Player player;

    public void Start()
    {
        canvas = InGameMgr.GetComponent<Canvas>();
        
        frameText = GameObject.Find("Canvas1/frameText");
        clientIDText = GameObject.Find("Canvas1/clientIDText");
        infoText = GameObject.Find("Canvas1/InfoText");

    }
    public void show_text(string text)
    {
        Text ftext = frameText.GetComponent<Text>();
        ftext.font = Arial;
        ftext.fontSize = 18;
        ftext.color = Color.red;
        ftext.text = text;
    }
    public void show_textID(string text)
    {
        Text ftext = clientIDText.GetComponent<Text>();
        ftext.font = Arial;
        ftext.fontSize = 18;
        ftext.color = Color.red;
        ftext.text = text;
    }

    public void show_infotext(string text)
    {
        Text ftext = infoText.GetComponent<Text>();
        ftext.font = Arial;
        ftext.fontSize = 18;
        ftext.color = Color.red;
        ftext.text = text;
    }

}

