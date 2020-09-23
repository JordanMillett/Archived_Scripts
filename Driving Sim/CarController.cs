using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarController : MonoBehaviour
{
    public float WheelForce;
    public float WheelTurnLimit;
    public float Reverse = 1f;
    public float RPM = 0f;
    public bool Gas = false;  
    
    public AnimationCurve TorqueCurve;
    public AnimationCurve TurnCurve;
    float RPMMax = 6000f;
    float RPMAlpha = 0f;
    float RPMSpeed = 0.005f;
    float RPMIdle = 0.2f;

    float WheelDirection = 0f;
    float WheelAlpha = 0f;

    Rigidbody R;

    void Start()
    {
        R = GetComponent<Rigidbody>();
    }

    public void Turn(float Speed)
    {
        if(Speed > 0f) //if Turning right
        {
            if(WheelAlpha < 1f)
                WheelAlpha += Speed;

            if(WheelAlpha >= 1f)
                WheelAlpha = 1f;

        }else if(Speed < 0f)          //if Turning left
        {
            if(WheelAlpha > 0f)
                WheelAlpha += Speed;

            if(WheelAlpha <= 0f)
                WheelAlpha = 0f;
        }else if(Speed == 0f)         //if not turning
        {   
            if(Mathf.Abs(WheelAlpha - 0.5f) <= 0.2f)
            {
                WheelAlpha = 0.5f;
            }else
            {
                if(WheelAlpha > 0.5f)
                    Turn(-.1f);
                else
                    Turn(0.1f);
            }

            
        }

        WheelDirection = Mathf.Lerp(-WheelTurnLimit, WheelTurnLimit, WheelAlpha);
    }
    
    void Update()
    {
        if(Gas)
        {
            if(RPMAlpha < 1f)
                RPMAlpha += RPMSpeed;

            if(RPMAlpha >= 1f)
                RPMAlpha = 1f;
        }else
        {
            if(RPMAlpha > RPMIdle)
                RPMAlpha += -RPMSpeed;

            if(RPMAlpha <= RPMIdle)
                RPMAlpha = RPMIdle;
        }

        RPM = Mathf.Lerp(0f, RPMMax, RPMAlpha);
    }

    void FixedUpdate()
    {
        if(Gas)
        {
            R.AddTorque(transform.up * WheelDirection * Reverse * TurnCurve.Evaluate(RPMAlpha), ForceMode.Acceleration);
            R.AddForce(WheelForce * transform.forward * Reverse * TorqueCurve.Evaluate(RPMAlpha), ForceMode.Acceleration);
        }
    }
}
