using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPC : MonoBehaviour
{
    
	Rigidbody r;
    GameObject Player;
    GameObject Dialogue;

	//public Renderer Stripe;

	//public bool Shop;
	//public GameObject[] ShopContents;

    public string Name;
    public string Prefix;
    public string Location;

	public GameObject Wheel;
	public bool Moving = false;
	public float WalkSpeed = 1f;
    public float Follow_Distance = 1f;
	bool Forward = true;
	bool Stop = false;
	int count = 0;
    


    bool Talking = false;
    bool Following = false;

    void Start()
    {
		r = this.gameObject.GetComponent<Rigidbody>();
        Player = GameObject.FindWithTag("Player");
        Dialogue = this.transform.GetChild(1).gameObject;
        Dialogue.SetActive(false);

        Name = Prefix + GetName();

		//Stripe.material.SetColor("_BaseColor", Random.ColorHSV(0f, 1f, 0.5f, 0.5f, 1f, 1f));
    }
		
    void Update()
    {
        if(Talking)
            Talk();

        if(Following)
            Follow();
        
    }

	void FixedUpdate()
	{

		//if(Moving)
			//Spin();

		//if(!Stop)
			//Move(Forward);
		//else
			//Rotate();

	}

	void Move(bool F)
	{
		if(F)
        {
			r.MovePosition(r.position + (transform.forward * WalkSpeed) * Time.fixedDeltaTime);
            Spin(5f);
        }
		else
        {
			r.MovePosition(r.position + (transform.forward * -WalkSpeed) * Time.fixedDeltaTime);
            Spin(-5f);
        }


		Moving = true;

	}

	void Rotate()
	{
		//count++;
		//float Rot = Random.Range(-2f,2f);
		//if(count > 30)
		//{
			//count = 0;
			//Rot = Random.Range(-25f,25f);

		//}


		//this.transform.rotation = Quaternion.Lerp(this.transform.rotation,  this.transform.rotation * Quaternion.Euler(0f,Rot,0f), Time.time);
		this.transform.Rotate(0f,1f,0f);

	}

	void Spin(float Dir = 5f)
	{
		Wheel.transform.Rotate(Dir,0f,0f);

	}

	void OnCollisionEnter(Collision collision)
	{
		
		StartCoroutine(Halt());
	}

	IEnumerator Halt()
	{
		Forward = false;
		yield return new WaitForSeconds(1f);
		Stop = true;
		yield return new WaitForSeconds(1f);
		Forward = true;
		Stop = false;


	}

    public void StartTalk()
    {

        Talking = !Talking;
        Dialogue.SetActive(Talking);

    }

    void Talk()
    {

        

        Vector3 Target = Player.transform.position - this.transform.position;
        Target.y = 0;
        Quaternion newRotation = Quaternion.LookRotation(Target);
        this.transform.rotation = Quaternion.Slerp(this.transform.rotation, newRotation, Time.fixedDeltaTime * 3f);

        //float Direction = Vector3.Angle(Difference,transform.forward);

        //this.transform.rotation = Quaternion.Euler(0f,Direction,0f);

    }

    public void StartFollow()
    {

        Following = !Following;
        StartTalk();

    }

    void Follow()
    {

        float Distance = Vector3.Distance(this.transform.position, Player.transform.position);

        if(Distance > Follow_Distance) 
        {

            r.MovePosition(r.position + (transform.forward * WalkSpeed) * Time.fixedDeltaTime);
            Spin(8f);

        }


        Vector3 Target = Player.transform.position - this.transform.position;
        Target.y = 0;
        Quaternion newRotation = Quaternion.LookRotation(Target);
        this.transform.rotation = Quaternion.Slerp(this.transform.rotation, newRotation, Time.fixedDeltaTime * 3f);
            

    }

    string GetName()
    {

        return Random.Range(1000,9999).ToString();

    }

}
