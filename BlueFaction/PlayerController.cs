using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public GameObject Cam;
    public float SmoothCamSpeed = 10f;
    public float Maxspeed = 4f;

    GameObject CamEmpty;
    Rigidbody r;

    float MouseSens = 1f;
    float camLimits = 80f;
    float yaw = 0f;
    float pitch = 0f;

    void Start()
    {
        CamEmpty = transform.GetChild(0).transform.gameObject;
        r = GetComponent<Rigidbody>();
    }

    void Update()
    {
        CameraControls();
    }

    void FixedUpdate()
	{   
        Movement();
	}

    void Movement()
    {
		if(Input.GetKey("w") || Input.GetKey("a") || Input.GetKey("s") || Input.GetKey("d"))
        {
            Vector3 MoveDirection = Vector3.zero;

            if (Input.GetKey("w"))
                MoveDirection += transform.forward;

            if (Input.GetKey("a")) 
                MoveDirection += -transform.right * 0.75f;

            if (Input.GetKey("s")) 
                MoveDirection += -transform.forward * 0.85f;

            if (Input.GetKey("d")) 
                MoveDirection += transform.right * 0.75f;

            if(r.velocity.magnitude < Maxspeed)
            {
                r.velocity += MoveDirection;
            }
        }
    }

    void CameraControls()  
    {
        yaw += MouseSens * Input.GetAxis("Mouse X");
        this.transform.localEulerAngles = new Vector3(0f, yaw, 0f);
        pitch -= MouseSens * Input.GetAxis("Mouse Y");
        
        if(pitch >= camLimits)
            pitch = camLimits;
            
        if(pitch <= -camLimits)
            pitch = -camLimits;

        float y = CamEmpty.transform.localEulerAngles.y;
        float z = CamEmpty.transform.localEulerAngles.z;

        CamEmpty.transform.localEulerAngles = new Vector3(pitch, y, z);

        Cam.transform.position = CamEmpty.transform.position;
        Cam.transform.rotation = Quaternion.Slerp(Cam.transform.rotation, CamEmpty.transform.rotation, Time.fixedDeltaTime * SmoothCamSpeed);
        
    }
}