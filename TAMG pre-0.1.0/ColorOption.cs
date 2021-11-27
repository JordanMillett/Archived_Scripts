using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ColorOption : MonoBehaviour
{
    public TextMeshProUGUI Label;
    public TextMeshProUGUI Cost;

    ColorOptionList COL;

    bool Sub = false;
    int Index = 0;

    public void Init(ColorOptionList _COL, string Name, int _Index, bool _Sub, string Value)
    {
        COL = _COL;
        Sub = _Sub;
        Index = _Index;

        Label.text = Name;

        if(Sub)
        {
            Cost.text = "$" + Value;
        }
        else
        {
            Label.alignment = TextAlignmentOptions.Center;
            Label.fontSize = 30f;
            Cost.text = "";
        }
    }

    public void Activate()
    {
        COL.PlaySound();

        if(!Sub)
        {
            COL.OpenColorMenu(Index);
        }else
        {
            COL.SetColor(Index);
        }
    }

    public void Hover()
    {
        COL.HoverColor(Index, Sub);
    }
}
