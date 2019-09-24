using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Arrow : MonoBehaviour 
{

    int Damage = 25;
    bool fired = false;

    void OnCollisionEnter(Collision collision)
    {

        if(fired == false)
        {

            Rigidbody r = this.gameObject.GetComponent<Rigidbody>();
            Destroy(r);
            Collider col = this.gameObject.GetComponent<Collider>();
            Destroy(col);

            try
            {
                Stats Entity = collision.gameObject.GetComponent<Stats>();

                if(Entity != null)
                {
                    this.gameObject.transform.SetParent(collision.gameObject.transform);
                    fired = true;
                    Entity.Health -= Damage;
            
                }
                    

            }
            catch (NullReferenceException) {}
    
        }

    }

}
