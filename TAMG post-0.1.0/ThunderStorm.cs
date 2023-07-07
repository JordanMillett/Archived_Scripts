using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThunderStorm : MonoBehaviour
{
    public GameObject LightningPrefab;
    public float DelayBetweenLightning = 1f;

    public float SpawnArea = 800f;

    void Start()
    {
        InvokeRepeating("Strike", DelayBetweenLightning, DelayBetweenLightning);
    }

    void Strike()
    {
        Vector3 SpawnPos = new Vector3(Random.Range(-SpawnArea, SpawnArea), 70f, Random.Range(-SpawnArea, SpawnArea));
        Instantiate(LightningPrefab, SpawnPos, Quaternion.identity);
    }
}
