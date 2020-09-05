using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ADS : MonoBehaviour
{
    public Vector3 StartingPosition;
    public Vector3 ADSPosition;

    public Vector3 PositionOffset;
    public Vector3 AnglesOffset;

    public Vector3 SprintingPosition;
    public Vector3 SprintingAngles;

    public float ADSFov;
    public float SprintingFov;

    public float ADSSpeed;
    public float SprintingSpeed;

    public bool Sprinting = false;

    float StartingFov;
    float ADSAlpha = 0f;
    float SprintingAlpha = 0f;

    Camera Cam;

    void Start()
    {

        Cam = GameObject.FindWithTag("Camera").gameObject.GetComponent<Camera>();
        StartingFov = Cam.fieldOfView;

    }

    void Update()
    {
        if(SprintingAlpha > 0f)
            Sprinting = true;
        else
            Sprinting = false;

        if(Input.GetMouseButton(1) && !Sprinting)
        {
            if(ADSAlpha < 1f)
                ADSAlpha += ADSSpeed;

            if(ADSAlpha >= 1f)
                ADSAlpha = 1f;
        }else
        {
            if(ADSAlpha > 0f)
                ADSAlpha -= ADSSpeed;

            if(ADSAlpha <= 0f)
                 ADSAlpha = 0f;
        }
    
        if(Input.GetKey(KeyCode.LeftShift) && ADSAlpha == 0f)
        {
            if(SprintingAlpha < 1f)
                SprintingAlpha += SprintingSpeed;

            if(SprintingAlpha >= 1f)
                SprintingAlpha = 1f;
        }else
        {
            if(SprintingAlpha > 0f)
                SprintingAlpha -= SprintingSpeed;

            if(SprintingAlpha <= 0f)
                SprintingAlpha = 0f;
        }

        if(!Sprinting)
        {
            this.transform.localPosition = Vector3.Lerp(StartingPosition, ADSPosition, ADSAlpha) + PositionOffset;
            Cam.fieldOfView = Mathf.Lerp(StartingFov, ADSFov, ADSAlpha);
            this.transform.localEulerAngles = Vector3.zero + AnglesOffset;
        }else
        {
            this.transform.localPosition = Vector3.Lerp(StartingPosition, SprintingPosition, SprintingAlpha);
            Cam.fieldOfView = Mathf.Lerp(StartingFov, SprintingFov, SprintingAlpha);
            this.transform.localEulerAngles = Vector3.Lerp(Vector3.zero, SprintingAngles, SprintingAlpha);
        }


    }
}
