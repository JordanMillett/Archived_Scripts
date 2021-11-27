using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChanceToSpawn : MonoBehaviour
{
    public GameObject ToSpawn;
    public Transform Location;
    public Transform LocationTwo;
    public int OneIn = 100;

    void Start()
    {
        int Guess = Random.Range(1, OneIn + 1);
        if(Guess == Random.Range(1, OneIn + 1))
            Instantiate(ToSpawn, Location.position, Quaternion.identity);
        if(Guess == Random.Range(1, OneIn + 1))
            Instantiate(ToSpawn, LocationTwo.position, Quaternion.identity);
    }
}
