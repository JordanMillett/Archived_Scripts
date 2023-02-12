using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public WeaponInfo Info;
    public Transform Model;

    public Texture2D SelectionImage;

    public Transform FirePosition;

    public Collider Col;

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

    public GameObject Scope;

    AudioSource AS;

    Light MuzzleFlash;

    bool Flash = false;

    public bool AttackerInaccuracy = false;

    public Unit Holder;

    public Vector3 HoldOffset = new Vector3(0f, 0f, 0f);

    public Vector3 Photo_Offset = Vector3.zero;
    public float Photo_CameraSize = 0.5f;

    void Start()
    {
        AS = FirePosition.GetComponent<AudioSource>();   
        MuzzleFlash = FirePosition.GetComponent<Light>();

        CurrentMagazine = Info.MagazineSize;

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
        //if(Time.time > t + 1f)
        //if(!TriggerReleased)
        //{
            //Debug.Log(Info.name + " - " + (shot/((Time.time - t)/60f)) +  " RPM ---- " + Info.RPM);
        //}

        if(TriggerPulled)
            Fire();

        if(TriggerPulled)
            TriggerPulled = false;
        else
            TriggerReleased = true;
            
        if(Scope)
        {
            if(Holder)
            {
                if(Holder.Controller)
                {
                    Scope.SetActive(true);
                }else
                {
                    Scope.SetActive(false);
                }
            }else
            {
                Scope.SetActive(false);
            }
        }
            
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
            if(CurrentMagazine == 0 && Info.MagazineSize != 0)
            {
                //PLAY FIRE PIN CLICK SOUNDS FOR OUT OF AMMO
                StartReload();
                return;
            }

            AS.pitch = Info.FirePitch;
            AS.PlayOneShot(Info.FireSounds.GetRandom(), (Info.FireSounds.Volume * (Settings._sfxVolume/100f)) * (Settings._masterVolume/100f));
            
            Flash = true;
            
            if(CurrentMagazine > 0 || Info.MagazineSize == 0)
                Shoot((AttackerInaccuracy ? Mathf.Min(Info.Accuracy, Game.PossibleAccuracy) : Info.Accuracy));

            float Dif = (Time.time - t) - ((shot * 60f)/Info.RPM);

            int behind = Dif > 0f ? Mathf.RoundToInt(Dif/(60f/Info.RPM)) : 0;

            for(int i = 0; i < behind; i++)
                if(CurrentMagazine > 0 || Info.MagazineSize == 0)
                    Shoot((AttackerInaccuracy ? Mathf.Min(Info.Accuracy, Game.PossibleAccuracy) : Info.Accuracy));
                    //Shoot(Mathf.Lerp(99f, 97f, CurrentTurnDistance/MaxTurnDistance));

            if(!Info.Automatic)    //update after shot to not effect rounds accuracy
            {
                float maxBack = MaxBackDistance * Info.RecoilMultiplier;
                float maxTurn = MaxTurnDistance * Info.RecoilMultiplier;
                CurrentBackDistance = maxBack;
                CurrentTurnDistance = maxTurn;
                //0f so no downward facing recoil
                CurrentTurnOffset = new Vector3(Random.Range(-1f, 0f), Random.Range(-1f, 1f), Random.Range(-1f, 1f));
            }

            if(Holder)
            {
                if(Holder.Type == Unit.Types.Infantry)
                {
                    Holder.inf.pitch += -Info.RecoilMultiplier * 0.7f;
                    Holder.inf.yaw += CurrentTurnOffset.y * 0.35f;
                    //Holder.inf.Chest.transform.localEulerAngles += new Vector3(-Info.RecoilMultiplier *  0.2f, 0f, 0f);
                    //Holder.inf.transform.localEulerAngles += new Vector3(0f, CurrentTurnOffset.y * 0.1f, 0f);
                }else if(Holder.Type == Unit.Types.Tank)
                {
                    if (this == Holder.tan.Primary)
                    {
                        Holder.tan.ShootExplode();
                        Holder.GetComponent<Rigidbody>().AddForceAtPosition(-FirePosition.forward * 100000f, FirePosition.position);
                    }
                }
            }

            OnCooldown = true;
        }
    }

    void Shoot(float Accuracy)
    {
        if(Info.Automatic)  //Recoil before shot to offset each round, first shots are accurate
        {
            if (Info.AccurateTime != 0f)
            {
                float maxBack = MaxBackDistance * Info.RecoilMultiplier;
                float maxTurn = MaxTurnDistance * Info.RecoilMultiplier;
                CurrentBackDistance = maxBack;
                CurrentTurnDistance = (Time.time - t) > Info.AccurateTime ? maxTurn : Mathf.Lerp(maxTurn * 0.2f, maxTurn, (Time.time - t) / Info.AccurateTime);
                CurrentTurnOffset = new Vector3(Random.Range(-1f, 0f), Random.Range(-1f, 1f), Random.Range(-1f, 1f));
            }else
            {
                float maxBack = MaxBackDistance * Info.RecoilMultiplier;
                float maxTurn = MaxTurnDistance * Info.RecoilMultiplier;
                CurrentBackDistance = maxBack;
                CurrentTurnDistance = maxTurn;
                CurrentTurnOffset = new Vector3(Random.Range(-1f, 0f), Random.Range(-1f, 1f), Random.Range(-1f, 1f));
            }
            UpdateModel();
        }

        shot++;

        if(Info.MagazineSize > 0)
        {
            CurrentMagazine--;
            //if(CurrentMagazine == 0)
                //StartReload();
        }

        Vector3 AimVector = Random.insideUnitSphere.normalized * Random.Range(0f, 1f);
        AimVector *= 100f - Accuracy;
        AimVector /= 250f;
        AimVector += FirePosition.transform.forward;

        GameObject Fired = Instantiate(Info.ProjectilePrefab, FirePosition.position, Quaternion.LookRotation(AimVector));
        //Fired.transform.localScale = new Vector3(Info.ProjectileScale, Info.ProjectileScale, Info.ProjectileScale);
        Fired.transform.localScale = new Vector3(Info.Caliber.x/10f, Info.Caliber.x/10f, Info.Caliber.x/10f);

        Fired.transform.SetParent(GameObject.FindWithTag("Trash").transform);
        
        if(Holder)
            Fired.GetComponent<Projectile>().FiredByPlayer = Holder.Controller;
        else
            Fired.GetComponent<Projectile>().FiredByPlayer = false;
            
        Fired.GetComponent<Projectile>().Launch(AimVector, Info);
    }

    IEnumerator Cooldown()
    {
        float CooldownDelay = (float)Info.RPM/60f;
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
        if(!Reloading && CurrentMagazine != Info.MagazineSize)
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
        float CooldownDelay = Info.ReloadTime;
        float Timer = Time.time;
        
        while(Time.time <= Timer + CooldownDelay)
        {
            yield return null;
        }

        CurrentMagazine = Info.MagazineSize;
        Reloading = false;
    }

    public void Drop(Vector3 _velocity)
    {
        this.transform.SetParent(null);
        Col.enabled = true;
        Rigidbody r = this.gameObject.AddComponent(typeof(Rigidbody)) as Rigidbody;
        r.collisionDetectionMode = CollisionDetectionMode.Continuous;
        r.velocity = _velocity;
        r.drag = 0f;
        this.transform.SetParent(GameObject.FindWithTag("Trash").transform);

        Despawn D = this.gameObject.AddComponent(typeof(Despawn)) as Despawn;
        D.DespawnTime = 10f;
    }
}
