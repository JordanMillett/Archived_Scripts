using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosion : MonoBehaviour
{
    public float Radius;
    public float Speed;

    float Current_Rad = 0;

    void Update()
    {
            Current_Rad += Speed;

            transform.localScale = new Vector3(Current_Rad, Current_Rad, Current_Rad);

            if(Current_Rad > Radius)
                Destroy(this.transform.gameObject);

    }
}