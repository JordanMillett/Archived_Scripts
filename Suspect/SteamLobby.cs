using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Steamworks;
using Mirror;
using System.Text;

public class SteamLobby : MonoBehaviour
{
    protected Callback<LobbyCreated_t> lobbyCreated;
    protected Callback<GameLobbyJoinRequested_t> gameLobbyJoinRequested;
    protected Callback<LobbyEnter_t> lobbyEntered;
    protected Callback<LobbyMatchList_t> Callback_lobbyList;
    protected Callback<LobbyDataUpdate_t> Callback_lobbyInfo;
    
    protected Callback<LobbyChatUpdate_t> LobbyStateUpdate;
    protected Callback<LobbyChatMsg_t> LobbyChatMessage;

    public ulong current_lobbyID;
    public List<CSteamID> lobbyIDS = new List<CSteamID>();

    private const string HostAddressKey = "HostAddress";

    public void Initialize()
    {
        lobbyCreated = Callback<LobbyCreated_t>.Create(OnLobbyCreated);
        gameLobbyJoinRequested = Callback<GameLobbyJoinRequested_t>.Create(OnGameLobbyJoinRequested);
        lobbyEntered = Callback<LobbyEnter_t>.Create(OnLobbyEntered);
        Callback_lobbyList = Callback<LobbyMatchList_t>.Create(OnGetLobbiesList);
        Callback_lobbyInfo = Callback<LobbyDataUpdate_t>.Create(OnGetLobbyInfo);
        
        LobbyStateUpdate = Callback<LobbyChatUpdate_t>.Create(OnLobbyStateUpdate);
        LobbyChatMessage = Callback<LobbyChatMsg_t>.Create(OnLobbyChatMessage);
        
    }
    
    public void JoinLobby(CSteamID lobbyId)
    {
        Debug.Log("Joining lobby with steam id: " + lobbyId.ToString());
        SteamMatchmaking.JoinLobby(lobbyId);
    }
    
    public void Leave()
    {
        SteamMatchmaking.LeaveLobby(new CSteamID(current_lobbyID));

        if (NetworkServer.active)
        {
            OnlineManager.singleton.StopHost();
        }else
        {
            OnlineManager.singleton.StopClient();
        }
        
        Debug.Log("Lobby Left");
    }
    
    public void OnLobbyStateUpdate(LobbyChatUpdate_t callback)
    {
        //string message = SteamFriends.GetFriendPersonaName((CSteamID ) callback.m_ulSteamIDUserChanged).ToString();
        string message = Server.instance.ServerRoster[Server.instance.SteamRoster[(CSteamID) callback.m_ulSteamIDUserChanged]].Username;

        switch(callback.m_rgfChatMemberStateChange)
        {
            case (uint) EChatMemberStateChange.k_EChatMemberStateChangeEntered : message += " has joined the game"; break;
            case (uint) EChatMemberStateChange.k_EChatMemberStateChangeLeft : message += " has left the game"; break;
            case (uint) EChatMemberStateChange.k_EChatMemberStateChangeDisconnected : message += " has lost connection"; break;
            case (uint) EChatMemberStateChange.k_EChatMemberStateChangeKicked : message += " has been kicked"; break;
            case (uint) EChatMemberStateChange.k_EChatMemberStateChangeBanned : message += " has been banned"; break;
        }
        
        if(SteamMatchmaking.GetLobbyOwner((CSteamID) current_lobbyID) == SteamUser.GetSteamID())
        {
            Server.instance.Say(message);
            
            if(callback.m_rgfChatMemberStateChange != (uint) EChatMemberStateChange.k_EChatMemberStateChangeEntered)
                Server.instance.ClearFromRoster((CSteamID) callback.m_ulSteamIDUserChanged);
        }
    }
    
    public void OnLobbyChatMessage(LobbyChatMsg_t callback)
    {
        if((EChatEntryType) callback.m_eChatEntryType == EChatEntryType.k_EChatEntryTypeChatMsg)
        {
            byte[] bytes = new byte[4000];
            SteamMatchmaking.GetLobbyChatEntry((CSteamID) current_lobbyID, (int) callback.m_iChatID, out _, bytes, bytes.Length, out _);

            UIManager.instance.M_Game.Chat.Hear(Encoding.ASCII.GetString(bytes).TrimEnd((char)0));
        }
    }
    
    public void GetListOfLobbies()
    {
        lobbyIDS.Clear();

        SteamMatchmaking.AddRequestLobbyListFilterSlotsAvailable(1);
        
        SteamAPICall_t try_getList = SteamMatchmaking.RequestLobbyList();
    }

    private void OnLobbyCreated(LobbyCreated_t callback)
    {
        Debug.Log("Lobby Created");
        if (callback.m_eResult != EResult.k_EResultOK)
        {
            return;
        }

        OnlineManager.singleton.StartHost();
        
        SteamMatchmaking.SetLobbyData(
            new CSteamID(callback.m_ulSteamIDLobby),
            HostAddressKey,
            SteamUser.GetSteamID().ToString());
            
        if (UIManager.instance.M_Main.ServerNameField.text != "")
        {
            SteamMatchmaking.SetLobbyData(
            new CSteamID(callback.m_ulSteamIDLobby),
            "name",
            UIManager.instance.M_Main.ServerNameField.text);
        }
        else
        {
            SteamMatchmaking.SetLobbyData(
            new CSteamID(callback.m_ulSteamIDLobby),
            "name",
            SteamFriends.GetPersonaName().ToString() + "'s lobby");
        }

    }

    private void OnGameLobbyJoinRequested(GameLobbyJoinRequested_t callback)
    {
        Debug.Log("Joining lobby with steam id: " + callback.m_steamIDLobby.ToString());
        SteamMatchmaking.JoinLobby(callback.m_steamIDLobby);
    }

    private void OnLobbyEntered(LobbyEnter_t callback)
    {
        
        current_lobbyID = callback.m_ulSteamIDLobby;
        Debug.Log("Lobby entered with id: " + current_lobbyID.ToString());
        if (NetworkServer.active) { return; }

        string hostAddress = SteamMatchmaking.GetLobbyData(
            new CSteamID(callback.m_ulSteamIDLobby),
            HostAddressKey);

        OnlineManager.singleton.networkAddress = hostAddress;
        OnlineManager.singleton.StartClient();
        lobbyIDS.Clear();
    }
    
    void OnGetLobbiesList(LobbyMatchList_t result)
    {
        Debug.Log("Found " + result.m_nLobbiesMatching + " lobbies!");

        for (int i = 0; i < result.m_nLobbiesMatching; i++)
        {
            CSteamID lobbyID = SteamMatchmaking.GetLobbyByIndex(i);
            lobbyIDS.Add(lobbyID);
            SteamMatchmaking.RequestLobbyData(lobbyID);

        }
        
    }
    
    void OnGetLobbyInfo(LobbyDataUpdate_t result)
    {
        UIManager.instance.M_Main.Browser.DisplayLobbies(lobbyIDS, result);
    }

    public void CreateNewLobby(ELobbyType lobbyType)
    {
       SteamMatchmaking.CreateLobby(lobbyType, OnlineManager.singleton.maxConnections);
    }
}
