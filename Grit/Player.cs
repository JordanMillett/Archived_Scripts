using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public Transform Eyes;
    
    float movementForce = 60f;
    float maxSpeed = 5f;
    float yaw = 0f;
    float pitch = 0f;

    float camLimits = 80f;
    
    Rigidbody _rigidbody;

    Transform _camera;

    void Start()
    {
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = 60;
        
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        
        _rigidbody = GetComponent<Rigidbody>();
        _camera = GameObject.FindWithTag("Camera").transform;
    }
    
    void Update()
    {
        _camera.position = Eyes.transform.position;
        _camera.rotation = Eyes.transform.rotation;
        
        CameraControls();
    }
    
    void FixedUpdate()
    {
        MovementControls();
    }
    
    void MovementControls()
    {
         Vector3 MoveDirection = Vector3.zero;
        if (Input.GetKey("w") || Input.GetKey("a") || Input.GetKey("s") || Input.GetKey("d"))
        {
            if (Input.GetKey("w"))
                MoveDirection += this.transform.forward;

            if (Input.GetKey("a"))
                MoveDirection += -this.transform.right;

            if (Input.GetKey("s"))
                MoveDirection += -this.transform.forward;

            if (Input.GetKey("d"))
                MoveDirection += this.transform.right;
        }

        float lerp = Mathf.Lerp(1f, 0f, _rigidbody.velocity.magnitude / maxSpeed);
        _rigidbody.AddForce((MoveDirection * movementForce) * lerp, ForceMode.Acceleration);
    }
    
    void CameraControls()
    {
        int _mouseSensitivity = 100; //1.00

        yaw += (_mouseSensitivity/100f) * Input.GetAxis("Mouse X");
        pitch -= (_mouseSensitivity/100f) * Input.GetAxis("Mouse Y");
        
        if(pitch >= camLimits)
            pitch = camLimits;
            
        if(pitch <= -camLimits)
            pitch = -camLimits;

        this.transform.localRotation = Quaternion.Euler(0f, yaw, 0f);
        Eyes.localRotation = Quaternion.Euler(pitch, 0f, 0f);
    }
}
