using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    VehicleController VC;

    void Start()
    {
        VC = GameObject.FindWithTag("Player").GetComponent<VehicleController>();
    }

    void Update()
    {
        this.transform.position = VC.transform.position;
        this.transform.rotation = Quaternion.Lerp(this.transform.rotation, VC.transform.rotation, Time.fixedDeltaTime);
    }
}
