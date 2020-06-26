using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class AmmoCounter : MonoBehaviour
{
    TextMeshProUGUI ActiveAmmo;
    TextMeshProUGUI ReserveAmmo;

    void Start()
    {

        ActiveAmmo = transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        ReserveAmmo = transform.GetChild(1).GetComponent<TextMeshProUGUI>();

    }

    public void SetActiveAmmo(int Amount)
    {
        ActiveAmmo.text = Amount.ToString();
    }

    public void SetReserveAmmo(int Amount)
    {
        ReserveAmmo.text = Amount.ToString();
    }
}
