using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Payload : MonoBehaviour
{
    public Projectile Info;

    void OnCollisionEnter(Collision collision)
    {

        try
        {
            Ship Entity = collision.transform.root.gameObject.GetComponent<Ship>();

            if(Entity != null)
            {

                Entity.Damage(Info.Damage);
        
            }
                

        }
        catch (NullReferenceException) {}

        Destroy(this.gameObject);

    }
}
