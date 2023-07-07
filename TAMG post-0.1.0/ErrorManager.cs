using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;

public class ErrorManager : MonoBehaviour
{
    public GameObject ErrorPopup;
    public TextMeshProUGUI ErrorText;
    public GameObject CloseButton;

    void OnEnable()
    {
        Application.logMessageReceived += HandleLog;
    }

    void OnDisable()
    {
        Application.logMessageReceived -= HandleLog;
    }
    
    public void ClosePopup()
    {
        ErrorPopup.SetActive(false);
    }
    
    //Debug.LogError("ERROR SteamConnectionFailed");
    void HandleLog(string logString, string stackTrace, LogType type)
    {
        if(type != LogType.Error)
            return;
            
        if(logString.Split()[0] == "ERROR")
        {
            switch(logString.Split()[1])
            {
                case "SteamConnectionFailed" : SteamConnectionFailed(); break;
                case "FailedToJoinInvite" : FailedToJoinInvite(); break;
                default : Debug.LogError("ERROR " + logString.Split()[1] + " NOT RECOGNIZED"); break;
            }
        }
    }
    
    void SteamConnectionFailed()
    {
        ErrorPopup.SetActive(true);
        CloseButton.SetActive(false);

        ErrorText.text = "Steam must be running\nin order to play\n\nPlease restart after\nopening Steam";
    }
    
    void FailedToJoinInvite()
    {
        ErrorPopup.SetActive(true);
        CloseButton.SetActive(true);

        ErrorText.text = "Failed to join server";
    }
}
