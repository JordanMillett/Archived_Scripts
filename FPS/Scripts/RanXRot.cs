using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RanXRot : MonoBehaviour
{
    void Start()
    {
        this.transform.localEulerAngles = new Vector3(Random.Range(0f,360f),0f,0f);
    }
}
