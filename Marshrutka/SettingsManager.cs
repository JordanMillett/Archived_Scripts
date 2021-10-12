using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;

public class SettingsManager : MonoBehaviour
{
    public string path = "";

    void Start()
    {
        GameSettings.SM = this;
        path = Application.persistentDataPath + "settings.txt";

        LoadSettings();
    }

    void LoadSettings()
    {
        if(!File.Exists(path)) 
        {
            Debug.LogError("settings file non existant, creating new from default.");
            SaveSettings();
        }

        StreamReader SR = new StreamReader(path);
        string settingText = SR.ReadToEnd();
        SR.Close();

        try
        {
            string[] settingsLines = settingText.Split("\n"[0]);

            if(settingsLines.Length != (9) + 1)     //ALWAYS MATCH LENGTH OR ELSE IT CAN NEVER GENERATE SAVE FILE PROPERLY
                throw new Exception("Settings file length is incorrect");
            
            Settings._displayMode = (Settings.DisplayMode) int.Parse(settingsLines[0]);
            Settings._resolutionIndex = int.Parse(settingsLines[1]);
            Settings._framerateLimitIndex = int.Parse(settingsLines[2]);

            Settings._devConsoleEnabled = int.Parse(settingsLines[3]) == 1;

            Settings._mouseSensitivity = int.Parse(settingsLines[4]);
            Settings._invertedLook = int.Parse(settingsLines[5]) == 1;

            Settings._masterVolume = int.Parse(settingsLines[6]);
            Settings._sfxVolume = int.Parse(settingsLines[7]);
            Settings._musicVolume = int.Parse(settingsLines[8]);

            ApplySettings();

        }catch
        {
            Debug.LogError("Error loading settings file, creating new from default.");
            File.Delete(path);
            SaveSettings();
        }
    }

    public void SaveSettings()
    {
        File.WriteAllText(path, "");

        StreamWriter SW = new StreamWriter(path);

        string settingsLines = "";
        settingsLines += (int) Settings._displayMode + "\n";
        settingsLines += Settings._resolutionIndex + "\n";
        settingsLines += Settings._framerateLimitIndex + "\n";

        settingsLines += System.Convert.ToInt32(Settings._devConsoleEnabled) + "\n";

        settingsLines += Settings._mouseSensitivity + "\n";
        settingsLines += System.Convert.ToInt32(Settings._invertedLook) + "\n";

        settingsLines += Settings._masterVolume + "\n";
        settingsLines += Settings._sfxVolume + "\n";
        settingsLines += Settings._musicVolume + "\n";

        SW.Write(settingsLines);
        SW.Close();

        ApplySettings();
    }

    void ApplySettings()
    {
        FullScreenMode FSM = FullScreenMode.Windowed;
        switch(Settings._displayMode)
        {
            case Settings.DisplayMode.Fullscreen:   FSM = FullScreenMode.ExclusiveFullScreen;   break;
            case Settings.DisplayMode.Windowed:     FSM = FullScreenMode.Windowed;              break;
            case Settings.DisplayMode.Borderless:   FSM = FullScreenMode.FullScreenWindow;      break;
        }

        Screen.SetResolution(
            Settings.Resolution[Settings._resolutionIndex].x, 
            Settings.Resolution[Settings._resolutionIndex].y, 
            FSM, 
            Settings.FramerateLimit[Settings._framerateLimitIndex]);

        Application.targetFrameRate = Settings.FramerateLimit[Settings._framerateLimitIndex];
    }
}