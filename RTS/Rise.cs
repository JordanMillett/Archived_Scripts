using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rise : MonoBehaviour
{
    
    public float Speed;

    void Update()
    {
        transform.Translate(Vector3.up * Time.fixedDeltaTime * Speed, Space.World);
    }
}
