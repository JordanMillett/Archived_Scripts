using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="New Inventory Item", menuName = "Inventory Info")]
public class InvInfo : ScriptableObject
{
    public Vector2 TileSize;
    public Texture Icon;
    public List<Vector2> UsedLocations; //anchored in upper left
}
