using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class RandomEventSpawner : MonoBehaviour
{
    
    [System.Serializable]
    public struct PossibleEvent
    {
	    public int PercentChance;
        public GameObject EventPrefab;
    }

    public int OutOf = 100;

    public List<PossibleEvent> Events;
    GameObject EventReference;

    //ServerManager Server;

    public void SpawnEvent()
    {
        /*
        GM = GameObject.FindWithTag("Manager").GetComponent<GameManager>();
        
        if(EventReference != null)
            Destroy(EventReference);

        int Guess = Random.Range(1, OutOf + 1);   //Random guess out of 100 (1 - 100)
        Guess = GetGuessIndex(Guess);       //Find what it landed on
        if(Guess != -1)           //If it landed on something
        {
            EventReference = Instantiate(Events[Guess].EventPrefab, this.transform.position, this.transform.rotation);
            //NetworkServer.Spawn(EventReference, GM.NC);
        }
        */
        
    }

    int GetGuessIndex(int Guess) //return index value of guess
    {
        int TotalOddsOfSpawn = 0;
        foreach(PossibleEvent E in Events)
        {
            TotalOddsOfSpawn += E.PercentChance;
        }

        if(TotalOddsOfSpawn > OutOf)              //if there are some events that cant spawn because their odds get pushed out of bounds
        {
            Debug.LogError("Total Odds of Spawnable events greater than 100%", this.gameObject);
            return -1;
        }

        //Debug.Log("Total Odds Of Spawn : " + TotalOddsOfSpawn);

        if(Guess > TotalOddsOfSpawn)    //if guess is out of range of all odds
            return -1;

        bool IndexFound = false;
        int IndexValue = 0;
        while(!IndexFound)
        {
            if(Guess > Events[IndexValue].PercentChance)
            {
                Guess -= Events[IndexValue].PercentChance;
                IndexValue++;
            }else
            {
                IndexFound = true;
            }
        }

        //Debug.Log(Events[IndexValue].PercentChance);  //Used to get odds of gameobject spawns as they occure

        return IndexValue;
    }
}
