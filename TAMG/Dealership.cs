using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Dealership : MonoBehaviour
{
    public int CarChanceOutOf = 100;

    public List<string> Cars;

    public Transform ParkingSpotParent;
    
    List<Transform> ParkingSpots = new List<Transform>();

    GameManager GM;

    bool Initialized = false;

    public void SpawnEvent()
    {
        GM = GameObject.FindWithTag("Manager").GetComponent<GameManager>();

        if(!Initialized)
        {
            foreach (Transform child in ParkingSpotParent)
            {
                ParkingSpots.Add(child);
            }

            Initialized = true;
        }

        for (int i = 0; i < ParkingSpots.Count; i++)
        {
            int Guess = Random.Range(1, CarChanceOutOf + 1);  
            //Debug.Log(Guess);
            if(Guess == 1)
            {
                MakeCar(i);
            }
        } 
    }

    void MakeCar(int Index)
    {
        GameServer.GS.SpawnObject(Cars[Random.Range(0, Cars.Count)], ParkingSpots[Index].position, ParkingSpots[Index].rotation, true, false);
    }
}
