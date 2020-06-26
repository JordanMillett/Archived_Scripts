using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="New Gun Info", menuName = "Gun Info")]
public class GunInfo : ScriptableObject
{
    public float Damage;
    public float Accuracy;
    public float ProjectileVelocity;
    public float ReloadTime;
    
    public float RecoilBackAmount;      //Total Recoil Distance
    public float RecoilRecoverySpeed;   //Lerp Recovery Speed (Below 1)

    public float MaxAccuracyPenalty;
    public float AccuracyPenaltyPerShot;//Lerp Recovery Speed (Below 1) Must be larger than recovery speed
    public float AccuracyRecoverySpeed; //Lerp Recovery Speed (Below 1)

    public float MaxUpwardsRecoil;
    public float UpwardsRecoilPerShot; //Lerp Recovery Speed (Below 1) Must be larger than recovery speed
    public float UpwardsRecoilRecovery;//Lerp Recovery Speed (Below 1)

    public int ProjectileCount;
    public int MagazineSize;
    public int AmmoUsePerShot;
    public int InitialReserveAmmo;
    public int MaxReserveAmmo;
    public int AmmoPickupAmount;

    public List<AudioClip> FireSounds;

    //Animation Curve Damage Falloff

}
