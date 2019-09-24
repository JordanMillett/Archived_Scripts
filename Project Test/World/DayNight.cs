using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering;

public class DayNight : MonoBehaviour 
{

	public float TimeScale;
	public Color Sun_Light_Color;
	public Color Moon_Light_Color;
	public Material Cloud_Mat;

	public bool IsDay;

	GameObject Sun;
	GameObject Moon;
	Light Sun_Light;
	Light Moon_Light;

	float CurrentAngle;
	float time;

	void Start () 
	{
		
		Sun = this.gameObject.transform.GetChild(0).gameObject;
		Moon = this.gameObject.transform.GetChild(1).gameObject;

		Sun_Light = Sun.GetComponent<Light>();
		Moon_Light = Moon.GetComponent<Light>();

	}
	

	void FixedUpdate () 
	{
		if(time > 1)
			time = 0;



		time += TimeScale * Time.deltaTime;  //lerp between everything



		//if(time > .25f && time < .75){
		if(time > .30f && time < .70){
			Sun_Light.color = Color.black;
			Moon_Light.color = Moon_Light_Color;
			Cloud_Mat.SetColor("_EmissionColor", Color.black);
			IsDay = false;
		}else
		{
			Sun_Light.color = Sun_Light_Color;
			Moon_Light.color = Color.black;
			Cloud_Mat.SetColor("_EmissionColor", Color.white);
			IsDay = true;
		}

		
	

		//Sun_Light.color = Color.Lerp(Sun_Light_Color,Color.black,time);
		//Moon_Light.color = Color.Lerp(Color.black,Moon_Light_Color,time);
		//Cloud_Mat.SetColor("_EmissionColor", Color.Lerp(Color.white,Color.black,time));

		
		this.transform.rotation = Quaternion.Euler(time * 360,0,0);


	}
}
