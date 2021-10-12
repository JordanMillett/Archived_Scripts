using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Road_Mesh : MonoBehaviour
{
    public Road Associated;
    public int CarCount = 0;

    public void SpawnCars(Manager M)
    {

        for(int i = 0; i < CarCount; i++)
        {
            GameObject NewCar = Instantiate(M.Cars[Random.Range(0, M.Cars.Count)], GetRandomPosition(), Quaternion.Euler(0f, Random.Range(-180f, 180f), 0f));
            NewCar.GetComponent<Driver>().CurrentRoad = Associated;
        }

    }

    Vector3 GetRandomPosition()
    {
        float Spread = 4f;
        return Associated.Nodes[Random.Range(0, Associated.Nodes.Count)].Pos + new Vector3(Random.Range(-Spread, Spread), 2f, Random.Range(-Spread, Spread));
    }
}

/*

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Road_Mesh : MonoBehaviour
{
    public Road Associated;
    public int CarCount = 0;

    public void SpawnCars(Manager M)
    {

        for(int i = 0; i < CarCount; i++)
        {
            //int Ran = Random.Range(0, Associated.Nodes.Count);
            //GameObject NewCar = Instantiate(M.Cars[Random.Range(0, M.Cars.Count)], Associated.Nodes[Ran].Pos + new Vector3(0f, 3f, 0f), Quaternion.LookRotation(Associated.Nodes[Ran].PerpDir, Associated.Nodes[Ran].UpDir));
            NewCar.GetComponent<Driver>().CurrentRoad = Associated;
        }

    }
    
    
    Transform GetRandom()
    {
        //return Associated.Nodes[Random.Range(0, Associated.Nodes.Count)];
        //float Spread = 5f;
        //return Associated.Nodes[Random.Range(0, Associated.Nodes.Count)].Pos + new Vector3(Random.Range(-Spread, Spread), 2f, Random.Range(-Spread, Spread));
    }
}
*/