using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Planet : MonoBehaviour
{

    public string PlanetName = "Planet Name";

    public int PlayerReputation = 0;
    public bool PlayerOwned = false;

    public float PlanetCost;
    public float PlanetIncome;

    public TextMeshProUGUI Name;

    string[] PossibleNames = new string[]
    {
        "Earth", "Coruscant", "Dagobah", "Betazed", "Bajor", "Cardassia", "Risa", "Vulcan", "Romulus", "Tatooine", "Hoth", "Alderaan", 
        "Bespin", "Mustafar", "Yavin", "Geonosis", "Kamino", "Mandalore", "Aegis 7", "Omega", "Ragnarok", "Vekta", "Trantor", "Solaris",
        "Reach", "Pandora", "Genesis", "Ulara", "Aetis", "Rorus", "Crait", "Endor", "Jakku", "Kessel", "Mygeeto", "Naboo",
        "Cetus", "Daran 5", "Epsilon 4", "Gothos", "Harlak", "Hindmar", "Iconia", "Jaros 2", "Kataan", "Minos", "Nimbus 3", "Omicron 4",
        "Orion", "Pandro", "Quarra", "Talos 2", "Taurus 4", "Theta", "Trill"
    };

    public List<Ship> ShipPossibilities;
    public List<Ship> Shipyard;

    /*
        Deliver Cargo to nearby planets to improve reputation
    */
    
    void Start()
    {
        int Count = Random.Range(1, 5);

        for(int i = 0; i < Count; i++)
        { 
            Shipyard.Add(ShipPossibilities[Random.Range(0, ShipPossibilities.Count)]);
        }

        PlanetName = PossibleNames[Random.Range(0, PossibleNames.Length)];
        Name.text = PlanetName;
    }
}
