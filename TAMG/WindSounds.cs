using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindSounds : MonoBehaviour
{
    public Rigidbody Player;
    Rigidbody r;

    AudioSource AS;

    public float PitchLow = 1f;
    public float PitchHigh = 1.3f;

    public float MaxSpeed = 20f;

    public float Alpha = 0f;

    Player PC;

    void Start()
    {
        AS = GetComponent<AudioSource>();
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

        Alpha = r.velocity.magnitude/MaxSpeed;

        if(Alpha > 1f)
            Alpha = 1f;

        AS.pitch = Mathf.Lerp(PitchLow, PitchHigh, Alpha);

        Alpha = Mathf.Lerp(.01f, .6f, Alpha);

        AS.volume = (Alpha * (Settings._sfxVolume/100f)) * (Settings._masterVolume/100f);
    }
}

/*
float speedAlpha = r.velocity.magnitude/200f;

        if(speedAlpha > 1f)
            speedAlpha = 1f;

        MM.fovOffset = Mathf.Lerp(0f, 25f, speedAlpha);
*/
