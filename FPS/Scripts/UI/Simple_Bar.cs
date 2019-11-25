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

	void Start () 
	{
		Background_OBJ = gameObject.transform.GetChild (0).gameObject;
		Bar_OBJ = gameObject.transform.GetChild (1).gameObject;

		Background = Background_OBJ.GetComponent<RectTransform>();
		Bar = Bar_OBJ.GetComponent<RectTransform>();

		default_width = Background.sizeDelta.x;
		bar_width = default_width;

        //HideFull();
	}
	
	void Update () 
	{

		bar_width = Mathf.Lerp(0, default_width, Current/Max);
		bar_xpos = (default_width - bar_width) / -2f;

		Bar.sizeDelta = new Vector2(bar_width,Bar.sizeDelta.y);
		Bar.anchoredPosition = new Vector2(bar_xpos,Bar.anchoredPosition.y);

        //HideFull();
	}

	void HideFull()
	{
		if(Current == 100f)
        {
			Bar_OBJ.SetActive (false);
			Background_OBJ.SetActive (false);
        }
		else
        {
            Bar_OBJ.SetActive (true);
			Background_OBJ.SetActive (true);
        }
	
	}
}