using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour
{
    public Weapon Info;
    
    public Transform End;
    public GameObject ProjectilePrefab;
    
    bool TriggerPulled = false;
    bool TriggerReleased = true;

    bool LeftToRight = true;
    int Spray = 0;

    float RecenterTime = 0.25f;

    float LastFired = -100f;

    Transform Projectiles;
    Transform Trash;
    public Transform ShootDirection;

    public bool PlayerControlled = false;

    public MeshRenderer MR;

    public float Cooldown = 0f;

    void Start()
    {
        Spray = Info.StartSpray;
        LeftToRight = !Info.ShootRightToLeft;
        Projectiles = GameObject.FindWithTag("Projectiles").transform;
        Trash = GameObject.FindWithTag("Trash").transform;
    }

    public void PullTrigger()
    {
        if(TriggerReleased)
            TriggerReleased = false;
        
        TriggerPulled = true;
    }

    void Update()
    {
        if (TriggerPulled)
        {
            TryFire();
        }

        if(TriggerPulled)
            TriggerPulled = false;
        else
            TriggerReleased = true;

        Cooldown = Mathf.Max(0f, Cooldown - Time.deltaTime);
    }
    
    public void TryFire()
    {
        if (Cooldown <= 0f)
        {
            if (Info.Firemode == FireModes.Burst)
            {
                for (int i = 0; i < Info.BurstWaves; i++)
                    Invoke("Fire", i * 0.1f);
                Cooldown = Info.GetRPMDelay(PlayerControlled);
                return;
            }
            
            if (Info.Firemode == FireModes.Auto || !PlayerControlled)
            {
                Fire();
                Cooldown = Info.GetRPMDelay(PlayerControlled);
                return;
            }
            
            if (Info.Firemode == FireModes.Semi)
            {
                Fire();
                Cooldown = Info.GetRPMDelay(PlayerControlled);
                return;
            }
        }
    }
    
    void Fire()
    {
        if(Info.SprayInterval == 0 && Info.SprayMax != 0)
        {
            Debug.LogError("SPRAY INTERVAL IS ZERO");
            return;
        }
        
        if (!Info.AllAtOnce)
        {
            if (Time.time > LastFired + RecenterTime)
            {
                Spray = Info.StartSpray;
                LeftToRight = !Info.ShootRightToLeft;
            }

            CreateProjectile(Spray);

            if (Spray == (LeftToRight ? Info.SprayMax : -Info.SprayMax))
                LeftToRight = !LeftToRight;

            Spray += LeftToRight ? Info.SprayInterval : -Info.SprayInterval;
        }else
        {
            for (int i = -Info.SprayMax; i <= Info.SprayMax; i += Info.SprayInterval)
            {
                CreateProjectile(i);
            }
        }
    }
    
    void CreateProjectile(float Skew)
    {
        GameObject P = null;
        
        if(Projectiles.childCount > 0)
            P = Projectiles.GetChild(0).gameObject;

        if (!P)
        {
            P = GameObject.Instantiate(ProjectilePrefab, End.position, Quaternion.identity);
            P.GetComponent<Projectile>().Projectiles = Projectiles;
        }else
        {
            P.gameObject.SetActive(true);
            P.transform.position = End.position;
        }
        
        P.transform.SetParent(Trash);
        float Speed = PlayerControlled ? Game.PlayerProjectileSpeeds[Info.Speed] : Game.EnemyProjectileSpeeds[Info.Speed];
        P.GetComponent<Rigidbody>().velocity = Quaternion.Euler(0f, Random.Range(-Info.MaxRandomOffset, Info.MaxRandomOffset), 0f) * Quaternion.Euler(0f, Skew, 0f) * ShootDirection.forward * Speed;
        P.GetComponent<Projectile>().Configure(Info.CalculateDamage(PlayerControlled), Info.Size, Info.Bonus, PlayerControlled);
    }
}
