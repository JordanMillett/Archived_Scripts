using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    Camera cam;
    Rigidbody r;

    public float Maxspeed = 7f;
	public float MouseSens = 1f;
    public float camLimits = 80f;
    public float JumpForce = 100f;

    public float yaw = 0f;
    public float pitch = 0f;
    float speed = 0f;

    void Start()
    {
        InitComponents();
    }

    void Update()
    {
        CameraControls(); //Looking
        JumpControls();   //Jumping
    }

    void FixedUpdate()
	{
		Movement();       //Move
	}

    void InitComponents()
    {
		cam = this.gameObject.transform.GetChild(0).gameObject.GetComponent<Camera>();
        r = this.gameObject.GetComponent<Rigidbody>();
    }

    void JumpControls()
    {
 
        if(Input.GetKeyDown(KeyCode.Space)) //Jumping
            if(isGrounded())
                r.AddForce(Vector3.up * JumpForce);

    }

    void Movement()
    {

        float heading = Mathf.Atan2(Input.GetAxis("Horizontal"),Input.GetAxis("Vertical"));

		if(Input.GetKey("w") || Input.GetKey("a") || Input.GetKey("s") || Input.GetKey("d"))
        {
            
            if(speed < Maxspeed)
                speed += 0.4f;

			if (Input.GetKey("w"))
                r.MovePosition(r.position + (transform.forward * speed) * Time.fixedDeltaTime);

			if (Input.GetKey("a")) 
                r.MovePosition(r.position + (-transform.right * speed) * Time.fixedDeltaTime);

			if (Input.GetKey("s")) 
                r.MovePosition(r.position + (-transform.forward * speed) * Time.fixedDeltaTime);

			if (Input.GetKey("d")) 
                r.MovePosition(r.position + (transform.right * speed) * Time.fixedDeltaTime);


		}else
        {
            speed = 0f;
        }

    }

    void CameraControls()  
    {

		yaw += MouseSens * Input.GetAxis ("Mouse X");
        transform.localEulerAngles = new Vector3(0f, yaw, 0f);

        pitch -= MouseSens * Input.GetAxis ("Mouse Y");
        
		if(pitch >= camLimits)
			pitch = camLimits;
        
		
		if(pitch <= -camLimits)
			pitch = -camLimits;

        float y = cam.transform.localEulerAngles.y;
        float z = cam.transform.localEulerAngles.z;

        cam.transform.localEulerAngles = new Vector3(pitch, y, z);

    }

    bool isGrounded()
    {

        RaycastHit hit;


        if(Physics.Raycast(this.transform.position + new Vector3(0f, 0.05f, 0f), -Vector3.up, out hit, 0.1f))
            return true;
        else
            return false;

    }
}
