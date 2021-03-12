using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PartOption : MonoBehaviour
{
    public TextMeshProUGUI Label;
    public TextMeshProUGUI Cost;

    PartMenu PM;

    VehiclePart VP = null;

    public void Init(PartMenu _PM, string Name, VehiclePart _VP, string Value)
    {
        PM = _PM;
        VP = _VP;

        Label.text = Name;
        Cost.text = "$" + Value;
        
    }

    public void Activate()
    {
        PM.SetPart(VP);
    }

    public void Hover()
    {
        PM.HoverPart(VP);
    }
}
