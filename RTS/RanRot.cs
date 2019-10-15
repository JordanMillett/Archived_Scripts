using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RanRot : MonoBehaviour
{
    void Start()
    {
        this.transform.eulerAngles = new Vector3(this.transform.eulerAngles.x, Random.Range(0f,360f), this.transform.eulerAngles.z);
    }
}
