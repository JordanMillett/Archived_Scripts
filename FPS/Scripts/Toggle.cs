using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Toggle : MonoBehaviour
{
    public GameObject OnState;
    public GameObject OffState;

    public bool On = true;

    public void Change(bool Value)
    {

        On = Value;

        OnState.SetActive(On);
        OffState.SetActive(!On);

    }
}
