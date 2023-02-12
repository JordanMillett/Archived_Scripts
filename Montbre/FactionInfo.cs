using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="New Faction Info", menuName = "Faction Info")]
public class FactionInfo : ScriptableObject
{
    public Faction Name;
    public string Prefix;

    public GameObject InfantryPrefab;
    public GameObject HeavyTankPrefab;
    public GameObject LightTankPrefab;
    public GameObject FighterPlanePrefab;
    public GameObject CargoPlanePrefab;

    public Texture2D Flag;
    public Texture2D Symbol;
    public List<InfantryKit> PossibleKits;
}
