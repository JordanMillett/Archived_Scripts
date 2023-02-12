using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenLight : MonoBehaviour
{
    public RenderTexture Screen;
    Texture2D ScreenPixels;

    Light L;

    void Start()
    {
        L = GetComponent<Light>();
        ScreenPixels = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, true);
    }

    void Update()
    {
        RenderTexture.active = Screen;
        ScreenPixels.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0, true);
        L.color = AverageColorFromTexture(ScreenPixels);
    }

    Color32 AverageColorFromTexture(Texture2D tex)
    {
    
        Color32[] texColors = tex.GetPixels32(tex.mipmapCount - 1);

        int total = texColors.Length;

        float r = 0;
        float g = 0;
        float b = 0;

        for(int i = 0; i < total; i++)
        {

            r += texColors[i].r;

            g += texColors[i].g;

            b += texColors[i].b;

        }

        return new Color32((byte)(r / total) , (byte)(g / total) , (byte)(b / total) , 0);
    
    }
}
