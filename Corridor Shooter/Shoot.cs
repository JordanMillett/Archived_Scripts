using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shoot : MonoBehaviour
{
    public Vector2 StartingAngles;
    public Vector2 EndingAngles;

    public Color StartingColor;
    public Color EndingColor;

    public float FireAlpha = 0f;
    public float FireSpeed = 0.05f;

    public float StartingIntensity;
    public float EndingIntensity;

    public float RechargeTime = 2f;
    public float FadeBackTime = 0.5f;
    public bool Charging = false;

    public GameObject Projectile;
    public float FireForce;

    Light SL;

    void Start()
    {
        SL = GetComponent<Light>();
    }

    void Update()
    {
        if(Input.GetMouseButton(0) && !Charging && !Input.GetMouseButton(1))
        {

            if(FireAlpha < 1f)
                FireAlpha += FireSpeed;

            if(FireAlpha >= 1f)
                Fire();
        }else
        {
            if(FireAlpha > 0f)
                FireAlpha -= FireSpeed;

            if(FireAlpha <= 0f)
                FireAlpha = 0f;
        }

        if(!Charging)
        {
            if(Input.GetMouseButton(1))
                SL.intensity = 0f;
            else
                SL.intensity = Mathf.Lerp(StartingIntensity, EndingIntensity, FireAlpha);

            SL.color = Color.Lerp(StartingColor, EndingColor, FireAlpha);
            SL.innerSpotAngle = Mathf.Lerp(StartingAngles.x, EndingAngles.x, FireAlpha);
            SL.spotAngle = Mathf.Lerp(StartingAngles.y, EndingAngles.y, FireAlpha);
        }
    }

    void Fire()
    {
        if(!Charging)
        {
            Charging = true;
            
            SL.intensity = 0f;
            SL.color = StartingColor;
            SL.innerSpotAngle = StartingAngles.x;
            SL.spotAngle = StartingAngles.y;

            StartCoroutine(Recharge());

            GameObject P = Instantiate(Projectile, this.transform.position + (this.transform.forward * 1.5f), Quaternion.identity);
            P.GetComponent<Rigidbody>().AddForce(this.transform.forward * FireForce);

        } 
    }

    IEnumerator Recharge()
    {
        yield return new WaitForSeconds(RechargeTime);

        float Alpha = 0f;
        float Increment = 60f;

        for(int i = 0; i < Mathf.RoundToInt(Increment); i++)
        {
            Alpha = i/Increment;
            
            if(Input.GetMouseButton(1))
                SL.intensity = 0f;
            else
                SL.intensity = Mathf.Lerp(0f, StartingIntensity, Alpha);

            yield return new WaitForSeconds(FadeBackTime/Increment);
        }

        Charging = false;
    }
}
