using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FreeCamMovement : MonoBehaviour 
{
    Camera cam;
    Rigidbody r;

    float speed = 5f;
    float yaw = 0.0f;
    float pitch = 0.0f;
	float speedH = 2.0f;
    float speedV = 2.0f;


	void Start () 
    {
		cam = this.transform.GetChild(0).GetComponent<Camera>();
        r = this.transform.GetComponent<Rigidbody>();
	}
	
	void Update () 
    {
		FOVcontrol();
        Flycontrol();
        Lookcontrol();
	}

    void Flycontrol()
    {   
        if(Input.GetKey(KeyCode.LeftShift))
            speed = 30f;
        else
            speed = 15f;

        if (Input.GetKey("e"))
			r.MovePosition(r.position + (transform.up * speed) * Time.fixedDeltaTime);

        if (Input.GetKey("q"))
			r.MovePosition(r.position + (-transform.up * speed) * Time.fixedDeltaTime);

        if (Input.GetKey("w"))
			r.MovePosition(r.position + (transform.forward * speed) * Time.fixedDeltaTime);

		if (Input.GetKey("a")) 
			r.MovePosition(r.position + (-transform.right * speed) * Time.fixedDeltaTime);

		if (Input.GetKey("s")) 
			r.MovePosition(r.position + (-transform.forward * speed) * Time.fixedDeltaTime);

		if (Input.GetKey("d")) 
			r.MovePosition(r.position + (transform.right * speed) * Time.fixedDeltaTime);

    }

    void Lookcontrol()
    {

		yaw += speedH * Input.GetAxis ("Mouse X");
		pitch -= speedV * Input.GetAxis ("Mouse Y");
		

		if(pitch > 89f)
		{
			pitch = 89f;
		}

		
		if(pitch < -89f)
		{

			pitch = -89f;

		}

		transform.eulerAngles = new Vector3(0.0f, yaw, 0.0f);

        float y = this.transform.eulerAngles.y;
        float z = this.transform.eulerAngles.z;

		this.transform.eulerAngles = new Vector3(pitch, y, z); 

    }

    void FOVcontrol()
    {

        if(Input.GetAxis("Mouse ScrollWheel") > 0)
		{
			cam.fieldOfView -= 5;
		}else if(Input.GetAxis("Mouse ScrollWheel") < 0)
		{
			cam.fieldOfView += 5;
		}

    }
    
}
