using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Popup_Container : Popup
{
    public GameObject M_Close;
    public GameObject M_Open;
    public Container C;
    public TextMeshProUGUI Info;
    public TextMeshProUGUI Description;

    public Popup_Item[] Items = new Popup_Item[4];
    string[] ItemInfos = new string[4];

    int TakenCount = 0;
    
    protected override void Update()
    {
        base.Update();

        if (!Items[SelectorIndex].Taken)
        {
            Info.text = GetText(C.Contents[SelectorIndex]);
            Description.text = C.Contents[SelectorIndex].Description;
        }else
        {
            Info.text = "";
            Description.text = "";
        }

        if (Selected == this)
        {
            if (!SelectorActive)
            {
                if (Input.GetKeyDown("e"))
                    Open();
            }else
            {
                if(Input.GetKeyDown("e"))
                {
                    if (!Items[SelectorIndex].Taken)
                    {
                        TakenCount += Items[SelectorIndex].Take() ? 1 : 0;
                        
                        if(TakenCount == 4)
                        {
                            Destroy(C.Effect.gameObject);
                            Destroy(C);
                            Destroy(this.gameObject);
                        }
                    }
                }
            }
        }
    }

    string GetText(Item I)
    {
        string output = "";

        switch (I)
        {
            case Medicine:
            {
                Medicine Casted = I as Medicine;
                output += Casted.Type.ToString() + "\n";
                output += "+" + Casted.Health + " Health";
                return output;
            }
            case Special:
            {
                Special Casted = I as Special;
                if(Casted.Type == Special.Types.FuelRod)
                    output = "Increases station power if deposited in the reactor room";
                return output;
            }
            case Valuable:
            {
                Valuable Casted = I as Valuable;
                output += Casted.Type.ToString() + "\n";
                output += "+" + Casted.Value + " Credits";
                return output;
            }
            case Weapon:
            {
                Weapon Casted = I as Weapon;
                output += Casted.Firemode.ToString() + Casted.Type.ToString().ToLower() + "\n";
                if(!Casted.AllAtOnce)
                    output += (Casted.Damage + Upgrades.GetDamageBonus(Casted.Type)) + " DMG\tLVL " + (Game.SaveData.WeaponDamageUpgrades[(int)Casted.Type] + 1) + "\n";
                else
                    output += (Casted.Damage + Upgrades.GetDamageBonus(Casted.Type)) * (((Casted.SprayMax * 2)/Casted.SprayInterval) + 1) + " DMG\tLVL " + (Game.SaveData.WeaponDamageUpgrades[(int)Casted.Type] + 1) + "\n";
                    //output += Casted.Damage + " * " + (((Casted.SprayMax * 2)/Casted.SprayInterval) + 1) + " DMG\n";
                output += (Casted.BurstWaves * (Casted.RPM + Upgrades.GetRPMBonus(Casted.Type))) + " RPM\tLVL " + (Game.SaveData.WeaponFirerateUpgrades[(int)Casted.Type] + 1) + "\n";
                
                if(Casted.Bonus == DamageBonus.Health)
                    output += "Bonus Health DMG";
                if(Casted.Bonus == DamageBonus.Shields)
                    output += "Bonus Shield DMG";
                
                return output;
            }
        }

        return output;
    }
    
    void Open()
    {
        M_Close.SetActive(false);
        M_Open.SetActive(true);
        SelectorActive = true;
    }
    
    public void SetItem(int Index, Item I)
    {
        Items[Index].Initialize(I);
    }
}
