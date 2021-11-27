using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class NetworkObjectPlaceholder : MonoBehaviour
{
    //public GameObject Prefab;
    public string PrefabName;

    float MinSpawnDelay = 0.5f;
    float MaxSpawnDelay = 2f;

    void Start()
    {
        //GameManager GM = GameObject.FindWithTag("Manager").GetComponent<GameManager>();

        //GameServer.GS.SpawnObject(PrefabName, this.transform.position, this.transform.rotation, false);

        //GameObject _prefab = Instantiate(Prefab, this.transform.position, this.transform.rotation);
        //NetworkServer.Spawn(_prefab, GameServer.GS.GameManagerInstance.NC);

        Invoke("Spawn", Random.Range(MinSpawnDelay, MaxSpawnDelay));
    }

    void Spawn()
    {
        GameServer.GS.SpawnObject(PrefabName, this.transform.position, this.transform.rotation, true, false);
    }
}
