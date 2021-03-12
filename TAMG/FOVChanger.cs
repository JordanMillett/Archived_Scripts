using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FOVChanger : MonoBehaviour
{
    public Rigidbody Player;
    Rigidbody r;

    MenuManager MM;

    public float PitchLow = 1f;
    public float PitchHigh = 1.3f;

    public float MaxSpeed = 20f;

    public float Alpha = 0f;

    Player PC;

    void Start()
    {
        MM = GameObject.FindWithTag("Camera").GetComponent<MenuManager>();
        PC = Player.GetComponent<Player>();
    }

    void Update()
    {
        if(PC.CurrentVehicle != null)
        {
            r = PC.CurrentVehicle.GetComponent<Rigidbody>();
        }else
        {
            r = Player;
        }

        Alpha = r.velocity.magnitude/300f;

        if(Alpha > 1f)
            Alpha = 1f;

        MM.fovOffset = Mathf.Lerp(0f, 40f, Alpha);
    }
}

/*
float speedAlpha = r.velocity.magnitude/200f;

        if(speedAlpha > 1f)
            speedAlpha = 1f;

        MM.fovOffset = Mathf.Lerp(0f, 25f, speedAlpha);
*/
