using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class ItemSpawner : MonoBehaviour
{
    public int Max = 50;
    public int AmountPerSpawn;
    public float TimeBetweenSpawns;
    public float SpawnRadius;
    public string[] Items;

    public int Current = 50;

    void Start()
    {
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
                Client.Instance.SpawnObject(Items[Index], Raydown(), Random.rotation, 1, false);
                //NetworkServer.Spawn(Item, NC);
                Current++;
            }
        }
    }

    Vector3 Raydown()
    {
        RaycastHit hit;

        if(Physics.Raycast(GetPointOnCircle(), -Vector3.up, out hit, 30f))
        {
            return hit.point + new Vector3(0f, 1f, 0f);
        }else
        {
            return GetPointOnCircle();
        }
    }

    Vector3 GetPointOnCircle()
    {
        Vector3 OffsetVector = new Vector3(Random.Range(-SpawnRadius,SpawnRadius),0f,Random.Range(-SpawnRadius,SpawnRadius));
        return this.transform.position + OffsetVector;
    }
}