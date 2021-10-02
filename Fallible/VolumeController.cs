using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.Rendering;

public class VolumeController : MonoBehaviour
{

    public AnimationCurve DamageCurve;

    IEnumerator DamageCoroutine;

    DepthOfField Blur;

    Vignette Blackout;

    void Awake()
    {
        Volume V = GetComponent<Volume>();
        
        if(V.profile.TryGet<Vignette>(out Vignette tmpBlack))
            Blackout = tmpBlack;
    }

    public void Damage()
    {
        DamageCoroutine = DamageLoop();
        StartCoroutine(DamageCoroutine);
    }

    IEnumerator DamageLoop()
    {
        float AnimationTime = 0.3f;
        float Timer = Time.time;
        float alpha = 0f;
        
        while(Time.time <= Timer + AnimationTime) //FADE IN
        {
            yield return null;
            alpha = (Time.time - Timer)/AnimationTime;

            Blackout.intensity.Override(DamageCurve.Evaluate(alpha));
        }
        
        Blackout.intensity.Override(0f);
    }
}