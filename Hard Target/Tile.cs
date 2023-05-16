using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
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
    };
    
    public Types Type;
}
