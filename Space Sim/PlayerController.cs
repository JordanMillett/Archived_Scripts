using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
	GameObject body;
    Camera cam;
    Rigidbody r;
    float yaw = 0.0f;
    public float pitch = 0.0f;
    public float currentPitch = 0.0f;

    bool Equipped = false;
    bool Stopped = true;
    float speedMult = 1f;

    float speed = 0f;
    public float Maxspeed = 5f;
	public float MouseSens = 1f;
    public float camLimits = 60f;

    public AnimationCurve Headbob;
    float DefaultCamY = 0f;
    float t = 0f;

    public GameObject EquippedSlot;
    public GameObject Crosshair;

    Vector3 SidePos;
    bool ADS = false;
    bool Aiming = false;

    void Start()
    {
        InitComponents();
        Cursor.visible = false;
        DefaultCamY = cam.gameObject.transform.localPosition.y;
        SidePos = EquippedSlot.transform.localPosition;
    }

    void Update()
    {

        CameraControls();

        if(Input.GetKeyDown("r"))
            Equip();

        if(Input.GetKey(KeyCode.LeftShift))
            speedMult = 1.5f;
        else
            speedMult = 1f;

        //if(Input.GetKeyDown("e"))
            //StartCoroutine(ScreenShake(MagTest,DurTest));

    }

    void FixedUpdate()
	{
        
		Movement();

        Fire();

	}

    void InitComponents()
    {

		cam = this.gameObject.transform.GetChild(0).gameObject.GetComponent<Camera>();
        body = this.gameObject.transform.GetChild(1).gameObject;
        r = this.gameObject.GetComponent<Rigidbody>();

    }

    void Equip()
    {

        Equipped = !Equipped;

        EquippedSlot.SetActive(Equipped);

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

            Stopped = false;

		}else
        {
            speed = 0f;
            Stopped = true;
        }

        Cambob();

    }

    void CameraControls()  //re did camera pitch so it adds so recoil can also be added and not overridded
    {

		yaw += MouseSens * Input.GetAxis ("Mouse X");
        transform.localEulerAngles = new Vector3(0.0f, yaw, 0.0f);


		pitch -= MouseSens * Input.GetAxis ("Mouse Y");
        //pitch = -MouseSens * Input.GetAxis ("Mouse Y");

        currentPitch -= MouseSens * Input.GetAxis("Mouse Y");

        
		if(currentPitch >= camLimits)
        {
            currentPitch = camLimits;
			pitch = camLimits;
        }
		
		if(currentPitch <= -camLimits)
        {
            currentPitch = -camLimits;
			pitch = -camLimits;
        }
        

        

        float y = cam.transform.localEulerAngles.y;
        float z = cam.transform.localEulerAngles.z;

        //Debug.Log(currentPitch);

        //if(currentPitch > camLimits)
			//pitch = camLimits;
		
		//if(currentPitch < -camLimits)
			//pitch = -camLimits;

        cam.transform.localEulerAngles = new Vector3(pitch, y, z);
        //cam.transform.localEulerAngles = cam.transform.localEulerAngles + new Vector3(pitch,0f,0f);

    }

    void Cambob()
    {
        if(!Stopped)
        {
            t += 0.05f * speedMult;
            Vector3 newPos = new Vector3(0f,DefaultCamY - (Headbob.Evaluate(t) * speedMult),0f);
            cam.transform.localPosition = newPos;

            if(t >= 1)
                t = 0;
        }else
        {
            t = 0;
            cam.transform.localPosition = Vector3.Lerp(cam.transform.localPosition,new Vector3(0f,DefaultCamY,0f),Time.fixedDeltaTime);

        }

    }

    void Fire()
    {
        if(Equipped == true)
        {
            Gun G = EquippedSlot.transform.GetChild(0).gameObject.GetComponent<Gun>();

            if(Input.GetMouseButton(0) && G.W.Automatic)
                    G.Fire();

            if(Input.GetMouseButtonDown(0) && !G.W.Automatic)
                    G.Fire();

            if(Input.GetMouseButtonDown(1))
                StartCoroutine(AimDownSights());

        }


    }

    IEnumerator AimDownSights()
    {
        
        if(!Aiming)
        {
            Aiming = true;
            float ADS_Time = .3f;
            Gun G = EquippedSlot.transform.GetChild(0).gameObject.GetComponent<Gun>();

            if(ADS)
            {
                Crosshair.SetActive(true);
                float elapsedTime = 0f;
                while (elapsedTime < ADS_Time)
                {
                    EquippedSlot.transform.localPosition = Vector3.Lerp(G.ADSPos, SidePos, (elapsedTime / ADS_Time));
                    cam.fieldOfView = Mathf.Lerp(G.W.ADS_Fov, 85f, (elapsedTime / ADS_Time));
                    elapsedTime += Time.deltaTime;
                    yield return null;
                }

                EquippedSlot.transform.localPosition = SidePos;
                cam.fieldOfView = 85f;

            }else
            {
                Crosshair.SetActive(false);
                float elapsedTime = 0f;
                while (elapsedTime < ADS_Time)
                {
                    EquippedSlot.transform.localPosition = Vector3.Lerp(SidePos, G.ADSPos, (elapsedTime / ADS_Time));
                    cam.fieldOfView = Mathf.Lerp(85f, G.W.ADS_Fov, (elapsedTime / ADS_Time));
                    elapsedTime += Time.deltaTime;
                    yield return null;
                }

                EquippedSlot.transform.localPosition = G.ADSPos;
                cam.fieldOfView = G.W.ADS_Fov;

            }
            Aiming = false;

            ADS = !ADS;
        }
    }

    public IEnumerator ScreenShake(float Magnitude,float Duration)
    {
        Vector3 DefaultPos = cam.transform.localPosition;

        float currentTime = 0;

        while(currentTime < Duration)
        {

            float x = Random.Range(-1f,1f) * Magnitude;
            float y = Random.Range(-1f,1f) * Magnitude;

            cam.transform.localPosition = new Vector3(x,y,0f) + DefaultPos;

            currentTime += Time.deltaTime;

            yield return null;

        }

        cam.transform.localPosition = DefaultPos;
    
    }

    public void ResetRots()
    {
        transform.localEulerAngles = new Vector3(0.0f, 0.0f, 0.0f);
        cam.transform.localEulerAngles = new Vector3(0.0f,0.0f,0.0f);
    }
}
