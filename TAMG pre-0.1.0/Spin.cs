using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spin : MonoBehaviour
{
    public float SpinRate;
    
    void Start()
    {

        this.transform.localEulerAngles += new Vector3(0f, Random.Range(0f, 360f), 0f);

    }

    void FixedUpdate()
    {
        this.transform.localEulerAngles += new Vector3(0f, SpinRate, 0f);
    }
}
