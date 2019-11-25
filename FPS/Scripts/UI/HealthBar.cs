using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthBar : MonoBehaviour
{
    Simple_Bar Bar;
    public LifeManager LM;

    void Start()
    {

        Bar = GetComponent<Simple_Bar>();

    }

    void Update()
    {
        Bar.Current = LM.Health;
    }
}
