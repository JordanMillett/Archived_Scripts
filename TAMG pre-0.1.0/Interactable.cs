using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Interactable : MonoBehaviour
{
    public UnityEvent Function;
    public char Filter;

    public void Activate(char IncomingFilter)
    {
        if(IncomingFilter == Filter)
            Function.Invoke();
    }
}