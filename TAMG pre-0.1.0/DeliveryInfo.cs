using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DeliveryInfo : MonoBehaviour
{
    public TextMeshProUGUI Distance;
    public TextMeshProUGUI Damage;
    public TextMeshProUGUI MoneyEarned;
    Phone ThePhone;

    public void Show(Package P)
    {

        this.transform.gameObject.SetActive(true);
        ThePhone = GameObject.FindWithTag("Phone").GetComponent<Phone>();
        ThePhone.PopupOpen = true;

        Distance.text = Mathf.Round(P.InitialDistance).ToString() + "m";

        Damage.text = (100f - P.Durability).ToString() + "%";

        MoneyEarned.text = "+$" +
            Mathf.RoundToInt
            ((
                DifficultySettings.DistanceValueMultiplier * (P.InitialDistance/10f) * P.PackageValueBonus * (P.Durability/100f)
            )).ToString();
    }

    void Update()
    {
        if(Input.GetMouseButtonDown(1))
        {
            ThePhone.PopupOpen = false;
            this.transform.gameObject.SetActive(false);
        }
    }
}
