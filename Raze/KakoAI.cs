using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KakoAI : MonoBehaviour
{
    public float Speed;
    public float RotationSpeed;

    public List<AudioClip> HurtSounds;
    public List<AudioClip> DeathSounds;

    AudioSource Source;

    Transform Player;
    Rigidbody r;

    Vector3 Target;
    Quaternion newRotation;

    public int Health = 3;

    void Start()
    {
        Source = GetComponent<AudioSource>();
        Player = GameObject.FindWithTag("Player").transform.GetChild(0).transform;
        r = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        Target = Player.transform.position - this.transform.position;
        newRotation = Quaternion.LookRotation(Target);
        this.transform.rotation = Quaternion.Slerp(this.transform.rotation, newRotation, Time.fixedDeltaTime * RotationSpeed);

        r.MovePosition(r.position + (transform.forward * Speed) * Time.fixedDeltaTime);

        if(Input.GetKeyDown("i"))
        {
            Source.clip = HurtSounds[0];
            Source.Play(); 
        }

        if(Input.GetKeyDown("o"))
        {
            Source.clip = DeathSounds[0];
            Source.Play(); 
        }
    }
}
