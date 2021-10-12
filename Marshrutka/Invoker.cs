using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Invoker : MonoBehaviour
{
    public UnityEvent Function;

    void OnTriggerEnter(Collider Col)
    {



    }

    public void Activate()
    {
        Function.Invoke();
    }
}