using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Hotbar_Item : MonoBehaviour 
{

    public GameObject blank;

    public InvInfo info; //make blank when object is deleted 
    public int Index;

    RawImage blan;
    RawImage Image;
    TextMeshProUGUI Num;

    bool beenInit = false;

    void Init()
    {
        blan = blank.GetComponent<RawImage>();
		Image = this.GetComponent<RawImage>();
        Num = gameObject.GetComponentInChildren<TextMeshProUGUI>();
        Num.text = "";
        beenInit = true;
    }

    public void Refresh()
    {
        /* 
        if(beenInit == false)
            Init();

        if(info.Id != -1)
        {
            Image.texture = info.Icon;
            Image.color = Color.white;
            Num.text = info.Amount.ToString();
        }else
        {
            Image = blan;
            Num.text = "";
            Index = -1;
        }
        */
    }

}
