using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupableEffect : MonoBehaviour
{
    public float SpinRate;
    public float BobRate;
    public float BobHeight;
    
    float TotalTime = 0f;

    void Start()
    {

        this.transform.localEulerAngles += new Vector3(0f, Random.Range(0f, 360f), 0f);
        TotalTime = Random.Range(0f, 1000f);

    }

    void FixedUpdate()
    {
        TotalTime += Time.fixedDeltaTime;

        this.transform.localEulerAngles += new Vector3(0f, SpinRate, 0f);
        this.transform.localPosition = new Vector3(0f, (Mathf.Sin(TotalTime) + BobHeight) * BobRate, 0f);
    }   
}