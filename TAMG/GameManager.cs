using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using Mirror;

public class GameManager : NetworkBehaviour
{
    [System.Serializable]
    public struct TimeFormat
    {
	    public int Hours;
        public int Minutes;
    }

    public AudioClip PopInSound;

    public NetworkConnection NC;

    public TimeFormat CurrentTime;
    public TimeFormat MorningTime;
    public TimeFormat BedTime;
    public float TickRate = 0.5f;
    bool WillRain = false;
    DayNightController DNC;

    public void ServerReady(NetworkConnection conn)//server ready
    {
        Debug.Log("Server Ready");
        /*
        if(GameServer.GS != null)
        {
            foreach (GameServer.Spawnable G in GameServer.GS.Spawnables)
            {
                RegisterNewObject(G.Prefab);
            }
        }*/

        DNC = GameObject.FindWithTag("Volume").GetComponent<DayNightController>();
        DNC.Init();

        NC = conn;
        GameObject.FindWithTag("Manager").GetComponent<ItemSpawner>().TurnOn();
        StartCoroutine(NewDay());

        //InvokeRepeating("TickForward", TickRate, TickRate);
    }

    public void ClientReady()
    {
        Debug.Log("Client Ready");
        DNC = GameObject.FindWithTag("Volume").GetComponent<DayNightController>();
        DNC.Init();
        /*
        foreach (GameServer.Spawnable G in GameServer.GS.Spawnables)
        {
            RegisterNewObject(G.Prefab);
        }*/
    }
    /*
    void RegisterNewObject(GameObject Prefab)
    {
        ClientScene.RegisterPrefab(Prefab);
    }*/

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
            GameObject.FindWithTag("Player").GetComponent<Player>().Passout();
        }

        if(WillRain)
            DNC.SetRain(DNC.TimeAlpha);
        else
            DNC.SetRain(0f);
        
        DNC.DayNightUpdate();
    }

    IEnumerator NewDay()
    {
        while(!GameServer.GS.InitalizationComplete)
        {
            yield return null;
        }

        GameObject[] EndOfDayDeletion = GameObject.FindGameObjectsWithTag("EODD");
        //Debug.Log("Old Objects Cleared : " + EndOfDayDeletion.Length);
        foreach (GameObject O in EndOfDayDeletion)
        {
            Destroy(O);
        }

        WillRain = (Random.value > 0.75f);  //one quarter of time
        //WillRain = true;
    
        //Debug.Log("Will it Rain? " + WillRain);
        //Debug.Log(GameServer.GS.PossibleEvents.Count); //3011
        for(int i = 0; i < GameServer.GS.PossibleEvents.Count; i++)//foreach(WorldEvent E in GameServer.GS.PossibleEvents)
        {
            GameServer.GS.PossibleEvents[i].onNewDay();
            //E.onNewDay();
        }

        CurrentTime = MorningTime;
        DNC.CurrentSeconds = 0;
        DNC.TimeAlpha = 0f;
        DNC.SetRain(0f);
    }

    [Command(ignoreAuthority = true)]
    public void CmdSpawnObject(string PrefabName, Vector3 Pos, Quaternion Rot, bool EODD)
    {
        if(NC != null)
        {   
            if(GetSpawnable(PrefabName) != null)
            {
                //RPCRegisterNewObject(GetSpawnable(PrefabName));
                GameObject Object = Instantiate(GetSpawnable(PrefabName), Pos, Rot);
                NetworkServer.Spawn(Object, NC);
                CmdPopIn(Pos);
                if(EODD)
                    Object.tag = "EODD";
                Debug.Log("Object : " + PrefabName.ToLower() + " spawned at " + Pos);
            }else
            {
                Debug.LogError("Object name : " + PrefabName.ToLower() + " not recognized");
            }
        }else
        {
            Debug.LogError("Server Host Network Connect not Initalized");
        }
    }

    [Command(ignoreAuthority = true)]
    public void CmdSpawnItem(string ItemName, Vector3 Pos, Quaternion Rot)
    {
        if(NC != null)
        {   
            if(GetItemFromName(ItemName) != null)
            {
                GameObject Object = Instantiate(GetSpawnable("ItemObject"), Pos, Rot);
                NetworkServer.Spawn(Object, NC);
                Object.GetComponent<ItemObject>().CmdCreate(ItemName);
                Debug.Log("Item : " + ItemName.ToLower() + " spawned at " + Pos);
            }else
            {
                Debug.LogError("Item name : " + ItemName.ToLower() + " not recognized");
            }
        }else
        {
            Debug.LogError("Server Host Network Connect not Initalized");
        }
    }

    [Command]
    public void CmdSpawnObjectConsole(string PrefabName, Vector3 Pos, Quaternion Rot)
    {
        if(NC != null)
        {   
            if(GetSpawnable(PrefabName) != null)
            {
                //RPCRegisterNewObject(GetSpawnable(PrefabName));
                GameObject Object = Instantiate(GetSpawnable(PrefabName), Pos, Rot);
                NetworkServer.Spawn(Object, NC);
                CmdPopIn(Pos);
                Debug.Log("Object : " + PrefabName.ToLower() + " spawned at " + Pos + " by Admin");
            }else
            {
                Debug.LogError("Object name : " + PrefabName.ToLower() + " not recognized");
            }
        }else
        {
            Debug.LogError("Server Host Network Connect not Initalized");
        }
    }

    GameObject GetSpawnable(string Name)
    {
        for(int i = 0; i < GameServer.GS.Spawnables.Count; i++)
        {
            if(GameServer.GS.Spawnables[i].Name.ToLower() == Name.ToLower())
                return GameServer.GS.Spawnables[i].Prefab;
        }

        return null;
    }

    public Item GetItemFromName(string Name)
    {
        for(int i = 0; i < GameServer.GS.Items.Count; i++)
        {
            if(GameServer.GS.Items[i].FileName.ToLower() == Name.ToLower())
                return GameServer.GS.Items[i];
        }

        return null;
    }

    [Command]
    void CmdPopIn(Vector3 Pos)
    {
        RpcPopIn(Pos);
    }

    [ClientRpc]
    void RpcPopIn(Vector3 Pos)
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

    /*
    [System.Serializable]
    public struct TimeFormat
    {
	    public int Hours;
        public int Minutes;
    }
    
    public List<Vector3> Locations;
    public List<Vector3> SpawnPoints;
    public List<Transform> VehicleSpawnPoints;
    public List<WorldEvent> PossibleEvents;

    Object[] faces;
    TextAsset contentsFile;
    string[] contentsLines;
    
    public TimeFormat CurrentTime;
    public TimeFormat MorningTime;
    public TimeFormat BedTime;

    public float TickRate = 0.5f;

    public bool TestMode = false;
    bool TestCheck = false;
    
    bool WillRain = false;

    DayNightController DNC;

    public int InitialSpawnLocationIndex = 0;

    public NetworkConnection NC;

    public GameObject SCP;*/
    /*
    void Awake()
    {
        DNC = GameObject.FindWithTag("Volume").GetComponent<DayNightController>();

        GameObject[] Objects = GameObject.FindGameObjectsWithTag("DeliveryPosition");
        foreach (GameObject O in Objects)
        {
            Locations.Add(O.transform.position);
        }

        GameObject[] Spawns = GameObject.FindGameObjectsWithTag("SpawnPoint");
        foreach (GameObject S in Spawns)
        {
            SpawnPoints.Add(S.transform.position);
        }

        GameObject[] VehicleSpawns = GameObject.FindGameObjectsWithTag("VehicleSpawnPoint");
        foreach (GameObject S in VehicleSpawns)
        {
            VehicleSpawnPoints.Add(S.transform);
        }

        GameObject[] Events = GameObject.FindGameObjectsWithTag("Event");
        foreach (GameObject E in Events)
        {
            PossibleEvents.Add(E.GetComponent<WorldEvent>());
        }

        contentsFile = (TextAsset)Resources.Load("Texts/contents");
        contentsLines = contentsFile.text.Split("\n"[0]);

        faces = Resources.LoadAll("Textures/Faces/", typeof(Texture2D));
    }

    public void Ready(NetworkConnection conn)
    {
        NC = conn;
        NewDay();
        InvokeRepeating("TickForward", TickRate, TickRate);
    }

    void Update()
    {
        if(TestCheck != TestMode)
        {
            GameObject.FindWithTag("Player").GetComponent<PlayerController>().Passout();
            TestCheck = TestMode;
        }else
        {
            TestCheck = TestMode;
        }
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
            GameObject.FindWithTag("Player").GetComponent<PlayerController>().Passout();
        }

        if(WillRain)
            DNC.SetRain(DNC.TimeAlpha);
        else
            DNC.SetRain(0f);
        
        DNC.DayNightUpdate();
    }

    public void SpawnSCP(Vector3 Location)
    {
        if(NC != null)
        {
            GameObject Enemy = Instantiate(SCP, Location, Quaternion.identity);
            Enemy.transform.position = Location;
            NetworkServer.Spawn(Enemy, NC);
        }
    }

    public void NewDay()
    {
        WillRain = (Random.value > 0.75f);  //one quarter of time
        //WillRain = true;
    
        //Debug.Log("Will it Rain? " + WillRain);
        foreach(WorldEvent E in PossibleEvents)
        {
            E.onNewDay();
        }

        CurrentTime = MorningTime;
        DNC.CurrentSeconds = 0;
        DNC.TimeAlpha = 0f;
        DNC.SetRain(0f);
    }

    public Vector3 GetLocation()
    {
        return Locations[Random.Range(0, Locations.Count)];
    }

    public string GetContents()
    {
        return contentsLines[Random.Range(0, contentsLines.Length)];
    }

    public Texture2D GetRandomFace()
    {
        return (Texture2D)faces[Random.Range(0, faces.Length - 1)];
    }

    public Texture2D GetFace(int Index)
    {
        return (Texture2D)faces[Index];
    }

    public Vector3 GetNearestSpawn(Vector3 Point)
    {
        int MinDistIndex = 0;
        float MinDist = 999999; //there will always be a spawn point closer than this amount

        for(int i = 0; i < SpawnPoints.Count; i++)
        {
            if(Vector3.Distance(Point, SpawnPoints[i]) < MinDist)
            {
                if(!Physics.CheckSphere(SpawnPoints[i] + new Vector3(0f, 1f, 0f), 0.5f))
                {
                    MinDistIndex = i;
                    MinDist = Vector3.Distance(Point, SpawnPoints[i]);
                }
            }
        }

        return SpawnPoints[MinDistIndex];
    }

    public Transform GetNearestVehicleSpawn(Vector3 Point)
    {
        int MinDistIndex = 0;
        float MinDist = 999999; //there will always be a spawn point closer than this amount

        for(int i = 0; i < VehicleSpawnPoints.Count; i++)
        {
            if(Vector3.Distance(Point, VehicleSpawnPoints[i].position) < MinDist)
            {
                if(!Physics.CheckSphere(VehicleSpawnPoints[i].position + new Vector3(0f, 1f, 0f), 1f))
                {
                    MinDistIndex = i;
                    MinDist = Vector3.Distance(Point, VehicleSpawnPoints[i].position);
                }
                
            }
        }

        return VehicleSpawnPoints[MinDistIndex];
    }
    */
}
