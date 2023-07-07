using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Despawn : MonoBehaviour
{
    public float DespawnTime = 1f;

    void Start()
    {
        Invoke("Delete", DespawnTime);
    }

    void Delete()
    {
        Destroy(this.gameObject);
    }
}