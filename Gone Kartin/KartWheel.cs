using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="New Kart Wheel", menuName = "Kart Wheel")]
public class KartWheel : ScriptableObject
{
    public GameObject Prefab;
    public string Name;
    public float RideHeightOffset = 0f;
    public float Acceleration;
}
