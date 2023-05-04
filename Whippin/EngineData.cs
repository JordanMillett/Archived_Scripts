using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="New Engine", menuName = "Engine")]
public class EngineData : ScriptableObject
{
    public string Manufacturer;

    public float Displacement;
    public int Cylinders;

    public float Horsepower;

    public float IdleRPM;
    public float DangerRPM;
    public float MaxRPM;

    public AnimationCurve PowerCurve;
    public AnimationCurve TorqueCurve;
}
