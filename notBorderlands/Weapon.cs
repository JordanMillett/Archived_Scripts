using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

//make custom namespace sometime

public class Weapon : MonoBehaviour
{

    bool Cooling = false;
    bool Reloading = false;

    public WeaponConfig WC;
    
    //public GameObject Bullet;
    GameObject Camera;
    PlayerController PC;

    public bool PlayerControlled;

    GameObject EjectLocation;

    GameObject Slot;
    public int Ammo;
    public int ReserveAmmo = 90;

    float CurrentRecoil = 0f;
    float RecoilAlpha = 0f;

    float CurrentReload = 0f;
    float ReloadAlpha = 0f;

    public GameObject HitNumberPrefab;

    void Start()
    {
        PC = GameObject.FindWithTag("Player").GetComponent<PlayerController>();
        Camera = this.transform.parent.parent.gameObject;

        if(PlayerControlled)
        {
            Ammo = WC.MagSize;
            Slot = this.transform.parent.gameObject;
        }else
        {
            Ammo = WC.MagSize;
        }
    }

    void OnEnable()
    {
     

        Cooling = false;

        

    }

    void Update()
    {

            if(PlayerControlled && PC.enabled)
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


                Slot.transform.localPosition = new Vector3(0f, CurrentReload, CurrentRecoil);
              
                if(WC.Automatic)
                {
                    if(Input.GetMouseButton(0))
                        Fire();
                }else
                {

                    if(Input.GetMouseButtonDown(0))
                        Fire();

                }

                if(Input.GetKeyDown("r") && !Reloading && !Cooling && (Ammo != WC.MagSize) && (ReserveAmmo != 0))
                {
                    StartCoroutine(Reload());
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

                if(PlayerControlled)
                {
                    Ammo--;
                }

                RaycastHit hit;
                        
                if(Physics.Raycast(Camera.transform.position, Camera.transform.forward + OffsetVector, out hit,WC.Range))
                {
        
                    if(hit.transform.gameObject.GetComponent<EnemyStats>() != null)
                    {

                        hit.transform.gameObject.GetComponent<EnemyStats>().Damage(WC.Damage);
                        MakeHitNumber(hit.point, WC.Damage);

                    }else if(hit.transform.gameObject.GetComponent<Rigidbody>() != null)
                    {
                        hit.transform.gameObject.GetComponent<Rigidbody>().isKinematic = false;
                        Vector3 direction = Camera.transform.forward + OffsetVector;
                        hit.transform.gameObject.GetComponent<Rigidbody>().AddForceAtPosition(direction.normalized * WC.Force, hit.point);
                    }
                }

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

    void MakeHitNumber(Vector3 Pos, float Amount)
    {

        GameObject HN = Instantiate(HitNumberPrefab, Pos, Quaternion.identity);
        HN.transform.GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>().text = Amount.ToString();

    }

    IEnumerator Reload()
    {
    
        Reloading = true;
        yield return new WaitForSeconds(WC.ReloadTime);

        if(ReserveAmmo >= WC.MagSize)
        {
            ReserveAmmo -= WC.MagSize - Ammo;
            Ammo = WC.MagSize;
        }else
        {
            int Difference = WC.MagSize - Ammo;

            if(ReserveAmmo > Difference)
            {

                Ammo = WC.MagSize;
                ReserveAmmo -= Difference;

            }else
            {

                Ammo += ReserveAmmo;
                ReserveAmmo = 0;

            }
            

        }

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