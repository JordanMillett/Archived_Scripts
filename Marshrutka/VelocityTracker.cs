using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VelocityTracker : MonoBehaviour
{
    public float Velocity;

    Vector3 lastPosition;

    void Start()
    {
        lastPosition = this.transform.position;
    }

    void FixedUpdate()
    {
        Velocity = (this.transform.position - lastPosition).magnitude/Time.fixedDeltaTime;
        lastPosition = this.transform.position;
    }
}
