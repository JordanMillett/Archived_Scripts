using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="New Weapon Config", menuName = "Weapon Config")]
public class WeaponConfig : ScriptableObject
{
    public float Accuracy = 95f;
    public int RPM = 500;
    public int FireCount = 1;
    public int MaxDamage = 5;
    public float MaxRange = 50f;
    public AnimationCurve DamageFalloff;

    public float ProjectileScale = 1f;

    public float FirePitch = 1f;
}