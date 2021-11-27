using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarSpawn : MonoBehaviour
{
    void OnEnable()
    {
        Debug.Log("Strange Script Called");
        //GameObject.FindWithTag("Player").GetComponent<PlayerController>().PG.SpawnVehicle(null);
    }
}
