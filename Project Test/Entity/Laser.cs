using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Laser : MonoBehaviour 
{

    int Damage = 100;

    void OnCollisionEnter(Collision collision)
    {

        try
        {
            Stats Entity = collision.gameObject.GetComponent<Stats>();

            if(Entity != null)
            {

                Entity.Health -= Damage;
        
            }
                

        }
        catch (NullReferenceException) {}

        Destroy(this.gameObject);

    }

}
