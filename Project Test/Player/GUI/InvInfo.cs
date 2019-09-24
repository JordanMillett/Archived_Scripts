using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class InvInfo : MonoBehaviour {

	public int Id = 0;
    public string Name = "Default Name";
    public int Amount = 1;
	public int StackNum = 8;	
	public Texture Icon;
	public GameObject Model;
	public int Hold_Anim = 1;
	public int Action_Anim;
	public float EquipScaleValue = 1;
	public int Edible_Effective = 0;
    public int Attack_Effective = 0;
    public int Mine_Effective = 0;
	public bool Placeable = false;
	public bool wearable = false;
    public bool pickupable = true;
	
}
