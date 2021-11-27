using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DayNightController : MonoBehaviour
{
    public float TimeAlpha = 0f;

    GameManager GM;

    int DaySeconds = 0;
    public int CurrentSeconds = 0;

    public Light Main;
    public float MorningIntensity;
    public float NightIntensity;
    public float RainingIntensity;

    float DayLightIntensity;

    public Color MorningAmbient;
    public Color NightAmbient;
    public Color RainingAmbient;

    Color DayAmbient;

    public AnimationCurve LightCurve;

    public AnimationCurve RainFadeCurve;
    public AnimationCurve SkyboxRainFadeCurve;

    public MeshRenderer Clouds;

    public void Init()
    {

        CurrentSeconds = 0;

        DaySeconds = GameServer.GS.GameManagerInstance.BedTime.Hours - GameServer.GS.GameManagerInstance.MorningTime.Hours;
        if(DaySeconds != 0)
            DaySeconds *= 60;
        else
            DaySeconds = GameServer.GS.GameManagerInstance.BedTime.Minutes;

        //Debug.Log(DaySeconds);
        SetRain(0f);
        DayNightUpdate();
    }

    void OnDisable()
    {
        CurrentSeconds = 0;

        DayLightIntensity = MorningIntensity;
        DayAmbient = MorningAmbient;
        RenderSettings.skybox.SetFloat("_RainLerp", 0f);

        DayNightUpdate();
    }

    public void DayNightUpdate()
    {

        TimeAlpha = (float) CurrentSeconds/(float) DaySeconds;
        if(TimeAlpha > 1f)
            TimeAlpha = 1f;

        Main.intensity = Mathf.Lerp(NightIntensity, DayLightIntensity, LightCurve.Evaluate(TimeAlpha));
        RenderSettings.ambientLight = Color.Lerp(NightAmbient, DayAmbient, LightCurve.Evaluate(TimeAlpha));
        RenderSettings.skybox.SetFloat("_DayNight", LightCurve.Evaluate(TimeAlpha));
        if(Clouds != null)
            Clouds.material.SetFloat("_DayNight", LightCurve.Evaluate(TimeAlpha));

        CurrentSeconds++;
        //Debug.Log(TimeAlpha);
    }

    public void SetRain(float alpha)
    {
        if(alpha > 1f)
            alpha = 1f;

        RenderSettings.skybox.SetFloat("_RainLerp", SkyboxRainFadeCurve.Evaluate(alpha));
        Clouds.material.SetFloat("_RainLerp", RainFadeCurve.Evaluate(alpha));
        DayLightIntensity = Mathf.Lerp(MorningIntensity, RainingIntensity, RainFadeCurve.Evaluate(alpha));
        DayAmbient = Color.Lerp(MorningAmbient, RainingAmbient, RainFadeCurve.Evaluate(alpha));   
    }
}
