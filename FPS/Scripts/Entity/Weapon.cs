using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

//make custom namespace sometime

public class Weapon : MonoBehaviour
{

    bool Cooling = false;
    bool Reloading = false;

    public Vector3 Pos;
    public Vector3 Rot;


    public WeaponConfig WC;
    /*
    public float Damage;
    public float RPM;
    public float Accuracy;
    public float Velocity;
    public float Range;
    public float RecoilAmount;
    public float Force;
    public float ReloadTime;
    public int Bullets;
    public int WC.WC.Magsize;
    public bool Automatic;
    */
    
    public GameObject Bullet;
    public GameObject Decal;
    public GameObject MuzzleFlash;
    public GameObject Eject;
    public Vector3 EjectVector;
    GameObject SpawnLocation;
    //public float MuzzleFlashTime;
    //public float DecalDespawnTime;
    public float ADSfov = 30f;
    public Vector3 ADSLocation;
    Vector3 CurrentLocation;
    float ADSAlpha = 0;
    GameObject Camera;
    PlayerController PC;
    GameObject Empty;

    public bool PlayerControlled;

    //public bool RanOnEnable;

    GameObject EjectLocation;

    GameObject Slot;
    int Ammo;

    EquipSlot EQ;
    TextMeshProUGUI Ammo_GUI;

    float CurrentRecoil = 0f;
    float RecoilAlpha = 0f;

    float CurrentReload = 0f;
    float ReloadAlpha = 0f;

    UIController UIC;


    float GunRotateOffsetDegreesX = 15f;
    float GunRotateOffsetDegreesY = 5f;
    float GunRotateOffsetXAlpha = 0.5f;
    float GunRotateOffsetYAlpha = 0.5f;
    Vector3 CurrentRotation;

    float GunbobAlpha = 0f;
    float GunbobAmplitude = .01f;
    float GunbobOffset = 0f;
    float GunbobTime = 0f;

    float BackAmount = 0f;

    float CrouchRotationOffset = -35f;
    Vector3 CrouchPositionOffset = new Vector3(-.40f, -.05f, 0);
    float CrouchOffsetAlpha = 0f;
    float CurrentCrouchRotationOffset = 0f;
    Vector3 CurrentCrouchPositionOffset = Vector3.zero;
    float xRotAlpha = 0f;

    public List<AudioClip> FireSounds;

    void Start()
    {
        CurrentRotation = Rot;
        PC = GameObject.FindWithTag("Player").GetComponent<PlayerController>();
        Empty = GameObject.FindWithTag("Empty");
        UIC = GameObject.FindWithTag("Pause").GetComponent<UIController>();
        Camera = this.transform.parent.parent.gameObject;

        SpawnLocation = this.transform.GetChild(1).gameObject;
        EjectLocation = this.transform.GetChild(2).gameObject;

        if(PlayerControlled)
        {
            PC.ZoomedFov = ADSfov;
            EQ = this.transform.parent.GetComponent<EquipSlot>();
            Ammo_GUI = EQ.Ammo;
            Ammo = WC.MagSize;
            Ammo_GUI.text = Ammo.ToString();
            Slot = this.transform.parent.gameObject;
        }else
        {
            Ammo = WC.MagSize;
        }
    }

    void OnEnable()
    {
        /*if(RanOnEnable)
        {
            RanValues();
            Ammo = WC.MagSize;
        }*/

        if(EQ != null)
            Ammo_GUI.text = Ammo.ToString();
        
        if(PlayerControlled && PC != null)
            PC.ZoomedFov = ADSfov;

        Cooling = false;

        

    }

    void Update()
    {

       

        if(!UIC.Paused)
        {
  
            if(Input.GetMouseButton(1))
            {
                CurrentLocation = Vector3.Lerp(CurrentLocation, ADSLocation - CurrentCrouchPositionOffset, ADSAlpha);
                if(ADSAlpha <= 1f)
                    ADSAlpha += 0.02f;

                if(xRotAlpha <= 1f)
                    xRotAlpha += 0.2f;

            
            }else
            {
                CurrentLocation = Vector3.Lerp(CurrentLocation, Vector3.zero, 1f - ADSAlpha);
                if(ADSAlpha >= 0f)
                    ADSAlpha -= 0.02f;

                if(xRotAlpha >= 0f)
                    xRotAlpha -= 0.2f;
            }
    
            if(!PlayerControlled)
                Ammo = WC.MagSize;

            if(PlayerControlled)
            {

                CurrentRecoil = Mathf.Lerp(CurrentRecoil, 0f, RecoilAlpha);
                    if(RecoilAlpha < 1f)
                        RecoilAlpha += 0.02f;

                if(Reloading)
                {
                    CurrentReload = Mathf.Lerp(CurrentReload, -.15f, ReloadAlpha);
                        if(ReloadAlpha < 1f)
                            ReloadAlpha += 0.03f;
                }else
                {

                    CurrentReload = Mathf.Lerp(CurrentReload, 0, 1 - ReloadAlpha);
                        if(ReloadAlpha > 0f)
                            ReloadAlpha -= 0.03f;

                }

                
                if(PC.Crouching)
                {
                    CurrentCrouchPositionOffset = Vector3.Lerp(CurrentCrouchPositionOffset, CrouchPositionOffset, CrouchOffsetAlpha);
                    CurrentCrouchRotationOffset = Mathf.Lerp(CurrentCrouchRotationOffset, CrouchRotationOffset, CrouchOffsetAlpha);
                  
                    if(CrouchOffsetAlpha <= 1f)
                        CrouchOffsetAlpha += 0.02f;
                }else
                {
                    CurrentCrouchPositionOffset = Vector3.Lerp(CurrentCrouchPositionOffset, Vector3.zero, 1f - CrouchOffsetAlpha);
                    CurrentCrouchRotationOffset = Mathf.Lerp(CurrentCrouchRotationOffset, 0f, 1f - CrouchOffsetAlpha);

                    if(CrouchOffsetAlpha >= 0f)
                        CrouchOffsetAlpha -= 0.02f;
                }

            
                
                    

                Slot.transform.localPosition = new Vector3(0f, CurrentReload + GunbobOffset, CurrentRecoil + BackAmount) + CurrentLocation + CurrentCrouchPositionOffset;
                /*Slot.transform.localEulerAngles = new Vector3(0f,0f, CurrentCrouchRotationOffset);*/
              
                if(WC.Automatic)
                {
                    if(Input.GetMouseButton(0))
                        Fire();
                }else
                {

                    if(Input.GetMouseButtonDown(0))
                        Fire();

                }

                if(Input.GetKeyDown("r") && !Reloading && !Cooling && (Ammo != WC.MagSize))
                {
                    StartCoroutine(Reload());
                }

                RotateGun();
                Gunbob();
                GunBack();

            }
        }

    }

    public void Fire()
    {

        if(!Cooling && Ammo >= WC.Bullets && !Reloading)
        {   
            for(int i = 0; i < WC.Bullets;i++)
            {
                float Inaccuracy = 100f - WC.Accuracy;
                Vector3 OffsetVector = new Vector3(Random.Range(-Inaccuracy,Inaccuracy),Random.Range(-Inaccuracy,Inaccuracy),Random.Range(-Inaccuracy,Inaccuracy));
                OffsetVector = OffsetVector / 100f;

                GameObject Projectile;
                Projectile = Instantiate(Bullet, SpawnLocation.transform.position, Quaternion.identity);
                Projectile.transform.SetParent(Empty.transform);

                GameObject Sound = new GameObject();
                Sound.transform.position = SpawnLocation.transform.position;
                //GameObject Sound = Instantiate(Sound, SpawnLocation.transform.position, Quaternion.identity);
                Sound.transform.SetParent(Empty.transform);
                Despawn DSpawn = Sound.AddComponent(typeof(Despawn)) as Despawn;
                DSpawn.DespawnTime = 3f;
                AudioSource AS = Sound.AddComponent(typeof(AudioSource)) as AudioSource;
                AS.spatialBlend = .6f;
                AS.clip = GetFireSound();
                AS.Play();

                GameObject Flash = Instantiate(MuzzleFlash, Projectile.transform.position, SpawnLocation.transform.rotation);
                Flash.transform.SetParent(Empty.transform);
                //StartCoroutine(Delete(Flash, MuzzleFlashTime));

                GameObject Ejected = Instantiate(Eject, EjectLocation.transform.position, EjectLocation.transform.rotation);
                Ejected.transform.SetParent(Empty.transform);
                //StartCoroutine(Delete(Ejected, 4f));
                Ejected.GetComponent<Rigidbody>().AddRelativeForce(EjectVector);

                if(PlayerControlled)
                {
                    Ammo--;
                    Ammo_GUI.text = Ammo.ToString();
                }

                RaycastHit hit;
                        
                if(Physics.Raycast(Camera.transform.position, Camera.transform.forward + OffsetVector, out hit,WC.Range))
                {
                    StartCoroutine(Travel(Projectile, hit.point, WC.Velocity));
        
                    if(hit.transform.gameObject.GetComponent<Quadtree>() != null)
                    {

                        hit.transform.gameObject.GetComponent<Quadtree>().Split(hit.point);

                    }
                    else if(hit.transform.gameObject.GetComponent<LifeManager>() != null)
                    {
                        hit.transform.gameObject.GetComponent<LifeManager>().Damage(WC.Damage);
                        StartCoroutine(HitMarker());
                    }else if(hit.transform.gameObject.GetComponent<Rigidbody>() != null)
                    {
                        hit.transform.gameObject.GetComponent<Rigidbody>().isKinematic = false;
                        Vector3 direction = Camera.transform.forward + OffsetVector;
                        hit.transform.gameObject.GetComponent<Rigidbody>().AddForceAtPosition(direction.normalized * WC.Force, hit.point);

                        if(hit.transform.gameObject.GetComponent<Destructable>() != null)
                            hit.transform.gameObject.GetComponent<Destructable>().Break(hit.point);

                    }else
                        MakeDecal(hit.point,hit.transform.gameObject,hit.normal); 

                }else
                    StartCoroutine(Travel(Projectile, Projectile.transform.position + (-SpawnLocation.transform.right + OffsetVector) * WC.Range, WC.Velocity));

                    if(PlayerControlled)
                    {
                        PC.pitch -= WC.RecoilAmount;

                        if(Random.value > 0.5f)
                            PC.yaw -= WC.RecoilAmount;
                        else
                            PC.yaw += WC.RecoilAmount;
                        
                        RecoilAlpha = 0;
                        CurrentRecoil = -WC.RecoilAmount/5f;
                    }
                
            }

            Cooling = true;
            StartCoroutine(Recharge(1f/ (WC.RPM / 60f)));
    

        }

    }

    IEnumerator HitMarker()
    {
        if(PlayerControlled)
        {
            UIC.HitMarker.SetActive(true);
            yield return null;
            yield return null;
            UIC.HitMarker.SetActive(false);
            
        }

    }

    void GunBack()
    {
        float Backoffset = .5f;
        float Dist = .1f + Backoffset;
        

        RaycastHit R;
        if(Physics.Raycast(SpawnLocation.transform.position + (-SpawnLocation.transform.right * -BackAmount) + (-SpawnLocation.transform.right * -Backoffset), -SpawnLocation.transform.right, out R, Dist))
        {
            //if(BackAmount != -(Dist - R.distance))
            BackAmount = -(Dist - R.distance);

        }else
        {
            BackAmount = 0f;
        }

    }

    AudioClip GetFireSound()
    {

        int Index = Random.Range(0, FireSounds.Count);

        return FireSounds[Index];

    }

    void Gunbob()
    {

        if(Input.GetMouseButton(1))
            GunbobAmplitude = .01f/4f;
        else
            GunbobAmplitude = .01f;

        if(GunbobTime < 1f)
            GunbobTime += 0.04f * PC.speedMult;
        else
            GunbobTime = 0f;

        //Debug.Log(HeadbobCurve.Evaluate(HeadbobTime));

        if(PC.speed > 0f)
        {

            GunbobOffset = Mathf.Lerp(GunbobOffset, PC.HeadbobCurve.Evaluate(GunbobTime) * GunbobAmplitude * PC.speedMult, GunbobAlpha);
            if(GunbobAlpha < 1f)
                GunbobAlpha += 0.02f;

        }else
        {

            GunbobOffset = Mathf.Lerp(GunbobOffset, 0f, 1f - GunbobAlpha);
            if(GunbobAlpha > 0f)
                GunbobAlpha -= 0.02f;
        

        }

    }

    void RotateGun()
    {

        /*
        
        float Threshold = 0f;

        if(Input.GetMouseButton(1))
            Threshold = 100f;

        float Speed = 0.01f;

        if(Input.GetAxis ("Mouse X") < -Threshold)
        {

            if(GunRotateOffsetXAlpha > 0f)
                GunRotateOffsetXAlpha -= Speed;

        }else if(Input.GetAxis ("Mouse X") > Threshold)
        {

            if(GunRotateOffsetXAlpha < 1f)
                GunRotateOffsetXAlpha += Speed;

        }else
        {

            if(GunRotateOffsetXAlpha > 0.5f + Speed)
                GunRotateOffsetXAlpha -= Speed * 2f;
            else if(GunRotateOffsetXAlpha < 0.5f - Speed)
                GunRotateOffsetXAlpha += Speed * 2f;
            else
                GunRotateOffsetXAlpha = 0.5f;

        }



        if(Input.GetAxis ("Mouse Y") > Threshold)
        {

            if(GunRotateOffsetYAlpha > 0f)
                GunRotateOffsetYAlpha -= Speed;

        }else if(Input.GetAxis ("Mouse Y") < -Threshold)
        {

            if(GunRotateOffsetYAlpha < 1f)
                GunRotateOffsetYAlpha += Speed;

        }else
        {

            if(GunRotateOffsetYAlpha > 0.5f + Speed)
                GunRotateOffsetYAlpha -= Speed * 2f;
            else if(GunRotateOffsetYAlpha < 0.5f - Speed)
                GunRotateOffsetYAlpha += Speed * 2f;
            else
                GunRotateOffsetYAlpha = 0.5f;

        }
        */
        //Vector3 LerpVector = new Vector3(0f,C)   

        GunRotateOffsetXAlpha += Input.GetAxis ("Mouse X")/4f;
        GunRotateOffsetYAlpha -= Input.GetAxis ("Mouse Y")/3f;

        //if(Input.GetAxis ("Mouse X") > 0)



        float Threshold = 0.05f;

        if(GunRotateOffsetXAlpha > Threshold || GunRotateOffsetXAlpha < -Threshold)
            GunRotateOffsetXAlpha -= GunRotateOffsetXAlpha/2f;
        
        if(GunRotateOffsetYAlpha > Threshold || GunRotateOffsetYAlpha < -Threshold)
            GunRotateOffsetYAlpha -= GunRotateOffsetYAlpha/2f;

        

        CurrentRotation.x = Mathf.Lerp(CurrentCrouchRotationOffset, 0f,xRotAlpha);
        //if(Input.GetMouseButton(0))
        //{
            //CurrentRotation.y = 0f;
            //CurrentRotation.z = 0f;
        //}else
        //{
            CurrentRotation.y = GunRotateOffsetXAlpha;
            CurrentRotation.z = GunRotateOffsetYAlpha;
        //}
        //CurrentRotation.y = Mathf.Lerp(-GunRotateOffsetDegreesX, GunRotateOffsetDegreesX, GunRotateOffsetXAlpha);
        //CurrentRotation.z = Mathf.Lerp(-GunRotateOffsetDegreesY, GunRotateOffsetDegreesY, GunRotateOffsetYAlpha);
        
        this.transform.localEulerAngles = Rot + CurrentRotation;
        //Slot.transform.localEulerAngles = CurrentLocation;
        
        //CurrentRotation.y = Mathf.Lerp(CurrentRotation.y, GunRotateOffsetDegreesY, GunRotateOffsetYAlpha - .5f);

        /* 
        if(Input.GetKey(KeyCode.LeftShift)) //Sprinting
        {
            FovOffset = Mathf.Lerp(FovOffset, SprintingFov, FovOffsetAlpha);
            if(FovOffsetAlpha < 1f)
                FovOffsetAlpha += 0.03f;
        }
        else
        {
            FovOffset = Mathf.Lerp(FovOffset, 0f, 1 - FovOffsetAlpha);
            if(FovOffsetAlpha > 0f)
                FovOffsetAlpha -= 0.03f;
        }*/

    }

    IEnumerator Reload()
    {
        //Slot.transform.localPosition = new Vector3(0f,-.15f,0f);
    
        Reloading = true;
        yield return new WaitForSeconds(WC.ReloadTime);
        //Slot.transform.localPosition = new Vector3(0f,0f,0f);

        Ammo = WC.MagSize;
        if(this.transform.GetChild(0).gameObject.activeSelf)
            Ammo_GUI.text = Ammo.ToString();

        Reloading = false;

    }

    IEnumerator Travel(GameObject P, Vector3 D, float V)
    {
        float startTime = Time.time;
        float journeyLength = Vector3.Distance(P.transform.position, D);
        float fracJourney = 0f;
        float distCovered = 0f;

        while(fracJourney < 1f)
        {
            distCovered = (Time.time - startTime) * V;
            fracJourney = distCovered / journeyLength;
            P.transform.position = Vector3.Lerp(P.transform.position,D,fracJourney);
            yield return null;
        }

        try{
            Destroy(P);
        }catch {}

    }

    IEnumerator Delete(GameObject Gam, float Delay)
    {
        yield return new WaitForSeconds(Delay);
        try{
            Destroy(Gam);
        }catch {}

    }

    IEnumerator Recharge(float FireRate)
    {
        yield return new WaitForSeconds(FireRate);
        Cooling = false;
        //if(PlayerControlled)
            //CurrentRecoil = 0f;
            //Slot.transform.localPosition = new Vector3(0f,0f,0f);
        //StartCoroutine(Travel(Slot, Vector3.zero, 5f));
    }

    void MakeDecal(Vector3 D, GameObject P, Vector3 N)
    {
        Debug.DrawRay(D, N * 100f, Color.red);
        
        //Rotation needs to match normal

        //GameObject Dec = Instantiate(Decal, D, Quaternion.Euler(90f,0f,0f));
        //GameObject Dec = Instantiate(Decal, D, Quaternion.LookRotation(SpawnLocation.transform.position, Vector3.up));

        //Debug.Log(N);

        GameObject Dec = Instantiate(Decal, D, Quaternion.LookRotation(N, Vector3.up));

        //Dec.transform.localEulerAngles = N;
        
        //Dec.transform.SetParent(P.transform);
        //StartCoroutine(Delete(Dec,DecalDespawnTime));

    }

    /*void RanValues()
    {

        Damage = Random.Range(0f,25f);
        RPM = Random.Range(250f,2500f);
        Accuracy = Random.Range(60f,100f);
        Velocity = Random.Range(100f,1000f);
        Range = Random.Range(500f, 1000f);
        RecoilAmount = Random.Range(0.01f, 2f);
        Force = Random.Range(0f,250f);
        ReloadTime = Random.Range(.1f,4f);
        Bullets = Random.Range(1,5);
        WC.Magsize = Random.Range(1,250);

        float auto = Random.Range(0f,1f);
        if(auto > 0.5f)
            Automatic = true;
        else
            Automatic = false;

    }*/


}