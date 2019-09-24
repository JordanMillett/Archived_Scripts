using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="New Player Weapon", menuName = "Player Weapon")]
public class Player_Weapon : ScriptableObject
{
    
    public string Name;
    public float Damage;
    public float RPM;
    public float Recoil;
    public float Accuracy;
    public float ADS_Fov;
    public float Velocity;
    public float Range;
    public int Bullets;
    public int Clip_Size;
    public bool Automatic;
    public GameObject Bullet;

}
