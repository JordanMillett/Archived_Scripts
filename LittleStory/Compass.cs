using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Compass : MonoBehaviour
{

    Transform Needle;

    void Start()
    {
        Needle = this.transform.GetChild(0).transform;
    }

    void Update()
    {
        Needle.transform.rotation = Quaternion.LookRotation(new Vector3(.5f, 0f, .5f), Vector3.up);
        Needle.transform.localEulerAngles = new Vector3(-90f, Needle.transform.localEulerAngles.y, 0f);
    }
}
