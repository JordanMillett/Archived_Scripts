using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class DropBTN : MonoBehaviour 
{
	public RectTransform Background;
	public TextMeshProUGUI Current_Text;
	public TextMeshProUGUI[] Options;

//	bool isOpen = false;

	void Start () 
	{
		
	}

	public void OnPointerEnter(PointerEventData eventData)
    {
		if(Input.GetMouseButtonDown(0))
		{
			Open();
		}
    }

    
	void Open()
	{



	}

}
