using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="New Projectile", menuName = "Projectile")]
public class Projectile : ScriptableObject
{
    
    public string Name;
    public int Damage;
    public bool Explosive;
    public int DestroyTime;
    public int PowerCost;

}
