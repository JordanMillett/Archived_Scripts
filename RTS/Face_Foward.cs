using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Face_Foward : MonoBehaviour
{
    void Update()
    {
        this.transform.LookAt(transform.position + new Vector3(0f,0f,1f));
    }
}
