using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shoot : MonoBehaviour
{
    public GameObject FiringObject;
    public GameObject Decal;
    public float Velocity;

    GameObject Camera;

    PlayerController PC;
    PlayerStats PS;

    int magicCooldown = 0;

    void Start()
    {
        PS = GetComponent<PlayerStats>();
        Camera = this.transform.GetChild(0).gameObject;
        PC = GetComponent<PlayerController>();
    }

    void Update()
    {
        
        if(!PC.Frozen)
        {
            if(Input.GetKeyDown("q") && (PS.Magic >= 25f))
            {
                magicCooldown = 0;
                PS.AddValue(1, -25f);
                Fire();

            }else
            {

                if(magicCooldown > 100)
                {
                    PS.AddValue(1, .25f);
                }else
                {

                    magicCooldown++;

                }

            }
        }
    }

    public void Fire()
    {

        RaycastHit hit;
                        
        if(Physics.Raycast(Camera.transform.position, Camera.transform.forward, out hit, 100f))
        {
            GameObject Projectile;
            Projectile = Instantiate(FiringObject, Camera.transform.position, Quaternion.identity);
            
            StartCoroutine(Travel(Projectile, hit.point, Velocity, hit.normal));
            
    

        }

    }

    IEnumerator Travel(GameObject P, Vector3 D, float V, Vector3 N)
    {
        float startTime = Time.time;
        float journeyLength = Vector3.Distance(P.transform.position, D);
        float fracJourney = 0f;
        float distCovered = 0f;

        while(fracJourney < .5f)
        {
            distCovered = (Time.time - startTime) * V;
            fracJourney = distCovered / journeyLength;
            P.transform.position = Vector3.Lerp(P.transform.position,D,fracJourney);
            yield return null;
        }

        MakeDecal(D, N); 

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
