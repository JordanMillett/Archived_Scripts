using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveForward : MonoBehaviour
{
    Rigidbody r;

    public float Maxspeed = 5f;
    public float RotationSpeed = 1f;
    public float DirectionTime = 5f;

    Quaternion newRotation;

    void Start()
    {
        r = GetComponent<Rigidbody>();
        InvokeRepeating("UpdateDirection", 0f, DirectionTime);
    }

    void FixedUpdate()
	{   
		Movement(); 
        Turn();
    }

    void UpdateDirection()
    {
        Vector3 Target = this.transform.position + new Vector3(Random.Range(-1f, 1f), 0f, Random.Range(-1f, 1f));
        Vector3 Direction = Target - this.transform.position;
        newRotation = Quaternion.LookRotation(Direction);
    }

    void Turn()
    {
        this.transform.rotation = Quaternion.Slerp(this.transform.rotation, newRotation, Time.fixedDeltaTime * RotationSpeed);
    }

    void Movement()
    {
        Vector3 MoveDirection = transform.forward;
                
        if(r.velocity.magnitude < (Maxspeed))
        {
            r.velocity += MoveDirection;
        }
    }
}
