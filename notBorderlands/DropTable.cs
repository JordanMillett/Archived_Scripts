using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="New Drop Table", menuName = "Drop Table")]
public class DropTable : ScriptableObject
{
    public List<GameObject> Items;
    public List<int> ItemChances;
}