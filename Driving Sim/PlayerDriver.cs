using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDriver : MonoBehaviour
{
    CarController CC;

    void Start()
    {
        CC = GetComponent<CarController>();
    }

    void Update()
    {

        if(Input.GetKey("w") || Input.GetKey("s"))
        {
            CC.Gas = true;

            if(Input.GetKey("w"))
            {
                CC.Reverse = 1f;
            }

            if(Input.GetKey("s"))
            {
                CC.Reverse = -1f;
            }

        }else
        {
            CC.Gas = false;
        }

        if(Input.GetKey("a") || Input.GetKey("d"))
        {
            if(Input.GetKey("a"))
            {
                CC.Turn(-.05f);
            }

            if(Input.GetKey("d"))
            {
                CC.Turn(.05f);
            }
        }else
        {
            CC.Turn(0f);
        }
    }
}
