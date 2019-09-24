using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using TMPro;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class Inventory : MonoBehaviour 
{
	PlayerController mov;
	RawImage blan;
	TextMeshProUGUI na;
	Animator an;
	Life_Manager LifeManager;
	Placing p;
	RectTransform select_tran;
	GameObject held;

	GameObject placed;

	public GameObject body;
	public GameObject inv;

	public Notification_Manager note;

	public GameObject place_space;

	public GameObject equip_space;
	public GameObject storage;
	public GameObject global_dropped_items;
	public GameObject closet;

	public GameObject game_hud;
	public GameObject hotbar;
	public GameObject blank;
	public GameObject hotbar_blank;
	public GameObject backIMG;
	public GameObject blank_craft;
	public GameObject blank_ingredient;
	public GameObject blank_tab;
	public Recipe[] Craftables = new Recipe[5];
	public GameObject PauseMenu;
	public Simple_Bar PickupBar;

	public GameObject EnemyHealth;
	public GameObject Notifications;
	public GameObject Hotbar_Obj;
	public GameObject DeathScreen;
	public GameObject Map_Prefab;
	public GameObject Player_Model_Prefab;

	//remove publics and make getchilds

	InvInfo[] Contents = new InvInfo[35];
	RawImage[] InvGrid = new RawImage[35];
	TextMeshProUGUI[] Amounts = new TextMeshProUGUI[35];
	GameObject[] slots = new GameObject[35];

	//[HideInInspector]
	//public Hotbar_Item[] Hotbar_Contents = new Hotbar_Item[5];

	public bool open = false;
	public bool paused = false;

	bool donepickingup = false;
	GameObject lastobj;

	bool shifted = false;
	int current = 0;
	int slot = 1;
	Vector3 loc;
	public GameObject Selector;

	bool Equipped = false;
	bool ghostActive = false;
	int Equiped_num = 1;

	bool Busy = false;
    bool Eating = false;
	bool SideTabOpen = false;
    bool Equipped_Moved = false; //find if the currently equipped item has been pushed to the left when items were removed

	GameObject SlotsEmpty;
	GameObject CraftingTabEmpty;
	GameObject MapEmpty;
	GameObject PlayerEmpty;

    Crafting Opened;


	void Start ()
	{
		mov = gameObject.GetComponent<PlayerController>();
		blan = blank.GetComponent<RawImage>();
		na = blank.GetComponentInChildren<TextMeshProUGUI>();
		an = body.GetComponent<Animator> ();
		LifeManager = gameObject.GetComponent<Life_Manager>();
		select_tran = Selector.GetComponent<RectTransform>();

		CreateInventory();
		CreateHotbar();

		Cursor.visible = false;

	}

	void Update ()
	{
		if(mov.isAlive == false && open)
			OpenInventory();

		if(Input.GetKeyDown(KeyCode.Tab) && !paused && mov.isAlive != false)
			OpenInventory();

		if(Input.GetKeyDown(KeyCode.Escape))
			Pause();
		
		if(Input.GetMouseButtonDown(0) && !open)
		{
			if(Contents[Equiped_num - 1].Mine_Effective > 0)
				Mine();

			if(Contents[Equiped_num - 1].Edible_Effective > 0)
				StartCoroutine(Eat());
			
			if(Contents[Equiped_num - 1].Attack_Effective > 0)
				Attack();
		}

		if(Input.GetKey("r"))
        {
            if(Equipped)
                StartCoroutine(Unequip(-1));
            else if(Equiped_num <= current)
                Equip(Equiped_num);
        }
			

		Hotbar();
		
		Place();

		StartCoroutine(EmptyHands());

	}

	void FixedUpdate()
	{

		if(Input.GetKey(KeyCode.E))
		{
			Pickup();
		}else
			PickupBar.percent = 0f;

		if(Input.GetKeyDown(KeyCode.F) && open == false)
			Open();
		else if(Input.GetKeyDown(KeyCode.F) && open == true)
			OpenInventory();

	}

	void CreateInventory()
	{
		GameObject background = Instantiate (backIMG, new Vector3 (0, 0, 0), Quaternion.identity); //Make Background
		background.transform.SetParent (inv.transform);
		RectTransform bg = background.GetComponent<RectTransform> ();
		bg.anchoredPosition = new Vector3 (0, 0, 0);
		bg.sizeDelta = new Vector2 (900, 500);
		bg.transform.localScale = Vector3.one;
		bg.name = "Background";

		SlotsEmpty = new GameObject ("Slots");													//Make empty for slots to use as parent
		SlotsEmpty.transform.SetParent (inv.transform);
		RectTransform SE = SlotsEmpty.AddComponent (typeof(RectTransform)) as RectTransform;
		SE.anchoredPosition = Vector3.zero;
		SE.transform.localScale = Vector3.one;
			
		int count = 0;
		for (int y = 180; y > -240; y = y - 60){          									 //Make all inventory slots
			for (int x = -360; x < -60; x = x + 60) {
				GameObject slot = Instantiate (blank, new Vector3 (0, 0, 0), Quaternion.identity);
				slot.transform.SetParent (SlotsEmpty.transform);

				RectTransform t = slot.GetComponent<RectTransform> ();
				t.anchoredPosition = new Vector3 (x, y, 0);
				t.transform.localScale = Vector3.one;

				RawImage img = slot.GetComponent<RawImage> ();
				TextMeshProUGUI a = slot.GetComponentInChildren<TextMeshProUGUI> ();

				Contents [count] = slot.GetComponentInChildren<InvInfo> ();
				InvGrid [count] = img;
				Amounts [count] = a;

				slots [count] = slot;
				count++;
				slot.name = "slot_" + count;
			}
		}
				
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

		List<GameObject> MainTabs = new List<GameObject>();	//ARRAY FOR ALL MAIN TABS
  	
		CraftingTabEmpty = Instantiate(blank_tab, new Vector3(0, 0, 0), Quaternion.identity); //Make empty for crafting tabs to use as parent
		CraftingTabEmpty.transform.SetParent(inv.transform);
		RectTransform CTE = CraftingTabEmpty.GetComponent<RectTransform>();
		CTE.anchoredPosition = new Vector3(500,175,0);
		CTE.transform.localScale = Vector3.one;
		CraftingTabEmpty.name = "Crafting Tab";
		TextMeshProUGUI CraftingTabEmptyName = CraftingTabEmpty.GetComponentInChildren<TextMeshProUGUI> ();
		CraftingTabEmptyName.text = "Crafting";
		CraftingTabEmpty.GetComponent<Tab>().active = true;
		MainTabs.Add (CraftingTabEmpty);

		List<GameObject> allcraftingTabs = new List<GameObject>();									//Create tabs for craftables
		int main_tab_x_loc = 100;
		for(int x = 0; x < craftingtabNames.Count;x++,main_tab_x_loc = main_tab_x_loc + 110)
		{

			GameObject tab = Instantiate(blank_tab, new Vector3(0, 0, 0), Quaternion.identity);
			tab.transform.SetParent(CraftingTabEmpty.transform);

			RectTransform t_tab = tab.GetComponent<RectTransform>();
			RectTransform t_parent = CraftingTabEmpty.GetComponent<RectTransform>();
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
					RectTransform t_parentparent = CraftingTabEmpty.GetComponent<RectTransform>();
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
		
		MapEmpty = Instantiate(blank_tab, new Vector3(0, 0, 0), Quaternion.identity); 					//make map tab
		MapEmpty.transform.SetParent(inv.transform);
		RectTransform ME = MapEmpty.GetComponent<RectTransform>();
		ME.anchoredPosition = new Vector3(500,125,0);
		ME.transform.localScale = Vector3.one;
		MapEmpty.name = "Map Tab";
		TextMeshProUGUI MapEmptyName = MapEmpty.GetComponentInChildren<TextMeshProUGUI> ();
		MapEmptyName.text = "Map";
		MainTabs.Add (MapEmpty);

		GameObject Map = Instantiate(Map_Prefab, new Vector3(0, 0, 0), Quaternion.identity);            //MAKE MAP
		Map.transform.SetParent(MapEmpty.transform);
		RectTransform Map_t = Map.GetComponent<RectTransform>();
		Map_t.anchoredPosition = new Vector3(-325,-125,0);
		Map_t.transform.localScale = Vector3.one;
		Map.name = "Map";

		/* 
		PlayerEmpty = Instantiate(blank_tab, new Vector3(0, 0, 0), Quaternion.identity); 					//make map tab
		PlayerEmpty.transform.SetParent(inv.transform);
		RectTransform PE = PlayerEmpty.GetComponent<RectTransform>();
		PE.anchoredPosition = new Vector3(500,75,0);
		PE.transform.localScale = Vector3.one;
		PlayerEmpty.name = "Player Tab";
		TextMeshProUGUI PlayerEmptyName = PlayerEmpty.GetComponentInChildren<TextMeshProUGUI> ();
		PlayerEmptyName.text = "Player";
		MainTabs.Add (PlayerEmpty);

		GameObject Player_BG = Instantiate (backIMG, new Vector3 (0, 0, 0), Quaternion.identity);
		Player_BG.transform.SetParent(PlayerEmpty.transform);
		RectTransform Player_BG_t = Player_BG.GetComponent<RectTransform>();
		Player_BG_t.anchoredPosition = new Vector3(-150,-75,0);
		Player_BG_t.sizeDelta = new Vector2 (150, 425);
		Player_BG.name = "Player Background";

		GameObject Player_Model = Instantiate(Player_Model_Prefab, new Vector3(0, 0, 0), Quaternion.identity);            //MAKE MAP
		Player_Model.transform.SetParent(PlayerEmpty.transform);
		RectTransform Player_Model_t = Player_Model.GetComponent<RectTransform>();
		Player_Model_t.anchoredPosition = new Vector3(-150,-75,0);
		Player_Model_t.transform.localScale = Vector3.one;
		Player_Model.name = "Player_Model";
		*/

		for(int a = 0; a < MainTabs.Count;a++)											//Tell crafting tabs what the other tabs are LEAVE AT END
			for(int b = 0; b < MainTabs.Count;b++)
				if(MainTabs[b] != MainTabs[a])
					MainTabs[b].GetComponent<Tab>().otherTabs.Add(MainTabs[a].GetComponent<Tab>());

	}

	void CreateHotbar()
	{
		int name = 0;

		for(int x = -120; x < 180; x = x + 60) //remove this being hardcoded
		{
			GameObject slot = Instantiate(hotbar_blank, new Vector3(0, 0, 0), Quaternion.identity);
			slot.transform.SetParent(hotbar.transform);

            //Hotbar_Contents[name] = slot.GetComponent<Hotbar_Item>();
            //Hotbar_Contents[name].info = GetComponent<InvInfo>();
            

			name++;
			slot.name = "" + name;

			TextMeshProUGUI a = slot.GetComponentInChildren<TextMeshProUGUI>();
			RawImage img = slot.GetComponent<RawImage>();
			RectTransform t = slot.GetComponent<RectTransform>();
			t.anchoredPosition = new Vector3(x,-300,0);
			t.transform.localScale = Vector3.one;
		}
	}

	void Pickup()
	{
		GameObject cam = mov.camera;

		RaycastHit hit;
		if(Physics.Raycast(cam.transform.position,cam.transform.forward, out hit,10))
		{
			try 
			{
						
				InvInfo PickedUp = hit.transform.gameObject.GetComponent<InvInfo>();

				if(PickedUp != null && PickedUp.pickupable && PickedUp.gameObject.transform.parent != equip_space)
				{
					if(PickedUp.Placeable && !donepickingup)
					{

						Pickuptime(hit.transform.gameObject);

					}else if(donepickingup || !PickedUp.Placeable)
					{

						if(hit.transform.gameObject.GetComponent<Rigidbody>())
							Destroy(hit.transform.gameObject.GetComponent<Rigidbody>());

						hit.transform.gameObject.SetActive(false);
						hit.transform.SetParent(storage.transform);
						hit.transform.localPosition = Vector3.zero;
					
						Add(current,PickedUp);
						SortInv();
						donepickingup = false;
						PickupBar.percent = 0f;
					}
				}
			}
			catch(NullReferenceException){PickupBar.percent = 0f;}
		}else
			PickupBar.percent = 0f;
	}

	void Pickuptime(GameObject obj)
	{

		if(lastobj == obj)
		{
			PickupBar.percent += 1f;

			if(PickupBar.percent >= 100f)
			{
				donepickingup = true;
				PickupBar.percent = 0f;
			}
		}else
		{
			PickupBar.percent = 0f;
			lastobj = obj;
		}

	}

	public void Drop(int x, bool destroy, int amount = -1, bool dispose = false, bool DROP = false)  //later just make it pass itself, the gameobject_old
	{ 

		//pass -1 for entire stack, anything else for anything else

		if(!destroy)
		{

			note.ItemTransaction(Contents[x - 1],true);
			
			shifted = false;
            if(x < Equiped_num)
                Equipped_Moved = true;
			
			GameObject dropped = Contents[x - 1].Model;
			dropped.transform.SetParent(global_dropped_items.transform);
			GameObject cam = mov.camera;

			RaycastHit hit;
			if(Physics.Raycast(cam.transform.position,cam.transform.forward, out hit,10))
			{
				dropped.transform.position = hit.point;
			}else
			{
				dropped.transform.position = cam.transform.position + cam.transform.forward * 10f;
			}
				
			dropped.transform.localScale = Vector3.one;

			if(dropped.GetComponent<Rigidbody>() == null)
				dropped.AddComponent<Rigidbody>();

			dropped.layer = 0;
			Contents[x - 1].pickupable = true;
			dropped.SetActive(true);

			if(Contents[Equiped_num - 1].Placeable && ghostActive) //if player drops while placeable is active
			{
				ghostActive = false;
				Destroy(placed);
			}

			dropped.name = Contents[x - 1].Name;

			Refresh();
			

		}

        if(amount == -1 || Contents[x - 1].Amount == 1)
        {
            if(destroy && x == Equiped_num && dispose) //might be vital
		    {
			    GameObject dropped = Contents[x - 1].Model;
		    	Destroy(dropped);
	        }

            if(!DROP)
                shifted = false;

            if(x < Equiped_num)
                Equipped_Moved = true;
            GameObject slot = Instantiate(blank, new Vector3(0, 0, 0), Quaternion.identity);
            slot.transform.SetParent(SlotsEmpty.transform);
            slot.name = "slot_" + x;
            RectTransform t = slot.GetComponent<RectTransform>();
            RectTransform t_old = slots[x-1].GetComponent<RectTransform>();
            t.anchoredPosition = t_old.anchoredPosition;
            t.transform.localScale = Vector3.one;
            RawImage img = slot.GetComponent<RawImage>();
            TextMeshProUGUI a = slot.GetComponentInChildren<TextMeshProUGUI>();
            x--;
            
            Contents[x] = GetComponent<InvInfo>();
            InvGrid[x] = img;
            Amounts[x] = a;

            Destroy(slots[x]);
            slots[x] = slot;
        }else
        {
            shifted = true;
            Contents[x - 1].Amount -= amount;

        }




		Refresh();
	}

	void Add(int index, InvInfo Item)
	{
		note.ItemTransaction(Item,false);
		int AmountLeft = Item.Amount;

		for(int x = 0; x < Contents.Length; x++)  //check each inventory item
		{
			if(Contents[x].Id == Item.Id) //for the same id/item type
			{

				int SpaceAvailable = Contents[x].StackNum - Contents[x].Amount; //Find space left in slot

			

				for(int i = 0;(i < SpaceAvailable) && (AmountLeft != 0);i++)  //fill each space left with what's available
				{

					AmountLeft--;
					Contents[x].Amount++;

				}

				if(AmountLeft == 0)
					Destroy(Item.gameObject);

			}
		}

		if(AmountLeft >= 1)
		{
			
			Item.Amount = AmountLeft;
			Contents[current] = Item;	
			current++;

		}


	}

	public void Equip(int x) //Make exception for clothes
	{
		if(Busy || (x == Equiped_num && Equipped))
		{

		}else
		{
			if(Equipped)
			{
				StartCoroutine(Unequip(x));
			}else
			{				

			int holdAnim = Contents[x - 1].Hold_Anim;

			an.SetInteger("Hold",holdAnim);
			StartCoroutine(BusyTimer());
			

			held = Contents[x - 1].Model;
			
			held.transform.SetParent (equip_space.transform);
			held.transform.position = new Vector3(0,0,0);
			Contents[x - 1].pickupable = false;
		
			held.transform.localRotation = Quaternion.identity;
			held.transform.localPosition = Vector3.zero;
			float scalval = Contents[x - 1].EquipScaleValue;
			held.transform.localScale =  new Vector3(scalval,scalval,scalval);

			held.SetActive(true);
					
			held.layer = 10;

			held.name = Contents[x - 1].Name;

			Equipped = true;
			Equiped_num = x;
		
			if(Contents[x - 1].Placeable && Equipped && !ghostActive)
				MakeGhost();
			
			
			}
		}
	}

	IEnumerator Unequip(int ReEquip)
	{

		if(Equipped && !Busy)
		{

			an.SetInteger("Hold",0);
			StartCoroutine(BusyTimer());
			held = Contents[Equiped_num - 1].Model;
			
			
			yield return new WaitForSeconds(an.GetCurrentAnimatorStateInfo(1).length / 2f);

			held.SetActive(false);
			Contents[Equiped_num - 1].pickupable = true;


			held.layer = 0;
			held.transform.SetParent(storage.transform);
			held.transform.localPosition = Vector3.zero;
			held.transform.localScale = Vector3.one;
			Equipped = false;


			if(Contents[Equiped_num - 1].Placeable && ghostActive)
			{
				ghostActive = false;
				Destroy(placed);
			}
				
	

			if(ReEquip > -1)
			{
				yield return new WaitForSeconds(0.25f);
				Equip(ReEquip);

			}

		}

	}

	IEnumerator BusyTimer()
	{

		Busy = true;
		yield return new WaitForSeconds(an.GetCurrentAnimatorStateInfo(1).length / 2f);
		Busy = false;

	}

	IEnumerator EmptyHands()
	{
		if(equip_space.transform.childCount == 0 && Equipped)
		{
			an.SetInteger("Hold",0);
			StartCoroutine(BusyTimer());

		

			yield return new WaitForSeconds((an.GetCurrentAnimatorStateInfo(1).length / 2f) + 0.1f);
			Equipped = false;


			if(Contents[Equiped_num - 1].Placeable && ghostActive)
			{
				ghostActive = false;
				Destroy(placed);
			}
	
		}
	}

	void Open()
	{
		GameObject cam = mov.camera;

		RaycastHit hit;
		if(Physics.Raycast(cam.transform.position,cam.transform.forward, out hit,14))
		{
			try 
			{
				Opened = null;
			    Opened = hit.transform.gameObject.GetComponent<Crafting>();

				if(Opened != null)
				{
					SideTabOpen = !SideTabOpen;

					if(open == false)
						OpenInventory();

					Opened.Toggle(inv);
					CraftingTabEmpty.SetActive(!SideTabOpen);
					MapEmpty.SetActive(!SideTabOpen);

					
				}
			}
			catch(NullReferenceException){}
		}

		
	}

	public void Refresh()
	{
		SortInv();	

		for(int x = 0; x < Contents.Length; x++)
		{
			if(Contents[x].Id != 0)
			{
				InvGrid[x].texture = Contents[x].Icon;
				InvGrid[x].color = Color.white;
				Amounts[x].text = Contents[x].Amount.ToString();
			}
		}	

		//for(int x = 0; x < Hotbar_Contents.Length; x++) //draw
		//{
        //    Hotbar_Contents[x].Refresh();
		//}						
	}

	void SortInv()
	{

		for(int x = 0; x < current; x++) 
		{
			if(Contents[x].Id == 0)
			{	

				if(!shifted){
					current--;
                    if(Equipped_Moved)    //if equiped num is past the threshold of the shifted part of the array
                    {
                        Equiped_num--;
                        Equipped_Moved = false;
                    }

				}

				shifted = true;
				Contents [x] = Contents [x + 1];
                //Drop(x + 2,false,-1,true);
				Drop(x + 2,true,-1,false,true);
			}
		}
	}

	void Hotbar()
	{
		if(Input.GetAxis("Mouse ScrollWheel") > 0 && slot != 1)
		{
			loc =  new Vector2(-60, 0);
			slot--;
		}else if(Input.GetAxis("Mouse ScrollWheel") < 0 && slot != 5)
		{
			loc = new Vector2(60, 0);
			slot++;
		}else
		{
			loc = new Vector2(0, 0);
		}
		select_tran.anchoredPosition += (Vector2)loc;
	}

	public void OpenInventory()
	{

		Refresh();
		if(SideTabOpen && open == true)
			Open();
		open = !open;
		Cursor.visible = open;
		Rigidbody rb = mov.gameObject.GetComponent<Rigidbody>();
		rb.isKinematic = !rb.isKinematic;
		mov.invopen = !mov.invopen;

	

		if(open == true)
		{
			inv.SetActive(true);
			game_hud.SetActive(false);
			EnemyHealth.SetActive(false);
		}else
		if(open == false)
		{
			game_hud.SetActive(true);
			inv.SetActive(false);
			EnemyHealth.SetActive(true);
		}
	}

	public void MoveToHotbar(int x, int y, bool assign)
	{
        /* 
        if(assign)
        {
            Hotbar_Contents[x].info = Contents[y - 1];
            Hotbar_Contents[x].Index = y - 1;
        }else
        {
            Hotbar_Contents[x].info = GetComponent<InvInfo>();
            Hotbar_Contents[x].Index = -1;
        }

		Refresh();
        */
	}

	public void Craft(int y) //Look for all Ingredients and craft item  make it work with multiple stacks
	{
        if(!SideTabOpen){  //IF STATEMENT FOR IF CRAFTING OBJECT FROM EXTERNAL CRAFTING ARRAY SUCH AS A FURNACE OR WORKBENCH
		y--;

		int amountHas = 0; //Check if each ingredient has the right amount 0 = none have right amount 1 = one has right amount etc.
		int amountsGreater = 0;

		int[] amountsLeft = new int[Craftables[y].Ingredients.Length];  //Array to tick down how many matches there are
		int[] totalAvailable = new int[Craftables[y].Ingredients.Length];

		for(int j = 0; j < amountsLeft.Length;j++)   //for each ingredient
		{

			amountsLeft[j] = Craftables[y].Amounts[j];  //assign amounts to second array

			for(int x = 0; x < Contents.Length; x++)       //check each slot in inventory
			{
				if(Contents[x].Id == Craftables[y].Ingredients[j].Id)    //if id's match
				{
					totalAvailable[j] += Contents[x].Amount;
				}
			}

			if(totalAvailable[j] >= amountsLeft[j])
				amountsGreater++;

		}


		if(amountsGreater == Craftables[y].Ingredients.Length){
			for(int j = 0; j < amountsLeft.Length;j++)   //for each ingredient
			{
				
				for(int x = 0; x < Contents.Length; x++)       //check each slot in inventory
				{
					if(Contents[x].Id == Craftables[y].Ingredients[j].Id)    //if id's match
					{

						if(amountsLeft[j] >= Contents[x].Amount)  //and subtracting doesn't make it go negative
						{
		
							amountsLeft[j] -= Contents[x].Amount;
							Contents[x].Amount = 0;
							Drop(x + 1,true,-1,true);
							x--;
							
						}else
						{
							Contents[x].Amount = Contents[x].Amount - amountsLeft[j];
							amountsLeft[j] = 0;
							
						}
					}
				}
				if(amountsLeft[j] == 0)
					amountHas++;
			}
		}

		if(amountHas == Craftables[y].Ingredients.Length)
		{
			GameObject craftobj = Instantiate(Craftables[y].Item.Model, new Vector3(0, 0, 0), Quaternion.identity);
			craftobj.SetActive(false);
			craftobj.transform.SetParent(storage.transform);
			Add(current,craftobj.GetComponent<InvInfo>());
		}
		Refresh();
        
        }else //************************************************************************************************************************* */
        {
            
            y--;

		int amountHas = 0; //Check if each ingredient has the right amount 0 = none have right amount 1 = one has right amount etc.
		int amountsGreater = 0;

		int[] amountsLeft = new int[Opened.Craftables[y].Ingredients.Length];  //Array to tick down how many matches there are
		int[] totalAvailable = new int[Opened.Craftables[y].Ingredients.Length];

		for(int j = 0; j < amountsLeft.Length;j++)   //for each ingredient
		{

			amountsLeft[j] = Opened.Craftables[y].Amounts[j];  //assign amounts to second array

			for(int x = 0; x < Contents.Length; x++)       //check each slot in inventory
			{
				if(Contents[x].Id == Opened.Craftables[y].Ingredients[j].Id)    //if id's match
				{
					totalAvailable[j] += Contents[x].Amount;
				}
			}

			if(totalAvailable[j] >= amountsLeft[j])
				amountsGreater++;

		}

		if(amountsGreater == Opened.Craftables[y].Ingredients.Length){
			for(int j = 0; j < amountsLeft.Length;j++)   //for each ingredient
			{
				
				for(int x = 0; x < Contents.Length; x++)       //check each slot in inventory
				{
					if(Contents[x].Id == Opened.Craftables[y].Ingredients[j].Id)    //if id's match
					{

						if(amountsLeft[j] >= Contents[x].Amount)  //and subtracting doesn't make it go negative
						{
		
							amountsLeft[j] -= Contents[x].Amount;
							Contents[x].Amount = 0;
							Drop(x + 1,true,-1,true);
							x--;
							
						}else
						{
							Contents[x].Amount = Contents[x].Amount - amountsLeft[j];
							amountsLeft[j] = 0;
							
						}
					}
				}
				if(amountsLeft[j] == 0)
					amountHas++;
			}
		}

		if(amountHas == Opened.Craftables[y].Ingredients.Length)
		{
			GameObject craftobj = Instantiate(Opened.Craftables[y].Item.Model, new Vector3(0, 0, 0), Quaternion.identity);
			craftobj.SetActive(false);
			craftobj.transform.SetParent(storage.transform);
			Add(current,craftobj.GetComponent<InvInfo>());
		}
		Refresh();

        }
	}



	void Mine()
	{

		GameObject cam = mov.camera;

		int hitstate = an.GetInteger("Hit");

		int layer_mask = LayerMask.GetMask("Default");


		RaycastHit hit;
		if(Physics.Raycast(cam.transform.position,cam.transform.forward, out hit,10))
		{

			Mine HitObject = null;
			bool inParent = true;

			try {
				HitObject = hit.transform.gameObject.GetComponent<Mine>();
				if(HitObject != null)
					inParent = false;
			}catch(NullReferenceException){}


			if (inParent) {

				try {
					HitObject = hit.transform.parent.gameObject.GetComponent<Mine> ();
				} catch (NullReferenceException) {
				}

			}

				if(HitObject != null){
					if((Contents[Equiped_num - 1].Mine_Effective > 0) && (Equipped) && (hitstate == 0))
					{
						body.transform.eulerAngles = new Vector3(0f,cam.transform.eulerAngles.y,0f);
								
						HitObject.hitsLeft--;
						an.SetInteger("Hit",Contents[Equiped_num - 1].Action_Anim);
							

						GameObject minedObj = Instantiate(HitObject.Item.Model, new Vector3(0, 0, 0), Quaternion.identity);
						minedObj.SetActive(false);
						minedObj.transform.SetParent(storage.transform);
						Add(current,minedObj.GetComponent<InvInfo>());
						
						SortInv();
					}
				}
			
		}
	
	}

	void Attack()
	{
		GameObject cam = mov.camera;
		RaycastHit hit;
		held = Contents[Equiped_num - 1].Model;
		int hitstate = an.GetInteger("Hit");

        //body.transform.eulerAngles = new Vector3(0f,cam.transform.eulerAngles.y,0f);

		if(Physics.Raycast(cam.transform.position,cam.transform.forward, out hit,15))
		{
		try {
			Stats HitObject = hit.transform.gameObject.GetComponent<Stats> ();


			if(HitObject != null){
				HitObject.Health -= 25;
				}
		} catch (NullReferenceException) {}
		}
		//find out when attack is finished
		float angle = 0f;

		if (cam.transform.eulerAngles.x > 0 && cam.transform.eulerAngles.x < 180)
			angle = -cam.transform.eulerAngles.x;
		else
			angle = 360 - cam.transform.eulerAngles.x;

		an.SetFloat("Up/Down",angle);
		an.SetInteger("Hit",Contents[Equiped_num - 1].Action_Anim);
	}

	IEnumerator Eat()
	{
        if(!Eating)
        {
            Eating = true;
            an.SetInteger("Eat",Contents[Equiped_num - 1].Action_Anim);
            yield return new WaitForSeconds(an.GetCurrentAnimatorStateInfo(1).length);
            an.SetInteger("Eat",0);
            LifeManager.Hunger += Contents[Equiped_num - 1].Edible_Effective;
            Drop(Equiped_num,true,1,true);
            Eating = false;
        }
		

	}

	void MakeGhost()
	{

		placed = Instantiate(Contents[Equiped_num - 1].Model, new Vector3(0, 0, 0), Quaternion.identity);
		placed.transform.SetParent (place_space.transform);
		placed.transform.position = new Vector3(0,0,0);
		
		placed.transform.localRotation = Quaternion.identity;
		placed.transform.localPosition = Vector3.zero;
		placed.transform.localScale = Vector3.one;

		placed.name = Contents[Equiped_num - 1].Name;

		Collider[] colls = placed.gameObject.GetComponents<Collider>();
		foreach (Collider coll in colls)
			coll.isTrigger = true;
		
		placed.gameObject.GetComponent<Placing>().enabled = true;

		placed.layer = 0;

		placed.SetActive(true);

		ghostActive = true;

		p = placed.gameObject.GetComponent<Placing>();

	}

	void Place()
	{
	

		if(Input.GetMouseButtonDown(0) && !open)
		{
			if(Contents[Equiped_num - 1].Placeable && ghostActive)
			{
				
				if(p.canPlace && p.isGrounded) //what do
				{

					placed.transform.SetParent(global_dropped_items.transform);
					Collider[] colls = placed.gameObject.GetComponents<Collider>();
					foreach (Collider coll in colls)
						coll.isTrigger = false;
					placed.gameObject.GetComponent<Placing>().enabled = false;

					placed.GetComponent<InvInfo>().pickupable = true;

					Drop(Equiped_num,true,1,true);

					ghostActive = false;


				}
			}
		}

		if(Contents[Equiped_num - 1].Placeable && ghostActive)
		{

			if (Input.GetAxis("Mouse ScrollWheel") > 0) {
				p.transform.Rotate(Vector3.up * 8f, Space.Self);
			}

			if (Input.GetAxis("Mouse ScrollWheel") < 0) {
				p.transform.Rotate(Vector3.down * 8f, Space.Self);
			}

		}

	}

	public void Pause()
	{
		if(open)
			OpenInventory();

		Rigidbody rb = mov.gameObject.GetComponent<Rigidbody>();
		rb.isKinematic = !rb.isKinematic;
		mov.invopen = !mov.invopen;

		game_hud.SetActive(paused);
		
		Notifications.SetActive(paused);
		Hotbar_Obj.SetActive(paused);

		if(mov.isAlive == false)
			DeathScreen.SetActive(paused);
		
			

			
			
		paused = !paused;
		Cursor.visible = paused;
		PauseMenu.SetActive(paused);
		if(paused == true)
		{
			Time.timeScale = 0f;
			EnemyHealth.SetActive(false);
		}

		if(paused == false)
		{
			Time.timeScale = 1f;
			EnemyHealth.SetActive(true);
		}
		

	}

	public void Quit()
	{
		Pause ();
		Cursor.visible = true;
		GameObject loaderobj = GameObject.FindWithTag ("Loader");
		Destroy(loaderobj);
		SceneManager.LoadScene(0);
		

	}

}
