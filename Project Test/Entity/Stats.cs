using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stats : MonoBehaviour 
{
	Animator an;

	public int Health;
	public string Name;
	public bool isAlive = true;

	int Air = 40;
	
	

	void Start () 
	{
		InvokeRepeating ("Drown", 0.1f, 0.1f);
	}
	
	void Update () 
	{
		if(Health <= 0)
		{
			if(isAlive)
				StartCoroutine(Decay());
			isAlive = false;
			//Destroy(this.gameObject);
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

	void Drown()
	{
		if (this.gameObject.transform.position.y <= 8.5f) {
			Change(ref Air, false);
		} else
			Change(ref Air, true);

		if(Air == 0)
			Change(ref Health,false);
	}

	IEnumerator Decay()
	{
		yield return new WaitForSeconds(15f);
		Destroy(this.gameObject);

	}
}
