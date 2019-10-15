using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FogDestroy : MonoBehaviour
{
    void OnTriggerEnter(Collider C)
    {
        if(C.GetComponent<Unit>() != null)
            if(C.GetComponent<Unit>().Team == 0)
                Destroy(this.gameObject);
    }
}
