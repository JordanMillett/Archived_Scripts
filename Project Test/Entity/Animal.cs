using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Animal : MonoBehaviour 
{
	Rigidbody r;
	GameObject body;
	Animator an;
	GameObject Player;
	Stats stats;

    public GameObject ragdoll;
    GameObject ragdoll_model;
    bool Ragdolled = false;
    Vector3 lastposition;
	int lastpositioncount = 0;

	float speed;
	float yaw;
	float dir = 0f;
	float dist;
	bool safe = true;
	bool EnemySpotted = false;

	public float walkSpeed;
	public float runSpeed;

	int steps = 0;
	public int frequency;
	int currentAction;

	float ranSeed;


	void Start () 
	{
		r = GetComponent<Rigidbody>();
		body = this.gameObject.transform.GetChild(0).gameObject;
		an = body.GetComponent<Animator>();
		Player = GameObject.FindWithTag("Player");
		stats = GetComponent<Stats>();
		currentAction = Random.Range(1, 4);
		frequency = frequency * 120;
		ranSeed = Random.Range(100f, 40000f);
	}
	
	void FixedUpdate()
	{
		dist = Vector3.Distance(this.transform.position, Player.transform.position);

		if(stats.isAlive == false)
		{
			an.SetInteger("Move",0);
			enabled = false;

            if(!Ragdolled)
                Ragdoll();

		}else
        {
            lastpositioncount++;

            if(steps > frequency)
            {
                steps = 0;
                currentAction = Random.Range(1, 4);

            }

            if(dist < 20f)
                safe = false;

            if(dist > 30f)
                safe = true;


            if(!safe)
                RunFrom(Player.transform);
            else
            {
                if(currentAction == 1) //do nothing
                {
                    an.SetInteger("Move",0);
                    r.freezeRotation = true;

                }else
                {
                    RandomMovement();

                }
            }
            
            steps++;

            LookAt(Player.transform);

            if(lastpositioncount > 5)
		    {
			    lastposition = r.position;
			    lastpositioncount = 0;
		    }

        }

		//this.transform.rotation = Quaternion.Euler(this.transform.rotation.eulerAngles.x,this.transform.rotation.eulerAngles.y,0f);


	}

	void RunFrom(Transform Danger)
	{
        r.freezeRotation = false;
        r.isKinematic = false;
		an.SetInteger("Move",1);

		Vector3 Safety = this.gameObject.transform.position - Danger.position;

		Safety = new Vector3(Safety.x, 0f, Safety.z);


		//float newRot = Vector3.Angle(newDirection, this.gameObject.transform.position); 
		//transform.eulerAngles = new Vector3(0f,newRot,0f);

		

		//transform.rotation = Quaternion.LookRotation(Saftey, Vector3.up);

		transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(Safety, Vector3.up), Time.fixedDeltaTime / 0.25f);

		r.MovePosition(r.position + (transform.forward * runSpeed) * Time.fixedDeltaTime);

		

	}

	void RandomMovement()
	{
        r.freezeRotation = false;
        r.isKinematic = false;
		an.SetInteger("Move",1);

		yaw = Mathf.PerlinNoise(Time.time * 0.1f,ranSeed);

		yaw = (yaw - 0.5f) * 720f;

		//yaw = Mathf.Lerp(0,180,yaw)

		//Debug.Log("New Yaw : " + yaw);

		//new Vector3(0f, yaw, 0f);

		Quaternion newAngle = Quaternion.AngleAxis(yaw, Vector3.up);

		transform.rotation = Quaternion.Lerp(transform.rotation, newAngle, Time.fixedDeltaTime / 0.25f);
		//r.AddTorque(transform.up * angle);

		//transform.eulerAngles = new Vector3(0f, yaw, 0f);

		r.MovePosition(r.position + (transform.forward * walkSpeed) * Time.fixedDeltaTime);


		//transform.eulerAngles = new Vector3(transform.rotation.x,transform.rotation.y,0f);


			//body.transform.localEulerAngles = new Vector3(0f,heading*Mathf.Rad2Deg,0f);
			//an.SetBool("Moving",true);
		
			/* 
			if(Input.GetKey(KeyCode.LeftShift))
			{
				an.SetBool("Sprinting",true);
				speed_mult = 1;
				}else{
				an.SetBool("Sprinting",false);
				speed_mult = 0.5f;
			}
			*/

			//if (Input.GetKey("w"))
			//	r.MovePosition(r.position + (transform.forward * speed) * Time.fixedDeltaTime);

			//if (Input.GetKey("a")) 
			//	r.MovePosition(r.position + (-transform.right * speed) * Time.fixedDeltaTime);

			//if (Input.GetKey("s")) 
			//	

			//if (Input.GetKey("d")) 
			//	r.MovePosition(r.position + (transform.right * speed) * Time.fixedDeltaTime);

	

		//}else
		//{
		//	an.SetBool("Moving",false);
		//	an.SetBool("Sprinting",false);
		//	speed_mult = 0.5f;
		//}

	}

	void LookAt(Transform pos)
	{
		
		//float currentHeading = an.GetFloat("LR");

		if(dist < 50f)
		{

			Vector3 offset = this.gameObject.transform.position - pos.position;

			dir = Vector3.SignedAngle(offset, this.gameObject.transform.forward, Vector3.up); 
			

			if((dir > 90f) || (dir < -90f))
			{

				EnemySpotted = true;

				if(dir > 0)
					an.SetFloat("LR",90 - (dir - 90f)); //-90 to 90 for look direction
				
				if(dir < 0)
					an.SetFloat("LR",-(90 + (dir + 90f)));
			}
			else
			{
	
				
				an.SetFloat("LR",0f);
				EnemySpotted = false;

			}
			
		}else
		{


			an.SetFloat("LR",0f);
			EnemySpotted = false;

		}

		

	}

    void Ragdoll()
	{

		Ragdolled = true;

		body.GetComponent<Animator>().enabled = false;
		body.SetActive(false);
		r.isKinematic = true;
        Collider[] colls = this.gameObject.GetComponents<Collider>();
		foreach (Collider coll in colls)
			coll.enabled = false;
		//ragdoll.SetActive(true);
		ragdoll_model = Instantiate(ragdoll, this.gameObject.transform.position, this.gameObject.transform.rotation);
		ragdoll_model.transform.SetParent(this.gameObject.transform);
		ragdoll_model.SetActive(true);
		ragdoll_model.name = "Ragdoll";

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
			rigid.velocity = (r.position - lastposition) * 20f;
			//rigid.isKinematic = true;
		}
			
	}
}
