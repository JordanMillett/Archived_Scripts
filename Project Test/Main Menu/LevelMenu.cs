using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelMenu : MonoBehaviour 
{
	public SettingsMenu Settings;

	public SliderNumber[] Values;
	public MenuLoader Loader;

	public void PlayGame()
	{
		SlidersToLoader ();
		Settings.SlidersToLoader();


		SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
	}

	void SlidersToLoader()
	{
		Loader.Amplitude = Values[0].Value_Float;
		Loader.Frequency = Values[1].Value_Float;
		Loader.Trees = Values[2].Value;
		Loader.Rocks = Values[3].Value;
		Loader.Flints = Values[4].Value;
		Loader.Logs = Values[5].Value;
		Loader.Bushes = Values[6].Value;
		Loader.MapSize = Values[7].Value;
		Loader.YMult = Values[8].Value;
	}
}
