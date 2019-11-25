using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="New Weapon Config", menuName = "Weapon Config")]
public class WeaponConfig : ScriptableObject
{
    public float Damage;
    public float RPM;
    public float Accuracy;
    public float Velocity;
    public float Range;
    public float RecoilAmount;
    public float Force;
    public float ReloadTime;
    public int Bullets;
    public int MagSize;
    public bool Automatic;

    public GameObject DefaultGun;
}
