using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Car
{
    public override void Update()
    {
        base.Update();

        float Steer = 0f;
        
        if(Input.GetKey("a"))
            Steer -= 1f;
        if(Input.GetKey("d"))
            Steer += 1f;

        Control(Input.GetKey("w"), Input.GetKey("s"), Steer);
        
        if(Input.GetKeyDown(KeyCode.LeftShift))
            ShiftGear(true);
        
        if(Input.GetKeyDown(KeyCode.LeftControl))
            ShiftGear(false);
    }
}
