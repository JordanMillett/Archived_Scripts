using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnLocation : MonoBehaviour
{
    public GameObject Enemy;
    public int TotalAmount;
    public float Interval;
    int AmountLeft;
    public float CooldownTime;

    void Start()
    {
        AmountLeft = TotalAmount;
        InvokeRepeating("Spawn", Interval, Interval);
        InvokeRepeating("Cooldown", CooldownTime, CooldownTime);
    }

    void Spawn()
    {
        if(AmountLeft > 0)
        {
            AmountLeft--;
            Instantiate(Enemy, this.transform.position, Quaternion.identity);
        }
    }
    
    void Cooldown()
    {

        AmountLeft = TotalAmount;

    }
}
