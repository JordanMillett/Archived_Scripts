using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class EquipmentInfo : MonoBehaviour
{
    public string Name;
    public string Description;
    public Texture Image;

    public GameObject WeaponEmpty;

    //public EquipmentConfig EC;

    public GameObject Equipment;

    public void UpdateInfo()
    {

        WeaponEmpty.transform.GetChild(0).GetComponent<RawImage>().texture = Image;
        WeaponEmpty.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = Name;
        WeaponEmpty.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = Description;

    }

    public void Confirm()
    {
       
        GameObject.FindWithTag("EquipSlot").GetComponent<EquipSlot>().Throwable = Equipment;

        WeaponEmpty.GetComponent<MenuGunUpdater>().Confirm(Image, Name);

    }
}
