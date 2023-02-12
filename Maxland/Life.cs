using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Life : MonoBehaviour
{
    public int MaxHealth = 100;
    public int Health = 100;
    public bool Dead = false;
    public bool Invincible = false;

    public UnityEvent OnDamage;
    public UnityEvent OnDeath;
    
    void Start()
    {
        Health = MaxHealth;
    }

    public void Hurt(int Amount)
    {
        if (!Dead)
        {
            if(!Invincible)
                Health -= Amount;
                
            if (Health <= 0)
            {
                Health = 0;
                Dead = true;
                OnDeath.Invoke();
            }
            else
            {
                OnDamage.Invoke();
            }
        }
    }
}
