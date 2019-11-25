using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CaptureArea : MonoBehaviour
{
    BoxCollider Col;

    void Start()
    {
        Col = GetComponent<BoxCollider>();
    }

    void OnTriggerEnter(Collider C)
    {
        //if(C.GetComponent<LifeManager>() != null)
            //C.GetComponent<LifeManager>().Damage(10000f);

        if(C.GetComponent<ObjectiveManager>() != null)
            if(C.GetComponent<ObjectiveManager>().HostageCaptured)
                C.GetComponent<ObjectiveManager>().Win();
    }
}
