using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraRig : MonoBehaviour
{
    public Car Target;
    float PosSpeed = 3f;
    float RotSpeed = 10f;

    public Camera Cam;
    public Transform Spin;

    float yaw = 0f;
    float pitch = 0f;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void FixedUpdate()
    {
        float alpha = PosSpeed * Mathf.Max(Target.MPH, 1f);

        this.transform.position = Vector3.Lerp(this.transform.position, Target.CenterOfMass.position, Time.fixedDeltaTime * alpha);
        this.transform.rotation = Quaternion.Lerp(this.transform.rotation, Target.CenterOfMass.rotation, Time.fixedDeltaTime * RotSpeed);
    }
    
    void Update()
    {
        float alpha = Mathf.Clamp(Target.MPH, 0f, 60f) / 60f;

        Cam.fieldOfView = Mathf.Lerp(55f, 65f, alpha);

        yaw += Input.GetAxis("Mouse X");
        pitch -= Input.GetAxis("Mouse Y");

        yaw = Mathf.Clamp(yaw, -170f, 170f);
        pitch = Mathf.Clamp(pitch, -20f, 85f);

        Spin.transform.localEulerAngles = new Vector3(pitch, yaw, 0f);
    }
}