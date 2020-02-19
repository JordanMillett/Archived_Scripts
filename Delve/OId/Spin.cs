using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spin : MonoBehaviour
{
    public float Speed = 5f;
    public float Amplitude = 1f;

    float TotalTime = 0f;

    void FixedUpdate()
    {
        TotalTime += Time.fixedDeltaTime;

        this.transform.Rotate(0f,0f,Speed);
        this.transform.localPosition = new Vector3(0f, (Mathf.Sin(TotalTime) + 0.25f) * Amplitude,0f);
    }
}
