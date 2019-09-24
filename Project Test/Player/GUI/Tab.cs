using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using System;

public class Tab : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler 
{
	public List<Tab> otherTabs = new List<Tab>();
	public List<GameObject> Children = new List<GameObject>();

	bool over = false;
	public bool active = false;

	void Start()
	{
		int children = transform.childCount;
		for (int i = 1; i < children; i++)
			Children.Add(this.gameObject.transform.GetChild(i).gameObject);

		if(active == false)
			Toggle(false);
	}

	void Update()
	{
		Switch();
	}

	public void Switch()
    {
    	 if(Input.GetMouseButtonDown(0))
		 {
    		if(over)
			{
				active = true;
				foreach (GameObject child in Children)
					child.SetActive(active);

				foreach (Tab t in otherTabs)
					t.Toggle(false);
            			 
    		}
		 }

    }

	public void Toggle(bool setTo)
	{

		foreach (GameObject child in Children)
			child.SetActive(setTo);

		active = setTo;

	}

	public void OnPointerEnter(PointerEventData eventData)
    {
		over = true;
    }

	public void OnPointerExit(PointerEventData eventData)
	{
		over = false;
	}

	public void OnEnable()
	{
		over = false;
	}
}
