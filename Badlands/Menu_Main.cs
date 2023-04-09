using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Menu_Main : MonoBehaviour
{
    public TextMeshProUGUI PlayText;

    bool StartingNew = true;

    void Start()
    {
        if (Game.LoadSave())
        {
            StartingNew = false;
            PlayText.text = "Continue"; 
        }
    }
    
    public void Play()
    {
        if(StartingNew)
        {
            Game.NewSave();
        }

        UIManager.UI.SetScreen("World");
    }
}
