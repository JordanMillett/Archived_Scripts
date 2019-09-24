using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="New Ship Weapon", menuName = "Ship Weapon")]
public class Weapon : ScriptableObject
{

    public string Name;
    public float Damage;
    public float RPM;
    public float Ship_Shake;
    public float Accuracy;
    public float Velocity;
    public float Range;
    public int Bullets;
    public bool Automatic;
    public GameObject Bullet;
    public int PowerCost;

}
