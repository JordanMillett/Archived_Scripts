using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestSpawner : MonoBehaviour
{
    public GameObject Unit;

    public float spawnSize = 10f;
    public int spawnAmount = 1;

    void Update()
    {
        if(Input.GetKey("e"))
        {
            for(int i = 0; i < spawnAmount; i++)
                Spawn();

        }        
    }

    public void Spawn()
    {
        Vector3 spawnOffset = new Vector3(Random.Range(-spawnSize, spawnSize), 0f, Random.Range(-spawnSize, spawnSize));

        GameObject newUnit = Instantiate(Unit, this.transform.position + spawnOffset, Quaternion.Euler(0f, Random.Range(0f, 360f), 0f));
        newUnit.transform.parent = this.transform;
        newUnit.name = "Unit";

    }
}
