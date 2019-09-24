using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skybox : MonoBehaviour
{
    Transform Space;

    void Start()
    {
        Space = GameObject.FindWithTag("Space").transform;
    }

    void Update()
    {
        this.transform.rotation = Space.rotation;   
    }
}
