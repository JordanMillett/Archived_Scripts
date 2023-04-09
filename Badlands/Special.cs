using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="New Special", menuName = "Items/Special")]
public class Special : Item
{
    public enum Types
    {
        FuelRod
    }
    
    public Types Type;
}