using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class WorldEvent : MonoBehaviour
{
    public UnityEvent NewDay;

    public void onNewDay()
    {
        NewDay.Invoke();
    }
}
