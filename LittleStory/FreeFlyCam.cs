using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FreeFlyCam : MonoBehaviour
{   
    public float FlySpeed = 5f;
    public float SpeedMult = 2f;
    public float MouseSens = 1f;
    public float camLimits = 80f;

    public float yaw = 0f;
    public float pitch = 0f;

    public int SuperAmount;

    float SpeedMultiplier = 1f;

    void Start()
    {
        Cursor.visible = false;
    }

    void Update()
    {   
        CameraControls();
        SprintControls();
        SpeedControls();

        if(Input.GetMouseButtonDown(0))
        {
            ScreenCapture.CaptureScreenshot("NewestScreenshot.png", SuperAmount);
            Debug.Log("Screenshot Taken");
        }
    }

    void SpeedControls()
    {

        if(Input.mouseScrollDelta.y < 0)
        {
            
            FlySpeed = FlySpeed / 2f;

        }else if(Input.mouseScrollDelta.y > 0)
        {
            
            FlySpeed = FlySpeed * 2f;

        }

    }

    void FixedUpdate()
	{   
		Movement();
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

    void Movement()
    {
		if(Input.GetKey("w") || Input.GetKey("a") || Input.GetKey("s") || Input.GetKey("d"))
        {
         
                Vector3 MoveDirection = Vector3.zero;

                if (Input.GetKey("w"))
                    MoveDirection += transform.forward;

                if (Input.GetKey("a")) 
                    MoveDirection += -transform.right;

                if (Input.GetKey("s")) 
                    MoveDirection += -transform.forward;

                if (Input.GetKey("d")) 
                    MoveDirection += transform.right;

                this.transform.position +=  FlySpeed * MoveDirection * SpeedMultiplier;
                
            

		}
    }

    void CameraControls()  
    {
        
        yaw += MouseSens * Input.GetAxis("Mouse X");

        transform.localEulerAngles = new Vector3(0f, yaw, 0f);

        pitch -= MouseSens * Input.GetAxis("Mouse Y");
        
        if(pitch >= camLimits)
            pitch = camLimits;
            
        if(pitch <= -camLimits)
            pitch = -camLimits;

        float y = this.transform.localEulerAngles.y;
        float z = this.transform.localEulerAngles.z;

        this.transform.localEulerAngles = new Vector3(pitch, y, z);
        
    }
}
