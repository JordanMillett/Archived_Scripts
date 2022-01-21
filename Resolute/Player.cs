using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Player : MonoBehaviour
{    
    public float MaxSpeed;
    public float MovementForce;
    public float SprintMultiplier;
    public Transform ToPitch;

    Camera Cam;
    
    float CurrentSprint = 1f;

    Rigidbody r;

    float yaw = 0f;
    float pitch = 0f;
    float camLimits = 85f;
    float cameraSmoothing = 20f;

    void Start()
    {
        r = GetComponent<Rigidbody>();

        Cam = GameObject.FindWithTag("Camera").GetComponent<Camera>();
        Cam.transform.SetParent(ToPitch);
        Cam.transform.localEulerAngles = Vector3.zero;
        Cam.transform.localPosition = Vector3.zero;
        
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = 60;
    }

    void Update()
    {
        CameraControls();

        if(Input.GetMouseButtonDown(0))
        {
            RaycastHit hit;

            if(Physics.Raycast(transform.position, transform.forward, out hit, 25f))
            {
                try 
                {
                            
                    Glass G = hit.collider.transform.gameObject.GetComponent<Glass>();

                    if(G != null)
                    {
                        G.Break();
                    }
                    
                }
                catch{}
            }
        }
    }

    void FixedUpdate()
    {
        Movement();
        
        //CAM LOOK
        this.transform.localRotation = Quaternion.Lerp(this.transform.localRotation, Quaternion.Euler(0f, yaw, 0f), Time.fixedDeltaTime * cameraSmoothing);
        ToPitch.transform.localRotation = Quaternion.Lerp(ToPitch.transform.localRotation, Quaternion.Euler(pitch, 0f, 0f), Time.fixedDeltaTime * cameraSmoothing);
    }

    void CameraControls()
    {
        float _mouseSensitivity = 100f;

        yaw += (_mouseSensitivity/100f) * Input.GetAxis("Mouse X");
        pitch -= (_mouseSensitivity/100f) * Input.GetAxis("Mouse Y");
        
        if(pitch >= camLimits)
            pitch = camLimits;
            
        if(pitch <= -camLimits)
            pitch = -camLimits;
    }

    void Movement()
    {
		if(Input.GetKey("w") || Input.GetKey("a") || Input.GetKey("s") || Input.GetKey("d"))
        {
            if(Input.GetKey(KeyCode.LeftShift))
            {
                CurrentSprint = SprintMultiplier;
            }else
            {
                CurrentSprint = 1f;
            }

            Vector3 MoveDirection = Vector3.zero;

            if (Input.GetKey("w"))
                MoveDirection += transform.forward;

            if (Input.GetKey("a")) 
                MoveDirection += -transform.right * 0.75f;

            if (Input.GetKey("s")) 
                MoveDirection += -transform.forward;

            if (Input.GetKey("d")) 
                MoveDirection += transform.right * 0.75f;

            float lerp = Mathf.Lerp(1f, 0f, r.velocity.magnitude/(MaxSpeed * CurrentSprint));

            r.AddForce((MoveDirection * CurrentSprint * MovementForce) * lerp);
        }
    }
}