using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Placing : MonoBehaviour 
{
	Material[] originalMats;
	Renderer[] rend;

	public bool canPlace = true;
	public bool isGrounded = true;
	int currentCol = 0;
    public bool TraceNormals = false;

	public Material mat;

	

	//get all renderers in children and backup their materials

	void Start () 
	{
		rend = this.gameObject.transform.GetChild(0).GetComponentsInChildren<Renderer>();
		originalMats = new Material[rend.Length];

		int i = 0;
		foreach (Renderer r in rend)
		{
			r.enabled = false;
			originalMats[i] = r.material;
			r.material = mat;
			i++;
		}
	}
	
	void Update () 
	{

		if(currentCol == 0)
			canPlace = true;
		else
			canPlace = false;
		
		


		RaycastHit hit;
		if(Physics.Raycast(this.transform.position,new Vector3(0,-90,0),out hit,5f))
		{
			foreach (Renderer r in rend)
			{
				r.enabled = true;
			}
			//this.gameObject.SetActive(true);
			this.transform.position = new Vector3(this.transform.position.x,hit.point.y + .2f,this.transform.position.z);
            
               

			isGrounded = true;
		}else
		{
			foreach (Renderer r in rend)
			{
				r.enabled = false;
			}
			//this.gameObject.SetActive(false);
			this.transform.localPosition = new Vector3(0f,0f,0f);
			isGrounded = false;
		}

		if(canPlace && isGrounded)
		{
			foreach (Renderer r in rend)
			{
				r.material.SetFloat("_Vector_Valid", 0f);
			}
		}else
		{
			foreach (Renderer r in rend)
			{
				r.material.SetFloat("_Vector_Valid", 1f);
			}
		}
	}

	void OnDisable()
	{

		int i = 0;
		foreach (Renderer r in rend)
		{
			r.material = originalMats[i];
			i++;
		}

	}

	void OnTriggerEnter ()
	{
		currentCol++;

	}

	void OnTriggerExit()
	{
		currentCol--;
	}
}
