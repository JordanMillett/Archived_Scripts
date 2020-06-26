using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunRecoil : MonoBehaviour
{

    public float LerpAlpha = 0f;
    float InitialOffset = 0f;

    public bool CanShoot = true;

    Shoot S;

    void Start()
    {
        S = GetComponent<Shoot>();
        InitialOffset = transform.localPosition.z;
    }

    void FixedUpdate()
    {
        if(LerpAlpha > 0f)
        {
            LerpAlpha -= S.GI.RecoilRecoverySpeed;
        }else
        {
            CanShoot = true;
            LerpAlpha = 0f;
        }

        transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, InitialOffset + Mathf.Lerp(0f, S.GI.RecoilBackAmount, LerpAlpha));
    }
}
