using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class GunInfo : MonoBehaviour
{
    public string Name;
    public string Description;
    public Texture Image;

    public GameObject WeaponEmpty;
    public GameObject StatsEmpty;

    public WeaponConfig WC;

    public bool Primary = false;

    void Start()
    {
        if(StatsEmpty != null)
        {
            StatsEmpty.transform.GetChild(0).GetComponent<Simple_Bar>().Max = 5f;
            StatsEmpty.transform.GetChild(1).GetComponent<Simple_Bar>().Max = 60f;
            StatsEmpty.transform.GetChild(2).GetComponent<Simple_Bar>().Max = 1f;
            StatsEmpty.transform.GetChild(3).GetComponent<Simple_Bar>().Max = 2000f;
        }
        
    }

    public void UpdateInfo()
    {

        WeaponEmpty.transform.GetChild(0).GetComponent<RawImage>().texture = Image;
        WeaponEmpty.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = Name;
        WeaponEmpty.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = Description;
        
        if(StatsEmpty != null)
        {
            StatsEmpty.transform.GetChild(0).GetComponent<Simple_Bar>().Current = WC.Accuracy - 95f;
            StatsEmpty.transform.GetChild(1).GetComponent<Simple_Bar>().Current = WC.Damage;
            StatsEmpty.transform.GetChild(2).GetComponent<Simple_Bar>().Current = WC.RecoilAmount;
            StatsEmpty.transform.GetChild(3).GetComponent<Simple_Bar>().Current = WC.RPM;
        }

    }

    public void Confirm()
    {
        if(Primary)
            GameObject.FindWithTag("EquipSlot").GetComponent<EquipSlot>().Primary = WC;
        else
            GameObject.FindWithTag("EquipSlot").GetComponent<EquipSlot>().Secondary = WC;

        WeaponEmpty.GetComponent<MenuGunUpdater>().Confirm(Image, Name);

    }
}
