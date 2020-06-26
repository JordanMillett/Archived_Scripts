using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlasmaBall : MonoBehaviour
{
   
    public float StartForce = 5f;
    public float Damage;

    Rigidbody r;

    void Start()
    {
        
        r = GetComponent<Rigidbody>();
        r.AddForce(this.transform.forward * StartForce);

    }

    void OnTriggerEnter(Collider Col)
    {

        if(Col.transform.GetComponent<LifeManager>() != null)
            Col.transform.GetComponent<LifeManager>().Damage(Damage);

        Destroy(this.gameObject);

    }

}
