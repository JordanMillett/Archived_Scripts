using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="New Tileset", menuName = "Tileset")]
public class Tileset : ScriptableObject
{
    public string TilesetName = "New Tileset";

    //public Dictionary<Tile.Types, GameObject> Tiles = new Dictionary<Tile.Types, GameObject>();
    
    public GameObject[] Tiles = new GameObject[System.Enum.GetValues(typeof(Tile.Types)).Length];
    
    
}
/*
public enum Types
    {
        Ocean,
        Field,
        Woods,
        Mountain,
        Road,
        Building,
        Relay,
        Factory,
        City,
    }
    */