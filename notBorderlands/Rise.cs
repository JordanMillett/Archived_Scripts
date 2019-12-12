using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rise : MonoBehaviour
{
    public float Speed = .1f;

    void Update()
    {
        this.transform.position += new Vector3(0f, Speed, 0f);
    }
}
