using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Map_Control : MonoBehaviour 
{

	GameObject Player;
	Camera mapcam;

	void Start()
	{

		Player = GameObject.FindWithTag("Player");

		mapcam = Player.transform.GetChild(5).GetComponent<Camera>();

	}

	public void Zoom(bool zoomOut)
	{

		if(zoomOut)
		{
			if(mapcam.orthographicSize > 210f)
				mapcam.orthographicSize -= 200f;

		} else
		{
			if(mapcam.orthographicSize < 1000f)
				mapcam.orthographicSize += 200f;
		}

	}
}
