using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;

public class VisibleHandler : MonoBehaviour
{
    public UnityEvent OnShow;
    public UnityEvent OnHide;

    void OnBecameVisible()
    {
        OnShow.Invoke();
    }

    void OnBecameInvisible()
    {
        OnHide.Invoke();
    }
}
