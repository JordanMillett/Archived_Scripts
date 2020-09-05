using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlanetGravity : MonoBehaviour
{
    public float Gravity;
    public float SearchRadius;

    void FixedUpdate()
    {
        Collider[] Nearby = Physics.OverlapSphere(this.transform.position, SearchRadius);
        
        foreach(Collider Col in Nearby)
        {
            if(Col.transform.GetComponent<Rigidbody>() != null)
            {
                Vector3 ForceDirection = Col.transform.position - this.transform.position;
                Col.transform.GetComponent<Rigidbody>().AddForce(ForceDirection * Gravity);
            }
        }
    }
}
