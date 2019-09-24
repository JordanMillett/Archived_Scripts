using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuLoader : MonoBehaviour {

	public float Amplitude;
	public float Frequency;
	public int Trees;
	public int Rocks;
	public int Flints;
	public int Logs;
	public int Bushes;
	public int MapSize;
	public int YMult;

	public int ViewDistance;
	public int FOV;

	void Start () 
	{
		DontDestroyOnLoad(this.gameObject);
	}
}
