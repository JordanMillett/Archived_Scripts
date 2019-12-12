using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class WeaponConfigToPopup : MonoBehaviour
{
    
    public bool useRandom = true;
    public WeaponConfig WC;
    WeaponConfig PlayerWeapon;

    TextMeshProUGUI Name;

    TextMeshProUGUI Damage;
    TextMeshProUGUI Accuracy;
    TextMeshProUGUI FireRate;
    TextMeshProUGUI MagazineSize;

    Transform Equipslot;

    Arrow DamageArrow;
    Arrow AccuracyArrow;
    Arrow FireRateArrow;
    Arrow MagazineSizeArrow;

    TextMeshProUGUI Value;

    void Start()
    {
        Init();

        Refresh();

    }

    void Update()
    {
        if(!useRandom)
            Refresh();
        
    }

    void Refresh()
    {

        if(useRandom)
            WC = GetComponent<RandomWeaponConfig>().GetRandomWC();

        Name.text = WC.Name;

        Damage.text = WC.Damage.ToString();
        Accuracy.text = WC.Accuracy.ToString();
        FireRate.text = (WC.RPM/100f).ToString();
        MagazineSize.text = WC.MagSize.ToString();
        Value.text = "$" + WC.Value.ToString();


    }

    void Init()
    {
        if(useRandom)
            Equipslot = GameObject.FindWithTag("EquipSlot").transform;

        Name = transform.GetChild(0).GetChild(1).GetComponent<TextMeshProUGUI>();

        Damage = transform.GetChild(0).GetChild(2).GetChild(1).GetComponent<TextMeshProUGUI>();
        Accuracy = transform.GetChild(0).GetChild(3).GetChild(1).GetComponent<TextMeshProUGUI>();
        FireRate = transform.GetChild(0).GetChild(4).GetChild(1).GetComponent<TextMeshProUGUI>();
        MagazineSize = transform.GetChild(0).GetChild(5).GetChild(1).GetComponent<TextMeshProUGUI>();

        DamageArrow = Damage.transform.parent.GetChild(2).GetComponent<Arrow>();
        AccuracyArrow = Accuracy.transform.parent.GetChild(2).GetComponent<Arrow>();
        FireRateArrow = FireRate.transform.parent.GetChild(2).GetComponent<Arrow>();
        MagazineSizeArrow = MagazineSize.transform.parent.GetChild(2).GetComponent<Arrow>();

        if(useRandom)
            transform.parent.parent.GetChild(1).GetChild(0).GetComponent<Weapon>().WC = WC;

        Value = transform.GetChild(0).GetChild(6).GetComponent<TextMeshProUGUI>();

    }

    public void UpdateArrows()
    {
        if(useRandom)
            PlayerWeapon = Equipslot.GetChild(0).GetComponent<Weapon>().WC;
        else
            PlayerWeapon = WC;

        if(PlayerWeapon.Damage > WC.Damage)
            DamageArrow.Higher = false;
        else
            DamageArrow.Higher = true;

        if(PlayerWeapon.Accuracy > WC.Accuracy)
            AccuracyArrow.Higher = false;
        else
            AccuracyArrow.Higher = true;

        if(PlayerWeapon.RPM > WC.RPM)
            FireRateArrow.Higher = false;
        else
            FireRateArrow.Higher = true;

        if(PlayerWeapon.MagSize > WC.MagSize)
            MagazineSizeArrow.Higher = false;
        else
            MagazineSizeArrow.Higher = true;

        DamageArrow.Refresh();
        AccuracyArrow.Refresh();
        FireRateArrow.Refresh();
        MagazineSizeArrow.Refresh();

    }

}
