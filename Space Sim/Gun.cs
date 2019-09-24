using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour
{
    public Player_Weapon W;
    bool Cooling = false;
    Ship S;
    GameObject cam;
    public Vector3 ADSPos;
    PlayerController PC;

    bool isRunning = false;
    float upAmount = 0f;

    void Start()
    {
        S = GameObject.FindWithTag("Ship").GetComponent<Ship>();
        cam = GameObject.FindWithTag("Player").transform.GetChild(0).gameObject;
        PC = GameObject.FindWithTag("Player").GetComponent<PlayerController>();

    }

    public void Fire()
    {

        if(!Cooling)
        {   
            for(int i = 0; i < W.Bullets;i++)
            {
                float Inaccuracy = 100f - W.Accuracy;
                Vector3 OffsetVector = new Vector3(Random.Range(-Inaccuracy,Inaccuracy),Random.Range(-Inaccuracy,Inaccuracy),Random.Range(-Inaccuracy,Inaccuracy));
                OffsetVector = OffsetVector / 100f; //to make unit vector

                RaycastHit hit;

                GameObject Projectile = Instantiate(W.Bullet, this.transform.GetChild(0).transform.position, Quaternion.identity);

                if(Physics.Raycast(cam.transform.position,cam.transform.forward + OffsetVector, out hit,W.Range))
                {
                    StartCoroutine(Travel(Projectile,hit.point,W.Velocity));


                    try{
                        hit.collider.gameObject.GetComponent<Damageable>().Damage(W.Damage);
                    }catch{}

        

                }
                else
                    StartCoroutine(Travel(Projectile,cam.transform.position + ((cam.transform.forward + OffsetVector) * W.Range) ,W.Velocity));
            }

            Cooling = true;
            StartCoroutine(Recoil(W.Recoil * W.Bullets));

            if(W.Automatic)
                StartCoroutine(Recharge(1f/ (W.RPM / 60f)));
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

        try{
            Destroy(P);
        }catch {}

    }

    IEnumerator Delete(GameObject Gam)
    {
        yield return new WaitForSeconds(.25f);
        try{
            Destroy(Gam);
        }catch {}

    }

    IEnumerator Recharge(float FireRate)
    {
        yield return new WaitForSeconds(FireRate);
        Cooling = false;
    }

    IEnumerator Recoil(float Amount)
    {
        //if(isRunning)
            //upAmount += Amount;
        
        /* 
        if(!isRunning)
        {
            isRunning = true;
            */

            float camY = cam.transform.localEulerAngles.y;

            float RecoilDuration = 0.1f;
            float elapsedTime = 0f;

            PC.pitch += -Amount;
            PC.currentPitch += -Amount;

            while (elapsedTime < RecoilDuration)
            {
                //cam.transform.localEulerAngles += Vector3.Lerp(new Vector3(camY,0f,0f), new Vector3(-Amount,0f,0f), (elapsedTime / RecoilDuration));
                //PC.pitch += Mathf.Lerp(PC.pitch,-Amount * 0.001f,(elapsedTime/RecoilDuration));
                //PC.pitch += -Amount * 0.1f;
                this.transform.localPosition = Vector3.Lerp(Vector3.zero, new Vector3(0f,0f,-Amount/12f), (elapsedTime / RecoilDuration));
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            camY = cam.transform.localEulerAngles.y;

            float RecoverDuration = 0.25f;
            elapsedTime = 0f;
            while (elapsedTime < RecoverDuration)
            {
                //cam.transform.localEulerAngles += Vector3.Lerp(new Vector3(camY,0f,0f), Vector3.zero, (elapsedTime / RecoilDuration));
                this.transform.localPosition = Vector3.Lerp(new Vector3(0f,0f,-Amount/12f), Vector3.zero, (elapsedTime / RecoverDuration));
                elapsedTime += Time.deltaTime;
                yield return null;
            }



            //upAmount = 0f;
            //isRunning = false;
       // }

        /*
        float RecoilDuration = 0.1f; //random value
        float elapsedTime = 0f;
        while (elapsedTime < RecoilDuration)
        {
            cam.transform.eulerAngles = Vector3.Lerp(camRot, camRot + new Vector3(-Amount,0f,0f), (elapsedTime / RecoilDuration));
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        elapsedTime = 0f;
        while (elapsedTime < RecoilDuration * 2)
        {
            cam.transform.eulerAngles = Vector3.Lerp(camRot + new Vector3(-Amount,0f,0f), camRot, (elapsedTime / RecoilDuration));
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        */

        /* 
        int lerpFrames = 5;

        for(int i = 0; i < lerpFrames;i++)
        {
            cam.transform.eulerAngles = cam.transform.eulerAngles + new Vector3(Amount/lerpFrames,0f,0f);
            this.transform.localPosition += new Vector3(0f,0f,-Amount/lerpFrames/10f);
            yield return null;
        }
        
        for(int i = 0; i < lerpFrames;i++)
        {
            cam.transform.eulerAngles += new Vector3((-Amount * 2)/lerpFrames,0f,0f);
            this.transform.localPosition += new Vector3(0f,0f,-Amount/lerpFrames/20f);
            yield return null;
        }
        
        yield return new WaitForSeconds(.25f);
        this.transform.localPosition = GunPos;
        */

        //camera lurches up and returns to default rotation , tacks on and continues to add upward with each shot
        //gun shakes and goes back

    }
}
