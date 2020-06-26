using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//using UnityEngine.Rendering.PostProcessing; vignette when moving fast

public class PlayerController : MonoBehaviour
{
    Camera _camera;
    GameObject PitchOffset;
    Rigidbody _rigidbody;

    public float maxSpeed = 7f;
	public float mouseSensitivity = 1f;
    public float camLimits = 80f;

    public float yaw = 0f;
    public float pitch = 0f;

    public bool Moving = false;
    public bool Sprinting = false;

    float CurrentFov = 65f;
    float SprintingFov = 70f;
    float FovOffsetAlpha = 1f;
    float DefaultFov = 65f;
    float speedMult = 1f;

    void Start()
    {
        InitComponents();
    }

    void Update()
    {
        CameraControls();
        SprintControls(); 
    }

    void FixedUpdate()
	{   
		Movement();
	}

    void InitComponents()
    {
        PitchOffset = this.gameObject.transform.GetChild(0).gameObject;
		_camera = this.gameObject.transform.GetChild(0).GetChild(0).gameObject.GetComponent<Camera>();
        _rigidbody = this.gameObject.GetComponent<Rigidbody>();
    }

    void SprintControls()
    {

        if(Input.GetKey(KeyCode.LeftShift)) //Sprinting
        {
            Sprinting = true;
            speedMult = 2f;
            CurrentFov = Mathf.Lerp(DefaultFov, SprintingFov, FovOffsetAlpha);
            if(FovOffsetAlpha < 1f)
                FovOffsetAlpha += 0.05f;
        }
        else
        {
            Sprinting = false;
            speedMult = 1f;
            CurrentFov = Mathf.Lerp(SprintingFov, DefaultFov, 1f - FovOffsetAlpha);
            if(FovOffsetAlpha > 0f)
                FovOffsetAlpha -= 0.05f;
        }

        _camera.fieldOfView = CurrentFov;

    }

    void Movement()
    {

		if(Input.GetKey("w") || Input.GetKey("a") || Input.GetKey("s") || Input.GetKey("d"))
        {
            Moving = true;

            Vector3 moveDirection = Vector3.zero;

			if (Input.GetKey("w"))
                moveDirection += transform.forward;

			if (Input.GetKey("a")) 
                moveDirection += -transform.right * 0.75f;

			if (Input.GetKey("s")) 
                moveDirection += -transform.forward * 0.85f;

			if (Input.GetKey("d")) 
                moveDirection += transform.right * 0.75f;
          
            if(_rigidbody.velocity.magnitude < maxSpeed * speedMult)
                _rigidbody.velocity += moveDirection * speedMult;

		}else
        {

            Moving = false;

        }

    }

    void CameraControls()  
    {

		yaw += mouseSensitivity * Input.GetAxis ("Mouse X");

        transform.localEulerAngles = new Vector3(0f, yaw, 0f);

        pitch -= mouseSensitivity * Input.GetAxis ("Mouse Y");
        
		if(pitch >= camLimits)
			pitch = camLimits;
        
		
		if(pitch <= -camLimits)
			pitch = -camLimits;

        //float y = _camera.transform.localEulerAngles.y;
        //float z = _camera.transform.localEulerAngles.z;

        float y = PitchOffset.transform.localEulerAngles.y;
        float z = PitchOffset.transform.localEulerAngles.z;

        PitchOffset.transform.localEulerAngles = new Vector3(pitch, y, z);

        //_camera.transform.localEulerAngles = new Vector3(pitch, y, z);

    }
}