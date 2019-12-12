using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    public float Health;
    public float MaxHealth;
    public float Shields;
    public float MaxShields;
    public float XP;
    public float MaxXP;
    //public float TotalXP;
    public int Level;
    public int Money;

    public void Damage(float Amount)
    {
        
        if(Shields - Amount > 0f)
        {

            Shields -= Amount;

        }else
        {

            Amount -= Shields;
            Shields = 0f;

            if(Health - Amount > 0f)
            {

                Health -= Amount;

            }else
            {

                Health = 0f;
                StartCoroutine(Respawn());

            }
        }
    }

    public void AddXP(float Amount)
    {    

        XP += Amount;

        if(Amount > 0f)
            GameObject.FindWithTag("Hud").transform.GetChild(3).GetComponent<Notification>().SpawnNotification("+" + Amount + "xp");
        //TotalXP += Amount;

        if(XP >= MaxXP) //Level Up
        {

            XP -= MaxXP;
            Level++;
            GameObject.FindWithTag("Hud").transform.GetChild(5).GetComponent<Notification>().SpawnNotification("Level Up!");
            MaxXP += 25f;

            if(XP >= MaxXP)
                AddXP(0f);

        }
    }

    public void AddMoney(int Amount)
    {

        Money += Amount;

        if(Amount > 0)
            GameObject.FindWithTag("Hud").transform.GetChild(4).GetChild(2).GetComponent<Notification>().SpawnNotification("+$" + Amount);

    }

    public void AddHealth(float Amount)
    {

        Health += Amount;
    
    }

    IEnumerator Respawn()
    {  
        GameObject MainCam = GameObject.FindWithTag("MainCamera");

        GetComponent<PlayerController>().enabled = false;
        MainCam.SetActive(false);
        GameObject.FindWithTag("RespawnCam").GetComponent<RespawnRoom>().Toggle();
        yield return new WaitForSeconds(4f);
        this.transform.position = Vector3.zero;
        Health = MaxHealth;
        Shields = MaxShields;
        Money -= Mathf.RoundToInt(Mathf.Floor(Money * .1f));
        GameObject.FindWithTag("RespawnCam").GetComponent<RespawnRoom>().Toggle();
        MainCam.SetActive(true);
        GetComponent<PlayerController>().enabled = true;

    }
}
