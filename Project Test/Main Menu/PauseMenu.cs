using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenu : MonoBehaviour 
{
	public GameObject MainMenu;
	public GameObject SettingsMenu;

	void OnDisable()
	{
		MainMenu.SetActive(true);
		SettingsMenu.SetActive(false);
	}
}
