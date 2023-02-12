using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="New Weapon Info", menuName = "Weapon Info")]
public class WeaponInfo : ScriptableObject
{
    [System.Serializable]
    public struct Capabilities
    {
        public bool AntiInfantry;
        public bool AntiArmor;
        public bool AntiAir;
    };

    public float RPM = 600f;
    public float MuzzleVelocity = 250f;
    public float ReloadTime = 2f;

    public bool Automatic = false;
    public float AccurateTime = 1f;

    public float FOV = 40;
    
    public float Accuracy = 100f;
    public float RecoilMultiplier = 1f;
    public float DecalScale = 1f;
    public int MagazineSize = 5;
    
    public int Damage = 25;
    public Vector2 Caliber;

    public Capabilities UseCase;

    public bool Flak = false;
    public float FlakDistanceMin = 300f;
    public float FlakDistanceMax = 400f;

    public bool Explosive = false;
    public float ExplosiveSize = 1f;
    public int ExplosiveDamage = 10;

    public SoundGroup FireSounds;
    public float FirePitch = 1f;
    public float DecalVolume = 0.5f;

    public int AnimationIndex = 0;
    public GameObject Prefab;
    public GameObject ProjectilePrefab;
    
    public Weapon CreateWeapon(Transform T, bool Inaccurate)
    {
        GameObject Created = Instantiate(Prefab, T.position, T.rotation);
        Created.transform.SetParent(T);

        Weapon Gun = Created.GetComponent<Weapon>();

        if(Inaccurate)
            Gun.AttackerInaccuracy = true;

        Gun.Info = this;

        return Gun;
    }

    public float GetMuzzleVelocity()
    {
        return Mathf.Clamp(MuzzleVelocity/14f, 35f, 40f); //30f - 40f
    }
}