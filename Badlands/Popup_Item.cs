using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class Popup_Item : MonoBehaviour
{
    public TextMeshProUGUI Name;
    public RawImage Icon;
    Item I;

    public bool Taken = false;

    public void Initialize(Item _item)
    {
        I = _item;

        Name.text = I.Name;

        switch (I)
        {
            case Medicine:
            {
                Medicine Casted = I as Medicine;
                Icon.color = Casted.Type == Medicine.Types.Food ? Color.green : Color.blue;
                break;
            }
            case Special:
            {
                Icon.color = Color.magenta;
                break;
            }
            case Valuable:
            {
                Valuable Casted = I as Valuable;
                Icon.color = Casted.Type == Valuable.Types.Valuable ? Color.yellow : Color.gray;
                break;
            }
            case Weapon:
            {
                Icon.color = Color.red;
                break;
            }
        }
    }

    public bool Take()
    {
        if(Taken)
            return false;
        
        Taken = Player.P.PickupItem(I);
        
        if(Taken)
        {
            Name.text = "";
            Icon.enabled = false;
        }

        return Taken;
    }
}
