using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class PickupAble : MonoBehaviour
{
    public bool PickedUp = false;
    SpringJoint Connector;
    Rigidbody r;

    void Start()
    {
        r = GetComponent<Rigidbody>();
    }

    public void Trigger()
    {
        GameObject.FindWithTag("Player").GetComponent<Player>().ObjectPicked(this);
    }

    public void Pickup()       
    {
        Connector = this.gameObject.AddComponent(typeof(SpringJoint)) as SpringJoint;       //assign connector info
        Connector.connectedBody = GameObject.FindWithTag("Camera").GetComponent<Rigidbody>();
        Connector.autoConfigureConnectedAnchor = false;
        Connector.connectedAnchor = new Vector3(0f, 0, 0.8f);
        Connector.spring = 300f;
        Connector.damper = 0f;
        Connector.minDistance = 0.0f;
        Connector.maxDistance = 0.0f;
        Connector.enableCollision = true;

        r.drag = 7f;                                //change drag to make it work
        r.mass = 1f;
    }

    public void AuthorityGranted()
    {
        r = GetComponent<Rigidbody>();
        r.isKinematic = false;
        r.collisionDetectionMode = CollisionDetectionMode.Continuous;
    }

    public void AuthorityRevoked()
    {
        r = GetComponent<Rigidbody>();
        r.collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;
        r.isKinematic = true;
    }

    public void Drop()
    {
        Destroy(Connector);
        r.drag = 1f;
        r.mass = 4f;
    }
}
