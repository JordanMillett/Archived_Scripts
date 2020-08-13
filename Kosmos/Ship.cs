using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="New Ship", menuName = "Ship")]
public class Ship : ScriptableObject
{
    public GameObject ShipPrefab;
    //public list weapons;
    public Vector3 CameraPosition;
    public string Name;
    public float Hull;
    public float Shields;
    public float ShieldsRechargeRate;
    public float Energy;
    public float EnergyRechargeRate;
    public float Speed;
    public float TurnSpeed;
    
    //Separate weapons into list of scriptable gameobject with different attributes like model/color/damage/cost

    public float WeaponsRange;
    public float StopDistance;

    public float LaserDamage;
    public float LaserCost;
    public float LaserCoolDownTime;
    public float LaserVelocity;

    public float TargetMarkerScale;

    public int Cost;
}
