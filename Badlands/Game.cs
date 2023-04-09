using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public enum DamageBonus
{
    None,
    Health,
    Shields
}

public enum ProjectileSize
{
    Small,    
    Regular,
    Large,  
    Huge      
}

public enum ProjectileSpeed
{
    Slow,    
    Regular, 
    Fast
}

public enum FireModes
{
    Semi,
    Auto,
    Burst
}
    
public enum WeaponTypes
{
    Rifle,
    Carbine,
    Pistol,
    Shotgun,
    Compact,
    Sniper
}

public class Station
{
    public enum Level
    {
        Starter,
        Easy,
        Regular,
        Hard,
        Difficult,
        Impossible
    }
    
    public int MaxBlueprints = 3;
    public int BlueprintTime = 30;
    public int EnemyStartingLevel = 1;
    public float ValuableMultiplier = 1f;
    public float DamageMultiplier = 1f;
    
    public Station(int _blueprints, int _blueprintTime, int _enemyLevel, float _creditsMult, float _damageMult)
    {
        this.MaxBlueprints = _blueprints;
        this.BlueprintTime = _blueprintTime;
        this.EnemyStartingLevel = _enemyLevel;
        this.ValuableMultiplier = _creditsMult;
        this.DamageMultiplier = _damageMult;
    }
}

[System.Serializable]
public class Raider
{
    public string Name = "";
    public int SuccessfulRuns = 0;
    public int HireDay = 1;
    public int ModelIndex = 0;

    public enum UpgradeType
    {
        IncreaseStamina,
        IncreaseHealth,
        IncreaseShields
    }
    
    public int MaxStamina = Upgrades.Starting_Stamina;
    public int MaxHealth = Upgrades.Starting_Health;
    public int MaxShields = Upgrades.Starting_Shields;
    
    public void Load()
    {
        ApplyRaiderUpgrades();
    }

    public int[] RaiderUpgrades = new int[System.Enum.GetNames(typeof(UpgradeType)).Length];
    public static Dictionary<UpgradeType, string> RaiderUpgradesDefinitions = new Dictionary<UpgradeType, string>()
    {
        {UpgradeType.IncreaseStamina, "Increases this Raider's overall stamina."},
        {UpgradeType.IncreaseHealth,  "Increases this Raider's overall health."},
        {UpgradeType.IncreaseShields, "Increases this Raider's overall shields."}
    };
    public void ApplyRaiderUpgrades()
    {
        MaxStamina = Upgrades.Starting_Stamina + (RaiderUpgrades[(int) UpgradeType.IncreaseStamina] * Upgrades.IncreasePerLevel_Stamina);
        MaxHealth = Upgrades.Starting_Health + (RaiderUpgrades[(int) UpgradeType.IncreaseHealth] * Upgrades.IncreasePerLevel_Health);
        MaxShields = Upgrades.Starting_Shields + (RaiderUpgrades[(int) UpgradeType.IncreaseShields] * Upgrades.IncreasePerLevel_Shields);
    }
}

[System.Serializable]
public class Save
{   
    public int Credits = 0;
    public int Blueprints = 0;
    public int VaultCredits = 0;

    public int Day = 1;

    public List<Raider> Team = new List<Raider>();

    public enum UpgradeType
    {
        MaxHoldingCredits,
        MaxHealingMatter,
        ScannerLevel, //scanner targets, show station levels
        SavedCreditsOnDeath
    }
    
    public int MaxHoldingCredits = Upgrades.Starting_HoldingCredits;
    public int MaxHealingMatter = Upgrades.Starting_HealingMatter;
    public int ScannerLevel = 1;
    public float SavedCreditsOnDeath = Upgrades.Starting_SavedCreditsOnDeath;

    public void Load()
    {
        ApplySaveUpgrades();

        foreach(Raider R in Team)
            R.Load();
    }

    public int[] SaveUpgrades = new int[System.Enum.GetNames(typeof(UpgradeType)).Length];
    public static Dictionary<UpgradeType, string> SaveUpgradesDefinitions = new Dictionary<UpgradeType, string>()
    {
        {UpgradeType.MaxHoldingCredits, "Increases the max credits you can hold before needing to deposit."},
        {UpgradeType.MaxHealingMatter,  "Increases how much healing matter you can hold."},
        {UpgradeType.ScannerLevel, "Increases your scanner level. This will give you more intel on stations."},
        {UpgradeType.SavedCreditsOnDeath, "Increases the percent of credits that go into the vault on death."}
    };
    public void ApplySaveUpgrades()
    {
        MaxHoldingCredits = Upgrades.Starting_HoldingCredits + (SaveUpgrades[(int) UpgradeType.MaxHoldingCredits] * Upgrades.IncreasePerLevel_HoldingCredits);
        MaxHealingMatter = Upgrades.Starting_HealingMatter + (SaveUpgrades[(int) UpgradeType.MaxHealingMatter] * Upgrades.IncreasePerLevel_HealingMatter);
        ScannerLevel = 1 + (SaveUpgrades[(int) UpgradeType.ScannerLevel] * 1);
        SavedCreditsOnDeath = Upgrades.Starting_SavedCreditsOnDeath + (SaveUpgrades[(int) UpgradeType.SavedCreditsOnDeath] * Upgrades.IncreasePerLevel_SavedCreditsOnDeath);
    }
    
    public int[] WeaponDamageUpgrades = new int[System.Enum.GetNames(typeof(WeaponTypes)).Length];
    public int[] WeaponFirerateUpgrades = new int[System.Enum.GetNames(typeof(WeaponTypes)).Length];
}

public class Run
{
    public float StartTime = 0f;
    public float EndTime = 0f;
    public bool Escaped = false;

    public Raider ActiveRaider;

    public int CollectedBlueprints = 0;
    public int HealingMatter = 0;
    public int HoldingCredits = 0;
    public int CreditsDeposited = 0;

    public int FuelRods = 0;
}

public static class Game
{
    public static Dictionary<DamageBonus, Color> CommonColors = new Dictionary<DamageBonus, Color>()
    {
        {DamageBonus.None, new Color(191f / 255f, 152f / 255f, 56f / 255f)},
        {DamageBonus.Health, new Color(191f / 255f, 60f / 255f, 56f / 255f)},
        {DamageBonus.Shields, new Color(56f / 255f, 174f / 255f, 191f / 255f)}
    };
    
    public static Dictionary<string, string> UIColors = new Dictionary<string, string>()
    {
        {"Credits", "ffea00"},
        {"Blueprints", "00e5ff"}
    };

    public static Dictionary<ProjectileSize, float> ProjectileSizes = new Dictionary<ProjectileSize, float>()
    {
        {ProjectileSize.Small, 0.25f},
        {ProjectileSize.Regular, 0.30f},
        {ProjectileSize.Large, 0.35f},
        {ProjectileSize.Huge, 0.40f}
    };
    
    public static Dictionary<Station.Level, Station> StationLevels = new Dictionary<Station.Level, Station>()
    {
        {Station.Level.Starter,         new Station(2, 20, 1, 0.75f, 0.75f)},
        {Station.Level.Easy,            new Station(3, 20, 2, 1.00f, 1.20f)},
        {Station.Level.Regular,         new Station(3, 15, 3, 1.50f, 1.40f)},
        {Station.Level.Hard,            new Station(4, 15, 4, 2.00f, 1.60f)},
        {Station.Level.Difficult,       new Station(5, 10, 5, 2.50f, 1.80f)},
        {Station.Level.Impossible,      new Station(6, 10, 6, 3.00f, 2.00f)},
    };
    
    public static Dictionary<ProjectileSpeed, float> PlayerProjectileSpeeds = new Dictionary<ProjectileSpeed, float>()
    {
        {ProjectileSpeed.Slow, 19f},
        {ProjectileSpeed.Regular, 21f},
        {ProjectileSpeed.Fast, 23f}
    };
    
    public static Dictionary<ProjectileSpeed, float> EnemyProjectileSpeeds = new Dictionary<ProjectileSpeed, float>()
    {
        {ProjectileSpeed.Slow, 15f},
        {ProjectileSpeed.Regular, 17f},
        {ProjectileSpeed.Fast, 19f}
    };
    
    //Fixed values
    public static int EnemySpawnInterval = 8;
    public static int StartingGrace = 8;
    public static int RampUpTime = 32;
    public static float SafeCrackTime = 30f;
    public static int StartingRaiders = 4;
    
    public static Station StationData;
    public static Save SaveData;
    public static Run RunData;

    static string SaveGamePath;
    static Game()
    {
        SaveGamePath = Application.persistentDataPath + "/save.json";
    }
    
    public static void NewSave()
    {
        SaveData = new Save();
        
        TextAsset file = (TextAsset) Resources.Load("Names");
        string[] lines = file.text.Split("\n"[0]);

        for (int i = 0; i < StartingRaiders; i++)
        {
            Raider R = new Raider();
            R.Name = lines[Random.Range(0, lines.Length)];
            
            SaveData.Team.Add(R);
        }

        Debug.Log("Failed to load save. New save created.");

        SaveGame();
    }
    
    public static bool LoadSave()
    {
        if(!File.Exists(SaveGamePath))
            return false;

        try
        {
            string json = File.ReadAllText(SaveGamePath);
            SaveData = JsonUtility.FromJson<Save>(json);

            SaveData.Load();

            Debug.Log("Save loaded.");
            return true;
        }catch
        {
            return false;
        }
    }
    
    public static void SaveGame()
    {
        string json = JsonUtility.ToJson(SaveData);
        File.WriteAllText(SaveGamePath, json);
        Debug.Log("Game saved.");
    }

    public static void StartRun(Vector3 _SpawnPosition)
    {
        RunData = new Run();

        RunData.StartTime = Time.time;

        RunData.ActiveRaider = SaveData.Team[0];

        Player.P.SpawnIn(_SpawnPosition);
        UIManager.UI.SetScreen("Game");
    }

    public static void EndRun(bool _Escaped)
    {
        RunData.Escaped = _Escaped;
        RunData.EndTime = Time.time;
        
        if(RunData.Escaped)
        {
            SaveData.Blueprints += RunData.CollectedBlueprints;
            SaveData.Credits += RunData.CreditsDeposited;
            RunData.ActiveRaider.SuccessfulRuns++;
            
            SaveData.Day++;
            SaveGame();
        }else
        {
            Game.SaveData.VaultCredits += Mathf.RoundToInt((Game.RunData.HoldingCredits + Game.RunData.CreditsDeposited) * Game.SaveData.SavedCreditsOnDeath);
            if(SaveData.Team.Count > 1)
            {
                SaveData.Team.Remove(RunData.ActiveRaider);
                
                SaveData.Day++;
                SaveGame();
            }else
            {
                Debug.Log("All lives lost, wiping save");
                NewSave();
            }
        }
        
        //Generate recent space news using location, and outcome. also make it work for events that occur outside of runs. like decision phase events,

        UIManager.UI.SetScreen("Results");
    }
    
    public static void Deposit()
    {
        RunData.CreditsDeposited += RunData.HoldingCredits;
        RunData.HoldingCredits = 0;
    }

    public static string GetTimeFormatted(float Time)
    {
        string Formatted = "";

        int Minutes = (int)Mathf.Floor((Time) / 60);
        string MinutesSpacer = "";
        if (Minutes < 10)
            MinutesSpacer = "0";

        int Seconds = (int)Mathf.Floor(((Time) % 60));
        string SecondsSpacer = "";
        if (Seconds < 10)
            SecondsSpacer = "0";

        int Milliseconds = (int)Mathf.Floor(((Time) % 1) * 100f);
        string MillisecondsSpacer = "";
        if (Milliseconds < 10)
            MillisecondsSpacer = "0";

        Formatted = MinutesSpacer + Minutes + ":" + SecondsSpacer + Seconds + ":" + MillisecondsSpacer + Milliseconds;

        return Formatted;
    }
}