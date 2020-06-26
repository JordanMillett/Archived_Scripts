using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shoot : MonoBehaviour
{
    public GameObject Projectile;
    public GameObject Decal;

    public GunInfo GI;

    public CrosshairScaler CS;
    public PlayerController PC;
    public AmmoCounter AC;
    public GameObject MuzzleFlash;

    float AccuracyOffset = 0f;
    float AccuracyLerp = 0f;

    float UpwardsRecoilOffset = 0f;
    float UpwardsRecoilLerp = 0f;

    public int CurrentAmmo = 0;
    public int CurrentReserve = 0;

    bool CurrentlyReloading = false;

    Transform FireLocation;
    GunRecoil GR;

    Transform Camera;

    GameObject GunModel;

    void Start()
    {
        Camera = GameObject.FindWithTag("Player").transform.GetChild(0).transform;
        FireLocation = transform.GetChild(0).transform;
        GunModel = transform.GetChild(1).gameObject;
        GR = GetComponent<GunRecoil>();

        CurrentAmmo = GI.MagazineSize;
        CurrentReserve = GI.InitialReserveAmmo;

        //AC.SetActiveAmmo(CurrentAmmo);
        //AC.SetReserveAmmo(CurrentReserve);
    }

    void FixedUpdate()
    {
        CS.Accuracy = GI.Accuracy - AccuracyOffset - PC.CurrentSpeedAverage;
        AccuracyOffset = Mathf.Lerp(0f, GI.MaxAccuracyPenalty, AccuracyLerp);

        PC.pitchOffset = UpwardsRecoilOffset;
        UpwardsRecoilOffset = Mathf.Lerp(0f, GI.MaxUpwardsRecoil, UpwardsRecoilLerp);

        if(Input.GetMouseButton(0) && GR.CanShoot && CurrentAmmo > 0 && !CurrentlyReloading)
        {

            CurrentAmmo--;

            AC.SetActiveAmmo(CurrentAmmo);
            AC.SetReserveAmmo(CurrentReserve);

            if((AccuracyLerp + GI.AccuracyPenaltyPerShot) < 1f)
                AccuracyLerp += GI.AccuracyPenaltyPerShot;
            else
                AccuracyLerp = 1f;

            if((UpwardsRecoilLerp + GI.UpwardsRecoilPerShot) < 1f)
                UpwardsRecoilLerp += GI.UpwardsRecoilPerShot;
            else
                UpwardsRecoilLerp = 1f;

            GameObject Sound = new GameObject();
            Sound.transform.position = FireLocation.position;
            //Sound.transform.SetParent(Empty.transform);
            Despawn DSpawn = Sound.AddComponent(typeof(Despawn)) as Despawn;
            DSpawn.DespawnTime = 2f;
            AudioSource AS = Sound.AddComponent(typeof(AudioSource)) as AudioSource;
            AS.volume = 0.15f;
            AS.spatialBlend = 0.6f;
            AS.clip = GetFireSound();
            AS.Play();

            GameObject Flash = Instantiate(MuzzleFlash, FireLocation.position, FireLocation.rotation);

            for(int i = 0; i < GI.ProjectileCount; i++)
            {
                GR.CanShoot = false;
                GR.LerpAlpha = 1f;

                float Inaccuracy = 100f - CS.Accuracy;
                //Vector3 OffsetVector = new Vector3(Random.Range(-Inaccuracy,Inaccuracy),Random.Range(-Inaccuracy,Inaccuracy),Random.Range(-Inaccuracy,Inaccuracy));
                //OffsetVector = OffsetVector / 100f;
                Vector3 OffsetVector = Random.insideUnitSphere.normalized * Random.Range(0f, 1f);
                OffsetVector *= Inaccuracy;
                OffsetVector = OffsetVector / 200f;

                RaycastHit hit;
                            
                if(Physics.Raycast(Camera.transform.position, Camera.transform.forward + OffsetVector, out hit, 100f))
                {
                    GameObject Object;
                    Object = Instantiate(Projectile, FireLocation.transform.position, Quaternion.identity);
                    StartCoroutine(Travel(Object, hit.point, GI.ProjectileVelocity, hit.normal, true));

                    if(hit.transform.GetComponent<Enemy>() != null)
                        hit.transform.GetComponent<Enemy>().Damage(GI.Damage);

                }else
                {
                    GameObject Object;
                    Object = Instantiate(Projectile, FireLocation.transform.position, Quaternion.identity);
                    StartCoroutine(Travel(Object, FireLocation.transform.position + ((Camera.transform.forward + OffsetVector) * 100f), GI.ProjectileVelocity, Vector3.zero, false));
                }
            }

        }else
        {

            if(AccuracyLerp - GI.AccuracyRecoverySpeed > 0f)
                AccuracyLerp -= GI.AccuracyRecoverySpeed;
            else
                AccuracyLerp = 0f;

            if(UpwardsRecoilLerp - GI.UpwardsRecoilRecovery > 0f)
                UpwardsRecoilLerp -= GI.UpwardsRecoilRecovery;
            else
                UpwardsRecoilLerp = 0f;

        }

        if(Input.GetKey("r") && !CurrentlyReloading)
        {
            if(CurrentReserve > 0 && CurrentAmmo != GI.MagazineSize)
                StartCoroutine(Reload());

        }

    }

    IEnumerator Reload()
    {

        CurrentlyReloading = true;
        GunModel.SetActive(false);

        yield return new WaitForSeconds(GI.ReloadTime);

        CurrentReserve += CurrentAmmo;
        CurrentAmmo = 0;

        if(CurrentReserve >= GI.MagazineSize)
        {
            CurrentReserve -= GI.MagazineSize;
            CurrentAmmo = GI.MagazineSize;
        }
        else
        {
            CurrentAmmo = CurrentReserve;
            CurrentReserve = 0;
        }

        AC.SetActiveAmmo(CurrentAmmo);
        AC.SetReserveAmmo(CurrentReserve);

        GunModel.SetActive(true);
        CurrentlyReloading = false;

    }

    AudioClip GetFireSound()
    {

        int Index = Random.Range(0, GI.FireSounds.Count);

        return GI.FireSounds[Index];

    }

    public bool AddAmmo()
    {

        if(CurrentReserve == GI.MaxReserveAmmo)
            return false;

        if(CurrentReserve + GI.AmmoPickupAmount <= GI.MaxReserveAmmo)
        {

            CurrentReserve += GI.AmmoPickupAmount;

        }else
        {

            CurrentReserve = GI.MaxReserveAmmo;

        }

        AC.SetReserveAmmo(CurrentReserve);

        return true;
        

    }

    IEnumerator Travel(GameObject P, Vector3 D, float V, Vector3 N, bool CreateDecal)
    {   

        float Distance = Vector3.Distance(P.transform.position, D);

        while(Distance > 0.1f)
        {
            P.transform.position = Vector3.MoveTowards(P.transform.position, D, Time.fixedDeltaTime * V);
            Distance = Vector3.Distance(P.transform.position, D);
            yield return null;
        }
        /*
        float startTime = Time.time;        //Time Started
        float journeyLength = Vector3.Distance(P.transform.position, D);    //Total Distance
        float fracJourney = 0f;         //Percent traveled so far
        float distCovered = 0f;         //Distance traveled so far

        while(fracJourney < 1f)
        {
            distCovered = (Time.time - startTime) * V;
            fracJourney = distCovered / journeyLength;
            P.transform.position = Vector3.Lerp(P.transform.position, D, fracJourney);
            yield return null;
        }*/

        if(CreateDecal)
            MakeDecal(D, N);

        //Deal Damage 

        try
        {
            Destroy(P);
        }catch{}

    }

    void MakeDecal(Vector3 D, Vector3 N)
    {

        GameObject Dec = Instantiate(Decal, D, Quaternion.LookRotation(N, Vector3.up));

    }
}
