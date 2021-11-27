using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIBar : MonoBehaviour
{
    public RectTransform Bar;

    public float Max = 100f;
	public float Current = 100f;

	public float default_width = 0;
	float bar_width;
	float bar_xpos = 0;

	void Start() 
	{
		bar_width = default_width;
	}
	
	void Update() 
	{
		bar_width = Mathf.Lerp(0, default_width, Current/Max);
		bar_xpos = (default_width - bar_width) / -2f;

		Bar.sizeDelta = new Vector2(bar_width,Bar.sizeDelta.y);
		Bar.anchoredPosition = new Vector2(bar_xpos,Bar.anchoredPosition.y);
	}
}
