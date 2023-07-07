using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ColorPalette : MonoBehaviour
{
    public CustomizationMenu CM;

    public Texture2D Palette;

    public GameObject ColorPrefab;
    
    void Start()
    {
        float ColorSize = 50f;

        float StartingX = -((Palette.width * ColorSize)/2f);
        float StartingY = (Palette.height * ColorSize)/2f;

        float CurrentX = StartingX;
        float CurrentY = StartingY;

        for (int y = Palette.height - 1; y > -1; y--)
        {
            for (int x = 0; x < Palette.width; x++)
            {
                GameObject ColorButton = Instantiate(ColorPrefab, Vector3.zero, Quaternion.identity);
                ColorButton.transform.SetParent(this.transform);
                ColorButton.GetComponent<RectTransform>().anchoredPosition = new Vector3(CurrentX, CurrentY, 0f);
                ColorButton.GetComponent<RectTransform>().transform.localScale = Vector3.one;
                ColorButton.GetComponent<ColorButton>().CM = CM;
                ColorButton.GetComponent<RawImage>().color = Palette.GetPixel(x, y);
                
                CurrentX += ColorSize;
            }
            CurrentX = StartingX;
            CurrentY -= ColorSize;
        }
    }


}
