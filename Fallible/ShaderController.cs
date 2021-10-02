using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShaderController : MonoBehaviour
{
    public AnimationCurve AC;

    MeshRenderer MR;

    IEnumerator DamageCoroutine;

    void Start()
    {
        MR = GetComponent<MeshRenderer>();
    }

    public void Damage()
    {
        DamageCoroutine = DamageLoop();
        StartCoroutine(DamageCoroutine);
    }

    IEnumerator DamageLoop()
    {

        //float T = Time.time;

        float AnimationFrameRate = Application.targetFrameRate;
        float AnimationTime = 0.2f;

        float Timer = Time.time;
        float alpha = 0f;
        
        while(Time.time <= Timer + (AnimationTime/2f)) //FADE IN
        {
            yield return null;
            alpha = (Time.time - Timer)/(AnimationTime/2f);

            for(int i = 0; i < MR.materials.Length; i++)
                MR.materials[i].SetFloat("_Damage", AC.Evaluate(alpha));
        }

        alpha = 0f;
        Timer = Time.time;
        while(Time.time <= Timer + (AnimationTime/2f)) //FADE IN
        {
            yield return null;
            alpha = (Time.time - Timer)/(AnimationTime/2f);

            for(int i = 0; i < MR.materials.Length; i++)
                MR.materials[i].SetFloat("_Damage", AC.Evaluate(1f - alpha));
        }

        //Debug.Log(Time.time - T);
    }
}