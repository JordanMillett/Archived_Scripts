using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeleteZone : MonoBehaviour
{
    void OnTriggerEnter(Collider Col)
    {

        Destroy(Col.transform.gameObject);

    }
}
