using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pickupable : MonoBehaviour
{
    public float XPAmount = 25f;
    public Vector2Int MoneyAmount = Vector2Int.zero;
    public int AmmoAmount = 25;
    public float HealthAmount = 0f;

    public bool AddToInventory = false;

    bool running = false;

    public void Pickup()
    {
        if(!running)
            StartCoroutine(PickupOvertime());

    }

    IEnumerator PickupOvertime()
    {
        running = true;
        Destroy(this.transform.GetComponent<BoxCollider>());

        GameObject Player = GameObject.FindWithTag("Player");

        float lerpAlpha = 0f;
        Vector3 StartPosition = this.transform.position;


        while(lerpAlpha < 1f)
        {

            this.transform.position = Vector3.Lerp(StartPosition, Player.transform.position + new Vector3(0f,1f,0f), lerpAlpha);
            lerpAlpha += 0.10f;
            yield return null;

        }

        if(XPAmount > 0f)
            Player.GetComponent<PlayerStats>().AddXP(XPAmount);

        //Add Money
        if(MoneyAmount.x > 0)
            Player.GetComponent<PlayerStats>().AddMoney(Random.Range(MoneyAmount.x, MoneyAmount.y + 1));

        if(HealthAmount > 0f)
            Player.GetComponent<PlayerStats>().AddHealth(HealthAmount);

        if(AmmoAmount > 0)
            GameObject.FindWithTag("EquipSlot").transform.GetChild(0).GetComponent<Weapon>().ReserveAmmo += AmmoAmount;

        if(AddToInventory)
        {

            GameObject.FindWithTag("EquipSlot").transform.GetChild(0).GetComponent<Weapon>().WC =
            transform.GetChild(0).GetChild(0).GetComponent<WeaponConfigToPopup>().WC;

            GameObject.FindWithTag("EquipSlot").transform.GetChild(0).GetComponent<Weapon>().Ammo = 
            GameObject.FindWithTag("EquipSlot").transform.GetChild(0).GetComponent<Weapon>().WC.MagSize;

        }

        Destroy(transform.gameObject);

    }
}
