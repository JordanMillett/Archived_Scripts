using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using System;

public class ItemInfo : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {

	RawImage Icon;
	TextMeshProUGUI Amount;
	InvInfo Data;
	public GameObject Popup;
	GameObject p;
	GameObject Player;

	bool over = false;

	Inventory inv;

	string nam;


	void Start()
	{
		RawImage Icon = gameObject.GetComponent<RawImage>();
		TextMeshProUGUI Amount = gameObject.GetComponentInChildren<TextMeshProUGUI>();
//		InvInfo Data = gameObject.GetComponent<InvInfo>();

		GameObject Player = GameObject.FindWithTag("Player");
		inv = Player.gameObject.GetComponent<Inventory>();

		nam = gameObject.name;
		nam = nam.Replace("slot_", "");
	}

	void Update()
	{
		//HotBarAssign();
		Drop();
		Equip ();

	//	Refresh();

	}

	void Refresh()
	{
/* 
		if(Data.Icon != null)
		{
			Icon.texture = Data.Icon;
			Icon.color = Color.white;
			Amount.text = Data.Amount.ToString();
		}
*/
	}


    public void OnPointerEnter(PointerEventData eventData)
    {
		RawImage Icon = gameObject.GetComponent<RawImage>();
		if(Icon.texture != null){

			Destroy(p);

			p = Instantiate(Popup, this.transform.position, Quaternion.identity);

			p.transform.SetParent(gameObject.transform.parent);

			TextMeshProUGUI p_text = p.GetComponent<TextMeshProUGUI>();
			RectTransform p_t = p.GetComponent<RectTransform>();

			p_t.transform.localScale = Vector3.one;

			p_text.text = Icon.texture.name;

			over = true;

		}
     }

	public void Equip()
	{
		if (over && Input.GetMouseButtonDown (0)) 
		{
			inv.Equip(int.Parse(nam));
			//StartCoroutine(inv.IEquip(int.Parse(nam)));
		}
	}

    public void HotBarAssign()
    {
    	if(Input.GetKeyDown("1"))
    		if(over)
                inv.MoveToHotbar(0,int.Parse(nam),true); 
    		else
                inv.MoveToHotbar(0,int.Parse(nam),false); 
            
        


        /* 
		if(Input.GetKeyDown("2")){
    		if(over){inv.MoveToHotbar(1,int.Parse(nam)); 
    		}else if(over == false){InvInfo b = new InvInfo();
				inv.HotbarContents[1] = b;}}

		if(Input.GetKeyDown("3")){
    		if(over){inv.MoveToHotbar(2,int.Parse(nam)); 
    		}else if(over == false){InvInfo b = new InvInfo();
				inv.HotbarContents[2] = b;}}

		if(Input.GetKeyDown("4")){
    		if(over){inv.MoveToHotbar(3,int.Parse(nam)); 
    		}else if(over == false){InvInfo b = new InvInfo();
				inv.HotbarContents[3] = b;}}

		if(Input.GetKeyDown("5")){
    		if(over){inv.MoveToHotbar(4,int.Parse(nam)); 
    		}else if(over == false){InvInfo b = new InvInfo();
				inv.HotbarContents[4] = b;}}
            */

    }

    void Drop()
    {
		if(Input.GetKeyDown(KeyCode.Q)){
    		if(over){

				Destroy(p);
				over = false;
				inv.Drop(int.Parse(nam),false);
				


    		}else if(over == false){}
    	}
    }

	public void OnPointerExit(PointerEventData eventData)
	{
		Destroy(p);
		over = false;
	}

	public void OnEnable()
	{
		Destroy(p);
		over = false;
	}

	public void OnDestroy()
	{

		Destroy(p);
		over = false;

	}

}

