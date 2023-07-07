using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using Steamworks;

public class Client : MonoBehaviour
{   
    public static Client Instance;
    
    public List<Spawnable> Spawnables;
    
    [NonReorderable]
    public List<Item> Items;

    public List<VehicleConfig> Configs;

    [HideInInspector]
    public List<Vector3> Locations;
    [HideInInspector]
    public List<Transform> SpawnPoints;
    [HideInInspector]
    public List<Transform> VehicleSpawnPoints;

    Object[] faces;

    public GameObject ServerPrefab;
    public Server ServerInstance;

    public bool LoadingComplete = false;

    bool alreadyRegistered = false;

    public PlayerInfo SessionSave;

    void Awake()
    { 
        if(Instance && Instance != this)
        {
            Destroy(this.gameObject);
        }else
        {
            Instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
    }

    public void WipeSave()
    {
        SessionSave.Name = SteamClient.Name.ToString();
        SessionSave.SteamID = SteamClient.SteamId.ToString();

        SessionSave.Balance = 100;
        SessionSave.TotalDeliveries = 0;
        SessionSave.TotalScore = 0;
    }

    public void SyncSessionSave()
    {
        ServerInstance.CmdSyncSessionSave(SessionSave);
    }

    public void Reset()
    {
        LoadingComplete = false;
        NetworkClient.Shutdown();
        Locations.Clear();
        SpawnPoints.Clear();
        VehicleSpawnPoints.Clear();
        System.Array.Clear(faces, 0, faces.Length);

        WipeSave();
    }

    public IEnumerator Init()
    {
        WipeSave();

        yield return null;
        GameObject[] Objects = GameObject.FindGameObjectsWithTag("DeliveryPosition");
        foreach (GameObject O in Objects)
        {
            Locations.Add(O.transform.position);
        }

        yield return null;
        GameObject[] Spawns = GameObject.FindGameObjectsWithTag("Respawn");
        for (int i = 0; i < Spawns.Length; i++)
            SpawnPoints.Add(Spawns[i].transform);

        yield return null;
        GameObject[] VehicleSpawns = GameObject.FindGameObjectsWithTag("VehicleSpawnPoint");
        for (int i = 0; i < VehicleSpawns.Length; i++)
            VehicleSpawnPoints.Add(VehicleSpawns[i].transform);

        yield return null;
        faces = Resources.LoadAll("Textures/Faces/", typeof(Texture2D));

        //if(!alreadyRegistered)
        if(true)//FUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUCK
        {
            foreach (Spawnable G in Spawnables)  
            {
                if(G.Prefab != null)
                    NetworkClient.RegisterPrefab(G.Prefab);
                yield return null;
            }
            alreadyRegistered = true;
        }

        LoadingComplete = true;
        
    }

    public void SpawnManager(NetworkConnection NC)
    {
        GameObject GM = Instantiate(ServerPrefab, Vector3.zero, Quaternion.identity);
        NetworkServer.Spawn(GM, NC);
        ServerInstance = GM.GetComponent<Server>();
        ServerInstance.SetupServer(NC, long.Parse(SteamClient.SteamId.ToString()));
    }
    
    public void SpawnObject(string PrefabName, Vector3 Pos, Quaternion Rot, int Number, bool Console)
    {
        if(Console)
            ServerInstance.CmdSpawnObjectConsole(PrefabName, Pos, Rot, Number);  
        else
            ServerInstance.CmdSpawnObject(PrefabName, Pos, Rot);
    }

    public void SpawnItem(string ItemName, Vector3 Pos, Quaternion Rot, int Number, bool Console)
    {
        if(Console)
        {
            ServerInstance.CmdSpawnItemConsole(ItemName, Pos, Rot, Number);   
        }else
        {
            ServerInstance.CmdSpawnItem(ItemName, Pos, Rot);   
        }
    }

    public Vector3 GetLocation()
    {
        return Locations[Random.Range(0, Locations.Count)];
    }

    public Item GetItem(Vector3 BoxSize)
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

    public Spawnable GetSpawnable(string Name)
    {
        for(int i = 0; i < Spawnables.Count; i++)
        {
            if(Spawnables[i].name.ToLower() == Name.ToLower())
                return Spawnables[i];
        }

        return Spawnables[0];
    }

    public Vector3 NearbyPoint(Vector3 Point, float RespawnRadius)
    {
        if(RespawnRadius >= 200f)
            return GetNearestSpawn(Point).position;
        
        Vector3 OffsetVector = new Vector3(Random.Range(-RespawnRadius, RespawnRadius), 60f, Random.Range(-RespawnRadius, RespawnRadius));
        OffsetVector += new Vector3(Point.x, 0f, Point.z);

        RaycastHit hit;

        if (Physics.Raycast(OffsetVector, -Vector3.up, out hit, 80f))
        {
            //Debug.DrawRay(OffsetVector,-Vector3.up * 80f, Color.red, 3f);
            
            if(hit.transform.gameObject.CompareTag("Ground") && hit.point.y > 1f)
                return hit.point;
            else if(hit.point.y <= 1f)
                return NearbyPoint(Point, RespawnRadius * 2f);   //if land in water
            else
                return NearbyPoint(Point, RespawnRadius);        //if not hit land object
        }else
        {
            return NearbyPoint(Point, RespawnRadius * 2f);       //if hit nothing
        }
    }

    public Transform GetNearestSpawn(Vector3 Point)
    {
        int MinDistIndex = 0;
        float MinDist = Vector3.Distance(Point, SpawnPoints[0].position);

        for (int i = 0; i < SpawnPoints.Count; i++)
        {
            if (Vector3.Distance(Point, SpawnPoints[i].position) < MinDist)
            {
                if (!Physics.CheckSphere(SpawnPoints[i].position + new Vector3(0f, 1f, 0f), 0.5f))
                {
                    MinDistIndex = i;
                    MinDist = Vector3.Distance(Point, SpawnPoints[i].position);
                }
            }
        }

        return SpawnPoints[MinDistIndex];
    }

    public Transform GetNearestVehicleSpawn(Vector3 Point)
    {
        int MinDistIndex = 0;
        float MinDist = Vector3.Distance(Point, VehicleSpawnPoints[0].position);

        for(int i = 0; i < VehicleSpawnPoints.Count; i++)
        {
            if(Vector3.Distance(Point, VehicleSpawnPoints[i].position) < MinDist)
            {
                MinDistIndex = i;
                MinDist = Vector3.Distance(Point, VehicleSpawnPoints[i].position);
                /*
                if(!Physics.CheckSphere(VehicleSpawnPoints[i].position + new Vector3(0f, 1f, 0f), 1f))
                {
                    MinDistIndex = i;
                    MinDist = Vector3.Distance(Point, VehicleSpawnPoints[i].position);
                }*/
                
            }
        }

        return VehicleSpawnPoints[MinDistIndex];
    }
}
