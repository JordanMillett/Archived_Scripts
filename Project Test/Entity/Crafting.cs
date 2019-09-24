using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using TMPro;
using UnityEngine.EventSystems;

public class Crafting : MonoBehaviour 
{
	public GameObject blank_craft;
	public GameObject blank_ingredient;
	public GameObject blank_tab;

	public Recipe[] Craftables;

	GameObject Empty;
	RectTransform ER;

	bool open = false;
	

	void Start () 
	{
		CreateMenu();
	}
	
	void CreateMenu()
	{

		List<string> craftingtabNames = new List<string>();  										//Find how many different tabs we need
		for(int i = 0; i < Craftables.Length;i++)
		{
			int matches = 0;

			if (craftingtabNames.Count == 0) {
				craftingtabNames.Add (Craftables [i].Tab);
			} else {
				int len = craftingtabNames.Count;
				for (int j = 0; j < len; j++)
					if (Craftables [i].Tab != craftingtabNames [j])
						matches++;
						
				if (matches == craftingtabNames.Count)
					craftingtabNames.Add (Craftables [i].Tab);
			}
		}

		Empty = new GameObject("Menu");
		Empty.transform.SetParent(this.transform);
        Empty.transform.eulerAngles = Vector3.zero;
        Empty.transform.localScale = Vector3.one;
		ER = Empty.AddComponent(typeof(RectTransform)) as RectTransform;
		ER.anchoredPosition = Vector3.zero;
		ER.transform.localScale = Vector3.one;

		List<GameObject> allcraftingTabs = new List<GameObject>();									//Create tabs for craftables
		int main_tab_x_loc = 100;
		for(int x = 0; x < craftingtabNames.Count;x++,main_tab_x_loc = main_tab_x_loc + 110)
		{

			GameObject tab = Instantiate(blank_tab, new Vector3(0, 0, 0), Quaternion.identity);
			tab.transform.SetParent(Empty.transform);

			RectTransform t_tab = tab.GetComponent<RectTransform>();
			RectTransform t_parent = Empty.GetComponent<RectTransform>();
			//t_tab.anchoredPosition = new Vector3(main_tab_x_loc,270,0);
			t_tab.anchoredPosition =  new Vector3(main_tab_x_loc,270,0) - (Vector3)t_parent.anchoredPosition;
			t_tab.transform.localScale = Vector3.one;

			TextMeshProUGUI name_tab = tab.GetComponentInChildren<TextMeshProUGUI> ();
			name_tab.text = craftingtabNames[x];
			tab.name = craftingtabNames[x] + "_" + (x + 1);

			allcraftingTabs.Add(tab);

		}

		for(int a = 0; a < craftingtabNames.Count;a++)											//Tell crafting tabs what the other tabs are
			for(int b = 0; b < craftingtabNames.Count;b++)
				if(allcraftingTabs[b] != allcraftingTabs[a])
					allcraftingTabs[b].GetComponent<Tab>().otherTabs.Add(allcraftingTabs[a].GetComponent<Tab>());


		allcraftingTabs[0].GetComponent<Tab>().active = true;									//init y positions and enable starting tab
		int[] y_loc = new int[craftingtabNames.Count];
		for(int l = 0; l < y_loc.Length;l++) 											//Manage y Position for craftables on each tab
			y_loc[l] = 155;

		for(int t = 0; t < craftingtabNames.Count;t++)
			for(int x = 0; x < Craftables.Length;x++)									//make all craftables for each tab
			{
				if(Craftables[x].Tab == craftingtabNames[t])
				{
					GameObject craft = Instantiate(blank_craft, new Vector3(0, 0, 0), Quaternion.identity);
					craft.transform.SetParent(allcraftingTabs[t].transform);

					RectTransform t_craft = craft.GetComponent<RectTransform>();
					RectTransform t_parent = allcraftingTabs[t].GetComponent<RectTransform>();
					RectTransform t_parentparent = Empty.GetComponent<RectTransform>();
					t_craft.anchoredPosition = new Vector3(200,y_loc[t],0) - (Vector3)t_parent.anchoredPosition - (Vector3)t_parentparent.anchoredPosition;
					t_craft.transform.localScale = Vector3.one;

					GameObject img_craft_obj = craft.transform.GetChild (0).gameObject;
					RawImage img_craft = img_craft_obj.GetComponent<RawImage> ();
					img_craft.texture = Craftables [x].GetComponentInChildren<InvInfo> ().Icon;

					TextMeshProUGUI name_craft = craft.GetComponentInChildren<TextMeshProUGUI> ();  //Set name in gui
					name_craft.text = Craftables [x].name;

					int x_loc = 0; 

					for(int i = 0; i < Craftables [x].Ingredients.Length;i++,x_loc = x_loc + 100)  //Assign the amounts to the inv info 
					{
						GameObject ing = Instantiate(blank_ingredient, new Vector3(0, 0, 0), Quaternion.identity);
						ing.transform.SetParent(craft.transform);

						RectTransform t_ing = ing.GetComponent<RectTransform>();
						t_ing.anchoredPosition = new Vector3(x_loc,0,0); 					 //shift it right as it goes later, for multiple ingridients
						t_ing.transform.localScale = Vector3.one;

						TextMeshProUGUI ing_amount = ing.GetComponentInChildren<TextMeshProUGUI> ();
						ing_amount.text = Craftables[x].Amounts[i].ToString() + "x";

						GameObject img_ing_obj = ing.transform.GetChild (1).gameObject;
						RawImage img_ing = img_ing_obj.GetComponent<RawImage>();
						img_ing.texture = Craftables [x].Ingredients [i].Icon;
					}

					craft.name = "craftable_" + (x + 1);
					y_loc[t] = y_loc[t] - 110;
				}
			}

			Empty.SetActive(false);

	}

	public void Toggle(GameObject parent)
	{

		open = !open;

		if(open == true)
			Empty.transform.SetParent(parent.transform);

		if(open == false)
			Empty.transform.SetParent(this.transform);

        Empty.transform.eulerAngles = Vector3.zero;
		ER.anchoredPosition = Vector3.zero;
        Empty.transform.localScale = Vector3.one;
		Empty.SetActive(open);

	}
}
