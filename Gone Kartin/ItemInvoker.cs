using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ItemInvoker : MonoBehaviour
{
    public UnityEvent Function;
    public KartController KC;

    public void Activate()
    {
        Function.Invoke();
    }
}