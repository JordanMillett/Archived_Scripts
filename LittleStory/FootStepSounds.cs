using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FootStepSounds : MonoBehaviour
{
    public float DelayBetweenSteps = 0.5f;
    public float SprintingModifier = .75f;

    public List<AudioClip> StepSounds;

    PlayerController PC;
    AudioSource AS;

    float Timer = 0;
    float SprintMult;

    bool LastMoveState = false;
    bool LastGroundedState = false;
    
    void Start()
    {

        PC = GetComponent<PlayerController>();
        AS = GetComponent<AudioSource>();

    }

    void FixedUpdate()
    {
        
        if(LastGroundedState != PC.Grounded)
        {
            
            PlaySound();
            Timer = 0;

        }else if(PC.Moving)
        {

            if(PC.Sprinting)
                SprintMult = SprintingModifier;
            else
                SprintMult = 1f;

            Timer += Time.fixedDeltaTime;
            if(Timer >= (DelayBetweenSteps * SprintMult))
            {
                if(PC.Grounded)
                    PlaySound();
                Timer = 0;
            }

        }else if(LastMoveState)
        {
            if(PC.Grounded)
                PlaySound();
            Timer = 0;

        }else
        {
            Timer = 0;
        }

        LastMoveState = PC.Moving;
        LastGroundedState = PC.Grounded;

    }

    void PlaySound()
    {
        AS.clip = StepSounds[Random.Range(0, StepSounds.Count)];
        AS.Play();
    }

}
