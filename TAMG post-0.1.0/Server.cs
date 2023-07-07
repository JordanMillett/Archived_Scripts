using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Server : NetworkBehaviour
{
    [System.Serializable]
    public struct TimeFormat
    {
	    public int Hours;
        public int Minutes;
    }

    public int CurrentDay = 0;

    public AudioClip PopInSound;

    [SyncVar]
    public int SpawnLocationIndex = 0;

    //[SyncVar]
    //public string ServerName = "MISSING";

    public NetworkConnection NC;

    public TimeFormat CurrentTime;
    public TimeFormat MorningTime;
    public TimeFormat BedTime;
    public float TickRate = 0.5f;

    [SyncVar]
    public GameRules ServerRules;
    
    [NonReorderable]
    [SyncVar]
    public List<PlayerInfo> PlayerStats;

    public void SetupServer(NetworkConnection conn, long ServerId)//server ready
    {
        Debug.Log("Server Loading");
        NC = conn;

        SetupPlayer();
        NewDay();
        InvokeRepeating("TickForward", TickRate, TickRate);
        Debug.Log("Server Started");
    }

    public void SetupPlayer()
    {
        Debug.Log("Client Loading");

        CmdSyncSessionSave(Client.Instance.SessionSave);

        Online.Instance.StatusInGame(CurrentDay);
        
        UI.Instance.SetScreen("HUD");
        
        Debug.Log("Client Started");
    }

    void TickForward()
    {
        if(CurrentTime.Minutes < 59)
        {
            CurrentTime.Minutes++;
        }
        else
        {
            CurrentTime.Hours++;
            CurrentTime.Minutes = 0;
        }

        if(CurrentTime.Hours >= BedTime.Hours && CurrentTime.Minutes >= BedTime.Minutes)
        {
            //GameObject.FindWithTag("Player").GetComponent<Player>().Passout();
            NewDay();
        }
    }

    void NewDay()
    {
        CurrentTime = MorningTime;
        CurrentDay++;
        //GameObject.FindWithTag("Information").GetComponent<DiscordController>().StatusInGame(CurrentDay);

        //SPAWN NEW BOXES
        transform.GetChild(0).GetComponent<ItemSpawner>().Current = 0;
    
    }
    
    [Command(requiresAuthority  = false)]
    public void CmdSyncSessionSave(PlayerInfo SessionSave)  //cross reference with players on the server 
    {
        for (int i = 0; i < PlayerStats.Count; i++)
        {
            if (PlayerStats[i].SteamID == SessionSave.SteamID)
            {
                PlayerStats[i] = SessionSave;
                return;
            }
        }
        
        PlayerStats.Add(SessionSave);
    }

    [Command(requiresAuthority  = false)]
    public void CmdSpawnObject(string PrefabName, Vector3 Pos, Quaternion Rot, NetworkConnectionToClient sender = null)
    {
        if(NC != null)
        {   
            if(Client.Instance.GetSpawnable(PrefabName).Prefab != null)
            {
                GameObject Object = Instantiate(Client.Instance.GetSpawnable(PrefabName).Prefab, Pos, Rot);
                NetworkServer.Spawn(Object, sender);
                if(Client.Instance.GetSpawnable(PrefabName).PopSound)
                    CmdPopSound(Pos);
            }else
            {
                Debug.LogError("Object name : " + PrefabName.ToLower() + " not recognized");
                GameObject Object = Instantiate(Client.Instance.GetSpawnable("err_error").Prefab, Pos, Rot);
                NetworkServer.Spawn(Object, sender);
            }
        }else
        {
            Debug.LogError("Server Host Network Connect not Initalized");
        }
    }

    [Command]
    public void CmdSpawnObjectConsole(string PrefabName, Vector3 Pos, Quaternion Rot, int Number)
    {
        if(NC != null)
        {   
            if(Client.Instance.GetSpawnable(PrefabName).Prefab != null)
            {
                for (int i = 0; i < Number; i++)
                {
                    GameObject Object = Instantiate(Client.Instance.GetSpawnable(PrefabName).Prefab, Pos + new Vector3(0f, Client.Instance.GetSpawnable(PrefabName).ConsoleOffset, 0f), Rot);
                    NetworkServer.Spawn(Object, NC);
                }
                if(Client.Instance.GetSpawnable(PrefabName).PopSound)
                    CmdPopSound(Pos);
                Debug.Log("Object : " + PrefabName.ToLower() + " spawned at " + Pos + " by Admin x" + Number);
            }else
            {
                Debug.LogError("Object name : " + PrefabName.ToLower() + " not recognized");
            }
        }else
        {
            Debug.LogError("Server Host Network Connect not Initalized");
        }
    }

    [Command(requiresAuthority = false)]
    public void CmdSpawnItem(string ItemName, Vector3 Pos, Quaternion Rot, NetworkConnectionToClient sender = null)
    {
        if(NC != null)
        {   
            if(GetItemFromName(ItemName) != null)
            {
                GameObject Object = Instantiate(Client.Instance.GetSpawnable("ent_item").Prefab, Pos, Rot);
                NetworkServer.Spawn(Object, sender);
                Object.GetComponent<ItemObject>().CmdCreate(ItemName);
            }else
            {
                Debug.LogError("Item name : " + ItemName.ToLower() + " not recognized");
                GameObject Object = Instantiate(Client.Instance.GetSpawnable("err_error").Prefab, Pos, Rot);
                NetworkServer.Spawn(Object, sender);
            }
        }else
        {
            Debug.LogError("Server Host Network Connect not Initalized");
        }
    }

    [Command]
    public void CmdSpawnItemConsole(string ItemName, Vector3 Pos, Quaternion Rot, int Number)
    {
        if(NC != null)
        {   
            if(GetItemFromName(ItemName) != null)
            {
                for (int i = 0; i < Number; i++)
                {
                    GameObject Object = Instantiate(Client.Instance.GetSpawnable("ent_item").Prefab, Pos + new Vector3(0f, Client.Instance.GetSpawnable(ItemName).ConsoleOffset, 0f), Rot);
                    NetworkServer.Spawn(Object, NC);
                    Object.GetComponent<ItemObject>().CmdCreate(ItemName);
                }
                Debug.Log("Item : " + ItemName.ToLower() + " spawned at " + Pos + " by Admin x" + Number);
            }else
            {
                Debug.LogError("Item name : " + ItemName.ToLower() + " not recognized");
            }
        }else
        {
            Debug.LogError("Server Host Network Connect not Initalized");
        }
    }

    public Item GetItemFromName(string Name)
    {
        for(int i = 0; i < Client.Instance.Items.Count; i++)
        {
            if(Client.Instance.Items[i].FileName.ToLower() == Name.ToLower())
                return Client.Instance.Items[i];
        }

        return null;
    }

    [Command]
    void CmdPopSound(Vector3 Pos)
    {
        RpcPopSound(Pos);
    }

    [ClientRpc]
    void RpcPopSound(Vector3 Pos)
    {
        GameObject Sound = new GameObject();
        Sound.transform.position = Pos;
        Despawn DSpawn = Sound.AddComponent(typeof(Despawn)) as Despawn;
        AudioClip Clip = PopInSound;

        DSpawn.DespawnTime = Clip.length + 0.1f;
        AudioSource AS = Sound.AddComponent(typeof(AudioSource)) as AudioSource;
        AS.spatialBlend = 1f;
        AS.volume = (.2f * (Settings._sfxVolume/100f)) * (Settings._masterVolume/100f);
        AS.maxDistance = 50f;
        AS.clip = Clip;
        AS.Play();
    }
}
