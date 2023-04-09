using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="New Valuable", menuName = "Items/Valuable")]
public class Valuable : Item
{
    public enum Types
    {
        Junk,
        Valuable
    }
    
    public Types Type;
    
    public int Value;
}