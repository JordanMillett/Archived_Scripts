using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Popup_Vault : Popup
{
    public SimpleBar StatusBar;
    public TextMeshProUGUI Status;
    public TextMeshProUGUI Credits;

    float StartTime = 0f;
    float EndTime = 0f;
    bool CanCollect = false;

    protected override void Start()
    {
        base.Start();

        SelectorActive = true;
    }
    
    protected override void Update()
    {
        base.Update();

        if (Selected == this)
        {
            if (Time.time < EndTime)
            {
                Status.text = "Cracking...";
                StatusBar.Current = ((Time.time - StartTime) / (EndTime - StartTime)) * 100f;
            }
            else
            {   
                Credits.text = Game.SaveData.VaultCredits + " Credits";
                
                if(CanCollect)
                {
                    
                    Status.text = "Collect Credits";
                    StatusBar.Current = 100f;
                    
                    if(Input.GetKeyDown("e"))
                    {
                        if (Game.SaveData.VaultCredits > 0)
                        {
                            if (Game.RunData.HoldingCredits + Game.SaveData.VaultCredits < Game.SaveData.MaxHoldingCredits)
                            {
                                Game.RunData.HoldingCredits += Game.SaveData.VaultCredits;
                                Game.SaveData.VaultCredits = 0;
                            }else
                            {
                                Game.SaveData.VaultCredits -= Game.SaveData.MaxHoldingCredits - Game.RunData.HoldingCredits;
                                Game.RunData.HoldingCredits = Game.SaveData.MaxHoldingCredits;
                            }
                        }
                    }
                }else
                {
                    Status.text = "Crack Safe";
                    
                    if(Input.GetKeyDown("e"))
                    {
                        StartTime = Time.time;
                        EndTime = StartTime + Game.SafeCrackTime;
                        CanCollect = true;
                    }
                    StatusBar.Current = 0f;
                }
            }
        }
    }
}
