using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using System;

public class SettingsMenu : MonoBehaviour 
{

	public SliderNumber[] Values;
	public CheckBox[] Bools;
	public MenuLoader Loader;
	
	GameObject Player;
	bool playerfound = false;

	public void SlidersToLoader()
	{

	
		Loader.ViewDistance = Values[0].Value;
		Loader.FOV = Values[2].Value;

		
	}

	public void SliderUpdate()
	{

		if(!playerfound)
		{
			Player = GameObject.FindWithTag ("Player");
			playerfound = true;
		}

		Player.GetComponent<PlayerController>().fov = Values[2].Value;

	}

	public void ApplySettings()
	{

		string string_res = Values[1].gameObject.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text.ToString();

		string[] dims = string_res.Split('x');

		Screen.SetResolution(int.Parse(dims[0]), int.Parse(dims[1]), Bools[0].toggled);

		

	}
}
