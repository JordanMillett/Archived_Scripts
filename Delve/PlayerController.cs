using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//using UnityEngine.Rendering.PostProcessing; vignette when moving fast

public class PlayerController : MonoBehaviour
{
    Camera cam;
    Rigidbody r;
    CapsuleCollider Col;
    PlayerStats PS;

    public float Maxspeed = 7f;
	public float MouseSens = 1f;
    public float DefaultMouseSens = 1f;
    public float camLimits = 80f;
    public float JumpForce = 100f;
    public float SpeedMult = 1.25f;
    public float SprintingFov = 90f;

    public bool Crouching = false;
    public bool Moving = false;
    public bool Frozen = false;

    public int SmoothedFrames = 5;

    public float yaw = 0f;
    public float pitch = 0f;

    public AnimationCurve HeadbobCurve;
    float HeadbobAlpha = 0f;
    float HeadbobAmplitude = -.025f;
    float HeadbobOffset = 0f;
    float HeadbobTime = 0f;
    float DefaultCameraHeight = 1.5f;

    float SpeedMultiplier = 1f;

    float DefaultFov = 85f; 
    float CurrentFov = 85f;
    float FovOffsetAlpha = 0f;

    Vector3 LastMoveDirection = Vector3.zero;

    List<float> pastYaw = new List<float>();
    List<float> pastPitch = new List<float>();

    int staminaCooldown = 0;

    //public AudioClip[] Sounds;
    //AudioSource Source;

    void Start()
    {
        //Source = GetComponent<AudioSource>();
        
        InitComponents();
    }

    void Update()
    {
        if(!Frozen)
        {
            Headbob(); //Headbob

            CameraControls(); //Looking
            JumpControls();   //Jumping
            CrouchControls(); //Crouching
            SprintControls(); //Sprinting
        }
    }

    void FixedUpdate()
	{   
        if(!Frozen)
        {
		    Movement();       //Move
        }
	}

    void InitComponents()
    {
        PS = GetComponent<PlayerStats>();
        Col = GetComponent<CapsuleCollider>();
		cam = this.gameObject.transform.GetChild(0).gameObject.GetComponent<Camera>();
        r = this.gameObject.GetComponent<Rigidbody>();

        InitArray(pastYaw);
        InitArray(pastPitch);

    }

    void InitArray(List<float> Values)
    {

        for(int i = 0; i < SmoothedFrames; i++)
        {

            Values.Add(0f);

        }

    }

    void Headbob()
    {
        
        if(HeadbobTime < 1f)
            HeadbobTime += 0.04f * SpeedMult;
        else
            HeadbobTime = 0f;


        if(Moving)
        {

            HeadbobOffset = Mathf.Lerp(HeadbobOffset, (1f - HeadbobCurve.Evaluate(HeadbobTime)) * HeadbobAmplitude * SpeedMult, HeadbobAlpha);
            if(HeadbobAlpha < 1f)
                HeadbobAlpha += 0.04f;

        }else
        {

            HeadbobOffset = Mathf.Lerp(HeadbobOffset, 0f, 1f - HeadbobAlpha);
            if(HeadbobAlpha > 0f)
                HeadbobAlpha -= 0.04f;
        

        }

        cam.transform.localPosition = new Vector3(0f, DefaultCameraHeight + HeadbobOffset, 0f);


    }

    void SprintControls()
    {

        float AlphaInterval = 0.1f;

        if(!Crouching)
        {   
            /*if(Input.GetKeyDown(KeyCode.LeftShift))
            {

                Source.clip = Sounds[Random.Range(0, Sounds.Length)];
                Source.enabled = true;
                Source.Play(0);

            }else
            {

                //Source.clip = Sounds[Random.Range(0, Sounds.Length)];
                //Source.enabled = false;

            }*/

            if(Input.GetKey(KeyCode.LeftShift) && (PS.Stamina > 0f)) //Sprinting
            {
                
                
                SpeedMultiplier = SpeedMult;
                staminaCooldown = 0;

                if(Moving)
                {
                    PS.AddValue(2, -1f);

                    CurrentFov = Mathf.Lerp(DefaultFov, SprintingFov, FovOffsetAlpha);
                    if(FovOffsetAlpha <= 1f)
                        FovOffsetAlpha += AlphaInterval;
                }else
                {

                    CurrentFov = Mathf.Lerp(SprintingFov, DefaultFov, 1 - FovOffsetAlpha);
                    if(FovOffsetAlpha >= 0f)
                        FovOffsetAlpha -= AlphaInterval;

                }

            }else
            {
                
                if(staminaCooldown > 100)
                {
                    PS.AddValue(2, .25f);
                }else
                {

                    staminaCooldown++;

                }

                SpeedMultiplier = 1f;

                CurrentFov = Mathf.Lerp(SprintingFov, DefaultFov, 1 - FovOffsetAlpha);
                if(FovOffsetAlpha >= 0f)
                    FovOffsetAlpha -= AlphaInterval;

            }
        }

        cam.fieldOfView = CurrentFov;

    }

    void JumpControls()
    {
 
        if(Input.GetKeyDown(KeyCode.Space)) //Jumping
            if(isGrounded() && !Crouching)
                r.AddForce(Vector3.up * JumpForce);

    }

    void CrouchControls()
    {

        if(Input.GetKey(KeyCode.LeftControl)) //Crouching
        {
            Col.height = 1.5f;
            Crouching = true;
        }
        else
        {
            Col.height = 2f;
            Crouching = false;
        }
        

    }

    void Movement()
    {



		if(Input.GetKey("w") || Input.GetKey("a") || Input.GetKey("s") || Input.GetKey("d"))
        {
           

            Moving = true;

            Vector3 MoveDirection = Vector3.zero;

			if (Input.GetKey("w"))
                MoveDirection += transform.forward;

			if (Input.GetKey("a")) 
                MoveDirection += -transform.right * 0.75f;

			if (Input.GetKey("s")) 
                MoveDirection += -transform.forward * 0.85f;

			if (Input.GetKey("d")) 
                MoveDirection += transform.right * 0.75f;

            LastMoveDirection = MoveDirection;
        

            //float dist = 0.25f;

            //RaycastHit hit;
            //if(!Physics.Raycast(this.transform.position, MoveDirection, out hit, dist))
            //if(isGrounded())
            //r.velocity += (MoveDirection * speed * SpeedMultiplier) * Time.fixedDeltaTime;
            if(r.velocity.magnitude < (Maxspeed * SpeedMultiplier))
                r.velocity += MoveDirection * SpeedMultiplier;
                //r.velocity += MoveDirection/2f;
            //r.MovePosition(r.position + (MoveDirection * speed * SpeedMultiplier) * Time.fixedDeltaTime);

            

		}else
        {
            Moving = false;

            /*if(speed > 0f)
            {
                if(isGrounded())
                    speed -= 0.4f;
                else
                    speed -= 0.1f;

                r.MovePosition(r.position + (LastMoveDirection * speed) * Time.fixedDeltaTime);
            }
            else
                speed = 0f;*/
            
        }

    }

    void CameraControls()  
    {

		//yaw += MouseSens * Input.GetAxis ("Mouse X");
        UpdateList(pastYaw, Input.GetAxis ("Mouse X"));
        if(SpeedMultiplier == 1f)
            yaw += MouseSens * GetAverage(pastYaw);
        else
            yaw += MouseSens * GetAverage(pastYaw) * 0.35f;


        transform.localEulerAngles = new Vector3(0f, yaw, 0f);

        //pitch -= MouseSens * Input.GetAxis ("Mouse Y");
        UpdateList(pastPitch, Input.GetAxis ("Mouse Y"));
        if(SpeedMultiplier == 1f)
            pitch -= MouseSens * GetAverage(pastPitch);
        else
            pitch -= MouseSens * GetAverage(pastPitch) * 0.35f;
        
		if(pitch >= camLimits)
			pitch = camLimits;
        
		
		if(pitch <= -camLimits)
			pitch = -camLimits;

        float y = cam.transform.localEulerAngles.y;
        float z = cam.transform.localEulerAngles.z;

        cam.transform.localEulerAngles = new Vector3(pitch, y, z);

    }

    void UpdateList(List<float> Values, float newNum)
    {

        Values.Add(newNum);
        Values.RemoveAt(0);

    }

    float GetAverage(List<float> Values)
    {

        float sum = 0f;

        foreach(float V in Values)
        {

            sum += V;

        }

        return sum/Values.Count;

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
