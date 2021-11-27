  
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="New Engine Kit", menuName = "Engine Kit")]
public class EngineKit : ScriptableObject
{
    public float Acceleration = 4000f;
    public float TopSpeed = 15f;
    public float GearSwapTime = 0.1f;

    public int Cost = 0;
}