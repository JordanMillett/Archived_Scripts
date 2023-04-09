using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Popup_Enemy : Popup
{
    public SimpleBar HealthBar;
    public SimpleBar ShieldsBar;
    public TextMeshProUGUI Name;
    public TextMeshProUGUI Level;

    public Enemy E;
    public Life L;

    protected override void Update()
    {
        base.Update();

        Name.text = E.Name;
        Level.text = E.Level.ToString();
        
        HealthBar.Current = L.Health;
        HealthBar.Max = L.MaxHealth;
        ShieldsBar.Current = L.Shields;
        ShieldsBar.Max = L.MaxShields;
    }
}
