using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using Mirror;

public class GameServer : MonoBehaviour
{   
    public static GameServer GS;

    [System.Serializable]
    public struct Spawnable
    {
	    public string Name;
        public GameObject Prefab;
    }
    
    public List<Spawnable> Spawnables;
    
    public List<Item> Items;

    public List<VehicleConfig> Configs;

    [HideInInspector]
    public List<Vector3> Locations;
    [HideInInspector]
    public List<Vector3> SpawnPoints;
    [HideInInspector]
    public List<Transform> VehicleSpawnPoints;
    [HideInInspector]
    public List<WorldEvent> PossibleEvents;

    Object[] faces;
    //TextAsset contentsFile;
    //string[] contentsLines;

    public int InitialSpawnLocationIndex = 0;

    //public GameObject SCP;

    public GameObject GameManagerPrefab;
    public GameManager GameManagerInstance;

    public bool InitalizationComplete = false;

    void Awake()
    {
        if(GS != null)
            Destroy(this);

        GS = this;

        DontDestroyOnLoad(this);

        foreach (GameServer.Spawnable G in GameServer.GS.Spawnables)
        {
            RegisterNewObject(G.Prefab);
        }

        foreach (VehicleConfig V in GameServer.GS.Configs)
        {
            foreach (VehiclePart VP in V.InstallableParts)
            {
                if(VP.PartPrefab != null)
                    RegisterNewObject(VP.PartPrefab);
            }
        }
    }

    public IEnumerator Init()
    {
        yield return null;
        LoadingScreen.LS.NextStep("Finding Destinations");

        GameObject[] Objects = GameObject.FindGameObjectsWithTag("DeliveryPosition");
        foreach (GameObject O in Objects)
        {
            Locations.Add(O.transform.position);
        }

        yield return null;
        LoadingScreen.LS.NextStep("Finding Spawnpoints");

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

        yield return null;
        LoadingScreen.LS.NextStep("Finding Events");

        GameObject[] Events = GameObject.FindGameObjectsWithTag("Event");
        foreach (GameObject E in Events)
        {
            PossibleEvents.Add(E.GetComponent<WorldEvent>());
        }

        yield return null;
        LoadingScreen.LS.NextStep("Loading Files");

        //contentsFile = (TextAsset)Resources.Load("Texts/contents");
        //contentsLines = contentsFile.text.Split("\n"[0]);

        faces = Resources.LoadAll("Textures/Faces/", typeof(Texture2D));

        if(GameManagerInstance == null)
        {
            GameManagerInstance = GameObject.FindWithTag("GMI").GetComponent<GameManager>();
            GameManagerInstance.ClientReady();
        }

        InitalizationComplete = true;
    }

    public void ServerReady(NetworkConnection NC)
    {
        //Debug.Log("Init started");
        //DNC = GameObject.FindWithTag("Volume").GetComponent<DayNightController>();

        GameObject GM = Instantiate(GameManagerPrefab, Vector3.zero, Quaternion.identity);
        NetworkServer.Spawn(GM, NC);
        GameManagerInstance = GM.GetComponent<GameManager>();
        GameManagerInstance.ServerReady(NC);

        /*
        if(!isServer)   //if the client connects then it needs to search the scene and find the game manager instead of making it
        {
            GameManagerInstance = GameObject.FindWithTag("GMI").GetComponent<GameManager>();
            GameManagerInstance.ClientReady();
        }*/

        //Debug.Log("Init finished");
    }

    void RegisterNewObject(GameObject Prefab)
    {
        ClientScene.RegisterPrefab(Prefab);
    }

    public void SpawnObject(string PrefabName, Vector3 Pos, Quaternion Rot, bool EODD, bool Console)
    {
        if(Console)
        {
            GameManagerInstance.CmdSpawnObjectConsole(PrefabName, Pos, Rot);  
        }else
        {
            GameManagerInstance.CmdSpawnObject(PrefabName, Pos, Rot, EODD);  
        }
    }

    public void SpawnItem(string ItemName, Vector3 Pos, Quaternion Rot)
    {
        GameManagerInstance.CmdSpawnItem(ItemName, Pos, Rot);   
    }

    public Vector3 GetLocation()
    {
        return Locations[Random.Range(0, Locations.Count)];
    }

    public Item GetItem(Item.ItemSize BoxSize)
    {
        return Items[Random.Range(0, Items.Count)];
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
}
