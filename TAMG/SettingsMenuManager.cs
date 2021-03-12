using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;

public class SettingsMenuManager : MonoBehaviour
{
    [System.Serializable]
    public struct Menu
    {
	    public GameObject Screen;
    }

    SettingsManager SM;

    public List<Menu> MenuScreens;
    public int CurrentScreen = 0;
    int PreviousScreen = 0;

    public DropDown DisplayMode;
    public DropDown ResolutionIndex;
    public DropDown FramerateLimitIndex;

    public DropDown TextureQuality;
    public DropDown AntiAliasing;
    public DropDown ViewDistance;
    public Checkbox QuickBlackout;

    public IntSlider MouseSensitivity;
    public Checkbox InvertedLook;

    public IntSlider MasterVolume;
    public IntSlider SFXVolume;
    public IntSlider MusicVolume;
    public IntSlider VoiceVolume;

    MenuManager MM;

    void Start()
    {
        MM = GameObject.FindWithTag("Camera").GetComponent<MenuManager>();
        SM = GameObject.FindWithTag("Information").GetComponent<SettingsManager>();
        SetScreen(0);
        InitializeOptions();
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            MM.GoToLastMenu();
        }
    }

    void OnEnable()
    {
        InitializeOptions();
    }

    void OnDisable()
    {
        SetScreen(0);
    }

    void InitializeOptions()
    {
        DisplayMode.Initialize(System.Enum.GetNames(typeof(Settings.DisplayMode)), (int) Settings._displayMode);
        ResolutionIndex.Initialize(Vector2IntToOptions(Settings.Resolution), Settings._resolutionIndex);
        FramerateLimitIndex.Initialize(IntToOptions(Settings.FramerateLimit), Settings._framerateLimitIndex);

        TextureQuality.Initialize(System.Enum.GetNames(typeof(Settings.TextureQuality)), (int) Settings._textureQuality);
        AntiAliasing.Initialize(IntToOptions(Settings.AntiAliasing), Settings._antiAliasingIndex);
        ViewDistance.Initialize(System.Enum.GetNames(typeof(Settings.ViewDistance)), (int) Settings._viewDistance);

        MouseSensitivity.Initialize(Settings._mouseSensitivity);
        InvertedLook.Initialize(Settings._invertedLook);

        MasterVolume.Initialize(Settings._masterVolume);
        SFXVolume.Initialize(Settings._sfxVolume);
        MusicVolume.Initialize(Settings._musicVolume);

        QuickBlackout.Initialize(Settings._quickBlackout);

        VoiceVolume.Initialize(Settings._voiceVolume);
    }

    public void Apply()
    {
        Settings._displayMode = (Settings.DisplayMode) DisplayMode.CurrentIndex;
        Settings._resolutionIndex = ResolutionIndex.CurrentIndex;
        Settings._framerateLimitIndex = FramerateLimitIndex.CurrentIndex;

        Settings._textureQuality = (Settings.TextureQuality) TextureQuality.CurrentIndex;
        Settings._antiAliasingIndex = AntiAliasing.CurrentIndex;
        Settings._viewDistance = (Settings.ViewDistance) ViewDistance.CurrentIndex;

        Settings._mouseSensitivity = MouseSensitivity.Value;
        Settings._invertedLook = InvertedLook.isOn;

        Settings._masterVolume = MasterVolume.Value;
        Settings._sfxVolume = SFXVolume.Value;
        Settings._musicVolume = MusicVolume.Value;

        Settings._quickBlackout = QuickBlackout.isOn;

        Settings._voiceVolume = VoiceVolume.Value;

        SM.SaveSettings();
    }

    string[] Vector2IntToOptions(Vector2Int[] toConvert)
    {
        string[] converted = new string[toConvert.Length];

        for(int i = 0; i < converted.Length; i++)
        {
            converted[i] = toConvert[i].x + "x" + toConvert[i].y;
        }

        return converted;
    }

    string[] IntToOptions(int[] toConvert)
    {
        string[] converted = new string[toConvert.Length];

        for(int i = 0; i < converted.Length; i++)
        {
            converted[i] = toConvert[i].ToString();
        }

        return converted;
    }

    public void SetScreen(int Index)
    {
        PreviousScreen = CurrentScreen;
        CurrentScreen = Index;

        for(int i = 0; i < MenuScreens.Count; i++)
        {
            if(i == Index)
            {
                MenuScreens[i].Screen.SetActive(true);
            }
            else
            {
                MenuScreens[i].Screen.SetActive(false);
            }
        }
    }

    public void GoToLastMenu()
    {
        SetScreen(PreviousScreen);
    }
}
