using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="New Plane Info", menuName = "Plane Info")]
public class PlaneInfo : ScriptableObject
{
    [System.Serializable]
    public struct Gear
    {   
        public float Speed;
        public float Turn;
    }
    
    public Gear Dogfight;
    public Gear Normal;
    public Gear Strafe;
}
