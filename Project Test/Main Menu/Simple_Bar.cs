using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Simple_Bar : MonoBehaviour {


	GameObject Background_OBJ;
	GameObject Bar_OBJ;

	RectTransform Background;
	RectTransform Bar;

	public float percent = 100f;
	public bool Enabled = true;  //IMPLEMENT FOR AIR
	public bool HideOnEmpty = false;

	float default_width;
	float bar_width;
	float bar_xpos = 0;

	void Start () 
	{
		Background_OBJ = gameObject.transform.GetChild (0).gameObject;
		Bar_OBJ = gameObject.transform.GetChild (1).gameObject;

		Background = Background_OBJ.GetComponent<RectTransform>();
		Bar = Bar_OBJ.GetComponent<RectTransform>();

		default_width = Background.sizeDelta.x;
		bar_width = default_width;
	}
	
	void Update () 
	{

		bar_width = Mathf.Lerp(0, default_width, percent/100f);
		bar_xpos = (default_width - bar_width) / -2f;

		Bar.sizeDelta = new Vector2(bar_width,Bar.sizeDelta.y);
		Bar.anchoredPosition = new Vector2(bar_xpos,Bar.anchoredPosition.y);

		SwapHidden ();
	}

	void SwapHidden()
	{
		if(HideOnEmpty && percent == 0f)
			Background_OBJ.SetActive (false);
		else
			Background_OBJ.SetActive (Enabled);
		

		Bar_OBJ.SetActive (Enabled);
	
	}
}
