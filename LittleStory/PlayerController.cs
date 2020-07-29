using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    Camera cam;
    Rigidbody r;
    CapsuleCollider Col;

    public float Maxspeed = 7f;
	public float MouseSens = 1f;
    public float DefaultMouseSens = 1f;
    public float camLimits = 80f;
    public float JumpForce = 100f;
    public float SpeedMult = 1.25f;

    public bool Moving = false;
    public bool Sprinting = false;
    public bool Grounded = true;

    public bool CanUseMouse = true;
    public bool CanMove = true;

    public float yaw = 0f;
    public float pitch = 0f;

    float SpeedMultiplier = 1f;
    public float ItemSpeedMultiplier = 1f;

    Vector3 MoveDirection = Vector3.zero;

    void Start()
    {
        Cursor.visible = false;
        InitComponents();
    }

    void Update()
    {   
        CameraControls(); //Looking

        if(CanMove)
        {
            JumpControls();   //Jumping
            SprintControls(); //Sprinting
        }
    }

    void FixedUpdate()
	{   
        if(CanMove)
		    Movement();       //Move
	}

    void InitComponents()
    {
        Col = GetComponent<CapsuleCollider>();
		cam = this.gameObject.transform.GetChild(0).gameObject.GetComponent<Camera>();
        r = this.gameObject.GetComponent<Rigidbody>();
    }

    void SprintControls()
    {
        if(Input.GetKey(KeyCode.LeftShift)) //Sprinting
        {
            SpeedMultiplier = SpeedMult;
            Sprinting = true;
        }else
        {
            SpeedMultiplier = 1f;
            Sprinting = false;
        }
    }

    void JumpControls()
    {
        if(Input.GetKeyDown(KeyCode.Space)) //Jumping
            if(Grounded)
                r.AddForce(Vector3.up * JumpForce);
    }

    void Movement()
    {
        Grounded = isGrounded();

		if(Input.GetKey("w") || Input.GetKey("a") || Input.GetKey("s") || Input.GetKey("d"))
        {
            if(Grounded)
            {
                Moving = true;
                MoveDirection = Vector3.zero;

                if (Input.GetKey("w"))
                    MoveDirection += transform.forward;

                if (Input.GetKey("a")) 
                    MoveDirection += -transform.right * 0.75f;

                if (Input.GetKey("s")) 
                    MoveDirection += -transform.forward * 0.85f;

                if (Input.GetKey("d")) 
                    MoveDirection += transform.right * 0.75f;

                if(r.velocity.magnitude < (Maxspeed * SpeedMultiplier * ItemSpeedMultiplier))
                {
                    r.velocity += MoveDirection * SpeedMultiplier * ItemSpeedMultiplier;
                }
            }

		}else
        {
            Moving = false;
        }

    }

    void CameraControls()  
    {
        if(CanUseMouse)
            yaw += MouseSens * Input.GetAxis("Mouse X");

        transform.localEulerAngles = new Vector3(0f, yaw, 0f);

        if(CanUseMouse)
            pitch -= MouseSens * Input.GetAxis("Mouse Y");
        
        if(pitch >= camLimits)
            pitch = camLimits;
            
        if(pitch <= -camLimits)
            pitch = -camLimits;

        float y = cam.transform.localEulerAngles.y;
        float z = cam.transform.localEulerAngles.z;

        cam.transform.localEulerAngles = new Vector3(pitch, y, z);
        
    }

    public bool isGrounded()
    {
        RaycastHit hit;
        if(Physics.Raycast(this.transform.position + new Vector3(0f, 0.05f, 0f), -Vector3.up, out hit, 0.1f))
            return true;
        else
            return false;
    }
}