using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using Steamworks;
using System.Text;

[System.Serializable]
public class PlayerInfo
{
    public string Username = "";
    public int BankMoney = 0;
    public string CharacterName = "";
    public uint Target_Id = 0;
    public bool TargetKilled = false;
    public bool Hunting = false;

    public PlayerInfo(string _username)
    {
        this.Username = _username;
    }
    
    public PlayerInfo() {}
}

public class Server : NetworkBehaviour
{
    public readonly SyncDictionary<uint, PlayerInfo> ServerRoster = new SyncDictionary<uint, PlayerInfo>();
    public readonly SyncDictionary<CSteamID, uint> SteamRoster = new SyncDictionary<CSteamID, uint>();

    public static Server instance;

    public List<Player> Bots = new List<Player>();

    [SyncVar]
    public double NextRoundStart = 0f;

    [Client]
    void Awake()
    {
        instance = this;
        UIManager.instance.SetScreen("Game");
    }
    
    [Server]
    public override void OnStartServer()
    {
        for (int i = 0; i < Game.Bots; i++)
        {
            GameObject NewAI = Instantiate(OnlineManager.singleton.playerPrefab, OnlineManager.startPositions[Random.Range(0, OnlineManager.startPositions.Count)].position, Quaternion.identity);
            NewAI.GetComponent<Player>().AIControlled = true;
            NewAI.GetComponent<NetworkTransformReliable>().syncDirection = SyncDirection.ServerToClient;
            NetworkServer.Spawn(NewAI);
        }

        NextRoundStart = Time.time + 10f;
    }
    
    void Update()
    {
        if(isServer)
        {
            if(Time.time > NextRoundStart)
            {
                //NextRoundStart = Time.timeAsDouble + 120d;

                DepositMoney();
                
                if (ServerRoster.Count <= 1) //not enough players
                {
                    foreach (uint key in ServerRoster.Keys)
                    {
                        ServerRoster[key].Hunting = false;
                        ServerRoster[key].TargetKilled = false;
                    }
                    
                    NextRoundStart = Time.timeAsDouble + 10d;
                }else
                {
                    NextRoundStart = Time.timeAsDouble + 50d;
                    AssignTargets();
                }
            }
        }
    }
    
    [Server]
    void DepositMoney()
    {
        foreach (uint key in ServerRoster.Keys)
        {
            if (ServerRoster[key].TargetKilled)
                ServerRoster[key].BankMoney += 1000;
        }
    }
    
    [Server]
    void AssignTargets()
    {
        List<uint> Player_Ids = new List<uint>(ServerRoster.Keys); //list of uints to represent players

        uint Start = Player_Ids[0]; //the starting player is stored
        uint Selected_Id = Player_Ids[0]; //currently selected player
        while(Player_Ids.Count > 1)
        {
            uint RandomTarget_Id = Selected_Id;
            while(RandomTarget_Id == Selected_Id)
                RandomTarget_Id = Player_Ids[Random.Range(0, Player_Ids.Count)];

            ServerRoster[Selected_Id].Target_Id = RandomTarget_Id;
            Player_Ids.Remove(Selected_Id);
            Selected_Id = RandomTarget_Id;
        }

        ServerRoster[Player_Ids[0]].Target_Id = Start; //assign last player to starting player to complete loop
        
        foreach (uint key in ServerRoster.Keys)
        {
            ServerRoster[key].Hunting = true;
            ServerRoster[key].TargetKilled = false;
        }
    }
    
    [Server]
    public void Say(string message)
    {
        byte[] bytes = Encoding.ASCII.GetBytes(message);

        Debug.Log(message);

        SteamMatchmaking.SendLobbyChatMsg((CSteamID) OnlineManager.singleton.GetComponent<SteamLobby>().current_lobbyID, bytes, bytes.Length);
    }
    
    [Command(requiresAuthority = false)]
    public void CmdUpdateRoster(CSteamID user, uint netId, PlayerInfo _data)
    {
        if(SteamRoster.ContainsKey(user))
        {
            if(ServerRoster.ContainsKey(netId))
            {
                ServerRoster[netId] = _data;
            }else
            {
                ServerRoster.Add(netId, _data);
            }
        }else
        {
            SteamRoster.Add(user, netId);
            ServerRoster.Add(netId, _data);
        }
    }
    
    [Server]
    public void AddAIToRoster(uint netId, PlayerInfo _data)
    {
        if(ServerRoster.ContainsKey(netId))
        {
            ServerRoster[netId] = _data;
        }else
        {
            ServerRoster.Add(netId, _data);
        }
    }

    [Server]
    public void ClearFromRoster(CSteamID user)
    {
        ServerRoster.Remove(SteamRoster[user]);
        SteamRoster.Remove(user);
    }
    
    [Server]
    public void RegisterService(Service S)
    {
        
    }
}