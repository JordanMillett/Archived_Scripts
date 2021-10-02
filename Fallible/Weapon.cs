using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Weapon : MonoBehaviour
{
    public Health.Team Side = Health.Team.Hostile;
    public WeaponConfig WC;
    public List<AudioClip> FireSounds;
    public AudioSource AS;

    public Player P;
    public bool PlayerHeld = false;
    public GameObject ProjectilePrefab;
    float ProjectileVelocity = 150f;
    public Transform FirePosition;
    public Vector3 HoldPosition;
    public Vector3 ADSPosition;
    public float ADSFOV = 60f;
    
    float MaxBackDistance = -0.075f;
    float CurrentBackDistance = 0f;

    float MaxTurnDistance = 2f;
    float CurrentTurnDistance = 0f;
    Vector3 CurrentTurnOffset = Vector3.zero;
    
    public bool Aimed = false;

    IEnumerator CooldownRoutine;
    bool OnCooldown = false;

    void Start()
    {
        AS = FirePosition.GetComponent<AudioSource>();
    }

    public void Fire()
    {
        if(!OnCooldown)
        {   
            AS.clip = FireSounds[UnityEngine.Random.Range(0, FireSounds.Count)];
            AS.volume = 0.4f;
            AS.pitch = WC.FirePitch;
            AS.Play();

            for(int i = 0; i < WC.FireCount; i++)
                Shoot();

            CurrentBackDistance = MaxBackDistance;
            CurrentTurnDistance = MaxTurnDistance;

            CurrentTurnOffset = new Vector3(UnityEngine.Random.Range(-1f, 1f), UnityEngine.Random.Range(-1f, 1f), UnityEngine.Random.Range(-1f, 1f));

            OnCooldown = true;
            CooldownRoutine = Cooldown();
            StartCoroutine(CooldownRoutine);
        }
    }

    void FixedUpdate()
    {
        Vector3 Goal;
        if(PlayerHeld)
            Goal = Aimed ? ADSPosition : HoldPosition;
        else
            Goal = Vector3.zero;

        CurrentBackDistance = Mathf.Lerp(CurrentBackDistance, 0f, Time.fixedDeltaTime * 10f);   //Recovery
        this.transform.localPosition = Vector3.Lerp(this.transform.localPosition + new Vector3(0f, 0f, CurrentBackDistance), Goal, Time.fixedDeltaTime * 10f);  //Sync

        CurrentTurnDistance = Mathf.Lerp(CurrentTurnDistance, 0f, Time.fixedDeltaTime * 10f);   //Recovery
        this.transform.localEulerAngles = Vector3.Lerp(CurrentTurnOffset * CurrentTurnDistance, Vector3.zero, Time.fixedDeltaTime * 10f);
    }

    void Shoot()
    {
        RaycastHit hit;

        Vector3 AimVector = UnityEngine.Random.insideUnitSphere.normalized * UnityEngine.Random.Range(0f, 1f);
        AimVector *= 100f - WC.Accuracy;
        AimVector /= 250f; //get it in range

        AimVector += FirePosition.transform.forward;

        Color HitColor = Color.black;

        if(Physics.Raycast(FirePosition.position, AimVector, out hit, WC.MaxRange))
        {
            Debug.DrawRay(FirePosition.position, AimVector * hit.distance, Color.Lerp(Color.red, Color.green, WC.DamageFalloff.Evaluate(hit.distance/WC.MaxRange)), 1f);

            Health H = hit.collider.transform.root.gameObject.GetComponent<Health>();
            
            GameObject Fired = Instantiate(ProjectilePrefab, FirePosition.position, Quaternion.LookRotation(AimVector));
            Fired.transform.localScale = new Vector3(WC.ProjectileScale, WC.ProjectileScale, 1f);

            if(H != null)
            {
                if(H.Side != Side)
                {
                    Fired.GetComponent<Projectile>().Target = H;
                    Fired.GetComponent<Projectile>().Damage = Mathf.RoundToInt(WC.MaxDamage * WC.DamageFalloff.Evaluate(hit.distance/WC.MaxRange));
                    //H.TakeDamage(Mathf.RoundToInt(WC.MaxDamage * WC.DamageFalloff.Evaluate(hit.distance/WC.MaxRange)));
                    HitColor = H.HitMaterial.GetColor("_Color");
                }
            }else
            {
                HitColor = hit.collider.transform.GetComponent<MeshRenderer>().material.GetColor("_Color");
            }

            
            Fired.GetComponent<Projectile>().Travel(hit.point, hit.normal, ProjectileVelocity, true, HitColor);//H.SC.GetComponent<MeshRenderer>().material.GetColor("_Color")
           
        }else
        {
            GameObject Fired = Instantiate(ProjectilePrefab, FirePosition.position, Quaternion.LookRotation(AimVector));
            Fired.GetComponent<Projectile>().Travel(FirePosition.position + (AimVector * WC.MaxRange), Vector3.zero, ProjectileVelocity, false, HitColor);

            Debug.DrawRay(FirePosition.position, AimVector * WC.MaxRange, Color.red, 1f);
        }
    }

    IEnumerator Cooldown()
    {
        float CooldownDelay = (float)WC.RPM/60f;
        float Timer = Time.time;
        
        while(Time.time <= Timer + (1f/CooldownDelay))
        {
            yield return null;
        }

        OnCooldown = false;
    }
}