using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pebble : MonoBehaviour
{
    public float FreezeTime = 4f;
    float LastMoved = 0;

    Rigidbody r;

    void Start()
    {
        r = GetComponent<Rigidbody>();
        GetComponent<MeshRenderer>().material.SetFloat("_seed", Random.Range(1f, 100f));
        LastMoved = Time.time;
    }

    void Update()
    {
        if(Time.time > LastMoved + FreezeTime && r.velocity.magnitude < 0.25f)
        {
            r.isKinematic = true;
        }
    }
    
    void OnCollisionEnter(Collision Col)
    {
        if (r)
        {
            if (r.isKinematic)
            {
                if (!Col.gameObject.GetComponent<Pebble>())
                {
                    LastMoved = Time.time;
                    r.isKinematic = false;
                }
            }
        }
    }
}
