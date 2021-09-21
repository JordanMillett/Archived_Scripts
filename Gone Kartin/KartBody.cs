using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="New Kart Body", menuName = "Kart Body")]
public class KartBody : ScriptableObject
{
    public GameObject Prefab;
    public string Name;
    public float Weight;
    public float MaxSpeed;
    public bool HideWheels = false;
}
