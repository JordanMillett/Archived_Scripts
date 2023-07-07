using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SyncDelayed : MonoBehaviour
{
    public Transform Target;
    public float PosSpeed = 15f;
    public float RotSpeed = 15f;
    public float Factor = 1f;

    void FixedUpdate()
    {
        this.transform.position = Vector3.Lerp(this.transform.position, Target.position, Time.fixedDeltaTime * PosSpeed * Factor);
        this.transform.rotation = Quaternion.Lerp(this.transform.rotation, Target.rotation, Time.fixedDeltaTime * RotSpeed); //*Factor
    }
}