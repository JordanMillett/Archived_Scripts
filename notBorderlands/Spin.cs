using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spin : MonoBehaviour
{
    public float Speed = 2f;

    void Start()
    {

        this.transform.Rotate(0f, Random.Range(0f,360f),0f);

    }

    void FixedUpdate()
    {
        this.transform.Rotate(0f,Speed,0f);
    }
}
