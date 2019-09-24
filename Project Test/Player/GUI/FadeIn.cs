using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FadeIn : MonoBehaviour {

	public RawImage one;
	public RawImage two;

	Color colorOne;
	Color colorTwo;

	float alpha1 = 0f;
	float alpha2 = 0f;

	void Start () 
	{
		colorOne.a = alpha2;
		colorTwo.a = alpha1;

		InvokeRepeating ("Fade", 0.01f, 0.01f);
	}
	
	void Fade()
	{
		alpha1 = alpha1 + 0.008f;

		if(alpha2 < 0.92411)
			alpha2 = alpha2 + 0.008f;

		Color colorOne = one.color;
		Color colorTwo = two.color;

		colorOne.a = alpha2;
		colorTwo.a = alpha1;

		one.color = colorOne;
		two.color = colorTwo;

	}

}
