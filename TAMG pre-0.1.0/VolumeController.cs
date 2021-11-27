using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.Rendering;

public class VolumeController : MonoBehaviour
{

    DepthOfField Blur;

    Vignette Blackout;
    LensDistortion Distort;
    LiftGammaGain Darkness;

    void Awake()
    {
        Volume V = GetComponent<Volume>();

        if(V.profile.TryGet<DepthOfField>(out DepthOfField tmpBlur) )
            Blur = tmpBlur;
        
        if(V.profile.TryGet<Vignette>(out Vignette tmpBlack) )
            Blackout = tmpBlack;
        
        if(V.profile.TryGet<LensDistortion>(out LensDistortion tmpDistort) )
            Distort = tmpDistort;

        if(V.profile.TryGet<LiftGammaGain>(out LiftGammaGain tmpDark) )
            Darkness = tmpDark;

    }

    public void BackgroundBlur(bool Status)
    {
        //Blur.enabled.value = Status;
        //Blur.DepthOfFieldModeParameter = DepthOfFieldMode.Off;
        if(Status)
            Blur.mode.Override(DepthOfFieldMode.Gaussian);
        else
            Blur.mode.Override(DepthOfFieldMode.Off);
        //DepthOfField.mode.Off;
    }

    public void DeathFade(float alpha)
    {
        if(alpha == 0)
        {
            Blackout.intensity.Override(0f);
            Distort.intensity.Override(0f);
            Darkness.gain.Override(Vector4.zero);
        }else if(alpha == 1)
        {
            Blackout.intensity.Override(1f);
            Distort.intensity.Override(-1f);
            Darkness.gain.Override(new Vector4(0f,0f,0f,-1f));
        }else
        {
            Blackout.intensity.Override(Mathf.Lerp(0f, 1f, alpha));
            Distort.intensity.Override(Mathf.Lerp(0f, -1f, alpha));
            Darkness.gain.Override(new Vector4(0f,0f,0f, Mathf.Lerp(0f, -1f, alpha)));
        }
    }
}
