using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadingScreen : MonoBehaviour 
{

	public GameObject LoadingObject;

	public Simple_Bar Loading_Bar;

	public GameObject Player;

	public int OutOf = 100;
	public int Current = 0;

//	int i = 0;

	void Start () 
	{
		Loading_Bar.percent = 0;
	}

	void Update () 
	{
		Loading_Bar.percent = Current * (100f / OutOf);

		if (Current >= OutOf)
			Close ();
	}

	void Close()
	{
		Player.SetActive(true);
		Destroy (LoadingObject);
	}
}
