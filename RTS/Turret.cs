using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Turret : MonoBehaviour
{
    public Transform Yaw;
    public Transform Pitch;
    public Transform Aim_Location;

    public Unit Target;

    Vector3 Pitch_Dir;
    Vector3 Yaw_Dir;

    //AudioSource Sound;

    public float DetectDistance;

    public float Dist;
    bool Cooling = false;
    public float Damage;
    public float RPM;
    public float Accuracy;
    public float Velocity;
    public float Range;
    public int Bullets;
    public bool Automatic;
    public GameObject Bullet; //line renderer
    public GameObject Decal;
    public GameObject MuzzleFlash;
    public float MuzzleFlashTime;
    public float DecalDespawnTime;

    bool Left = false; //alternating fire

    void Start()
    {

        //Sound = GetComponent<AudioSource>()

    }

    void Update()
    {
        Yaw_Dir = Yaw.position - Aim_Location.position;
        Yaw_Dir = new Vector3(Yaw_Dir.x, 0f, Yaw_Dir.z);

        Pitch_Dir = Pitch.position - Aim_Location.position;
        Pitch_Dir = new Vector3(0f , Pitch_Dir.y, Vector3.Distance(Pitch.position, Aim_Location.position));
        
        Yaw.eulerAngles = new Vector3(-90f, -Vector3.SignedAngle(Yaw_Dir, Vector3.forward, Vector3.up) + 180f,0f);

        Pitch.localEulerAngles = new Vector3(Vector3.SignedAngle(Pitch_Dir, Vector3.forward, Vector3.right), 0f, 0f);

        if(Input.GetKey("f"))
            Fire();

        if(Target == null || Target.isDead)
            Target = GetTarget();
        else
        {
            Aim_Location.transform.position = Target.transform.position;
            Fire();
        }

    }

    Unit GetTarget()
    {

        Collider[] hitColliders = Physics.OverlapSphere(transform.position, DetectDistance);
        foreach(Collider C in hitColliders)
        {
            if(C.GetComponent<Unit>() != null)
                if(!C.GetComponent<Unit>().isDead)
                    return C.GetComponent<Unit>();
        }

        return null;

    }

    void Fire()
    {

        if(!Cooling)
        {   
            for(int i = 0; i < Bullets;i++)
            {
                float Inaccuracy = 100f - Accuracy;
                Vector3 OffsetVector = new Vector3(Random.Range(-Inaccuracy,Inaccuracy),Random.Range(-Inaccuracy,Inaccuracy),Random.Range(-Inaccuracy,Inaccuracy));
                OffsetVector = OffsetVector / 100f; //to make unit vector

                RaycastHit hit;

                GameObject Projectile;

                if(Left)
                    Projectile = Instantiate(Bullet, Pitch.transform.position + (-Pitch.right * 0.3f) + (Pitch.up * -Dist), Quaternion.identity);
                else
                    Projectile = Instantiate(Bullet, Pitch.transform.position + (Pitch.right * 0.3f) + (Pitch.up * -Dist), Quaternion.identity); 

                Left = !Left;

                GameObject Flash = Instantiate(MuzzleFlash, Projectile.transform.position, Pitch.transform.rotation);
                StartCoroutine(Delete(Flash, MuzzleFlashTime));


                //Debug.Log(Pitch.transform.position + (Pitch.up * -Dist));

                //StartCoroutine(Travel(Projectile,Aim_Location.position,Velocity));

                //if(Physics.Raycast(Pitch.transform.position + (Pitch.up * -Dist), -Pitch.up + OffsetVector, out hit,Range))
                if(Physics.Raycast(Projectile.transform.position, -Pitch.up + OffsetVector, out hit,Range))
                {
                    StartCoroutine(Travel(Projectile, hit.point, Velocity));

                    try{
                        hit.collider.gameObject.GetComponent<Unit>().Damage(Damage);
                    }catch{}

                }else
                    StartCoroutine(Travel(Projectile, Projectile.transform.position + (Pitch.up + OffsetVector) * -Range, Velocity));
                
                    //StartCoroutine(Travel(Projectile,Pitch.position + ((Pitch.forward + (Pitch.up * -Dist) + OffsetVector) * Range) ,Velocity));
            }

            Cooling = true;

            if(Automatic)
                StartCoroutine(Recharge(1f/ (RPM / 60f)));
            else
                Cooling = false;
        }

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

        MakeDecal(D);

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

    void MakeDecal(Vector3 D)
    {

        GameObject Dec = Instantiate(Decal, D, Quaternion.Euler(90f,0f,0f));

        StartCoroutine(Delete(Dec,DecalDespawnTime));

    }


}
