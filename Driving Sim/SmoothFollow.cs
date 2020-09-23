using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmoothFollow : MonoBehaviour
{
    public GameObject Target;
    public float SmoothRotateSpeed = 10f;
    public float SmoothPositionSpeed = 10f;

    void FixedUpdate()
    {
        this.transform.position = Vector3.Slerp(this.transform.position, Target.transform.position, Time.fixedDeltaTime * SmoothPositionSpeed);
        this.transform.rotation = Quaternion.Slerp(this.transform.rotation, Target.transform.rotation, Time.fixedDeltaTime * SmoothRotateSpeed);
    }
}