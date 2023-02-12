using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Damage : MonoBehaviour
{
    public int MaxHealth = 100;
    public int Health = 100;
    public bool Dead = false;
    public bool Armored = false;

    public UnityEvent OnDamage;
    public UnityEvent OnDeath;
    public UnityEvent OnHeal;

    public string LastDamagedPart = "";

    void Start()
    {
        Health = MaxHealth;
    }

    public void Heal(int Amount)
    {
        if(!Dead && Health != MaxHealth)
        {
            Health += Amount;
            if(Health > MaxHealth)  
                Health = MaxHealth;
           
            OnHeal.Invoke();
        }
    }

    public void Hurt(int Amount, bool AntiArmor, bool Critical, bool PlayerDealt, string Part)
    {
        if(!Dead)
        {
            LastDamagedPart = Part;

            if(!Armored || AntiArmor)
            {
                if(Critical)
                    Amount *= 2;

                if(PlayerDealt && !Critical)
                    Player.H.Hit = 1;
                if(PlayerDealt && Critical)
                    Player.H.Hit = -1;

                Health -= Amount;
                if(Health <= 0)
                {   
                    if(PlayerDealt)
                        Player.H.Hit *= 2;
                    Health = 0;
                    Dead = true;
                    OnDeath.Invoke();
                }else
                {
                    OnDamage.Invoke();
                }
            }
        }
    }
}
