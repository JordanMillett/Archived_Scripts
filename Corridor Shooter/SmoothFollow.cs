using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmoothFollow : MonoBehaviour
{
    public GameObject Target;
    public float SmoothCamSpeed = 10f;

    void LateUpdate()
    {
        this.transform.position = Target.transform.position;
        this.transform.rotation = Quaternion.Slerp(this.transform.rotation, Target.transform.rotation, Time.fixedDeltaTime * SmoothCamSpeed);
    }
}
