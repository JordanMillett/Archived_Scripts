using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Buoyancy : MonoBehaviour
{
    float SeaLevel = -2f;
    
    Rigidbody r;

    void Start()
    {
        r = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        if(this.transform.position.y < SeaLevel)
        {
            r.velocity += new Vector3(0f, 10f, 0f);
        }
    }
}
