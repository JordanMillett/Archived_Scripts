using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Below_Water : MonoBehaviour {

	public GameObject Water;
	Camera cam;
	float default_fov = 60f;

	void Start () 
	{
		Water.SetActive (false);
		cam = gameObject.GetComponent<Camera>();
	}
	

	void Update () 
	{
		if (UnderWater()) {
			Water.SetActive (true);
		} else {
			Water.SetActive (false);
		}
	}

	public bool UnderWater()
	{
		if (this.gameObject.transform.position.y <= 8.5f){
			return true;
		}else
			return false;
	}
}
