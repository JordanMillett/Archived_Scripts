using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spin : MonoBehaviour
{
    public enum Axis
    {
        X,
        Y,
        Z
    }

    public Axis spinAxis;
    public float speed;

    void FixedUpdate()
    {
        switch (spinAxis)
        {
            case Axis.X : 
                this.transform.localEulerAngles += new Vector3(speed, 0f, 0f);
                break;
            case Axis.Y : 
                this.transform.localEulerAngles += new Vector3(0f, speed, 0f);
                break;
            case Axis.Z : 
                this.transform.localEulerAngles += new Vector3(0f, 0f, speed);
                break;
        }
        
    }
}