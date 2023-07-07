using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocalReflection : MonoBehaviour
{
    ReflectionProbe RP;
    Transform C;

    void Start()
    {
        RP = GetComponent<ReflectionProbe>();
        C = GameObject.FindWithTag("Camera").transform;
        RP.RenderProbe();
    }

    void Update()
    {
        if(Vector3.Distance(C.position, this.transform.position) < 8f)
            RP.RenderProbe();
    }
}
