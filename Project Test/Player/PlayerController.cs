using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

	
	Rigidbody r;
	Animator an;
	Animator hitan;
	Camera cam;
	Light l;
	GameObject body;
	GameObject ragdoll_model;
	MenuLoader Loader;

	public GameObject OBJTOSPAWN;

	public Below_Water wat;
	public GameObject camera;
	public GameObject player;
	public GameObject wep;
	public GameObject muzzle;
	public GameObject ragdoll;

	private float speed = 5;
    private float yaw = 0.0f;
    private float pitch = 0.0f;
	public float speedH = 2.0f;
    public float speedV = 2.0f;

	public bool invopen = false;
	
	public int fov = 60;

//	bool rifle = false;

	float speed_mult = 1;

	public bool isAlive = true;
	bool ragdoll_spawned = false;

	Vector3 lastposition;
	int lastpositioncount = 0;

	void Start () 
	{
		InitComponents();
		RayDown();
	}

	void Update () 
	{

		if(!ragdoll_spawned)
			Ragdoll();
	


		CamRot();

		if(isAlive){
			EquipTest();

			Spawn();
		}
		

		
		


	}

	void Spawn()
	{

		if(Input.GetKeyDown("t")){
			RaycastHit hit;
			if(Physics.Raycast(camera.transform.position,camera.transform.forward, out hit,100))
			{
				Instantiate(OBJTOSPAWN, hit.point + new Vector3(0,0f,0), Quaternion.identity);
			}
		}

	}

	void FixedUpdate()
	{
		if(isAlive && invopen == false)
			Movement();
		else
		{
			an.SetBool("Moving",false);
			an.SetBool("Sprinting",false);
		}

	}

	void InitComponents()
	{
		r = GetComponent<Rigidbody>();
		an = player.GetComponent<Animator>();
		cam = camera.GetComponent<Camera>();
		l = muzzle.GetComponent<Light>();
		body = this.gameObject.transform.GetChild(1).gameObject;
	}

	void Movement()
	{
		speed = 15 * speed_mult;

		 //Vector3 dir = new Vector3 (Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
		float heading = Mathf.Atan2(Input.GetAxis("Horizontal"),Input.GetAxis("Vertical"));
		//Quaternion heading_rot = Quaternion.Euler(0, heading, 0);
		
		lastpositioncount++;
		
		//if (Input.GetKeyDown("space") && OnGround())
				//	r.velocity += Vector3.up * 10f;

		if (Input.GetKeyDown("space") && OnGround())
			r.velocity = new Vector3(0f,6f,0f);
		else if(Input.GetKey("space") && this.transform.position.y < 5.5f)
		{
			float upspeed = Mathf.Lerp(0f, 6f, 5.5f - this.transform.position.y);
			r.velocity = new Vector3(0f,upspeed,0f);
		}





		if(Input.GetKey("w") || Input.GetKey("a") || Input.GetKey("s") || Input.GetKey("d")){

			player.transform.localEulerAngles = new Vector3(0f,heading*Mathf.Rad2Deg,0f);
			an.SetBool("Moving",true);
			//player.transform.rotation = Quaternion.Lerp(player.transform.rotation,heading_rot, Time.deltaTime);
			
			//new Vector3(0f,heading*Mathf.Rad2Deg,0f);

			if(Input.GetKey(KeyCode.LeftShift))
			{
				an.SetBool("Sprinting",true);
				speed_mult = 1;
				}else{
				an.SetBool("Sprinting",false);
				speed_mult = 0.5f;
			}

			if (Input.GetKey("w")) //MAKE MOVEMENT LIKE SYKRIIIIIM 
				r.MovePosition(r.position + (transform.forward * speed) * Time.fixedDeltaTime);

			if (Input.GetKey("a")) 
				r.MovePosition(r.position + (-transform.right * speed) * Time.fixedDeltaTime);

			if (Input.GetKey("s")) 
				r.MovePosition(r.position + (-transform.forward * speed) * Time.fixedDeltaTime);

			if (Input.GetKey("d")) 
				r.MovePosition(r.position + (transform.right * speed) * Time.fixedDeltaTime);

			//transform.eulerAngles = new Vector3(0.0f, yaw, 0.0f);

		}else
		{
			an.SetBool("Moving",false);
			an.SetBool("Sprinting",false);
			speed_mult = 0.5f;
		}

		if(lastpositioncount > 5)
		{
			lastposition = r.position;
			lastpositioncount = 0;
		}

	}

	void CamRot()
	{
		if (invopen == false) {
			yaw += speedH * Input.GetAxis ("Mouse X");
			pitch -= speedV * Input.GetAxis ("Mouse Y");
		} else {
			yaw = yaw;
			pitch = pitch;
		}

		if(pitch > 60f)
		{

			pitch = 60f;

		}

		
		if(pitch < -60f)
		{

			pitch = -60f;

		}


        //spin player with mouse x
		//transform.eulerAngles = new Vector3(0.0f, 0.0f, 0.0f); 
		//camera.transform.RotateAround(r.position, Vector3.up, 10 * Time.deltaTime); //cool rotate affect
		//camera.transform.RotateAround(r.position, Vector3.up, yaw);
		if(isAlive)
			transform.eulerAngles = new Vector3(0.0f, yaw, 0.0f);

		//an.SetFloat("ang",-pitch);  pitch gun up or down, obsolete

        float y = camera.transform.eulerAngles.y;
        float z = camera.transform.eulerAngles.z;

		camera.transform.eulerAngles = new Vector3(pitch, y, z);  //tilt camera with mouse y

		GunCtrl();
			
	}

	void EquipTest()
	{
		/* if (Input.GetKey("1"))
			{
				an.SetBool("lmb",true);
			}else
				an.SetBool("lmb",false);*/
	}

	void Ragdoll()
	{
		if (Input.GetKeyDown("y") || !isAlive)
		{
			Inventory inv = this.gameObject.GetComponent<Inventory> ();

			if (inv.open)
				inv.OpenInventory();

			if (inv.paused)
				inv.Pause();
				

			ragdoll_spawned = true;

			player.GetComponent<Animator>().enabled = false;
			body.SetActive(false);
			r.isKinematic = true;
			this.gameObject.GetComponent<CapsuleCollider>().enabled = false;
			//ragdoll.SetActive(true);
			ragdoll_model = Instantiate(ragdoll, this.gameObject.transform.position, this.gameObject.transform.rotation);
			ragdoll_model.transform.SetParent(this.gameObject.transform);
			ragdoll_model.SetActive(true);
			ragdoll_model.name = "Ragdoll";
			isAlive = false;

			GameObject rig = body.gameObject.transform.GetChild(1).gameObject;
			GameObject rig_rag = ragdoll_model.gameObject.transform.GetChild(1).gameObject;

			Rigidbody[] rr;
			Transform[] rigpos;
			Transform[] rigpos_rag;

			rr = ragdoll_model.GetComponentsInChildren<Rigidbody>();
			rigpos = rig.GetComponentsInChildren<Transform>();
			rigpos_rag = rig_rag.GetComponentsInChildren<Transform>();

			for(int i = 0; i < rigpos.Length;i++)
			{
				rigpos_rag[i].position = rigpos[i].position;
				rigpos_rag[i].rotation = rigpos[i].rotation;
			}
			
			foreach (Rigidbody rigid in rr)
			{
				rigid.velocity = (r.position - lastposition) * 10f;
				//rigid.isKinematic = true;
			}
			

			
		}
	}

	void GunCtrl()
	{
		if (wat.UnderWater ())
			cam.fieldOfView = fov - 10;
		else if (Input.GetMouseButton (1) && invopen == false) 
			cam.fieldOfView = fov - 40;
		else
			cam.fieldOfView = fov;

		if(Input.GetKey("z"))
		{
			an.SetBool("sal",true);
		}else
			an.SetBool("sal",false);
	
		/* if (Input.GetMouseButtonDown(0))
		{
			an.SetBool("fire",true);
			l.intensity = 1000;
			Debug.DrawRay(wep.transform.position, wep.transform.forward * 50,Color.green,5);
			RaycastHit hit;
			if(Physics.Raycast(wep.transform.position,wep.transform.forward,out hit,100))
			{
				Debug.Log(hit.transform.name);
			}
		}else{
			an.SetBool("fire",false);
			l.intensity = 0;
			}*/
	}

	void OnDisable()
	{
		an.SetBool("Moving",false);
		an.SetBool("Sprinting",false);
		speed_mult = 0.5f;
	}

	bool OnGround()
	{
		return Physics.Raycast(transform.position, -Vector3.up + new Vector3(0f,0.3f,0f) , 0.6f);
	}

	void RayDown()
	{

		transform.position += new Vector3(0f,20,0f);

		RaycastHit hit;
		if(Physics.Raycast(transform.position,new Vector3(0,-90,0),out hit,150))
			transform.position = hit.point;
			
		try{
			GameObject loaderobj = GameObject.FindWithTag ("Loader");
			Loader = loaderobj.GetComponent<MenuLoader> ();
		}catch {}

		if(Loader != null)
			fov = Loader.FOV;
	}

}
