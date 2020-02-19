using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OptionMenu : MonoBehaviour
{
    public Checkbox Fullscreen;
    public DropDown Resolutions;
    public DropDown Antialising;
    public DropDown TextureSize;

    public void Apply()
    {

        string[] Split = Resolutions.Current.text.Split('x');
        int Width = int.Parse(Split[0]);
        int Height = int.Parse(Split[1]);

        string antilevel = Antialising.Current.text;

        int texSize = 0;

        switch(TextureSize.Current.text)
        {

            case "1/1" : texSize = 0; break;
            case "1/2" : texSize = 1; break;
            case "1/4" : texSize = 2; break;
            case "1/8" : texSize = 3; break;
            case "1/16" : texSize = 4; break;
            case "1/32" : texSize = 5; break;

        }

        Screen.SetResolution(Width, Height, Fullscreen.isEnabled);

        QualitySettings.antiAliasing = (int)System.Char.GetNumericValue(antilevel[0]);

        QualitySettings.masterTextureLimit = texSize;

    }
}
