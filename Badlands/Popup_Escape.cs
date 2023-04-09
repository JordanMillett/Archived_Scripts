using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Popup_Escape : Popup
{
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
            if(Input.GetKeyDown("e"))
            {
                if(SelectorIndex == 0)
                {
                    Game.Deposit();
                    
                }else
                {
                    Game.Deposit();
                    Game.EndRun(true);
                }
            }
        }
    }
}
