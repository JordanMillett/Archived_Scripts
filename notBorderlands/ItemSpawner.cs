using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemSpawner : MonoBehaviour
{
    public GameObject ToSpawn;

    public float Interval = 1f;

    void Start()
    {

        InvokeRepeating("Spawn", Interval, Interval);

    }

    void Spawn()
    {

        Instantiate(ToSpawn, transform.position, Quaternion.identity);

    }
}
