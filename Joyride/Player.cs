using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public float walkSpeed = 4f;
    public float movementForce = 5f;
 
    public Transform head;
    
    Rigidbody _rigidbody;
    SpringJoint _springJoint;

    float yaw = 0f;
    float pitch = 0f;
    float camLimits = 85f;

    void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _springJoint = GetComponent<SpringJoint>();
    }

    void FixedUpdate()
    {
        MoveControls();
    }

    void Update()
    {
        LookControls();
        
        if(Input.GetMouseButtonUp(0))
        {
            if (_springJoint)
            {
                Destroy(_springJoint);
            }
        }
        
        if(Input.GetMouseButtonDown(0))
        {
            RaycastHit hit;
            if(Physics.Raycast(head.transform.position, head.transform.forward, out hit, 5f, ~LayerMask.GetMask("Zone")))
            {
                if (hit.collider.attachedRigidbody)
                {
                    if (hit.collider.attachedRigidbody.gameObject.CompareTag("Moveable"))
                    {
                        _springJoint = hit.collider.attachedRigidbody.gameObject.AddComponent(typeof(SpringJoint)) as SpringJoint;
                        _springJoint.spring = 250f;
                        _springJoint.damper = 50f;
                        //_springJoint.anchor = hit.collider.transform.gameObject.GetComponent<Rigidbody>().centerOfMass;
                        _springJoint.anchor = hit.collider.attachedRigidbody.centerOfMass;
                        _springJoint.connectedBody = head.GetComponent<Rigidbody>();
                        //_springJoint.connectedAnchor = hit.collider.transform.gameObject.GetComponent<Rigidbody>().centerOfMass;
                        _springJoint.connectedAnchor = hit.point;
                    }

                    if (hit.collider.attachedRigidbody.gameObject.CompareTag("Interactable"))
                    {
                        hit.collider.attachedRigidbody.gameObject.GetComponent<MyButton>().Activate();
                    }
                }
            }
        }
        
        if(Input.GetMouseButtonDown(1))
        {
            RaycastHit hit;
            if(Physics.Raycast(head.transform.position, head.transform.forward, out hit, 5f, LayerMask.GetMask("Zone")))
            {
                try 
                {     
                    MyLever L = hit.collider.transform.gameObject.GetComponent<MyLever>();

                    if(L != null)
                        L.centering = true;

                }
                catch{}
            }
        }
        
        if(Input.mouseScrollDelta.y != 0f)
        {
            RaycastHit hit;
            if(Physics.Raycast(head.transform.position, head.transform.forward, out hit, 5f, LayerMask.GetMask("Zone")))
            {
                try 
                {     
                    MyLever L = hit.collider.transform.gameObject.GetComponent<MyLever>();

                    if(L != null)
                        L.Push(Input.mouseScrollDelta.y > 0);

                }
                catch{}
            }
        }
        
        
    }
    
    void LookControls()
    {
        float _mouseSensitivity = 100f;

        yaw += (_mouseSensitivity/100f) * Input.GetAxis("Mouse X");
        pitch -= (_mouseSensitivity/100f) * Input.GetAxis("Mouse Y");
        
        if(pitch >= camLimits)
            pitch = camLimits;
            
        if(pitch <= -camLimits)
            pitch = -camLimits;

        this.transform.localEulerAngles = new Vector3(0f, yaw, 0f);
        head.transform.localEulerAngles = new Vector3(pitch, 0f, 0f);
    }
    
    void MoveControls()
    {
        if(Input.GetKey("w") || Input.GetKey("a") || Input.GetKey("s") || Input.GetKey("d"))
        {
            Vector3 MoveDirection = Vector3.zero;
            
            if (Input.GetKey("w"))
                MoveDirection += this.transform.forward;

            if (Input.GetKey("a")) 
                MoveDirection += -this.transform.right * 0.75f;

            if (Input.GetKey("s")) 
                MoveDirection += -this.transform.forward;

            if (Input.GetKey("d")) 
                MoveDirection += this.transform.right * 0.75f;
            
            float lerp = Mathf.Lerp(1f, 0f, _rigidbody.velocity.magnitude/walkSpeed);
            _rigidbody.AddForce((MoveDirection * movementForce) * lerp);
        }
    }
}
