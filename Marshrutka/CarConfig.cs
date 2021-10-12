using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="New Car Config", menuName = "Car Config")]
public class CarConfig : ScriptableObject
{
    public float CarWeight = 400f;
    public float PitchLow = 1f;
    public float PitchHigh = 3.5f;
    public float MotorForce = 200f;
    public float BrakeForce = 500f;
}
