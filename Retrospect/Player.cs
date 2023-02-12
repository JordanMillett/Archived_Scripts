using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    static bool InHead = true;

    public bool Soul = false;

    float Speed = 3f;
    float MovementForce = 50f;
    float pitchLimit = 85f;

    public Transform Eyes;

    Rigidbody r;

    float yaw = 0f;
    float pitch = 0f;

    void Start()
    {
        r = GetComponent<Rigidbody>();
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Tab) && Soul)
            InHead = !InHead;

        if(InHead == Soul)
        {
            float _mouseSensitivity = 100f;

            yaw += (_mouseSensitivity/100f) * Input.GetAxis("Mouse X");
            pitch -= (_mouseSensitivity/100f) * Input.GetAxis("Mouse Y");

            if(pitch >= pitchLimit)
                pitch = pitchLimit;
                
            if(pitch <= -pitchLimit)
                pitch = -pitchLimit;

            transform.localRotation = Quaternion.Euler(0f, yaw, 0f);
            Eyes.localRotation = Quaternion.Euler(pitch, 0f, 0f);
        }
    }

    void FixedUpdate()
    {
        if(InHead == Soul)
        {
            Vector3 MoveDirection = Vector3.zero;
            if(Input.GetKey("w") || Input.GetKey("a") || Input.GetKey("s") || Input.GetKey("d"))
            {
                if (Input.GetKey("w"))
                    MoveDirection += transform.forward;

                if (Input.GetKey("a")) 
                    MoveDirection += -transform.right * 0.75f;

                if (Input.GetKey("s")) 
                    MoveDirection += -transform.forward;

                if (Input.GetKey("d")) 
                    MoveDirection += transform.right * 0.75f;

                float lerp = Mathf.Lerp(1f, 0f, r.velocity.magnitude/Speed);
                r.AddForce(MoveDirection * MovementForce * lerp);
            }
        }
    }
}
