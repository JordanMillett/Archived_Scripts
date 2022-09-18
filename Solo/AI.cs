using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AI : MonoBehaviour
{
    void Update()
    {
        this.transform.Translate((this.transform.forward * 50f) * Time.deltaTime, Space.World);
    }
}
