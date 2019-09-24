using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Decay : MonoBehaviour
{
    public float DecayTime = 10f;

    void Start()
    {
        Invoke("Delete",DecayTime);
    }

    void Delete()
    {

        Destroy(this.gameObject);

    }
}
