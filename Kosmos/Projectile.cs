using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float LifeSpan = 5f;
    public float Damage = 0f;
    public int Team = 0;

    public GameObject Decal;

    bool hit = false;

    void Start()
    {
        Invoke("Delete", LifeSpan);
    }

    void OnCollisionEnter(Collision Col)
    {
    
        if(!hit)
        {   
            if(Col.transform.GetComponent<Projectile>() == null)
            {

                if(Col.transform.GetComponent<ShipStats>() != null)
                    if(Team != Col.transform.GetComponent<ShipStats>().Team)
                        Col.transform.GetComponent<ShipStats>().Damage(Damage);

                Destroy(GetComponent<Collider>());
                Destroy(GetComponent<Rigidbody>());

                if(Decal != null)
                {
                    GameObject Exp = Instantiate(Decal, this.transform.position, Quaternion.identity);  //explode with effect
                    Exp.transform.SetParent(this.transform.parent);
                }
            }
            //GetComponent<Rigidbody>().velocity = Vector3.zero;
            //GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
            //GetComponent<Rigidbody>().isKinematic = true;
        }

        hit = true;
    }

    void Delete()
    {
        Destroy(this.gameObject);
    }
}
