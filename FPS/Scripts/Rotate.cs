using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotate : MonoBehaviour
{
    
    public Vector3 Angles;

    void Update()
    {
        this.transform.localEulerAngles += Angles;



    }
}
