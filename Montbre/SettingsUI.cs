using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;

public class SettingsUI : MonoBehaviour
{
    [System.Serializable]
    public struct Menu
    {
	    public GameObject Screen;
    }

    SettingsManager SM;

    public List<Menu> MenuScreens;
    public int CurrentScreen = 0;

    public DropDown DisplayMode;
    public DropDown ResolutionIndex;
    public DropDown FramerateLimitIndex;

    public IntSlider FOV;
    public IntSlider PlaneFOV;

    public IntSlider MouseSensitivity;
    public Checkbox InvertedLook;

    public IntSlider MasterVolume;
    public IntSlider SFXVolume;

    void Start()
    {
        SetScreen(0);
        InitializeOptions();
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

        MouseSensitivity.Initialize(Settings._mouseSensitivity);
        InvertedLook.Initialize(Settings._invertedLook);

        FOV.Initialize(Settings._FOV);
        PlaneFOV.Initialize(Settings._planeFOV);

        MasterVolume.Initialize(Settings._masterVolume);
        SFXVolume.Initialize(Settings._sfxVolume);
    }

    public void Apply()
    {
        Settings._displayMode = (Settings.DisplayMode) DisplayMode.CurrentIndex;
        Settings._resolutionIndex = ResolutionIndex.CurrentIndex;
        Settings._framerateLimitIndex = FramerateLimitIndex.CurrentIndex;

        Settings._FOV = FOV.Value;
        Settings._planeFOV = PlaneFOV.Value;

        Settings._mouseSensitivity = MouseSensitivity.Value;
        Settings._invertedLook = InvertedLook.isOn;

        Settings._masterVolume = MasterVolume.Value;
        Settings._sfxVolume = SFXVolume.Value;

        SettingsManager.S.SaveSettings();
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
}