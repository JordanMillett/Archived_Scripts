using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Zombie : MonoBehaviour
{
    public float Maxspeed;
    public float UpdateFrequency;
    public float ZombieUpForce;

    GameObject Target;
    Rigidbody r;

    void Start()
    {
        Target = GameObject.FindWithTag("Player").gameObject;
        r = GetComponent<Rigidbody>();

        InvokeRepeating("Turn", 0f, UpdateFrequency);
    }

    void Update()
    {
        if(r.velocity.magnitude < Maxspeed)
        {
            r.AddForce(this.transform.forward * (Maxspeed * 100f));
        }
    }

    void Turn()
    {
        LookAt(Target.transform.position);
    }

    void LookAt(Vector3 Pos)
    {
        Vector3 yaw = Vector3.zero;

        yaw = this.transform.position - Pos;
        yaw = new Vector3(yaw.x, 0f, yaw.z);
        this.transform.eulerAngles = new Vector3(0f, -Vector3.SignedAngle(yaw, Vector3.forward, Vector3.up) + 180f, 0f);
    }
}
