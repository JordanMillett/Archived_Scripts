using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Steamworks;
using Mirror;

public class Menu_Main : Menu
{
    public TMP_InputField ServerNameField;
    public E_ServerBrowser Browser;

    public void Host()
    {
        if(!NetworkServer.active)
            OnlineManager.singleton.GetComponent<SteamLobby>().CreateNewLobby(ELobbyType.k_ELobbyTypePublic);
    }
    
    public void RefreshLobbies()
    {
        OnlineManager.singleton.GetComponent<SteamLobby>().GetListOfLobbies();
    }
}