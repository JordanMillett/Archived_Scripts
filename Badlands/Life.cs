using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Life : MonoBehaviour
{
    public bool isPlayer = false;

    public int MaxHealth = 100;
    public int Health = 100;
    public int MaxShields = 0;
    public int Shields = 0;

    public bool Dead = false;

    public int ShieldRechargeSpeed = 1;
    public int ShieldRechargeDelay = 3;
    bool ShieldsRecharging = false;
    IEnumerator ShieldsRechargeCoroutine;

    bool EffectFading = false;
    IEnumerator EffectFadingCoroutine;

    public UnityEvent OnDamage;
    public UnityEvent OnDeath;

    public SkinnedMeshRenderer SMR;

    void Start()
    {
        Health = MaxHealth;
        Shields = MaxShields;
    }
    
    public void Hurt(int Amount, DamageBonus Bonus)
    {
        if(!Dead)
        {
            if (Amount > 0)
                OnDamage.Invoke();
            else
                return;

            int AdditionalDamage = Bonus == DamageBonus.Shields ? Mathf.RoundToInt((float)Amount / 4) : 0;

            if(Shields >= Amount + AdditionalDamage)
            {
                Shields -= Amount + AdditionalDamage;
                Amount = 0;

                if (SMR)
                {
                    SMR.material.SetColor("_Color", Color.blue * Mathf.Pow(2, 3f));
                    if (EffectFading)
                        StopCoroutine(EffectFadingCoroutine);
                    EffectFadingCoroutine = FadeEffect();
                    StartCoroutine(EffectFadingCoroutine);
                }
            }else
            {       
                Amount -= Shields;
                Shields = 0;

                if (SMR)
                {
                    SMR.material.SetColor("_Color", Color.red * Mathf.Pow(2, 3f));
                    if (EffectFading)
                        StopCoroutine(EffectFadingCoroutine);
                    EffectFadingCoroutine = FadeEffect();
                    StartCoroutine(EffectFadingCoroutine);
                }
            }
            
            AdditionalDamage = Bonus == DamageBonus.Health ? Mathf.RoundToInt((float)Amount / 4) : 0;
            
            if(Health > Amount + AdditionalDamage)
            {
                Health -= Amount + AdditionalDamage;
            }else
            {
                Health = 0;
                Dead = true;
                OnDeath.Invoke();
            }
            
            if(ShieldsRecharging)
                StopCoroutine(ShieldsRechargeCoroutine);
            ShieldsRechargeCoroutine = RechargeShields();
            StartCoroutine(ShieldsRechargeCoroutine);
        }
    }
    
    IEnumerator RechargeShields()
    {
        ShieldsRecharging = true;
        yield return new WaitForSeconds(ShieldRechargeDelay);

        while(Shields < MaxShields)
        {
            Shields += ShieldRechargeSpeed;
            yield return null;
        }

        ShieldsRecharging = false;
    }
    
    IEnumerator FadeEffect()
    {
        EffectFading = true;
        SMR.material.SetFloat("_Intensity", 1f);
        yield return null;

        float alpha = 1f;
        while(alpha > 0)
        {
            alpha -= 0.05f;
            SMR.material.SetFloat("_Intensity", alpha);
            yield return null;
        }
        
        SMR.material.SetFloat("_Intensity", 0f);

        EffectFading = false;
    }
}
