using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="New Medicine", menuName = "Items/Medicine")]
public class Medicine : Item
{
    public enum Types
    {
        Food,
        Medicine
    }
    
    public Types Type;
    
    public int Health;
}