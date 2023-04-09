using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class Element_Station : MonoBehaviour
{
    public Station.Level Level;
    public Texture2D Map;

    public RawImage Texture;
    public TextMeshProUGUI Difficulty;

    Menu_World World;

    public void Set(Station.Level _Level, Texture2D _Map, Menu_World _World)
    {
        World = _World;
        Level = _Level;
        Map = _Map;

        Texture.texture = Map;
        Difficulty.text = _Level.ToString();
    }
    
    public void Select()
    {
        World.Launch(this);
    }
}
