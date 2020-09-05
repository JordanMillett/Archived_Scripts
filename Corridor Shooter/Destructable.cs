using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Destructable : MonoBehaviour
{
    public float Mass = 1f;
    float DespawnTime = 5f;
    bool Broken = false;

    public void Activate(Vector3 Direction, float Force)
    {
        if(!Broken)
        {
            Rigidbody R = this.gameObject.AddComponent(typeof(Rigidbody)) as Rigidbody;
            R.mass = Mass;
            R.AddForce(Direction * Force);
            Invoke("Despawn", DespawnTime);
        }
    }

    void Despawn()
    {
        Destroy(this.gameObject);
    }
}
