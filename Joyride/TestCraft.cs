/*using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestCraft : MonoBehaviour
{
    public Lever throttle;
    public Lever turn;

    Rigidbody _rigidbody;

    void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
    }
    
    void FixedUpdate()
    {
        _rigidbody.MovePosition(this.transform.position + (this.transform.forward * (throttle.alpha/3f)));
        
        Quaternion deltaRotation = Quaternion.Euler(new Vector3(0f, Mathf.Lerp(-50f, 50f, turn.alpha), 0f) * Time.fixedDeltaTime);
        _rigidbody.MoveRotation(_rigidbody.rotation * deltaRotation);
    }
}
*/