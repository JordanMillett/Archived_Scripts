using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public int MaxEnemies;
    public GameObject ToSpawn;
    public float SpawnDelay;

    void Start()
    {
        InvokeRepeating("Spawn", SpawnDelay, SpawnDelay);
    }

    void Spawn()
    {
        if(this.transform.childCount < MaxEnemies)
        {
            Vector2 Ran = Random.insideUnitCircle.normalized;

            if(Random.value < 0.5f)
                Ran = -Ran;

            Vector3 RandomPosition = new Vector3(Ran.x, 0f, Ran.y);
            RandomPosition *= 100f;
            
            GameObject Enemy = Instantiate(ToSpawn, RandomPosition, Quaternion.identity);
            Enemy.transform.SetParent(this.transform);
        }
    }
    
    /*
    public float Remap(float Value, float FromMin, float ToMin, float FromMax, float ToMax) 
    {
        return (Value - FromMin) / (ToMin - FromMin) * (ToMax - FromMax) + FromMax;
    }
    */

}
