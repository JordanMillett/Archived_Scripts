using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using TMPro;
using UnityEngine.EventSystems;

public class Notification_Manager : MonoBehaviour 
{

	public GameObject itemTemplate;
	public Color dropColor;
	public Color pickupColor;

	List <Notification> notes = new List <Notification>();

	public void ItemTransaction(InvInfo Item, bool drop)
	{

		if(MatchFound(Item.Id,drop) != -1)
		{
			notes[MatchFound(Item.Id,drop)].Amount += Item.Amount; 
		}else
		{

			//ShiftNotifications();

			GameObject blip = Instantiate(itemTemplate, new Vector3(0, 0, 0), Quaternion.identity);
			RectTransform blip_t = blip.GetComponent<RectTransform>();
			RawImage blip_bg = blip.GetComponent<RawImage> ();
			RawImage blip_img = blip.transform.GetChild (0).gameObject.GetComponent<RawImage> ();
			blip.transform.SetParent(this.gameObject.transform);
			blip_t.anchoredPosition = new Vector3(0,0,0);
			blip_t.transform.localScale = Vector3.one;
			blip_t.eulerAngles = new Vector3(0,0,0);
			blip_img.texture = Item.Icon;
			blip.name = Item.Name;

			Notification note = blip.GetComponent<Notification>();

			note.drop = drop;
			note.Name = Item.Name;
			note.Id = Item.Id;
			note.Amount = Item.Amount;

			notes.Add(note);
			StartCoroutine(IRemove(note));
			
			if(!drop)
				blip_bg.color = pickupColor;
			else
				blip_bg.color = dropColor;

		}

	}

	IEnumerator IRemove(Notification toRemove)
	{
		yield return new WaitForSeconds(4f);
		notes.Remove(toRemove);
	}

	void ShiftNotifications()
	{

		for(int i = 0; i < notes.Count;i++)
		{

			if(notes[i].y_loc < 50)
			{
				notes[i].y_loc += 75;
			}
		}

	}

	int MatchFound(int Id, bool drop)
	{

		for(int i = 0; i < notes.Count;i++)
		{

			if(notes[i].Id == Id && notes[i].drop == drop)
				return i;

		}

		return -1;
	}
}
