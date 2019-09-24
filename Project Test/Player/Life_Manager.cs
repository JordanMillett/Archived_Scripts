using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Life_Manager : MonoBehaviour {

	PlayerController playcon;

	public Below_Water Wat;

	public int Health;
	public int Hunger;
	public int Air;

	public Simple_Bar HealthBar;
	public Simple_Bar HungerBar;
	public Simple_Bar AirBar;

	public GameObject Death_Screen;

	void Start () 
	{
		init();

		InvokeRepeating("Starve", 5f, 5f);
		InvokeRepeating ("Drown", 0.1f, 0.1f);
		InvokeRepeating ("Heal", 6f, 6f);
	}
	
	void Update () 
	{
		HealthBar.percent = Health;
		HungerBar.percent = Hunger;
		AirBar.percent = Air;

		if(Input.GetKeyDown("k"))
			Health = 0;

		if(Health <= 0){
			playcon.isAlive = false;
			Death_Screen.SetActive(true);
		}

	}

	void init()
	{

		playcon = this.gameObject.GetComponent<PlayerController>();

	}

	void Starve()
	{
		
		Change(ref Hunger,false);

		
		if (Hunger == 0)
			Change(ref Health,false);

		if (Hunger > 100)
			Hunger = 100;

	}

	void Drown()
	{
		if (Wat.UnderWater ()) {
			Change(ref Air, false);
		} else
			Change(ref Air, true);
		
		if (Air < 100)
			AirBar.Enabled = true;
	
		if (Air == 100)
			AirBar.Enabled = false;
			

		if(Air == 0)
			Change(ref Health,false);
	}

	void Heal()
	{
		if (Hunger >= 95) {
			Change (ref Health, true);
		}
	}

	void Change(ref int Value, bool Add)
	{
		if (Add) 
		{
			if (Value < 100)
				Value++;

		}

		if (!Add) 
		{
			if (Value > 0)
				Value--;
		}
				
		
	}
}
