using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour
{
    public GameObject Decal;
    public GameObject MuzzleFlash;

    public float Accuracy;

    public float RecoilAmount;
    public float BackAmount = 0f;
    public float BackAlpha = 0f;
    public float MaxBackAmount = 0f;
    public float AngleMagnitude = 1f;

    public float Cooldown;

    bool Recharging = false;

    Transform FirePosition;

    ADS A;

    void Start()
    {
        FirePosition = transform.GetChild(1).transform;
        A = GetComponent<ADS>();
    }

    void Update()
    {
        //BackAmount = OverlapHit();
        
        if(!Recharging)
        {
            A.PositionOffset = new Vector3(0f, 0f, BackAmount);
        }

        if(Input.GetMouseButton(0) && !Recharging && !A.Sprinting)
        {
            Fire();
        }
    }

    void Fire()
    {
        Recharging = true;
        StartCoroutine(Recharge());

        GameObject Flash = Instantiate(MuzzleFlash, FirePosition.transform.position, Quaternion.identity);

        RaycastHit hit;

        float Inaccuracy = 100f - Accuracy;
        Vector3 OffsetVector = Random.insideUnitSphere.normalized * Random.Range(0f, 1f);
        OffsetVector *= Inaccuracy;
        OffsetVector = OffsetVector / 200f;

        if(Physics.Raycast(FirePosition.transform.position, FirePosition.transform.forward + OffsetVector, out hit, 100f))
        {
            
            if(hit.transform.GetComponent<LifeManager>() != null)
                Destroy(hit.transform.gameObject);

            MakeDecal(hit.point, hit.normal);

        }else
        {
            MakeDecal(FirePosition.transform.position + ((FirePosition.transform.forward + OffsetVector) * 100f), Vector3.zero);
        }
    }

    void MakeDecal(Vector3 D, Vector3 N)
    {
        GameObject Dec;

        if(N == Vector3.zero)
        {
            Dec = Instantiate(Decal, D, Quaternion.identity);
        }else
        {
            Dec = Instantiate(Decal, D, Quaternion.LookRotation(N, Vector3.up));
        }

        

        Vector3[] Pos = new Vector3[2];

        Pos[0] = FirePosition.transform.position;
        Pos[1] = D;

        Dec.GetComponent<LineRenderer>().SetPositions(Pos);
    }

    IEnumerator Recharge()
    {
        float Alpha = 0f;
        float Increment = 60f * Cooldown;

        A.PositionOffset = new Vector3(0f, 0f, -RecoilAmount + BackAmount);

        Vector3 RanRot = new Vector3
        (
            Random.Range(-1f,1f) * AngleMagnitude,
            Random.Range(-1f,1f) * AngleMagnitude,
            Random.Range(-1f,1f) * AngleMagnitude
        );

        A.AnglesOffset = RanRot;

        for(int i = 0; i < Mathf.RoundToInt(Increment); i++)
        {
            Alpha = i/Increment;
            
            A.PositionOffset = Vector3.Lerp(new Vector3(0f, 0f, -RecoilAmount + BackAmount), Vector3.zero, Alpha);
            A.AnglesOffset = Vector3.Lerp(RanRot, Vector3.zero, Alpha);
            yield return new WaitForSeconds(Cooldown/Increment);
        }

        A.PositionOffset = Vector3.zero;
        A.AnglesOffset = Vector3.zero;

        Recharging = false;
    }

    float OverlapHit()
    {
        //Mathf.Lerp(0f, MaxBackAmount, BackAlpha);

        RaycastHit hit;

        if(Physics.Raycast(FirePosition.transform.position + new Vector3(0f, 0f, BackAmount), FirePosition.transform.forward, out hit, 1f))
        {
            return MaxBackAmount;
        }else
        {
            return 0f;
        }
    }
}
