using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TractorBeam : MonoBehaviour
{
    void OnTriggerStay(Collider col)
    {
        if (col.attachedRigidbody)
            col.attachedRigidbody.AddForce((Vector3.up * 20f) * col.attachedRigidbody.mass);
    }
}
