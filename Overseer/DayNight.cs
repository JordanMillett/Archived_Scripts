using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DayNight : MonoBehaviour
{
    public float Speed = 1f;

    void Update()
    {
        this.transform.localEulerAngles += new Vector3(0f, Speed * Time.deltaTime, 0f);
    }
}
