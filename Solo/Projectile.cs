using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float Speed = 100f;

    void Update()
    {
        this.transform.Translate(transform.forward * Time.deltaTime * Speed,  Space.World);
        //Debug.DrawRay(this.transform.position, transform.forward * 10f, Color.blue);
    }
    
    void OnCollisionEnter(Collision col)
    {
        Debug.Log(col.gameObject.name);
        Destroy(this.gameObject);
    }
}
