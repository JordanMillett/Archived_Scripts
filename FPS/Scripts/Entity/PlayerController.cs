using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{

	GameObject body;
    CapsuleCollider Col;
    Camera cam;
    Rigidbody r;
    LifeManager LM;

    public float yaw = 0.0f;
    public float pitch = 0.0f;
    public float Maxspeed = 7;
	public float MouseSens = 1f;
    public float ZoomedSensMult = 0.5f;
    public float camLimits = 60f;
    public float DefaultFov = 80f; 
    public float ZoomedFov = 30f;
    public float LeanDegrees = 20f;
    public float JumpForce = 5f;
    public AnimationCurve HeadbobCurve;
    public GameObject Crosshair;
    float MouseSensMult = 1f;
    public float speedMult = 1f;
    public float speed = 0f;
    float CurrentFov;
    float ZoomAlpha = 0;
    float SprintingFov = 5f;
    float FovOffset = 0f;
    float FovOffsetAlpha = 0f;
    float leanOffset = 0;
    float leanAlpha = 0.5f;
    float defaultHeight;
    float HeadbobAlpha = 0f;
    float HeadbobAmplitude = .05f;
    float HeadbobOffset = 0f;
    float DefaultCameraHeight = 0f;
    float HeadbobTime = 0f;
    public bool Crouching = false;


    void Start()
    {
        InitComponents();
    }

    void Update()
    {

        if(!LM.isDead)
        {
            CameraControls(); //Looking
            SprintControls(); //Sprinting
            ZoomControls();   //Zooming
            LeanControls();   //Leaning
            CrouchControls(); //Crouching
            JumpControls();   //Jumping

            Headbob();

            if(Input.GetKeyDown("f"))
                Use();
        }

    }

    void FixedUpdate()
	{
        if(!LM.isDead)
        {
		    Movement();     //Move
        }
	}

    void InitComponents()
    {

		cam = this.gameObject.transform.GetChild(0).gameObject.GetComponent<Camera>();
        body = this.gameObject.transform.GetChild(1).gameObject;
        r = this.gameObject.GetComponent<Rigidbody>();
        Col = this.gameObject.GetComponent<CapsuleCollider>();
        LM = this.gameObject.GetComponent<LifeManager>();
        defaultHeight = Col.height;

        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = false;
        CurrentFov = DefaultFov;

        DefaultCameraHeight = cam.transform.localPosition.y;
        
    }

    void Headbob()
    {
        
        if(HeadbobTime < 1f)
            HeadbobTime += 0.04f * speedMult;
        else
            HeadbobTime = 0f;

        //Debug.Log(HeadbobCurve.Evaluate(HeadbobTime));

        if(speed > 0f)
        {

            HeadbobOffset = Mathf.Lerp(HeadbobOffset, HeadbobCurve.Evaluate(HeadbobTime) * HeadbobAmplitude * speedMult, HeadbobAlpha);
            if(HeadbobAlpha < 1f)
                HeadbobAlpha += 0.02f;

        }else
        {

            HeadbobOffset = Mathf.Lerp(HeadbobOffset, 0f, 1f - HeadbobAlpha);
            if(HeadbobAlpha > 0f)
                HeadbobAlpha -= 0.02f;
        

        }

        cam.transform.localPosition = new Vector3(0f, DefaultCameraHeight + HeadbobOffset, 0f);


    }

    void CrouchControls()
    {
        if(Input.GetKey(KeyCode.LeftControl)) //Crouching
        {
            Col.height = defaultHeight/1.5f;
            Crouching = true;
        }
        else
        {
            Col.height = defaultHeight;
            Crouching = false;
        }
    }

    void JumpControls()
    {

        if(Input.GetKeyDown(KeyCode.Space)) //Jumping
            if(isGrounded())
                r.AddForce(Vector3.up * JumpForce);

    }

    void LeanControls()
    {

        if(Input.GetKey("q") && !Input.GetKey("e")) //Leaning
        {
            if(leanAlpha < 1f)
                leanAlpha += 0.05f;
        }
        if(Input.GetKey("e") && !Input.GetKey("q"))
        {   
            if(leanAlpha > 0f)
                leanAlpha -= 0.05f;
        }
        if(!Input.GetKey("e") && !Input.GetKey("q"))
        {
            if(leanAlpha > 0.55f)
                leanAlpha -= 0.05f;
            else if(leanAlpha < 0.45f)
                leanAlpha += 0.05f;
            else
                leanAlpha = 0.5f;
        }

        leanOffset = Mathf.Lerp(-LeanDegrees, LeanDegrees, leanAlpha);

    }

    void ZoomControls()
    {

        if(Input.GetMouseButton(1)) //Zooming
        {
            CurrentFov = Mathf.Lerp(CurrentFov, ZoomedFov, ZoomAlpha);
            if(ZoomAlpha < 1f)
                ZoomAlpha += 0.02f;
        
            MouseSensMult = ZoomedSensMult;
            Crosshair.SetActive(false);
        }
        else
        {
            CurrentFov = Mathf.Lerp(CurrentFov, DefaultFov, 1f - ZoomAlpha);
            if(ZoomAlpha > 0f)
                ZoomAlpha -= 0.02f;
        
            MouseSensMult = 1f;
            Crosshair.SetActive(true);
        }

        cam.fieldOfView = CurrentFov + FovOffset;

    }

    void SprintControls()
    {

        if(Input.GetKey(KeyCode.LeftShift)) //Sprinting
        {
            speedMult = 1.5f;
            FovOffset = Mathf.Lerp(FovOffset, SprintingFov, FovOffsetAlpha);
            if(FovOffsetAlpha < 1f)
                FovOffsetAlpha += 0.03f;
        }
        else
        {
            speedMult = 1f;
            FovOffset = Mathf.Lerp(FovOffset, 0f, 1 - FovOffsetAlpha);
            if(FovOffsetAlpha > 0f)
                FovOffsetAlpha -= 0.03f;
        }

    }

    void Movement()
    {

        float heading = Mathf.Atan2(Input.GetAxis("Horizontal"),Input.GetAxis("Vertical"));

		if(Input.GetKey("w") || Input.GetKey("a") || Input.GetKey("s") || Input.GetKey("d"))
        {
            
            if(speed < Maxspeed)
                speed += 0.4f;

			body.transform.localEulerAngles = new Vector3(0f,heading*Mathf.Rad2Deg,0f);

			if (Input.GetKey("w"))
                r.MovePosition(r.position + (transform.forward * speed * speedMult) * Time.fixedDeltaTime);

			if (Input.GetKey("a")) 
                r.MovePosition(r.position + (-transform.right * speed * speedMult) * Time.fixedDeltaTime);

			if (Input.GetKey("s")) 
                r.MovePosition(r.position + (-transform.forward * speed * speedMult) * Time.fixedDeltaTime);

			if (Input.GetKey("d")) 
                r.MovePosition(r.position + (transform.right * speed * speedMult) * Time.fixedDeltaTime);


		}else
        {
            speed = 0f;
        }

    }

    void CameraControls()  
    {

		yaw += MouseSens * MouseSensMult * Input.GetAxis ("Mouse X");
        transform.localEulerAngles = new Vector3(0.0f, yaw, leanOffset);

        pitch -= MouseSens * MouseSensMult * Input.GetAxis ("Mouse Y");
        
		if(pitch >= camLimits)
			pitch = camLimits;
        
		
		if(pitch <= -camLimits)
			pitch = -camLimits;

        float y = cam.transform.localEulerAngles.y;
        float z = cam.transform.localEulerAngles.z;

        cam.transform.localEulerAngles = new Vector3(pitch, y, z);

    }

    IEnumerator VectorLerp(Vector3 S, Vector3 D, float V) //Make Work
    {
        float startTime = Time.time;
        float journeyLength = Vector3.Distance(S, D);
        float fracJourney = 0f;
        float distCovered = 0f;

        while(fracJourney < 1f)
        {
            distCovered = (Time.time - startTime) * V;
            fracJourney = distCovered / journeyLength;
            S = Vector3.Lerp(S,D,fracJourney);
            yield return null;
        }

    }

    bool isGrounded()
    {

        RaycastHit hit;


        if(Physics.Raycast(this.transform.position + new Vector3(0f, 0.05f, 0f), -Vector3.up, out hit, 0.1f))
            return true;
        else
            return false;

    }

    void Use()
    {

        RaycastHit hit;

		if(Physics.Raycast(cam.transform.position,cam.transform.forward, out hit,1.5f))
		{
			try 
			{
						
				Interact I = hit.collider.transform.gameObject.GetComponent<Interact>();

				if(I != null)
					I.Activate();
				
			}
			catch{}
       }

    }

}