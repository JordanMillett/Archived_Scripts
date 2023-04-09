using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Popup_Lab : Popup
{
    public SimpleBar StatusBar;
    public TextMeshProUGUI Status;
    bool CanCollect = false;

    float StartTime = 0f;
    float EndTime = 0f;

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
                Status.text = "Generating...";
                StatusBar.Current = ((Time.time - StartTime) / (EndTime - StartTime)) * 100f;
            }
            else
            {   
                if(CanCollect)
                {
                    Status.text = "Collect Blueprint";
                    StatusBar.Current = 100f;
                    
                    if(Input.GetKeyDown("e"))
                    {
                        Game.RunData.CollectedBlueprints++;
                        CanCollect = false;
                    }
                }else
                {
                    if (Game.RunData.CollectedBlueprints != Game.StationData.MaxBlueprints)
                    {
                        Status.text = "Generate Blueprint";
                        
                        if(Input.GetKeyDown("e"))
                        {
                            StartTime = Time.time;
                            EndTime = StartTime + Game.StationData.BlueprintTime;
                            CanCollect = true;
                        }
                    }
                    else
                    {
                        Status.text = "Out of Blueprints";
                    }
                    StatusBar.Current = 0f;
                }
            }
        }
    }
}
