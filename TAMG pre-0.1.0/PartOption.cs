using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PartOption : MonoBehaviour
{
    public TextMeshProUGUI Label;
    public TextMeshProUGUI Cost;

    PartMenu PM;

    int PartIndex = 0;

    public void Init(PartMenu _PM, string Name, int _PartIndex, string Value)
    {
        PM = _PM;
        PartIndex = _PartIndex;

        Label.text = Name;
        Cost.text = "$" + Value;
        
    }

    public void Activate()
    {
        PM.SetPart(PartIndex);
    }

    public void Hover()
    {
        PM.HoverPart(PartIndex);
    }
}
