using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ship_Gun : MonoBehaviour
{
    public Weapon W;
    GameObject Space;
    bool Cooling = false;
    Ship S;
    PlayerController PC;

    void Start()
    {
        Space = GameObject.FindWithTag("Space");
        S = GameObject.FindWithTag("Ship").GetComponent<Ship>();
        PC = GameObject.FindWithTag("Player").GetComponent<PlayerController>();
    }

    public void Fire()
    {
        if(!Cooling && S.Power >= W.PowerCost)
        {   
            for(int i = 0; i < W.Bullets;i++)
            {
                S.Power -= W.PowerCost;
                float Inaccuracy = 100f - W.Accuracy;
                Vector3 OffsetVector = new Vector3(Random.Range(-Inaccuracy,Inaccuracy),Random.Range(-Inaccuracy,Inaccuracy),Random.Range(-Inaccuracy,Inaccuracy));
                OffsetVector = OffsetVector / 100f; //to make unit vector

                RaycastHit hit;

                GameObject Projectile = Instantiate(W.Bullet, this.transform.GetChild(0).transform.position, Quaternion.identity);

                if(Physics.Raycast(this.transform.GetChild(0).transform.position,this.transform.GetChild(0).transform.forward + OffsetVector, out hit,W.Range))
                {
                    StartCoroutine(Travel(Projectile,hit.point,W.Velocity));

                    try{
                        hit.collider.gameObject.GetComponent<Damageable>().Damage(W.Damage);
                    }catch{}

                }
                else
                    StartCoroutine(Travel(Projectile,this.transform.GetChild(0).transform.position + ((this.transform.GetChild(0).transform.forward + OffsetVector) * W.Range) ,W.Velocity));
            }

            Cooling = true;
            StartCoroutine(PC.ScreenShake(W.Ship_Shake * 0.001f,.3f));

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

}
