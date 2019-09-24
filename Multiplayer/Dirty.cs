using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dirty : MonoBehaviour
{
    void OnCollisionEnter(Collision Col)
    {

        try{

            Cookable C = Col.transform.gameObject.GetComponent<Cookable>();

            if(C != null)
                C.Dirty = true;

        }catch{}

    }
}
