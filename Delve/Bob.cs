using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bob : MonoBehaviour
{
    public PlayerController PC;
    public AnimationCurve HeadbobCurve;
    float HeadbobAlpha = 0f;
    float HeadbobAmplitude = -.025f;
    float HeadbobOffset = 0f;
    float HeadbobTime = 0f;

    void Update()
    {
        if(!PC.Frozen)
        {
            Handbob(); //Headbob
        }
    }

    void Handbob()
    {

        if(HeadbobTime < 1f)
            HeadbobTime += 0.04f * PC.SpeedMult;
        else
            HeadbobTime = 0f;


        if(PC.Moving)
        {

            HeadbobOffset = Mathf.Lerp(HeadbobOffset, (1f - HeadbobCurve.Evaluate(HeadbobTime)) * HeadbobAmplitude * PC.SpeedMult, HeadbobAlpha);
            if(HeadbobAlpha < 1f)
                HeadbobAlpha += 0.04f;

        }else
        {

            HeadbobOffset = Mathf.Lerp(HeadbobOffset, 0f, 1f - HeadbobAlpha);
            if(HeadbobAlpha > 0f)
                HeadbobAlpha -= 0.04f;
        

        }

        transform.localPosition = new Vector3(0f, HeadbobOffset, 0f);

    }
}
