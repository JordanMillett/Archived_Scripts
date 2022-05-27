using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sun : MonoBehaviour
{
    void Start()
    {
        this.transform.localEulerAngles = new Vector3(Random.Range(40f, 60f), Random.Range(0f, 360f), 0f);   
    }
}
