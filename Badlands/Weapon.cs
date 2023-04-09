using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="New Weapon", menuName = "Items/Weapon")]
public class Weapon : Item
{   
    public FireModes Firemode;
    public WeaponTypes Type;
    
    public GameObject Prefab;
    
    public int Damage = 25;
    public float RPM = 120f; //120 is half second delay
    public ProjectileSize Size = ProjectileSize.Regular;
    public ProjectileSpeed Speed = ProjectileSpeed.Regular;
    
    public float MaxRandomOffset = 0f;
    
    public int StartSpray = 0; //reset barrel index after amount of time
    public int SprayMax = 0;  //go max -15 to 15
    public int SprayInterval = 0; //count by 15

    public int BurstWaves = 1; 
    public bool ShootRightToLeft = false;
    public bool AllAtOnce = false;
    
    public DamageBonus Bonus = DamageBonus.None;
    
    //calculate damage
    public int CalculateDamage(bool PlayerShot)//check weapon upgrades or check station damage multiplier
    {
        if (PlayerShot)
        {
            int Total = Damage;

            Total += Upgrades.GetDamageBonus(Type);

            return Total;
        }
        else
        {
            return Mathf.RoundToInt(Damage * Game.StationData.DamageMultiplier);
        }
    }
    
    public float GetRPMDelay(bool PlayerShot)
    {
        float RealRPM = (float)RPM;

        if (PlayerShot)
        {
            if (Firemode == FireModes.Semi)
                RealRPM *= 1.25f; //player can fire base semi auto 1.25x as fast as AI can

            RealRPM += Upgrades.GetRPMBonus(Type);
        }

        return (60f/RealRPM);
    }
}