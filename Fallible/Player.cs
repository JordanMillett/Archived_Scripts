using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Player : MonoBehaviour
{
    public Weapon CurrentGun;

    
    public float MaxSpeed;
    public float MovementForce;
    public float SprintMultiplier;
    public Transform ToPitch;
    public Transform AimTarget;

    Camera Cam;
    
    float CurrentSprint = 1f;

    Rigidbody r;

    float yaw = 0f;
    float pitch = 0f;
    float camLimits = 85f;
    float cameraSmoothing = 20f;

    float DefaultFOV = 80f;
    float CurrentFOV = 80f;

    void Start()
    {
        r = GetComponent<Rigidbody>();

        Cam = GameObject.FindWithTag("Camera").GetComponent<Camera>();
        Cam.transform.SetParent(ToPitch);
        Cam.transform.localEulerAngles = Vector3.zero;
        Cam.transform.localPosition = Vector3.zero;

        CurrentGun.PlayerHeld = true;
        CurrentGun.P = this;

    }

    void Update()
    {
        CameraControls();

        if(CurrentGun != null)
        {
            if(Input.GetMouseButton(0))
                CurrentGun.Fire();

            if(Input.GetMouseButton(1))
            {
                CurrentGun.Aimed = true;
            }else
            {
                CurrentGun.Aimed = false;
            }
        }
    }

    void FixedUpdate()
    {
        Movement();
        
        //CAM LOOK

        this.transform.localRotation = Quaternion.Lerp(this.transform.localRotation, Quaternion.Euler(0f, yaw, 0f), Time.fixedDeltaTime * cameraSmoothing);
        ToPitch.transform.localRotation = Quaternion.Lerp(ToPitch.transform.localRotation, Quaternion.Euler(pitch, 0f, 0f), Time.fixedDeltaTime * cameraSmoothing);

        //FOV

        float Goal = DefaultFOV;
        if(CurrentGun != null)
            Goal = CurrentGun.Aimed ? CurrentGun.ADSFOV : DefaultFOV;

        CurrentFOV = Mathf.Lerp(CurrentFOV, Goal, Time.fixedDeltaTime * 10f);

        Cam.fieldOfView = Mathf.Lerp(CurrentFOV, Goal, Time.fixedDeltaTime * 10f);
    }

    public void TakeDamage()
    {
        GameObject.FindWithTag("Volume").GetComponent<VolumeController>().Damage();
    }

    void CameraControls()
    {
        yaw += (Settings._mouseSensitivity/100f) * Input.GetAxis("Mouse X");
        pitch -= (Settings._mouseSensitivity/100f) * Input.GetAxis("Mouse Y");
        
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

            r.AddForce((MoveDirection * CurrentSprint * MovementForce) * (1f - (r.velocity.magnitude/(MaxSpeed * CurrentSprint))));
        }
    }

    public void Die()
    {
        Debug.Log("DEAD");
    }
}