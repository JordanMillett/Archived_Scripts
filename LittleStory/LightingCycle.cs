using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;
using UnityEngine.Experimental.VFX;

public class LightingCycle : MonoBehaviour
{
    public Light Sun;
    public Light Moon;
    public MeshRenderer Stars;

    public float TimeScale = 1f;
    public float TimingFactor = 0.01f;

    public float MaxSunIntensity = 1f;
    public float MaxMoonIntensity = 0.5f;

    public float FadeoutDegrees = 15f;

    public AnimationCurve StarBrightness;

    public Material Skybox;

    public Color AmbientDay;
    public Color AmbientNight;

    float CurrentZ;
    float StarAlpha = 0f;

    float MaxSunSize = 0.04f;

    //public VisualEffectAsset TreeLeavesAsset;
    //public VisualEffect TreeLeaves;

    public int Day = 1;
    public int Hours = 0;
    public int Minutes = 0;

    void Start()
    {
        CurrentZ = this.transform.eulerAngles.z;
        //TreeLeaves = TreeLeavesAsset.GetComponent<VisualEffect>();
    }

    void FixedUpdate()
    {
        UpdateTime(CurrentZ);

        if(CurrentZ > 540f) //Stop the number from rising forever/One day passed
        {
            CurrentZ = 180f;
            Day++;
        }

        CurrentZ += TimingFactor * TimeScale;       //Move the sun to progress the day
        this.transform.eulerAngles = new Vector3(0f, -45f, CurrentZ);

        if((CurrentZ > (180 + FadeoutDegrees)) && (CurrentZ < (360f - FadeoutDegrees)))         //FULL SUN
        {
            Sun.intensity = MaxSunIntensity;
            Moon.intensity = 0f;
            Sun.shadowStrength = 1f;
            Moon.shadowStrength = 0f;
            Skybox.SetFloat("_SunSize", MaxSunSize);
            RenderSettings.ambientLight = AmbientDay;
            //TreeLeaves.SetFloat("GradientTime", 1f);
        }else if((CurrentZ > (360f + FadeoutDegrees)) && (CurrentZ < 540f - FadeoutDegrees))          //FULL MOON
        {
            
            Sun.intensity = 0f;
            Moon.intensity = MaxMoonIntensity;
            Sun.shadowStrength = 0f;
            Moon.shadowStrength = 1f;
            Skybox.SetFloat("_SunSize", 0f);
            RenderSettings.ambientLight = AmbientNight;
            //TreeLeaves.SetFloat("GradientTime", 0f);
        }else if((CurrentZ > (360f - FadeoutDegrees)) && (CurrentZ < 360f))                           //SUN FADE OUT
        {
            Sun.intensity =             Mathf.Lerp(MaxSunIntensity, 0f,     1f - ((360f - CurrentZ)/FadeoutDegrees));
            Sun.shadowStrength =        Mathf.Lerp(1f, 0f,                  1f - ((360f - CurrentZ)/FadeoutDegrees));
            Skybox.SetFloat("_SunSize", Mathf.Lerp(MaxSunSize, 0f,          1f - ((360f - CurrentZ)/FadeoutDegrees)));
            RenderSettings.ambientLight = Color.Lerp(AmbientDay, AmbientNight, 1f - ((360f - CurrentZ)/FadeoutDegrees));
        }else if(CurrentZ < (180f + FadeoutDegrees))                                                   //SUN FADE IN
        {
            Sun.intensity =             Mathf.Lerp(0f, MaxSunIntensity,     (CurrentZ - 180f)/FadeoutDegrees);
            Sun.shadowStrength =        Mathf.Lerp(0f, 1f,                  (CurrentZ - 180f)/FadeoutDegrees);
            Skybox.SetFloat("_SunSize", Mathf.Lerp(0f, MaxSunSize,          (CurrentZ - 180f)/FadeoutDegrees));
            Moon.intensity = 0f;
            RenderSettings.ambientLight = Color.Lerp(AmbientNight, AmbientDay, (CurrentZ - 180f)/FadeoutDegrees);
        }else if((CurrentZ > 360f) && (CurrentZ < (360f + FadeoutDegrees)))                            //MOON FADE IN
        {
            Moon.intensity =            Mathf.Lerp(0f, MaxMoonIntensity,    ((CurrentZ - 360f)/FadeoutDegrees));
            Moon.shadowStrength =       Mathf.Lerp(0f, 1f,                  ((CurrentZ - 360f)/FadeoutDegrees));
            Sun.intensity = 0f;
        }else if((CurrentZ > (540f - FadeoutDegrees)) && (CurrentZ < 540f))                               //MOON FADE OUT
        {
            Moon.intensity =            Mathf.Lerp(MaxMoonIntensity, 0f,    1f - ((540f - CurrentZ)/FadeoutDegrees));
            Moon.shadowStrength =       Mathf.Lerp(1f, 0f,                  1f - ((540f - CurrentZ)/FadeoutDegrees));
        }

        /*if(CurrentZ > 360f)
        {
            Sun.shadows = LightShadows.None;
            Moon.shadows = LightShadows.Hard;
        }else
        {
            Sun.shadows = LightShadows.Hard;
            Moon.shadows = LightShadows.None;
        }*/

        if((CurrentZ > 360f - FadeoutDegrees)) //STARS FADE
        { 
            StarAlpha = (CurrentZ - 360f)/180f;
            Stars.material.SetColor("BaseColor", new Color(1f, 1f, 1f, StarBrightness.Evaluate(StarAlpha)));
        }
       
    }

    void UpdateTime(float Degrees)
    {

        float Fraction = (Degrees - 180f)/360f;

        Hours = (int)Mathf.Floor(24f * Fraction);

        Minutes = (int)Mathf.Floor(Mathf.Repeat(24f * Fraction, 1f) * 60f);    

    }
}
