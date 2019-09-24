using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationHandler : MonoBehaviour {

	public Inventory inv;
	Animator an;

	public void Start() 
	{
		an = GetComponent<Animator>();
	}

	public void HitStart()
	{
		an.SetInteger("Hit",0);

	}

	public void EatStart()
	{
		//an.SetInteger("Eat",0);
        //an.SetInteger("Eat",0);
	}

	public void EatEnd()
	{
        //inv.eatstate = 0;
		//Destroy(inv.equip_space.transform.GetChild (0).gameObject);
	}

	public void PutAway()
	{
		
	}

}
