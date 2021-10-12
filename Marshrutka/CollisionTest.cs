using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionTest : MonoBehaviour
{
    void OnCollisionEnter(Collision col)
    {
        Debug.Log(col.impulse.magnitude + " " + this.gameObject.name);
        Vector3 Dir = col.contacts[0].point - transform.position;
        Dir = Dir.normalized;
        GetComponent<FixedJoint>().connectedBody.gameObject.transform.position -= Dir;
    }
}
