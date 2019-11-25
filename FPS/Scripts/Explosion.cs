using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosion : MonoBehaviour
{
    public float Radius;
    public float Speed;

    float Current_Rad = 0;

    public bool DestroyAfterTime = false;

    void Start()
    {

        Despawn d = this.gameObject.AddComponent(typeof(Despawn)) as Despawn;
        d.DespawnTime = 10f;

    }

    void Update()
    {
        if(!DestroyAfterTime)
        {
            Current_Rad += Speed;

            transform.localScale = new Vector3(Current_Rad, Current_Rad, Current_Rad);

            if(Current_Rad > Radius)
                Destroy(this.transform.gameObject);
        }

    }
}
