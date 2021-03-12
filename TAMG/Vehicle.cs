using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Vehicle : MonoBehaviour
{
    Rigidbody r;
    public Transform Seat;

    GameObject Player;


    public bool inUse = false;

    float Maxspeed = 7f;
    float TurnSpeed = 2f;
    float yaw = 0f;

    public AnimationCurve TurningSpeedCurve;

    void Start()
    {
        Player = GameObject.FindWithTag("Player").gameObject;

        r = GetComponent<Rigidbody>();
    }

    public void Toggle()
    {
        inUse = !inUse;
        
        Player.GetComponent<Rigidbody>().isKinematic = inUse;
        Player.GetComponent<Player>().ToggleColliders();
        Player.transform.position = Seat.position;

    }

    void Update()
    {
        if(inUse)
            Player.transform.position = Seat.position;
    }

    void FixedUpdate()
	{   
        if(inUse)
            Movement();
	}

    void Movement()
    {
		if(Input.GetKey("w") || Input.GetKey("a") || Input.GetKey("s") || Input.GetKey("d"))
        {
            Vector3 MoveDirection = Vector3.zero;
            float Torque = 0f;

            if (Input.GetKey("w"))
                MoveDirection += transform.forward;

            if (Input.GetKey("a")) 
                Torque += -1f;

            if (Input.GetKey("s")) 
                MoveDirection += -transform.forward * 0.5f;

            if (Input.GetKey("d")) 
                Torque += 1f;

            if(r.velocity.magnitude < Maxspeed)
            {
                r.velocity += MoveDirection;
            }

            yaw += Torque * Mathf.Lerp(0f, TurnSpeed, TurningSpeedCurve.Evaluate(r.velocity.magnitude/Maxspeed));
            this.transform.localEulerAngles = new Vector3(0f, yaw, 0f);
        }
    }
}
