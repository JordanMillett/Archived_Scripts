using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="New Faction", menuName = "Faction")]
public class Faction : ScriptableObject
{
    public GameObject RegularPrefab;
    public GameObject RusherPrefab;
    public GameObject SniperPrefab;

    public int StartingHealth = 500;
    public int StartingShields = 0;

    public int HealthPerLevel = 100;
    public int ShieldsPerLevel = 100;
    
    public int GetHealth(int Level)
    {
        return StartingHealth + ((Level - 1) * HealthPerLevel);
    }
    
    public int GetShields(int Level)
    {
        return StartingShields + ((Level - 1) * ShieldsPerLevel);
    }
}
