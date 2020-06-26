using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireTest : MonoBehaviour
{
    public GameObject FiringObject;
    public float Velocity = 10f;
    public Transform FireLocation;

    void Start()
    {
        
    }

    void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {

            RaycastHit hit;
                        
            if(Physics.Raycast(FireLocation.transform.position, FireLocation.transform.forward, out hit, 100f))
            {
                GameObject Projectile;
                Projectile = Instantiate(FiringObject, FireLocation.transform.position, Quaternion.identity);
                StartCoroutine(Travel(Projectile, hit.point, Velocity, hit.normal));

                if(hit.transform.gameObject.GetComponent<SimpleAI>() != null)
                {
                    hit.transform.gameObject.GetComponent<SimpleAI>().Die();
                }


            }else
            {
                GameObject Projectile;
                Projectile = Instantiate(FiringObject, FireLocation.transform.position, Quaternion.identity);
                StartCoroutine(Travel(Projectile, FireLocation.transform.position + (FireLocation.transform.forward * 100f), Velocity, hit.normal));
            }
        }
    }

    IEnumerator Travel(GameObject P, Vector3 D, float V, Vector3 N)
    {
        Vector3 f = FireLocation.transform.forward;
        float targetDistance = Vector3.Distance(P.transform.position, D);
        float lerp = 0f;
        Vector3 startPosition = P.transform.position;

        float stopDistance = targetDistance/100f;

        while(lerp <= stopDistance)
        {
            lerp += V;
            P.transform.position = Vector3.Lerp(startPosition, startPosition + (f * 100f), lerp);

            yield return null;
        }
        
        try
        {
            Destroy(P);
        }catch{}
        
    }

}
