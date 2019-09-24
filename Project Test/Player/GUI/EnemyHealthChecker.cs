using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using TMPro;
using UnityEngine.EventSystems;

public class EnemyHealthChecker : MonoBehaviour 
{
	Stats stat;
	Stats newStat;
	Simple_Bar Health;
	TextMeshProUGUI Name;

	public GameObject cam;

	int disableFrames;

	void Start () 
	{
		Name = transform.GetChild (0).gameObject.GetComponent<TextMeshProUGUI>();
		Health = transform.GetChild (1).gameObject.GetComponent<Simple_Bar>();
		Health.transform.gameObject.SetActive(false);
		Name.transform.gameObject.SetActive(false);
	}
	
	void Update () 
	{
		RaycastHit hit;
		if(Physics.Raycast(cam.transform.position,cam.transform.forward, out hit,100))
		{
			try {
				newStat = hit.transform.gameObject.GetComponent<Stats>();
	
				if(newStat != null)
				{
					stat = newStat;
					disableFrames = 0;
				}

			}catch(NullReferenceException){}
		}
		
		if(stat != null)
		{
			Health.percent = stat.Health;
			Name.text = stat.Name;
			Health.transform.gameObject.SetActive(true);
			Name.transform.gameObject.SetActive(true);
			
		}

		if(disableFrames > 120)
		{
       		Disable();
		}else
		{
			disableFrames++;
			if(disableFrames > 1000)
				disableFrames = 300;
		}
	}

	void Disable()
	{
		Health.transform.gameObject.SetActive(false);
		Name.transform.gameObject.SetActive(false);
	}
}
