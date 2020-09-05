using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Laser : MonoBehaviour
{
    void Start()
    {
        Invoke("Despawn", 5f);
    }

    void OnCollisionEnter(Collision Col)
    {
        Destroy(GetComponent<Collider>());
        GetComponent<Rigidbody>().isKinematic = true;
        Invoke("Despawn", .2f);
    }

    void Despawn()
    {
        Destroy(this.gameObject);
    }
}
