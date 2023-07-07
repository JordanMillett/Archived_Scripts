using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPC : MonoBehaviour
{
    public enum States
    {
        Idle,
        Chasing
    };
    
    float movementForce = 80f;
    float maxSpeed = 6f;
    
    public string Name = "";
    
    public States State;
    
    Rigidbody _rigidbody;
    
    void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();

        SetState(States.Idle);
        InvokeRepeating("ChangeBehavior", 2f, 2f);
    }
    
    void FixedUpdate()
    {
        switch(State)
        {
            case States.Idle :         S_Idle();     break;
            case States.Chasing :         S_Chasing();     break;
        }
    }
    
    public void ChangeBehavior()
    {
        SetState(GetState());
    }
    
    public void SetState(States S)
    {
        State = S;
        this.gameObject.name = "Enemy" + " - " + State.ToString().ToLower();
    }
    
    public void Die()
    {
        Destroy(this.gameObject);
    }
    
    public void Hurt()
    {
        
    }
    
    States GetState()
    {
        return States.Chasing;
    }
    
    void S_Chasing()
    {
        HeadTo(Player.Instance.transform.position);
    }
    
    void HeadTo(Vector3 Destination)
    {
        Vector3 TargetDirection = Destination - this.transform.position;
        Quaternion Look = Quaternion.LookRotation(TargetDirection, Vector3.up);
        this.transform.rotation = Quaternion.Lerp(this.transform.rotation, Look, Time.fixedDeltaTime * 3f);//speed
        this.transform.localRotation = new Quaternion(0f, this.transform.localRotation.y, 0f, this.transform.localRotation.w);
            
        float lerp = Mathf.Lerp(1f, 0f, _rigidbody.velocity.magnitude / maxSpeed);
        _rigidbody.AddForce((this.transform.forward * movementForce) * lerp, ForceMode.Acceleration);
    }
    
    void S_Idle()
    {
        
    }
}
