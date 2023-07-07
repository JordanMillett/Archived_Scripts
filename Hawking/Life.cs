using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Life : MonoBehaviour
{
    public int MaxHealth = 100;
    public int Health = 100;
    public bool Dead = false;
    
    public UnityEvent OnHurt;
    public UnityEvent OnDie;
    
    void Start()
    {
        Health = MaxHealth;
    }
    
    public void Hurt(int Amount)
    {
        if(!Dead)
        {
            Health -= Amount;

            Debug.Log(this.gameObject.name + " hit for " + Amount);

            if(Health <= 0)
            {   
                Health = 0;
                Dead = true;
                OnDie.Invoke();
            }else
            {
                OnHurt.Invoke();
            }
        }
    }
}
