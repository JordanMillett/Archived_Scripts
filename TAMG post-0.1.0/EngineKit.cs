using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="New Engine Kit", menuName = "Engine Kit")]
public class EngineKit : ScriptableObject
{
    public float Acceleration = 100f;
    public int Cost = 0;
}