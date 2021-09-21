using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OutOfBounds : MonoBehaviour
{
    void OnCollisionEnter(Collision Col)
    {
        if(Col.gameObject.GetComponent<KartController>() != null)
        {
            Col.gameObject.GetComponent<KartController>().OutOfBounds();
        }
    }

    void OnTriggerEnter(Collider Col)
    {
        if(Col.gameObject.GetComponent<KartController>() != null)
        {
            Col.gameObject.GetComponent<KartController>().OutOfBounds();
        }
    }
}
