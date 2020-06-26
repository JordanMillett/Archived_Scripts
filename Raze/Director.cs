using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Director : MonoBehaviour
{
    public float TimeToReachMadness = 4f;   //Minutes
    public float CurrentMadness = 0f;       //Lerp value (0f - 1f)
    public Vector2Int EnemyCount = new Vector2Int(0, 15);
    public Vector2 EnemyHealth = new Vector2(0.1f, 5f);

    /*
    Difficult ramps up over time and this script controls 
        -What enemies spawn
        -How many enemies spawn
        -How much ammo, health, and shields spawn
        -What effects start appearing (Fog, Skybox, Sounds, Post Processing)
        -Start spawning different props/swapping them out for different versions
            -Generic statues turn into satanic ones?
            -Lights turn red over time
            -Decals appear with symbols/spooky stuff
        -At high sanity things can disappear if you look away?
    */
}
