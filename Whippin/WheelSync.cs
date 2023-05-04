using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WheelSync : MonoBehaviour
{
    public Transform Sync;

    WheelCollider Wheel;

    void Start()
    {
        Wheel = GetComponent<WheelCollider>();
    }

    void Update()
    {
        Vector3 Pos = this.transform.position;
        Quaternion Rot = this.transform.rotation;

        Wheel.GetWorldPose(out Pos, out Rot);

        Sync.transform.position = Pos;
        Sync.transform.rotation = Rot;
    }
}
