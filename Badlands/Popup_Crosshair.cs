using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Popup_Crosshair : Popup
{
    public SimpleBar Primary;
    public SimpleBar Secondary;

    protected override void Update()
    {
        base.Update();
        
        //HealthBar.Current = L.Health;
        //HealthBar.Max = L.MaxHealth;
    }
}
