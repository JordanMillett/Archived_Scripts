using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Upgrades
{
    //all the upgrade values
    public static int MaxUpgradeLevel = 3;
    
    public static int UpgradeCostPerLevel_Save = 75;
    public static int UpgradeCostPerLevel_Raider = 100;
    public static int UpgradeCostPerLevel_Weapon = 75;

    //Save
    public static int Starting_HoldingCredits = 100;
    public static int Starting_HealingMatter =  500;
    public static float Starting_SavedCreditsOnDeath = 0.25f;

    public static int IncreasePerLevel_HoldingCredits = 50;
    public static int IncreasePerLevel_HealingMatter =  50;
    public static float IncreasePerLevel_SavedCreditsOnDeath = 0.25f;

    //Raider
    public static int Starting_Stamina =  1000;
    public static int Starting_Health =   1000;
    public static int Starting_Shields =  500;
    
    public static int IncreasePerLevel_Stamina = 250;
    public static int IncreasePerLevel_Health = 250;
    public static int IncreasePerLevel_Shields = 200;

    //Weapon
    public static int DamageIncreasePerLevel_Rifle =   40;
    public static int DamageIncreasePerLevel_Carbine = 50;
    public static int DamageIncreasePerLevel_Pistol =  30;
    public static int DamageIncreasePerLevel_Shotgun = 25;
    public static int DamageIncreasePerLevel_Compact = 30;
    public static int DamageIncreasePerLevel_Sniper =  75;
    
    public static int FirerateIncreasePerLevel_Rifle =   25;
    public static int FirerateIncreasePerLevel_Carbine = 25;
    public static int FirerateIncreasePerLevel_Pistol =  25;
    public static int FirerateIncreasePerLevel_Shotgun = 25;
    public static int FirerateIncreasePerLevel_Compact = 25;
    public static int FirerateIncreasePerLevel_Sniper =  25;
    
    public static int GetDamageBonus(WeaponTypes Type)//used for player only
    {
        switch(Type)
        {
            case WeaponTypes.Rifle :   return Game.SaveData.WeaponDamageUpgrades[(int) WeaponTypes.Rifle] *    DamageIncreasePerLevel_Rifle;
            case WeaponTypes.Carbine : return Game.SaveData.WeaponDamageUpgrades[(int) WeaponTypes.Carbine] *  DamageIncreasePerLevel_Carbine;
            case WeaponTypes.Pistol :  return Game.SaveData.WeaponDamageUpgrades[(int) WeaponTypes.Pistol] *   DamageIncreasePerLevel_Pistol;
            case WeaponTypes.Shotgun : return Game.SaveData.WeaponDamageUpgrades[(int) WeaponTypes.Shotgun] *  DamageIncreasePerLevel_Shotgun;
            case WeaponTypes.Compact : return Game.SaveData.WeaponDamageUpgrades[(int) WeaponTypes.Compact] *  DamageIncreasePerLevel_Compact;
            case WeaponTypes.Sniper :  return Game.SaveData.WeaponDamageUpgrades[(int) WeaponTypes.Sniper] *   DamageIncreasePerLevel_Sniper;
        }

        return 0;
    }
    
    public static float GetRPMBonus(WeaponTypes Type)//used for players only
    {
        switch (Type) //player upgrades are applied over that
        {
            case WeaponTypes.Rifle:   return Game.SaveData.WeaponFirerateUpgrades[(int)WeaponTypes.Rifle] *    FirerateIncreasePerLevel_Rifle;
            case WeaponTypes.Carbine: return Game.SaveData.WeaponFirerateUpgrades[(int)WeaponTypes.Carbine] *  FirerateIncreasePerLevel_Carbine;
            case WeaponTypes.Pistol:  return Game.SaveData.WeaponFirerateUpgrades[(int)WeaponTypes.Pistol] *   FirerateIncreasePerLevel_Pistol;
            case WeaponTypes.Shotgun: return Game.SaveData.WeaponFirerateUpgrades[(int)WeaponTypes.Shotgun] *  FirerateIncreasePerLevel_Shotgun;
            case WeaponTypes.Compact: return Game.SaveData.WeaponFirerateUpgrades[(int)WeaponTypes.Compact] *  FirerateIncreasePerLevel_Compact;
            case WeaponTypes.Sniper:  return Game.SaveData.WeaponFirerateUpgrades[(int)WeaponTypes.Sniper] *   FirerateIncreasePerLevel_Sniper;
        }

        return 0f;
    }
    
    public static int GetUpgradeCost(Raider.UpgradeType Type, Raider Chosen)
    {
        if(Chosen.RaiderUpgrades[(int)Type] >= MaxUpgradeLevel)
            return 0;
        return (Chosen.RaiderUpgrades[(int)Type] + 1) * UpgradeCostPerLevel_Raider;
    }
    public static void PurchaseUpgrade(Raider.UpgradeType Type, Raider Chosen)
    {
        Chosen.RaiderUpgrades[(int) Type]++;
        
        Chosen.ApplyRaiderUpgrades();
    }
    
    public static int GetUpgradeCost(Save.UpgradeType Type)
    {
        if(Game.SaveData.SaveUpgrades[(int) Type] >= MaxUpgradeLevel)
            return 0;
        return (Game.SaveData.SaveUpgrades[(int) Type] + 1) * UpgradeCostPerLevel_Save;
    }
    public static void PurchaseUpgrade(Save.UpgradeType Type)
    {
        Game.SaveData.SaveUpgrades[(int) Type]++;
        
        Game.SaveData.ApplySaveUpgrades();
    }
    
    public static int GetUpgradeCost(WeaponTypes Type, bool DMG)
    {
        if(DMG)
        {
            if(Game.SaveData.WeaponDamageUpgrades[(int) Type] >= MaxUpgradeLevel)
                return 0;
            return (Game.SaveData.WeaponDamageUpgrades[(int) Type] + 1) * UpgradeCostPerLevel_Weapon;
        }else
        {
            if(Game.SaveData.WeaponFirerateUpgrades[(int) Type] >= MaxUpgradeLevel)
                return 0;
            return (Game.SaveData.WeaponFirerateUpgrades[(int)Type] + 1) * UpgradeCostPerLevel_Weapon;
        }
    }
    public static void PurchaseUpgrade(WeaponTypes Type, bool DMG)
    {
        if(DMG)
            Game.SaveData.WeaponDamageUpgrades[(int) Type]++;
        else
            Game.SaveData.WeaponFirerateUpgrades[(int)Type]++;
    }
}
