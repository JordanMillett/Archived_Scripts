using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class ItemSpawner : MonoBehaviour
{
    public int AmountPerSpawn;
    public float TimeBetweenSpawns;
    public float SpawnRadius;
    public string[] Items;

    int Max = 50;

    int Current = 0;


    //OnlineManager OM;

    public void TurnOn()
    {
        //OM = GameObject.FindWithTag("Information").GetComponent<OnlineManager>();
        InvokeRepeating("Spawn", TimeBetweenSpawns, TimeBetweenSpawns);
    }

    void Spawn()
    {
        if(Current < Max)
        {
            for(int i = 0;i < AmountPerSpawn; i++)
            {
                int Index = Random.Range(0,Items.Length);
                //GameObject Item = Instantiate(Items[Index], GetPointOnCircle(), Random.rotation);
                GameServer.GS.SpawnObject(Items[Index], GetPointOnCircle(), Random.rotation, true, false);
                //NetworkServer.Spawn(Item, NC);
                Current++;
            }
        }
    }

    Vector3 GetPointOnCircle()
    {
        Vector3 OffsetVector = new Vector3(Random.Range(-SpawnRadius,SpawnRadius),0f,Random.Range(-SpawnRadius,SpawnRadius));
        return this.transform.position + OffsetVector;
    }
}