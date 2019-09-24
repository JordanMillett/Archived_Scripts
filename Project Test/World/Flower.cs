using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flower : MonoBehaviour 
{
	Renderer r;

	void Start () 
	{
		r = this.gameObject.transform.GetChild(0).GetChild(0).gameObject.GetComponent<Renderer>();
		r.material.SetColor("_BaseColor", Random.ColorHSV(0f, 1f, 1f, 1f, 1f, 1f));
		transform.localScale += new Vector3(Random.Range(0f,0.6f), Random.Range(0f,1f), Random.Range(0f,0.6f));
	}
}
