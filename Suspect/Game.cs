using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.InputSystem;
using System.Text.RegularExpressions;

public class Need
{   
    public int Current;
    public int Pool;
    
    public enum Effects
    {
        None,
        Kill,
        Dirty,
        Sleep,
        Stink
    }

    bool usePool = false; //either out of 100 and charges over time or out of total seconds

    public int Max = 100;

    Effects effect = Effects.None;

    public Need(int _minutes, Effects _effect)
    {
        Max = _minutes * 60;
        effect = _effect;
    }
    
    public Need(Effects _effect)
    {
        usePool = true;
        effect = _effect;
    }
    
    public Effects Tick()
    {
        if(Current >= Max)
            return Effects.None;

        if(usePool)
        {
            if(Pool > 0)
            {
                Pool--;
                Current++;
            }
        }else
        {
            Current++;
        }
        
        return Current == Max ? effect : Effects.None;
    }
    
    public void Increase(int amount)
    {
        if(usePool)
        {
            Pool += amount;
        }else
        {
            Current += Mathf.RoundToInt(Max * (amount / 100f));
            Current = Mathf.Clamp(Current, 0, Max);
        }
    }
}

public class Settings
{
    public int _masterVolume = 100;
    public int _sfxVolume = 50;
    public int _musicVolume = 50;
    public int _voiceVolume = 100;

    public float _micGain = 1f;
}

public static class Game
{
    public static InputActionAsset ActionMap;
    
    public static Settings SettingsData;
    
    static string SettingsDataPath;
    
    public enum Needs
    {
        None,
        Hunger,
        Thirst,
        Sleep,
        Social,
        Boredom,
        Hygiene,
        Solid,
        Liquid
    }

    public static List<string> Usernames = new List<string>();
    public static List<string> CharacterFirstNames = new List<string>();
    public static List<string> CharacterLastNames = new List<string>();
    
    public static LayerMask WeaponMask = LayerMask.GetMask("Default", "Player", "Blocker");
    public static LayerMask CameraMask = LayerMask.GetMask("Default", "Blocker");

    public static int Bots = 4; //6

    static Game()
    {
        ActionMap = (InputActionAsset) Resources.Load("ActionMap");

        SettingsDataPath = Application.persistentDataPath + "/settings.json";

        if(!LoadSettings())
            CreateSettings();
        
        Screen.SetResolution(1600, 900, FullScreenMode.Windowed, 60);
        Application.targetFrameRate = 60;

        LoadNames();
    }
    
    public static void CreateSettings()
    {
        SettingsData = new Settings();
        
        Debug.Log("Failed to load settings. New settings file created.");

        SaveSettings();
    }
    
    public static bool LoadSettings()
    {
        if(!File.Exists(SettingsDataPath))
            return false;

        try
        {
            string json = File.ReadAllText(SettingsDataPath);
            SettingsData = JsonUtility.FromJson<Settings>(json);

            Debug.Log("Settings loaded.");
            return true;
        }catch
        {
            return false;
        }
    }
    
    public static void SaveSettings()
    {
        string json = JsonUtility.ToJson(SettingsData);
        File.WriteAllText(SettingsDataPath, json);
        Debug.Log("Settings saved.");
    }
    
    static void LoadNames()
    {
        TextAsset file = (TextAsset) Resources.Load("Usernames");
        string[] lines = file.text.Split("\n"[0]);

        for(int i = 0; i < lines.Length; i++)
            Usernames.Add(Regex.Replace(lines[i], "[^\\w\\._]", ""));
            
            
        file = (TextAsset) Resources.Load("CharacterFirstNames");
        lines = file.text.Split("\n"[0]);

        for(int i = 0; i < lines.Length; i++)
            CharacterFirstNames.Add(Regex.Replace(lines[i], "[^\\w\\._]", ""));
            
            
        file = (TextAsset) Resources.Load("CharacterLastNames");
        lines = file.text.Split("\n"[0]);

        for(int i = 0; i < lines.Length; i++)
            CharacterLastNames.Add(Regex.Replace(lines[i], "[^\\w\\._]", ""));
    }
}
