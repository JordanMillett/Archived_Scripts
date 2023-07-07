using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum FireModes
{
    Semi,
    Automatic
}

[CreateAssetMenu(fileName ="New Weapon Stats", menuName = "Weapon Stats")]
public class WeaponStats : ScriptableObject
{
    public float Accuracy;
    public float Range;
    public float RPM;
    public float Damage;
    public int Shots;
    public float BackRecoil;
    public float TurnRecoil;

    public FireModes FireMode;
}