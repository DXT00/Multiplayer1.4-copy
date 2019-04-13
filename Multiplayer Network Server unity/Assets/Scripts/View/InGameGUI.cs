using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class InGameGUI:MonoBehaviour
{
    public Font Arial;
    public void show_text(string text)
    {
        Text ftext = GameObject.Find("Canvas/framecountText").GetComponent<Text>();
        ftext.font = Arial;
        ftext.fontSize = 18;
        ftext.color = Color.red;
        ftext.text = text;
    }



}

