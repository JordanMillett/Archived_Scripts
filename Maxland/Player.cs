using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public static bool DEBUG_DRAW_CanJump = false;

    public Transform Head;
    public Camera _camera;

    public float MovementForce = 50f;
    public float WalkSpeed = 6f;
    public float JumpForce = 250f;

    Rigidbody _rigidbody;
    
    float camLimits = 85f;
    float yaw = 0f;
    float pitch = 0f;

    float lastJump = -100f;
    float jumpRate = 0.25f;

    public Weapon W;

    void Start()
    {
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = 60;
        
        _rigidbody = GetComponent<Rigidbody>();

        W.RayFrom = _camera.transform;
    }

    void Update()
    {
        CameraControls();
        MovementControls();
        
        if(Input.GetMouseButton(0))
            W.PullTrigger();
    }
    
    void MovementControls()
    {
        if (Grounded())
        {
            _rigidbody.drag = 2f;
            
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
            
            float lerp = Mathf.Lerp(1f, 0f, _rigidbody.velocity.magnitude / WalkSpeed);
            _rigidbody.AddForce((MoveDirection * MovementForce) * lerp, ForceMode.Acceleration);
            
            if (Input.GetKeyDown("space"))
            {
                if (Time.time > lastJump + jumpRate)
                {
                    lastJump = Time.time;
                    //if (MoveDirection == Vector3.zero)
                        _rigidbody.AddForce(Vector3.up * JumpForce, ForceMode.Acceleration);
                    //else
                       // _rigidbody.AddForce((Vector3.up + Vector3.up + MoveDirection / 3f) * (JumpForce / 2f), ForceMode.Acceleration);
                }
            }
        }else
        {
            _rigidbody.drag = 0f;
        }
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
        Head.localRotation = Quaternion.Euler(pitch, 0f, 0f);
    }
    
    void OnDrawGizmos()
    {
        if (DEBUG_DRAW_CanJump)
        {
            if (Grounded())
                Gizmos.color = new Color(0f, 1f, 0f, 0.5f);
            else
                Gizmos.color = new Color(1f, 0f, 0f, 0.5f);

            Gizmos.DrawSphere(transform.position + new Vector3(0f, 0.2f, 0f), 0.3f);
        }
    }
    
    bool Grounded()
    {
        return Physics.CheckSphere(transform.position + new Vector3(0f, 0.2f, 0f), 0.3f, ~LayerMask.GetMask("Player"));

        //RaycastHit hit;
        //return Physics.Raycast(this.transform.position + new Vector3(0f, 0.4f, 0f), -Vector3.up, out hit, 0.5f);
    }
}
