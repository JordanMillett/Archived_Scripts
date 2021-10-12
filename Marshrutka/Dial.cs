using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dial : MonoBehaviour
{
    public float Value = 0f;
    public float Max = 100f;

    float StartAngle = -140f;
    float EndAngle = 140f;
    float Angle = 0f;

    public Transform Needle;

    void Update()
    {
        if(Value > Max)
            Value = Max;

        if(Value < 0f)
            Value = 0f;

        Angle = Mathf.Lerp(StartAngle, EndAngle, Value/Max);

        Needle.transform.localEulerAngles = new Vector3(0f, 0f, Angle);
    }
}
