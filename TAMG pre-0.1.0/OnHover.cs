using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;

public class OnHover : MonoBehaviour, IPointerEnterHandler
{
    public UnityEvent Event;

    public void OnPointerEnter(PointerEventData eventData)
    {
        Event.Invoke();
    }
}