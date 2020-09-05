using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WheelController : MonoBehaviour
{
    public Rigidbody R;

    float TurnLimits = 45f;

    public float CurrentHeading = 0f;

    public void Control(float Force, float Brake)
    {   
        R.AddForceAtPosition(transform.forward * Force * (1 - Brake), this.transform.position);
    }

    public void Turn(float Amount)
    {
        CurrentHeading += Amount;
        if(CurrentHeading > TurnLimits)
            CurrentHeading = TurnLimits;

        if(CurrentHeading < -TurnLimits)
            CurrentHeading = -TurnLimits;

        this.transform.localEulerAngles = new Vector3(0f, CurrentHeading, 0f);
    }
}
