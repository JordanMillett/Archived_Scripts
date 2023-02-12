using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public static bool DEBUG_DRAW_Bullets = false;
    
    public Transform Model;

    public Transform FirePosition;

    public Transform RayFrom;

    float MaxBackDistance = -0.075f;
    public float CurrentBackDistance = 0f;

    float MaxTurnDistance = 2f;
    float CurrentTurnDistance = 0f;
    Vector3 CurrentTurnOffset = Vector3.zero;

    IEnumerator CooldownRoutine;
    bool OnCooldown = false;

    IEnumerator ReloadRoutine;
    public bool Reloading = false;

    public int CurrentMagazine;

    AudioSource AS;

    Light MuzzleFlash;

    bool Flash = false;


    float RPM = 900f;
    int MagazineSize = 100;
    float ReloadTime = 2f;
    float Accuracy = 98f;
    bool Automatic = true;
    float RecoilMultiplier = 1f;
    float AccurateTime = 2f;
    float WeaponRange = 300f;
    //falloff curve for weapon types

    void Start()
    {
        //AS = FirePosition.GetComponent<AudioSource>();   
        MuzzleFlash = FirePosition.GetComponent<Light>();

        CurrentMagazine = MagazineSize;

        StartCoroutine(FlashLoop());

        CooldownRoutine = Cooldown();
        StartCoroutine(CooldownRoutine);
    }

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

    IEnumerator FlashLoop()
    {
        while(true)
        {
            MuzzleFlash.enabled = Flash;
            if(Flash)
                Flash = false;
            yield return null;
        }
    }

    float t = -100f;
    int shot = 0;
    bool TriggerPulled = false;
    bool TriggerReleased = true;

    void Update()
    {
        if(TriggerPulled)
            Fire();

        if(TriggerPulled)
            TriggerPulled = false;
        else
            TriggerReleased = true;      
    }

    public void PullTrigger()
    {
        if(!Reloading)
        {
            if(TriggerReleased)
            {
                TriggerReleased = false;
                t = Time.time;
                shot = 0;
            }
            
            TriggerPulled = true;
        }
    }

    void Fire()
    {
        if(!OnCooldown)
        {
            
            if(CurrentMagazine == 0 && MagazineSize != 0)
            {
                //PLAY FIRE PIN CLICK SOUNDS FOR OUT OF AMMO
                StartReload();
                return;
            }

            //AS.pitch = Info.FirePitch;
            //AS.PlayOneShot(Info.FireSounds.Clips[Random.Range(0, Info.FireSounds.Clips.Count)], (0.5f * (Settings._sfxVolume/100f)) * (Settings._masterVolume/100f));

            Flash = true;
            
            if(CurrentMagazine > 0 || MagazineSize == 0)
                Shoot((Accuracy));

            float Dif = (Time.time - t) - ((shot * 60f)/RPM);

            int behind = Dif > 0f ? Mathf.RoundToInt(Dif/(60f/RPM)) : 0;

            for(int i = 0; i < behind; i++)
                if(CurrentMagazine > 0 || MagazineSize == 0)
                    Shoot((Accuracy));

            if(Automatic)    //update after shot to not effect rounds accuracy
            {
                float maxBack = MaxBackDistance * RecoilMultiplier;
                float maxTurn = MaxTurnDistance * RecoilMultiplier;
                CurrentBackDistance = maxBack;
                CurrentTurnDistance = maxTurn;
                //0f so no downward facing recoil
                CurrentTurnOffset = new Vector3(Random.Range(-1f, 0f), Random.Range(-1f, 1f), Random.Range(-1f, 1f));
            }

            OnCooldown = true;
            
        }
    }

    void Shoot(float Accuracy)
    {
        
        if(Automatic)  //Recoil before shot to offset each round, first shots are accurate
        {
            float maxBack = MaxBackDistance * RecoilMultiplier;
            float maxTurn = MaxTurnDistance * RecoilMultiplier;
            CurrentBackDistance = maxBack;
            CurrentTurnDistance = (Time.time - t) > AccurateTime ? maxTurn : Mathf.Lerp(maxTurn * 0.2f, maxTurn, (Time.time - t)/AccurateTime);
            CurrentTurnOffset = new Vector3(Random.Range(-1f, 0f), Random.Range(-1f, 1f), Random.Range(-1f, 1f));
            UpdateModel();
        }

        shot++;

        if(MagazineSize > 0)
        {
            CurrentMagazine--;
        }

        Vector3 AimVector = Random.insideUnitSphere.normalized * Random.Range(0f, 1f);
        AimVector *= 100f - Accuracy;
        AimVector /= 250f;
        AimVector += RayFrom.transform.forward;
        
        /*
        if (DEBUG_DRAW_Bullets)
        {
            RaycastHit hit;
            if (Physics.Raycast(RayFrom.position, AimVector, out hit, WeaponRange))
            {
                Debug.DrawLine(RayFrom.position, hit.point, Color.red, 2f);
            }
            else
            {
                Debug.DrawRay(RayFrom.position, AimVector * WeaponRange, Color.black, 2f);
            }
        }*/

        RaycastHit hit;
        if (Physics.Raycast(RayFrom.position, AimVector, out hit, WeaponRange))
        {
            Life _life = hit.collider.gameObject.GetComponent<Life>();
            if(_life)
                _life.Hurt(15);
        }

        //GameObject Fired = Instantiate(Info.ProjectilePrefab, FirePosition.position, Quaternion.LookRotation(AimVector));
        //Fired.transform.localScale = new Vector3(Info.Caliber.x/10f, Info.Caliber.x/10f, Info.Caliber.x/10f);

        //Fired.transform.SetParent(GameObject.FindWithTag("Trash").transform);

        //Fired.GetComponent<Projectile>().FiredByPlayer = Holder.Controller;
        //Fired.GetComponent<Projectile>().Launch(AimVector, Info);
    }

    IEnumerator Cooldown()
    {
        float CooldownDelay = (float)RPM/60f;
        float Timer = Time.time;

        while(true)
        {
            if(OnCooldown)
            {
                Timer = Time.time;
                while(Time.time < Timer + (1f/CooldownDelay))
                {
                    yield return null;
                }

                OnCooldown = false;
            }else
            {
                yield return null;
            }
        }
        
        
    }

    public void StartReload()
    {
        if(!Reloading && CurrentMagazine != MagazineSize)
        {
            Reloading = true;
            ReloadRoutine = Reload();
            StartCoroutine(ReloadRoutine);
        }
    }
    
    public void StopReload()
    {
        if(Reloading)
        {
            StopCoroutine(ReloadRoutine);
            Reloading = false;
        }
    }

    IEnumerator Reload()
    {
        float CooldownDelay = ReloadTime;
        float Timer = Time.time;
        
        while(Time.time <= Timer + CooldownDelay)
        {
            yield return null;
        }

        CurrentMagazine = MagazineSize;
        Reloading = false;
    }
}