using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="New Infantry Kit", menuName = "Infantry Kit")]
public class InfantryKit : ScriptableObject
{
    [HideInInspector]
    public WeaponInfo Primary;

    [HideInInspector]
    public WeaponInfo Secondary;
    
    public List<WeaponInfo> Primaries;
    public List<WeaponInfo> Secondaries;

    public Weapon CreatePrimary(Transform T, Faction F)
    {
        Primary = Primaries[Random.Range(0, Primaries.Count)];

        return Primary.CreateWeapon(T, Game.IsInaccurate(F));
    }

    public Weapon CreateSecondary(Transform T, Faction F)
    {
        Secondary = Secondaries[Random.Range(0, Secondaries.Count)];

        return Secondary.CreateWeapon(T, Game.IsInaccurate(F));
    }
}
