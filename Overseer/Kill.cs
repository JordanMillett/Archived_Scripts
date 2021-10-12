using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Kill : MonoBehaviour
{
    void OnTriggerEnter(Collider Col)
    {
        Player P = Col.gameObject.GetComponent<Player>();

        if(P != null)
        {
            Destroy(P.gameObject);
        }
    }
}
