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

    public float yaw = 0f;
    public float pitch = 0f;

    public float yawOffset = 0f;
    public float pitchOffset = 0f;

    float SpeedMultiplier = 1f;

    public List<float> PastSpeeds = new List<float>();
    public float CurrentSpeedAverage = 0f;

    Vector3 MoveDirection = Vector3.zero;

    void Start()
    {
        InitComponents();
    }

    void Update()
    {
        CameraControls(); //Looking
        JumpControls();   //Jumping
        SprintControls(); //Sprinting
    }

    void FixedUpdate()
	{   
		Movement();       //Move
        //CurrentSpeed = r.velocity.magnitude;
        PastSpeeds.Add(r.velocity.magnitude);
        PastSpeeds.RemoveAt(0);

        CurrentSpeedAverage = 0f;

        for(int i = 0; i < PastSpeeds.Count; i++)
        {

            CurrentSpeedAverage += PastSpeeds[i];

        }

        CurrentSpeedAverage /= PastSpeeds.Count;

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
        }else
        {
            SpeedMultiplier = 1f;
        }
    }

    void JumpControls()
    {
        if(Input.GetKeyDown(KeyCode.Space)) //Jumping
            if(isGrounded())
                r.AddForce(Vector3.up * JumpForce);
    }

    void Movement()
    {
		if(Input.GetKey("w") || Input.GetKey("a") || Input.GetKey("s") || Input.GetKey("d"))
        {

            //RaycastHit hit;
			//if(!Physics.Raycast(this.transform.position, -Vector3.up, out hit, .1f) && Mathf.Abs(r.velocity.y) < 0.02f && Moving)

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
        
            /*if(!isGrounded()) //strange wall skating mechanic
            {
                if(Physics.OverlapCapsule(this.transform.position, this.transform.position + new Vector3(0f, 2f, 0f), .55f).Length > 1)
                {
                    MoveDirection = Vector3.zero;
                }
            }*/

            /*if(Physics.OverlapCapsule(this.transform.position + MoveDirection, this.transform.position + MoveDirection + new Vector3(0f, 2f, 0f), .5f).Length > 1)
            {
                MoveDirection = Vector3.zero;
            }*/
            /*
            if(MoveDirection != Vector3.zero)
                if((Mathf.Abs(r.velocity.x) > 1f) || (Mathf.Abs(r.velocity.z) > 1f))
                    MoveDirection = Vector3.zero;
            */

            //RaycastHit hit;
			//if(!Physics.Raycast(cam.transform.position, MoveDirection, out hit, 1f))

            
            if(r.velocity.magnitude < (Maxspeed * SpeedMultiplier))
            {
                r.velocity += MoveDirection * SpeedMultiplier;
                //r.AddForce(MoveDirection * SpeedMult * 10000f);
            }


		}else
        {
            Moving = false;
        }

    }

    void CameraControls()  
    {

		yaw += MouseSens * Input.GetAxis("Mouse X");

        transform.localEulerAngles = new Vector3(0f, yaw + yawOffset, 0f);

        pitch -= MouseSens * Input.GetAxis("Mouse Y");
    
		if(pitch >= camLimits)
			pitch = camLimits;
		
		if(pitch <= -camLimits)
			pitch = -camLimits;

        float y = cam.transform.localEulerAngles.y;
        float z = cam.transform.localEulerAngles.z;

        /*if(pitch < 0f)
        {
            if(pitchOffset + pitch > 0f)
            {
                pitchOffset += pitch;
            }else
            {
                pitchOffset = 0f;
            }
        }*/

        cam.transform.localEulerAngles = new Vector3(pitch - pitchOffset, y, z);
    }

    public bool isGrounded()
    {
        /*
        if(Physics.CheckSphere(this.transform.position + new Vector3(0f, -0.1f, 0f), 0.05f))
            return true;
        else
            return false;
            */
        
        RaycastHit hit;
        if(Physics.Raycast(this.transform.position + new Vector3(0f, 0.05f, 0f), -Vector3.up, out hit, 0.2f))
            return true;
        else
            return false;
        
    }
}