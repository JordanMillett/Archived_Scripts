using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.VFX;

public class Health : MonoBehaviour
{
    public enum Team
    {
        Friendly,
        Hostile
    }

    public Team Side = Team.Hostile;

    public int MaxHealth = 100;
    public int CurrentHealth = 100;
    public bool Dead = false;

    public GameObject DeathEffect;
    public Transform VFXOrigin;

    public UnityEvent OnDamage;
    public UnityEvent OnDeath;

    public Material HitMaterial;
    
    public void TakeDamage(int Amount)
    {
        //Debug.Log(Amount);

        if(!Dead)
        {
            if(Amount >= CurrentHealth)
            {   
                Dead = true;
                CurrentHealth = 0;
                OnDeath.Invoke();
            }else
            {
                CurrentHealth -= Amount;
                OnDamage.Invoke();
            }
        }
    }

    public void Die()
    {
        GameObject DE = Instantiate(DeathEffect, VFXOrigin.transform.position, Quaternion.identity);
        DE.transform.GetChild(0).GetComponent<VisualEffect>().SetVector4("Color", HitMaterial.GetColor("_Color"));
        Destroy(this.gameObject);
    }
}