using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spin : MonoBehaviour
{
    Rigidbody r;
    public float torque;

    void Start()
    {
        r = GetComponent<Rigidbody>();

    }

    void FixedUpdate()
    {
        r.AddTorque(transform.forward * torque);
    }
}
