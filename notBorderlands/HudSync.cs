using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class HudSync : MonoBehaviour
{
    public PlayerStats PS;
    GameObject EQ;

    Simple_Bar XPBar;
    Simple_Bar HealthBar;
    Simple_Bar ShieldBar;

    TextMeshProUGUI HealthNumber;
    TextMeshProUGUI ShieldNumber;
    TextMeshProUGUI Level;

    GameObject EnemyIndicator;
    Simple_Bar EnemyHealthBar;
    TextMeshProUGUI EnemyLevel;
    TextMeshProUGUI EnemyName;

    bool EnemyIndicatorActive = false;

    TextMeshProUGUI CurrentMoney;

    Simple_Bar AmmoBar;
    TextMeshProUGUI Ammo;

    void Start()
    {
        EQ = GameObject.FindWithTag("EquipSlot");

        GetVars();
        Refresh();

    }

    void Update()
    {
        Refresh();

        EnemyIndicator.SetActive(EnemyIndicatorActive);
        EnemyIndicatorActive = false;
    }

    public void EnableEnemyIndicator(float MaxHealth, float CurrentHealth, string Name, int Level)
    {
        EnemyIndicatorActive = true;
        EnemyHealthBar.Max = MaxHealth;
        EnemyHealthBar.Current = CurrentHealth;
        EnemyLevel.text = Level.ToString();
        EnemyName.text = Name;
    }

    void Refresh()
    {
        XPBar.Max = PS.MaxXP;
        XPBar.Current = PS.XP;

        HealthBar.Max = PS.MaxHealth;
        HealthBar.Current = PS.Health;

        ShieldBar.Max = PS.MaxShields;
        ShieldBar.Current = PS.Shields;

        HealthNumber.text = PS.Health.ToString();
        ShieldNumber.text = PS.Shields.ToString();
        Level.text = PS.Level.ToString();

        CurrentMoney.text = "$" + PS.Money.ToString();

        AmmoBar.Max = EQ.transform.GetChild(0).GetComponent<Weapon>().WC.MagSize;
        AmmoBar.Current = EQ.transform.GetChild(0).GetComponent<Weapon>().Ammo;

        Ammo.text = EQ.transform.GetChild(0).GetComponent<Weapon>().Ammo.ToString()
        + "/" + EQ.transform.GetChild(0).GetComponent<Weapon>().ReserveAmmo.ToString();
    }

    void GetVars()
    {

        EnemyIndicator = this.transform.GetChild(2).gameObject;
        EnemyHealthBar = EnemyIndicator.transform.GetChild(0).GetComponent<Simple_Bar>();
        EnemyLevel = EnemyIndicator.transform.GetChild(1).GetComponent<TextMeshProUGUI>();
        EnemyName = EnemyIndicator.transform.GetChild(2).GetComponent<TextMeshProUGUI>();

        XPBar = this.transform.GetChild(1).GetChild(0).GetComponent<Simple_Bar>();
        HealthBar = this.transform.GetChild(1).GetChild(1).GetComponent<Simple_Bar>();
        ShieldBar = this.transform.GetChild(1).GetChild(2).GetComponent<Simple_Bar>();

        HealthNumber = HealthBar.transform.GetChild(3).GetChild(0).GetComponent<TextMeshProUGUI>();
        ShieldNumber = ShieldBar.transform.GetChild(3).GetChild(0).GetComponent<TextMeshProUGUI>();
        Level = XPBar.transform.GetChild(3).GetChild(1).GetComponent<TextMeshProUGUI>();

        CurrentMoney = this.transform.GetChild(4).GetChild(1).GetComponent<TextMeshProUGUI>();

        AmmoBar = this.transform.GetChild(1).GetChild(3).GetComponent<Simple_Bar>();
        Ammo = AmmoBar.transform.GetChild(3).GetChild(0).GetComponent<TextMeshProUGUI>();

    }
}
