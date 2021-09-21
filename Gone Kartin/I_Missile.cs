using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class I_Missile : MonoBehaviour
{
    public GameObject ExplosionEffect;
    public float FireForce;
    public float BlastRadius;
    public float BlastForce;

    public bool Fired = false;

    public void Use()
    {
        this.transform.SetParent(null);
        this.gameObject.AddComponent<Rigidbody>();
        this.transform.GetComponent<Rigidbody>().AddForce(transform.forward * FireForce);
        Fired = true;
        this.transform.GetComponent<ItemInvoker>().KC.ItemReference = null;
    }

    void OnCollisionEnter(Collision Col)
    {
        if(Fired)
        {
            GameObject E = GameObject.Instantiate(ExplosionEffect, Vector3.zero, Quaternion.identity);
            E.GetComponent<Explosion>().Radius = BlastRadius;
            E.GetComponent<Explosion>().Speed = 1.5f;
            E.transform.position = Col.contacts[0].point;

            Collider[] hitColliders = Physics.OverlapSphere(this.transform.position, BlastRadius);

            int i = 0;
            while (i < hitColliders.Length)
            {
                if(hitColliders[i].GetComponent<Rigidbody>() != null)
                {
                    Vector3 dir = hitColliders[i].transform.position - this.transform.position;

                    float percent = (Vector3.Distance(this.transform.position, hitColliders[i].transform.position))/(BlastRadius);
                    if(percent > 1)
                        percent = 1;
                    float newForce = BlastForce * (1 - percent);

                    hitColliders[i].GetComponent<Rigidbody>().AddForce(dir.normalized * newForce);
                }

                i++;
            }   

            Destroy(this.gameObject);
        }
    }
}
