using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SliderNumber : MonoBehaviour 
{

	Slider slide;

	public int Value;
	public float Value_Float;
	public bool useFloat = false;
	public bool customIntervals = false;

	public string[] Texts;

	TextMeshProUGUI Number;

	void Start () 
	{
		slide = this.gameObject.GetComponent<Slider>();
		Number =  this.gameObject.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
	}

	void Update () 
	{
		if (useFloat)
			Number.text = slide.value.ToString("F2");
		else if(customIntervals)
		{

			Number.text = Texts[Value];

		}
		else
			Number.text = Mathf.Round(slide.value).ToString();


		Value = (int)Mathf.Round(slide.value);
		Value_Float = slide.value;
	}
}
