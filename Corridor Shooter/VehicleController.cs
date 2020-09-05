using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VehicleController : MonoBehaviour
{
    Rigidbody R;

    public float WheelForce;

    public bool GasDown = false;
    public float GasAmount = 0f;
    public bool BrakeDown = false;
    public float BrakeAmount = 0f;

    float PedalResponseSpeed = 0.1f;

    public WheelController FL;
    public WheelController FR;
    public WheelController BR;
    public WheelController BL;

    void Start()
    {
        R = GetComponent<Rigidbody>();

        FL.R = R;
        FR.R = R;
        BR.R = R;
        BL.R = R;
    }

    void FixedUpdate()
    {
        CalculatePedals();

        FL.Control(Mathf.Lerp(0f, WheelForce, GasAmount), BrakeAmount);
        FR.Control(Mathf.Lerp(0f, WheelForce, GasAmount), BrakeAmount);
        //BR.Control(Mathf.Lerp(0f, WheelTorque, GasAmount), BrakeAmount);
        //BL.Control(Mathf.Lerp(0f, WheelTorque, GasAmount), BrakeAmount);

        if(Input.GetKey("a"))
        {
            FL.Turn(-2f);
            FR.Turn(-2f);
        }

        if(Input.GetKey("d"))
        {
            FL.Turn(2f);
            FR.Turn(2f);
        }

    }

    public void GasPedal()
    {
        GasDown = true;
    }

    public void BrakePedal()
    {
        BrakeDown = true;
    }

    void CalculatePedals()
    {
        if(GasDown)
        {
            if(GasAmount < 1f)
                GasAmount += PedalResponseSpeed;

            if(GasAmount >= 1f)
                GasAmount = 1f;
        }else
        {
            if(GasAmount > 0f)
                GasAmount -= PedalResponseSpeed;

            if(GasAmount <= 0f)
                GasAmount = 0f;
        }

        if(BrakeDown)
        {
            if(BrakeAmount < 1f)
                BrakeAmount += PedalResponseSpeed;

            if(BrakeAmount >= 1f)
                BrakeAmount = 1f;
        }else
        {
            if(BrakeAmount > 0f)
                BrakeAmount -= PedalResponseSpeed;

            if(BrakeAmount <= 0f)
                BrakeAmount = 0f;
        }

        GasDown = false;
        BrakeDown = false;
    }

}
