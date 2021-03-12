using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Breakable : MonoBehaviour
{
    public float despawnTime = 10f;

    void OnJointBreak(float breakForce)
    {
        Invoke("Despawn", despawnTime);
    }

    void Despawn()
    {
        Destroy(this.gameObject);
    }
}
