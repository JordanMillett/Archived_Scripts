using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenerateBerries : MonoBehaviour {

	public GameObject Berry;
	Vector3 offset;
	int Amount;
	public int min_amount = 3;
	public int max_amount = 9;

	public float dist = 1.5f;


	void Start() 
	{	
		Amount = Random.Range(min_amount,max_amount + 1);

		GameObject empt;
		empt = new GameObject("_Berries");
		empt.transform.SetParent(gameObject.transform);

		for(int i = 0; i < Amount; i++)
		{
			float x_off = Random.Range(-dist,dist);
			float y_off = Random.Range(0,dist);
			float z_off = Random.Range(-dist,dist);

			offset = new Vector3(x_off,y_off,z_off);


			Vector3 Vec = offset.normalized * dist;

			GameObject b = Instantiate(Berry, gameObject.transform.position + Vec, Quaternion.identity);


			b.transform.SetParent(empt.transform);
			b.name = "Berry";
		}
		
		
	}

}
