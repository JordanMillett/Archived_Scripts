using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="New AI Class", menuName = "Class")]
public class AIClass : ScriptableObject
{
   
   public string Name;

   public int Health;
   public float RespawnTime;
   public int BurstAmount;
   public float Accuracy;

   public int DamageStrength;

   public Color GoodFireColor;
   public Color BadFireColor;

   public float FireColorStrength;
    
}
