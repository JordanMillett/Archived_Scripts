using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Settings
{
    public enum DisplayMode
	{
		Fullscreen,
		Windowed,
		Borderless
	};

    public static Vector2Int[] Resolution = new Vector2Int[]
	{
        new Vector2Int(2560, 1440),
		new Vector2Int(1920, 1080),
        new Vector2Int(1600, 900),
        new Vector2Int(1280, 720),
        new Vector2Int(3440, 1440),
        new Vector2Int(2560, 1080)
    };

    public static int[] FramerateLimit = new int[]
	{
        30,
		60,
        90,
        144
    };

    public static DisplayMode _displayMode = DisplayMode.Fullscreen;    //Default Fullscreen
    public static int _resolutionIndex = 1;                             //Default 1080p
    public static int _framerateLimitIndex = 1;                         //Default 60fps
    
    public static int _FOV = 70;                                        //Default 70
    public static int _planeFOV = 70;                                   //Default 70

    public static int _mouseSensitivity = 100;                          //Default 100;  (1.00)
    public static bool _invertedLook = false;                           //Default false

    public static int _masterVolume = 50;                               //Default 50;  (0.50)
    public static int _sfxVolume = 100;                                 //Default 100;  (1.00)
}
