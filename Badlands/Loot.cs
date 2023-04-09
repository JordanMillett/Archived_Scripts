using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="New Loot", menuName = "Loot")]
public class Loot : ScriptableObject
{
    public List<Item> Items;
}