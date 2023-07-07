using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Soundscape : MonoBehaviour
{
    public AudioClip Wind;

    AudioSource AS;

    float PitchLow = 1f;
    float PitchHigh = 1.3f;
    float MaxSpeed = 75f;
    float Alpha = 0f;

    Player P;

    Rigidbody Target;

    void Start()
    {
        AS = GetComponent<AudioSource>();
        AS.spatialBlend = 0f;
        AS.clip = Wind;
        AS.volume = 0f;
        AS.Play();
    }

    void Update()
    {
        if(!P)
        {
            try{
                P = GameObject.FindWithTag("Player").GetComponent<Player>();
            }
            catch{
                return;
            }
        }

        if(P.CurrentVehicle != null)
            Target = P.CurrentVehicle.GetComponent<Rigidbody>();
        else
            Target = P.GetComponent<Rigidbody>();

        Alpha = Target.velocity.magnitude/MaxSpeed;
        if(Alpha > 1f)
            Alpha = 1f;

        AS.pitch = Mathf.Lerp(PitchLow, PitchHigh, Alpha);
        Alpha = Mathf.Lerp(.05f, .5f, Alpha);
        AS.volume = (Alpha * (Settings._sfxVolume/100f)) * (Settings._masterVolume/100f);
        
    }
}
