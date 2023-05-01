using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public struct FireData
{
    public bool FromViewModel;
    public Vector3 Destination;
    public bool HitPlayer;
    
    public FireData(bool fromViewmodel, Vector3 destination, bool hitPlayer)
    {
        FromViewModel = fromViewmodel;
        Destination = destination;
        HitPlayer = hitPlayer;
    }
}

public class Weapon : MonoBehaviour
{
    public static bool DEBUG_DRAW_Bullets = false;
    
    public Transform Model;

    public Transform FirePosition;

    public GameObject SmokeTrail;

    float MaxBackDistance = -0.4f;
    public float CurrentBackDistance = 0f;

    float MaxTurnDistance = 10f;
    float CurrentTurnDistance = 0f;
    Vector3 CurrentTurnOffset = Vector3.zero;

    public AudioSourceController ASC;

    public GameObject HitPlayer;
    public GameObject HitWall;

    float Accuracy = 98f;
    float RecoilMultiplier = 1f;
    float WeaponRange = 100f;

    float lastFired = 0f;

    void FixedUpdate()
    {
        UpdateModel();
    }

    void UpdateModel()
    {
        CurrentBackDistance = Mathf.Lerp(CurrentBackDistance, 0f, Time.fixedDeltaTime * 10f);   //Recovery
        Model.localPosition = Vector3.Lerp(new Vector3(0f, 0f, CurrentBackDistance), Vector3.zero, Time.fixedDeltaTime * 10f);

        CurrentTurnDistance = Mathf.Lerp(CurrentTurnDistance, 0f, Time.fixedDeltaTime * 10f);   //Recovery
        Model.transform.localEulerAngles = Vector3.Lerp(CurrentTurnOffset * CurrentTurnDistance, Vector3.zero, Time.fixedDeltaTime * 10f);
    }

    void Recoil()
    {
        ASC.Play();

        float maxBack = MaxBackDistance * RecoilMultiplier;
        float maxTurn = MaxTurnDistance * RecoilMultiplier;
        CurrentBackDistance = maxBack;
        CurrentTurnDistance = maxTurn;
        CurrentTurnOffset = new Vector3(Random.Range(-1f, 0f), Random.Range(-1f, 1f), Random.Range(-1f, 1f));
        UpdateModel();
    }
    
    public bool CanFire()
    {
        return Time.time > lastFired + 0.25f;
    }

    public FireData Fire(Transform Origin, uint shooter)
    {
        bool FromViewModel = true;
        if (Origin == null)
        {
            Origin = NetworkServer.spawned[shooter].GetComponent<Player>().Eyes;
            FromViewModel = false;
        }

        lastFired = Time.time;

        Vector3 AimVector = Random.insideUnitSphere.normalized * Random.Range(0f, 1f);
        AimVector *= 100f - Accuracy;
        AimVector /= 250f;
        AimVector += Origin.transform.forward;

        Vector3 point = Vector3.zero;
        RaycastHit hit;
        bool playerHit = false;
        if (Physics.Raycast(Origin.position, AimVector, out hit, WeaponRange, Game.WeaponMask))
        {
            point = hit.point;

            Player Target = hit.transform.gameObject.GetComponent<Player>();

            if(Target)
            {
                if (Target.netId != shooter) //dont shoot self!
                {
                    Player.localPlayer.CmdDamage(Target.netId, shooter);
                    playerHit = true;
                }
            }

        }else
        {
            point = Origin.transform.position + (AimVector * WeaponRange);
        }

       
        FireData data = new FireData(FromViewModel, point, playerHit);

        DrawSmokeLine(data);

        return data;
    }
    
    public void DrawSmokeLine(FireData data)
    {
        Recoil();

        GameObject Trail = GameObject.Instantiate(SmokeTrail, Vector3.zero, Quaternion.identity);
        Vector3[] Points = new Vector3[2];


        if(data.FromViewModel)
        {
            Ray ray = UIManager.instance.Cam.ViewportPointToRay(UIManager.instance.View.WorldToViewportPoint(FirePosition.position));
            Points[0] = ray.GetPoint(1f);
        }else
        {
            Points[0] = FirePosition.position;
        }

        Points[1] = data.Destination;

        LineRenderer LR = Trail.GetComponent<LineRenderer>();
        LR.positionCount = 2;
        LR.SetPositions(Points);
        LR.material.SetFloat("_StartTime", Time.time);
        
        if(data.HitPlayer)
        {
            GameObject.Instantiate(HitPlayer, data.Destination, Quaternion.identity);
        }else
        {
            GameObject.Instantiate(HitWall, data.Destination, Quaternion.identity);
        }
    }
}