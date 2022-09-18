using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Train : MonoBehaviour
{
    public Health Engine;
    public Gun ActiveGun;
    public Player P;
    public float TrainMaxSpeed = 10f;
    
    void FixedUpdate()
    {
        this.transform.Rotate(0f, (TrainMaxSpeed * (Engine.HP/100f)) * Time.fixedDeltaTime, 0f);
    }
}
