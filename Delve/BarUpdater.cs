using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarUpdater : MonoBehaviour
{
    Bar[] Bars;

    void Start()
    {
        Bars = GetComponentsInChildren<Bar>();
    }

    void Update()
    {
        foreach(Bar B in Bars)
        {

            B.UpdateBar();

        }
    }

    public void UpdateValue(int Index, float Amount)
    {

        Bars[Index].Amount = Amount;

    }
}
