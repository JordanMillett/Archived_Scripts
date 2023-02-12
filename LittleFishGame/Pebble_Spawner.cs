using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pebble_Spawner : MonoBehaviour
{
    public GameObject PebblePrefab;
    public int SpawnCount = 1000;
    public float Speed = 0.25f;
    public int QuantityPerSpawn = 4;
    public Vector2 SpawnZone;
    int Spawned = 0;

    void Start()
    {
        InvokeRepeating("Spawn", Speed, Speed);
    }

    void Spawn()
    {
        for (int i = 0; i < QuantityPerSpawn; i++)
        {
            if (Spawned < SpawnCount)
            {
                Spawned++;
                Vector3 OffsetVector = new Vector3(Random.Range(-SpawnZone.x, SpawnZone.x), 0f, Random.Range(-SpawnZone.y, SpawnZone.y));
                GameObject Pebble = Instantiate(PebblePrefab, this.transform.position + OffsetVector, Quaternion.Euler(Random.Range(-90f, 90f), Random.Range(-90f, 90f), Random.Range(-90f, 90f)));
                Pebble.transform.SetParent(this.transform);
            }
        }
    }
}
