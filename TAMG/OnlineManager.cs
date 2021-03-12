using System.Collections;
using System.Collections.Generic;
using System;
using Steamworks;
using UnityEngine;
using Mirror;
using TMPro;

public class OnlineManager : NetworkManager
{
    public override void OnStartServer()
    {
        NetworkServer.RegisterHandler<CreateMessage>(OnCreateMessage);
    }

    public override void OnClientConnect(NetworkConnection NC)  //Called on the client and sent to the server
    {
        ClientScene.Ready(NC);
        //base.OnClientConnect(NC);
        CreateMessage characterMessage = new CreateMessage {name=SteamClient.Name.ToString(), id=SteamClient.SteamId.ToString()};//On create message for client connections
        //Request to spawn with specified info
        NC.Send(characterMessage);
    }

    public override void OnClientDisconnect(NetworkConnection NC)
    {
        StopClient();
    }

    public override void OnServerDisconnect(NetworkConnection NC)
    {
        base.OnServerDisconnect(NC);        //Destroys the client when they disconnect
    }

    void OnCreateMessage(NetworkConnection NC, CreateMessage message)
    {   
        StartCoroutine(CreatePlayer(NC, message));
    }

    IEnumerator CreatePlayer(NetworkConnection NC, CreateMessage message)
    {
        if(!GameServer.GS.InitalizationComplete)
            GameServer.GS.ServerReady(NC); //If server owner ready the GameManager

        //Wait for GameManager to Init
        while(!GameServer.GS.InitalizationComplete)
        {
            yield return null;
        }

        //Create new Player
        GameObject NewPlayer = Instantiate(playerPrefab, GameServer.GS.SpawnPoints[GameServer.GS.InitialSpawnLocationIndex], Quaternion.identity);
        NewPlayer.GetComponent<Player>().playerName = message.name;
        GameServer.GS.InitialSpawnLocationIndex++;

        //Spawn Player Vehicle
        GameServer.GS.SpawnObject("Veh_Golf", GameServer.GS.GetNearestVehicleSpawn(NewPlayer.transform.position).position, GameServer.GS.GetNearestVehicleSpawn(NewPlayer.transform.position).rotation, false, false);

        //Finalize
        NetworkServer.AddPlayerForConnection(NC, NewPlayer);
    }

    public struct CreateMessage : NetworkMessage
    {
        public string name;
        public string id;
    }
}
