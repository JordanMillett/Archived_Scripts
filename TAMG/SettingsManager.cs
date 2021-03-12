using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class SettingsManager : MonoBehaviour
{
    static SettingsManager SMInstance;

    void Awake()
    { 
        if(SMInstance == null)
        {
            SMInstance = this;
            DontDestroyOnLoad(this.gameObject);
        }else
        {
            Destroy(this.gameObject);
        }
    }

    void Start()
    {
        InitializeSettings();
    }

    void InitializeSettings()
    {
        //Debug.Log(Application.persistentDataPath);
        string path = Application.persistentDataPath + "settings.txt";
 
        if(!File.Exists(path)) 
        {
            SettingsToFile();
        }

        StreamReader SR = new StreamReader(path);
        string settingText = SR.ReadToEnd();
        SR.Close();

        try
        {
        
            string[] settingsLines = settingText.Split("\n"[0]);
            
            Settings._displayMode = (Settings.DisplayMode) int.Parse(settingsLines[0]);
            Settings._resolutionIndex = int.Parse(settingsLines[1]);
            Settings._framerateLimitIndex = int.Parse(settingsLines[2]);

            Settings._textureQuality = (Settings.TextureQuality) int.Parse(settingsLines[3]);
            Settings._antiAliasingIndex = int.Parse(settingsLines[4]);
            Settings._viewDistance = (Settings.ViewDistance) int.Parse(settingsLines[5]);

            Settings._mouseSensitivity = int.Parse(settingsLines[6]);
            Settings._invertedLook = int.Parse(settingsLines[7]) == 1;

            Settings._masterVolume = int.Parse(settingsLines[8]);
            Settings._sfxVolume = int.Parse(settingsLines[9]);
            Settings._musicVolume = int.Parse(settingsLines[10]);

            Settings._quickBlackout = int.Parse(settingsLines[11]) == 1;

            Settings._voiceVolume = int.Parse(settingsLines[12]);

            ApplySettings();

        }catch
        {
            Debug.LogError("Error loading settings file, creating new from default.");
            SettingsToFile();
        }
    }

    public void SaveSettings()
    {
        string path = Application.persistentDataPath + "settings.txt";
        File.WriteAllText(path, "");
        SettingsToFile();
        ApplySettings();
    }

    void SettingsToFile()
    {
        //Debug.LogError("Settings File Doesn't exist, creating new one");
        string path = Application.persistentDataPath + "settings.txt";

        StreamWriter SW = new StreamWriter(path);

        string settingsLines = "";
        settingsLines += (int) Settings._displayMode + "\n";
        settingsLines += Settings._resolutionIndex + "\n";
        settingsLines += Settings._framerateLimitIndex + "\n";

        settingsLines += (int) Settings._textureQuality + "\n";
        settingsLines += Settings._antiAliasingIndex + "\n";
        settingsLines += (int) Settings._viewDistance + "\n";
        
        settingsLines += Settings._mouseSensitivity + "\n";
        settingsLines += System.Convert.ToInt32(Settings._invertedLook) + "\n";

        settingsLines += Settings._masterVolume + "\n";
        settingsLines += Settings._sfxVolume + "\n";
        settingsLines += Settings._musicVolume + "\n";

        settingsLines += System.Convert.ToInt32(Settings._quickBlackout) + "\n";

        settingsLines += Settings._voiceVolume + "\n";

        SW.Write(settingsLines);
        SW.Close();
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

        int TextureRes = 0;
        switch(Settings._textureQuality)
        {
            case Settings.TextureQuality.High:      TextureRes = 0;     break;
            case Settings.TextureQuality.Medium:    TextureRes = 1;     break;
            case Settings.TextureQuality.Low:       TextureRes = 2;     break;
        }

        QualitySettings.masterTextureLimit = TextureRes;

        QualitySettings.antiAliasing = Settings.AntiAliasing[Settings._antiAliasingIndex];

        float LODDist = 0;
        switch(Settings._viewDistance)
        {
            case Settings.ViewDistance.Far:      LODDist = 2f;         break;
            case Settings.ViewDistance.Medium:    LODDist = 1.5f;     break;
            case Settings.ViewDistance.Short:       LODDist = 1f;    break;
            case Settings.ViewDistance.Potato:       LODDist = 0.5f;    break;
        }

        QualitySettings.lodBias = LODDist;
    }
}
