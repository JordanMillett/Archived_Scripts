using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spin : MonoBehaviour
{
    public float PanSpeed = 5f;

    void FixedUpdate()
    {
        this.transform.Rotate(0f,PanSpeed,0f);
    }
}
