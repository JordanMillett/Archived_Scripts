using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Spider_Test : MonoBehaviour {

	GameObject Player;
	public float Detect_Distance;
	Inventory inv;
	Stats stats;

	Life_Manager life;
	Animator an;
	Rigidbody r;
	public float speed = 5f;
	public float charge_speed = 12f;
	public float dist;
	bool Cooling = false;
	int CoolDown = 3;
	int count = 0;

	void Start () 
	{
		an = GetComponentInChildren<Animator>();
		r = GetComponent<Rigidbody>();
		Player = GameObject.FindWithTag("Player");
		life = Player.GetComponent<Life_Manager>();
		inv = Player.GetComponent<Inventory>();
		stats = GetComponent<Stats>();
		InvokeRepeating ("Damage", 0.20f, 0.20f);
	}

	void FixedUpdate () 
	{

		if(stats.isAlive == false)
		{
			enabled = false;
			an.SetBool("Dead", true);
			an.SetBool("Walking", false);
			an.SetBool("Charging", false);
		}else
		{

		dist = Vector3.Distance(this.transform.position, Player.transform.position);
		if(life.Health > 0 && !Cooling)
		{
			if(hasFire() && dist < Detect_Distance/3f)
			{
				RunFrom(Player.transform);
			}else
			{	
				if(dist < (Detect_Distance/4f))
				{
					r.isKinematic = false;
					this.transform.LookAt(Player.transform.position);
					an.SetBool("Charging", true);
					an.SetBool("Walking", true);
					r.MovePosition(r.position + (transform.forward * charge_speed) * Time.fixedDeltaTime);
				}else if(dist < Detect_Distance)
				{
					r.isKinematic = false;
					this.transform.LookAt(Player.transform.position);
					an.SetBool("Charging", false);
					an.SetBool("Walking", true);
					r.MovePosition(r.position + (transform.forward * speed) * Time.fixedDeltaTime);
				}else
				{
					r.isKinematic = true;
					an.SetBool("Walking", false);
					an.SetBool("Charging", false);
				}
			}
		}else
		{
			this.transform.LookAt(Player.transform.position);
			an.SetBool("Walking", false);
			an.SetBool("Charging", false);
		}

		}
	}

	void Damage()
	{
		if(Cooling)
			count++;

		if (dist < 1f && stats.isAlive && !Cooling) {
			life.Health = life.Health - 4;
			Cooling = true;
		}

		if(count > CoolDown)
		{
			count = 0;
			Cooling = false;
		}

	}

	void RunFrom(Transform Danger)
	{
		an.SetBool("Charging", false);
		an.SetBool("Walking", true);
		Vector3 Safety = this.gameObject.transform.position - Danger.position;
		//Safety = new Vector3(Safety.x, transform.localPosition.y, Safety.z);
		Safety = new Vector3(Safety.x, 0f, Safety.z);
		//this.transform.LookAt(Safety);
		transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(Safety, Vector3.up), Time.fixedDeltaTime / 0.25f);
		r.MovePosition(r.position + (transform.forward * charge_speed) * Time.fixedDeltaTime);
	}

	bool hasFire()
	{
		if(inv.equip_space.transform.childCount > 0)
			if(inv.equip_space.transform.GetChild(0).GetComponent<InvInfo>().Id == 10)//torch id
			{
				return true;
			}

		return false;
	}
}
