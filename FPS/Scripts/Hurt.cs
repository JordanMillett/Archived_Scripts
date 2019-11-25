using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hurt : MonoBehaviour
{
    public int DamageAmount;
    public float Interval;

    public List<LifeManager> Things;
    //hit.transform.gameObject.GetComponent<LifeManager>().Damage(Damage);

    void Start()
    {

        InvokeRepeating("Damage", Interval, Interval);

    }


    void OnCollisionEnter(Collision col)
    {

        try{

            LifeManager LM = col.transform.gameObject.GetComponent<LifeManager>();

            if(LM != null)
            {

                Things.Add(LM);

            }

        }catch{}
    }

    void OnCollisionExit(Collision col)
    {

        try{

            LifeManager LM = col.transform.gameObject.GetComponent<LifeManager>();

            if(LM != null)
            {

                Things.Remove(LM);

            }

        }catch{}

    }

    void Damage()
    {
        if(Things.Count > 0)
        {
            foreach(LifeManager x in Things)
            {

                x.Damage(DamageAmount);

            }
        }
    }
}
