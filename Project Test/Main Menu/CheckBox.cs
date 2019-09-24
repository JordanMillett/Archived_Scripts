using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CheckBox : MonoBehaviour 
{

	Button button;

	public bool toggled;
	GameObject Check;

	void Start () 
	{
		button = this.gameObject.GetComponent<Button>();
		Check =  this.gameObject.transform.GetChild(1).gameObject;
		Check.SetActive(toggled);
	}

	public void Toggle()
	{
		toggled = !toggled;
		Check.SetActive(toggled);

	}

}
