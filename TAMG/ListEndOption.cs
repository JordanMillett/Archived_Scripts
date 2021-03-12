using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ListEndOption : MonoBehaviour
{
    PartMenu PM;

    public void Init(PartMenu _PM)
    {
        PM = _PM;
    }

    public void Activate()
    {
        PM.Back();
    }
}
