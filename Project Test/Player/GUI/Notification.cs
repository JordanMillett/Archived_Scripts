using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Notification : MonoBehaviour 
{
	RectTransform blip_t;
	TextMeshProUGUI blip_content;
	
	public float y_loc = 0f;
	public int Amount;
	public int Id;
	public string Name;
	public bool drop;

	void Start () 
	{
		blip_t = GetComponent<RectTransform>();
		blip_content = transform.GetChild (1).gameObject.GetComponent<TextMeshProUGUI>();
		InvokeRepeating("Fade", 4.5f, 4.5f);
		UpdateText();
	}

	void Update () 
	{
		UpdateText();

		y_loc++;
		blip_t.anchoredPosition = new Vector3(0,y_loc,0);
	}

	void Fade()
	{
		Destroy(this.gameObject);
	}

	void UpdateText()
	{
		if(!drop)
			blip_content.text = ("+" + Amount + " " + Name).ToString();
		else
			blip_content.text = ("-" + Amount + " " + Name).ToString();
	}
}
