using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Buoyancy : MonoBehaviour
{
    public float SinkSpeed = 0f;
    public float Offset = 0f;

    public bool Floating = false;
    
    Rigidbody r;

    void Start()
    {
        r = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        if((Offset + this.transform.position.y) < 0f)
        {
            Floating = true;
            r.velocity += new Vector3(0f, 
            Mathf.Abs(
                (Offset + this.transform.position.y) * (1f - SinkSpeed)
                ), 0f);
        }else
        {
            Floating = false;
        }
    }
}
