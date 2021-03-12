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
		new Vector2Int(1920, 1080),
        new Vector2Int(1600, 900),
        new Vector2Int(1280, 720)
    };

    public static int[] FramerateLimit = new int[]
	{
        30,
		60,
        144
    };

    public enum TextureQuality
	{
		High,
		Medium,
		Low
	};

    public static int[] AntiAliasing = new int[]
	{
        0,
        2,
		4,
        8
    };

    public enum ViewDistance
	{
		Far,
		Medium,
		Short,
        Potato
	};

    public static DisplayMode _displayMode = DisplayMode.Fullscreen;    //Default Fullscreen
    public static int _resolutionIndex = 0;                             //Default 1080p
    public static int _framerateLimitIndex = 1;                         //Default 60fps

    public static TextureQuality _textureQuality = TextureQuality.High; //Default High
    public static int _antiAliasingIndex = 1;                           //Default 4x;
    public static ViewDistance _viewDistance = ViewDistance.Far;        //Default Far
    public static bool _quickBlackout = false;                          //Default false
    
    public static int _mouseSensitivity = 100;                          //Default 100;  (1.00)
    public static bool _invertedLook = false;                           //Default false

    public static int _masterVolume = 100;                              //Default 100;  (1.00)
    public static int _sfxVolume = 100;                                 //Default 100;  (1.00)
    public static int _musicVolume = 100;                               //Default 100;  (1.00)
    public static int _voiceVolume = 100;                               //Default 100;  (1.00)
    /*
    (Sound)
    -Main volume
    -Music volume

    */

    //Misc non file settings
    public static int _ragdollDespawnTime = 25;
}
