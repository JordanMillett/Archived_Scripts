using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class WheelPart : MonoBehaviour
{
    public bool Highlight = false;

    public string Title;
    RawImage Part;
    RawImage Icon;
    TextMeshProUGUI Info;

    public void Set(string Tit, Texture2D Tex, string Data)
    {
        Icon = transform.GetChild(0).GetComponent<RawImage>();
        Info = transform.GetChild(1).GetComponent<TextMeshProUGUI>();

        Title = Tit;
        if(Tex != null)
            Icon.texture = Tex;
        Info.text = Data;
    }

    void Update()
    {
        Part = GetComponent<RawImage>();

        if(Highlight)
        {
            Part.color = new Color(0f, 0f, 0f, 240f/255f);
        }else
        {
            Part.color = new Color(0f, 0f, 0f, 175f/255f);
        }
    }
}
