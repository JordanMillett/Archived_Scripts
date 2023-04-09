using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="New Loot Configuration", menuName = "Loot Configuration")]
public class LootConfiguration : ScriptableObject
{
    public Loot Weapons;
    public Loot Medicine;
    public Loot Valuables;
    public Loot Special;

    public int SpecialChance = 5;
    public int WeaponChance = 10;
    public int ValuableChance = 15;
    public int MedicineChance = 15;

    public Item GetLoot()
    {
        float Value = Random.value * 100f;
        
        if(Value < SpecialChance)
            return Special.Items[Random.Range(0, Special.Items.Count)];
        if(Value < SpecialChance + WeaponChance)
            return Weapons.Items[Random.Range(0, Weapons.Items.Count)];
        if(Value < SpecialChance + WeaponChance + ValuableChance)
            return Valuables.Items[Random.Range(0, Valuables.Items.Count)];
        if(Value < SpecialChance + WeaponChance + ValuableChance + MedicineChance)
            return Medicine.Items[Random.Range(0, Medicine.Items.Count)];

        return null;
    }
}