using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    VehicleController VC;

    void Start()
    {
        VC = GetComponent<VehicleController>();  
    }

    void Update()
    {
        if(Input.GetKey("w"))
        {
            VC.GasPedal();
        }

        if(Input.GetKey("s"))
        {
            VC.BrakePedal();
        }
    }
}
