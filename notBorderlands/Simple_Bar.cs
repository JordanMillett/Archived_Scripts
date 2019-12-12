using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Simple_Bar : MonoBehaviour 
{

	GameObject Background_OBJ;
	GameObject Bar_OBJ;

	RectTransform Background;
	RectTransform Bar;

    public float Max = 100f;
	public float Current = 100f;

	float default_width;
	float bar_width;
	float bar_xpos = 0;

	float Speed = 0.01f;
	float CurrentLerped = 0f;
	float LerpAlpha = 0f;

	void Start () 
	{
		CurrentLerped = Current;

		Background_OBJ = gameObject.transform.GetChild (0).gameObject;
		Bar_OBJ = gameObject.transform.GetChild (1).gameObject;

		Background = Background_OBJ.GetComponent<RectTransform>();
		Bar = Bar_OBJ.GetComponent<RectTransform>();

		default_width = Background.sizeDelta.x;
		bar_width = default_width;
	}
	
	void Update () 
	{

		//LERP
		CurrentLerped = Mathf.Lerp(CurrentLerped, Current, LerpAlpha);
		if(CurrentLerped != Current)
			LerpAlpha += Speed;
		else
			LerpAlpha = 0f;
			


		bar_width = Mathf.Lerp(0, default_width, CurrentLerped/Max);
		bar_xpos = (default_width - bar_width) / -2f;

		Bar.sizeDelta = new Vector2(bar_width,Bar.sizeDelta.y);
		Bar.anchoredPosition = new Vector2(bar_xpos,Bar.anchoredPosition.y);
	}
}