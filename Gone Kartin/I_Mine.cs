using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class I_Mine : MonoBehaviour
{
    public GameObject ExplosionEffect;
    public float FireForce;
    public float BlastRadius;
    public float BlastForce;

    public bool Armed = false;
    public int ArmTime = 1;

    public void Use()
    {
        this.transform.SetParent(null);

        RaycastHit hit;
        if(Physics.Raycast(this.transform.position, -Vector3.up, out hit, 5f, LayerMask.GetMask("Default")))
        {
            this.transform.position = hit.point + (hit.normal * 0.10f);
            this.transform.rotation = Quaternion.FromToRotation(Vector3.up, hit.normal);
            this.transform.GetComponent<ItemInvoker>().KC.ItemReference = null;
            Invoke("Arm", ArmTime);
        }else
        {
            this.transform.GetComponent<ItemInvoker>().KC.ItemReference = null;
            Destroy(this.gameObject);
        }
    }

    void Arm()
    {
        Armed = true;
        this.transform.gameObject.layer = LayerMask.NameToLayer("Kart");
    }

    void OnTriggerEnter(Collider Col)
    {
        if(Armed)
        {
            if(Col.gameObject.CompareTag("Kart"))
            {
                GameObject E = GameObject.Instantiate(ExplosionEffect, Vector3.zero, Quaternion.identity);
                E.GetComponent<Explosion>().Radius = BlastRadius;
                E.GetComponent<Explosion>().Speed = 1.5f;
                E.transform.position = this.transform.position;

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
}
