using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class Element_Minimap : MonoBehaviour
{
    Texture2D Fog;
    Texture2D Walls;

    public GameObject MinimapObject;
    public RawImage Image;
    public RectTransform PlayerIcon;
    public RectTransform Scaler;

    public Material CrewColor;
    public Material CargoColor;
    public Material MedicalColor;

    public Material PowerColor;
    public Material LabColor;
    public Material ArmoryColor;
    public Material VaultColor;

    public Material EscapeColor;

    public void SetStation(Texture2D Station)
    {
        //ScaleMultiplier = 16f / ;
        //Scaler.anchoredPosition = new Vector2(Scaler.anchoredPosition.x * ScaleMultiplier, Scaler.anchoredPosition.y * ScaleMultiplier);
        //Scaler.localScale = new Vector3(Scaler.localScale.x * ScaleMultiplier, Scaler.localScale.y * ScaleMultiplier, 1f);
        //PlayerIcon.localScale = new Vector3(PlayerIcon.localScale.x / ScaleMultiplier, PlayerIcon.localScale.y / ScaleMultiplier, 1f);

        Image.material.SetTexture("_MainTex", Station);

        Fog = new Texture2D(StationGenerator.MAP_SIZE, StationGenerator.MAP_SIZE);
        Color[] pixels = new Color[Fog.width * Fog.height];
        for(int i = 0; i < pixels.Length; i++)
            pixels[i] = Color.black;
        Fog.SetPixels(pixels);
        Fog.filterMode = FilterMode.Point;
        
        Walls = new Texture2D(StationGenerator.MAP_SIZE * 8, StationGenerator.MAP_SIZE * 8);
        Color[] wallPixels = new Color[Walls.width * Walls.height];
        for(int i = 0; i < wallPixels.Length; i++)
            wallPixels[i] = Color.white;
        Walls.SetPixels(wallPixels);
        Walls.filterMode = FilterMode.Point;

        Image.material.SetTexture("_Fog", Fog);
        Image.material.SetTexture("_Walls", Walls);

        PlayerIcon.localPosition = new Vector2(Player.P.transform.position.x, Player.P.transform.position.z);
    }
    
    void Update()
    {
        PlayerIcon.localPosition = new Vector2(Player.P.transform.position.x, Player.P.transform.position.z);
    }
    
    public void DiscoverRoom(int x, int y, Zone Area)
    {
        MinimapObject.SetActive(true);
        Image.enabled = true;

        if(!Fog)
            return;
            
        Color RoomColor = CrewColor.color;
        switch(Area)
        {
            case Zone.Cargo :   RoomColor = CargoColor.color; break;
            case Zone.Medical : RoomColor = MedicalColor.color; break;
            case Zone.Power :   RoomColor = PowerColor.color; break;
            case Zone.Vault :  RoomColor = VaultColor.color; break;
            case Zone.Armory :RoomColor = ArmoryColor.color; break;
            case Zone.Lab :RoomColor = LabColor.color; break;
            case Zone.Escape :  RoomColor = EscapeColor.color; break;
        }

        Fog.SetPixel(x, y, RoomColor);
        Fog.Apply();
    }
    
    public void WriteWall(Vector2 Location, bool Door, bool Turned)
    {
        if(!Walls)
            return;

        int Offset = ((StationGenerator.MAP_SIZE/2) * StationGenerator.TILE_SIZE);
        Location = new Vector2(Location.x + Offset, Location.y + Offset);
        int TextureSize = StationGenerator.MAP_SIZE * 8;

        Location = new Vector2((float)Location.x/(Offset * 2f) * TextureSize, (float)Location.y/(Offset * 2f) * TextureSize);

        Vector2Int Pixel = new Vector2Int(Mathf.FloorToInt(Location.x), Mathf.FloorToInt(Location.y));

        if (!Turned)
        {
            if (!Door)
            {
                Walls.SetPixel(Pixel.x, Pixel.y, Color.black);
                Walls.SetPixel(Pixel.x, Pixel.y + 1, Color.black);
                Walls.SetPixel(Pixel.x + 1, Pixel.y, Color.black);
                Walls.SetPixel(Pixel.x + 1, Pixel.y + 1, Color.black);
            }
            
            Walls.SetPixel(Pixel.x - 1, Pixel.y, Color.black);
            Walls.SetPixel(Pixel.x - 1, Pixel.y + 1, Color.black);
            Walls.SetPixel(Pixel.x - 2, Pixel.y, Color.black);
            Walls.SetPixel(Pixel.x - 2, Pixel.y + 1, Color.black);
            Walls.SetPixel(Pixel.x - 3, Pixel.y, Color.black);
            Walls.SetPixel(Pixel.x - 3, Pixel.y + 1, Color.black);

            Walls.SetPixel(Pixel.x + 2, Pixel.y, Color.black);
            Walls.SetPixel(Pixel.x + 2, Pixel.y + 1, Color.black);
            Walls.SetPixel(Pixel.x + 3, Pixel.y, Color.black);
            Walls.SetPixel(Pixel.x + 3, Pixel.y + 1, Color.black);
            Walls.SetPixel(Pixel.x + 4, Pixel.y, Color.black);
            Walls.SetPixel(Pixel.x + 4, Pixel.y + 1, Color.black);
        }else
        {
            if (!Door)
            {
                Walls.SetPixel(Pixel.x, Pixel.y, Color.black);
                Walls.SetPixel(Pixel.x + 1, Pixel.y, Color.black);
                Walls.SetPixel(Pixel.x, Pixel.y + 1, Color.black);
                Walls.SetPixel(Pixel.x + 1, Pixel.y + 1, Color.black);
            }
            
            Walls.SetPixel(Pixel.x, Pixel.y - 1, Color.black);
            Walls.SetPixel(Pixel.x + 1, Pixel.y - 1, Color.black);
            Walls.SetPixel(Pixel.x, Pixel.y - 2, Color.black);
            Walls.SetPixel(Pixel.x + 1, Pixel.y - 2, Color.black);
            Walls.SetPixel(Pixel.x, Pixel.y - 3, Color.black);
            Walls.SetPixel(Pixel.x + 1, Pixel.y - 3, Color.black);

            Walls.SetPixel(Pixel.x, Pixel.y + 2, Color.black);
            Walls.SetPixel(Pixel.x + 1, Pixel.y + 2, Color.black);
            Walls.SetPixel(Pixel.x, Pixel.y + 3, Color.black);
            Walls.SetPixel(Pixel.x + 1, Pixel.y + 3, Color.black);
            Walls.SetPixel(Pixel.x, Pixel.y + 4, Color.black);
            Walls.SetPixel(Pixel.x + 1, Pixel.y + 4, Color.black);
        }

        Walls.Apply();
    }
}
