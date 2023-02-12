using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class Projectile : MonoBehaviour
{
    public MeshRenderer M;

    WeaponInfo Info;

    public bool FiredByPlayer = false;

    /*
    void Start()
    {
        AudioSource AS = GetComponent<AudioSource>();
        AS.timeSamples = Random.Range(0, AS.clip.samples - 1);
        AS.Play();
    }*/

    public void Launch(Vector3 Dir, WeaponInfo I)
    {   
        Info = I;
        StartCoroutine(LaunchLoop(Dir));
    }

    IEnumerator LaunchLoop(Vector3 Dir)
    {   
        if(Info.Explosive)
            M.materials[0].SetInt("Explosive", 1);

        float collisionResolution = 1f;

        Vector3 firePosition = this.transform.position;

        int index = 0;
        Vector3 from = firePosition;
        Vector3 to = GetShellPositionAtTime(firePosition, Dir, Info.GetMuzzleVelocity(), index * collisionResolution);
        bool invertColor = false;

        RaycastHit Landed = new RaycastHit();
        Landed.point = Vector3.zero;

        bool Detonated = false;
        float DetonateDistance = Random.Range(Info.FlakDistanceMin, Info.FlakDistanceMax);
        Vector3 DetonateLocation = Vector3.zero;

        yield return null;

        while(index < 30)
        {
            RaycastHit marched = March(from, to, invertColor);

            if(Info.Flak && Vector3.Distance(firePosition, from) > DetonateDistance)
            {
                index = 30;
                DetonateLocation = to;
                Detonated = true;
            }

            if(marched.point != Vector3.zero)
            {
                index = 30;
                to = marched.point;
                Landed = marched;
            }

            float alpha = 0f;
            while(Vector3.Distance(this.transform.position, to) > 0.025f)
            {
                alpha += Time.deltaTime * Info.GetMuzzleVelocity();
                this.transform.position = Vector3.Lerp(from, to, alpha);
                yield return null;
            }

            if(marched.point == Vector3.zero)
            {
                from = to;    
                index++;
                to = GetShellPositionAtTime(firePosition, Dir, Info.GetMuzzleVelocity(), index * collisionResolution);
                
                invertColor = !invertColor;
            }
        }

        bool HitPerson = false;
        bool HitTank = false;

        if(Landed.point != Vector3.zero)
        {
            try
            {
                
                try{
                    Infantry Target = Landed.collider.transform.root.gameObject.GetComponent<Infantry>(); //remove sending the inf over projectile
                    if(Info.Explosive)
                        Target.HitDirection = Dir * 10f;
                    else
                        Target.HitDirection = Dir * 3f;
                    if(Target)
                        HitPerson = true;
                    
                    Target.CheckLocation(firePosition);
                }catch{}
                
            
                try{
                    Tank Target = Landed.collider.transform.root.gameObject.GetComponent<Tank>();
                    if(Info.Explosive)
                        Target.GetComponent<Rigidbody>().AddForceAtPosition(Dir * 100000f * 2.5f, Landed.point);
                    if(Target)
                        HitTank = true;
                }catch{}
                
                try{
                    Plane Target = Landed.collider.transform.root.gameObject.GetComponent<Plane>();
                    if(Target)
                        HitTank = true;
                }catch{}

                Damage_Part DP = Landed.collider.transform.gameObject.GetComponent<Damage_Part>();

                DP.Hurt(Info.Damage, Info.Explosive, FiredByPlayer);
                
            }catch{}
            
            
        }

        if(Detonated)
        {
            MakeEffect(Landed.point, Landed.normal, Manager.M.HitExplodeAir, true);
        }else
        {
            if (Landed.point != Vector3.zero)
            {
                if (HitTank)
                {
                    if (Info.Explosive)
                        MakeEffect(Landed.point, Landed.normal, Manager.M.HitExplode, true);
                    else
                        MakeEffect(Landed.point, Landed.normal, Manager.M.HitMetal, false);
                }
                else
                {
                    if (Info.DecalScale <= 2f && HitPerson)
                    {
                        if (Info.Explosive)
                            MakeEffect(Landed.point, Landed.normal, Manager.M.HitExplode, true);
                        else
                            MakeEffect(Landed.point, Landed.normal, Manager.M.HitPerson, false);
                    }
                    else
                    {
                        if (Info.Explosive)
                        {
                            MakeEffect(Landed.point, Landed.normal, Manager.M.HitExplode, true);
                        }
                        else
                        {
                            MakeEffect(Landed.point, Landed.normal, Manager.M.HitNormal, false);
                        }
                    }
                }
            }
        }

        Destroy(this.gameObject);
    }

    void MakeEffect(Vector3 Pos, Vector3 Nor, GameObject HitPrefab, bool ExplosiveDamage)
    {
        GameObject Dec = Instantiate(HitPrefab, Pos, Quaternion.identity);
            
        if(Nor != Vector3.zero)
            Dec.transform.rotation = Quaternion.LookRotation(Nor, Vector3.up);

        if(ExplosiveDamage)
            Dec.GetComponent<Explosion>().Explode(Info.DecalScale, Info.ExplosiveSize, Info.ExplosiveDamage, Info.DecalVolume * 0.75f, FiredByPlayer);
            
        Dec.GetComponent<AudioSourceController>().SetVolume(Info.DecalVolume * 0.75f);
        Dec.GetComponent<AudioSourceController>().PlayRandom();
        Dec.transform.GetChild(0).GetComponent<VisualEffect>().SetFloat("Size", Info.DecalScale);
        Dec.GetComponent<Despawn>().DespawnTime *= Info.DecalScale + 2f;
        Dec.transform.SetParent(GameObject.FindWithTag("Trash").transform);
        
    }

    Vector3 GetShellPositionAtTime(Vector3 firePosition, Vector3 fireDirection, float fireVelocity, float t)
    {
        Vector3 shellPosition = Vector3.zero;

        shellPosition = fireDirection * fireVelocity * t;

        shellPosition += firePosition;

        return shellPosition;
    }

    RaycastHit March(Vector3 from, Vector3 to, bool invertColor)
    {   
        Color rayColor = Color.red;
        if(invertColor)
            rayColor = Color.green;

        RaycastHit hit;
        Vector3 direction = Vector3.Normalize(to - from);
        float rayLength = Vector3.Distance(from, to);
        if(Physics.Raycast(from, direction, out hit, rayLength, Game.DamageOnlyMask))
        {
            rayLength = Vector3.Distance(from, hit.point);
            //Debug.DrawRay(from, direction * rayLength, Color.blue, 3f);
            return hit;
        }else
        {
            //Debug.DrawRay(from, direction * rayLength, rayColor, 3f);
            hit.point = Vector3.zero;
            return hit;
        }
    }
}