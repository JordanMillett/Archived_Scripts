using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Upright : MonoBehaviour
{
    void Update()
    {
        this.transform.eulerAngles = Vector3.zero;
    }
}
