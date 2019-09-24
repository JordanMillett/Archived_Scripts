using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sun : MonoBehaviour
{
    public int DamageAmount;
    public float TimeDelay;
    bool canDamage = false;
    Ship S;

    void Start()
    {
        S = GameObject.FindWithTag("Ship").GetComponent<Ship>();
        InvokeRepeating("Damage",TimeDelay,TimeDelay);
    }

    void Damage()
    {
        if(canDamage)
            S.Damage(DamageAmount);

    }

    void OnTriggerEnter(Collider Object) 
    {
        if(Object.transform.root.gameObject == S.gameObject)
            canDamage = true;
    }

    void OnTriggerExit(Collider Object)
    {
        if(Object.transform.root.gameObject == S.gameObject)
            canDamage = false;
    }
}
