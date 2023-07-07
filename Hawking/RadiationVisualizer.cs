using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RadiationVisualizer : MonoBehaviour
{
    public int mSv_hour = 0;
    public Collider Col;

    public static int Black_mSv = 100000; //6 minutes
    public static int Red_mSv = 10000; //1 hour
    public static int Yellow_mSv = 500; //20 hours
    public static int Green_mSv = 25; //400 hours

    //public static int Lethal_mSv = 10000; //10 Sv

    public static float TimesPerSecond = 4f;

    Player P;

    Vector3 Closest = Vector3.zero;

    void Start()
    {
        P = GameObject.FindWithTag("Player").GetComponent<Player>();

        InvokeRepeating("Radiate", 0, 1f/TimesPerSecond);
    }
    
    //The collider can only be BoxCollider, SphereCollider, CapsuleCollider or a convex MeshCollider.
    void Radiate()
    {
        float Source_uSv = ((mSv_hour * 1000f) / 60f / 60f / TimesPerSecond);

        bool UseDistance = true;
        if(Col.GetType() == typeof(MeshCollider))
        {
            MeshCollider Mesh = Col as MeshCollider;

            if(!Mesh.convex)
            {
                if(Mesh.bounds.Contains(P.transform.position))
                {
                    Closest = P.transform.position;
                    UseDistance = false;
                }else
                {
                    Closest = Mesh.bounds.ClosestPoint(P.transform.position);
                }
            }else
            {
                Closest = Col.ClosestPoint(P.transform.position);
            }
        }else
        {
            Closest = Col.ClosestPoint(P.transform.position);
        }


        int Exposure_uSv = Mathf.RoundToInt(Source_uSv);
        if(UseDistance)
        {
            float Distance = Vector3.Distance(Closest, P.transform.position);
            Exposure_uSv = Mathf.RoundToInt(Source_uSv / (Distance * Distance));
        }
        Exposure_uSv = Mathf.Clamp(Exposure_uSv, 0, Mathf.RoundToInt(Source_uSv));
        
        

        //Debug.Log("Exposure uSv : " + Exposure_uSv);
        P.uSv_Exposure += Exposure_uSv;
        //Debug.Log("mSv per hour : " + (Exposure_uSv * TimesPerSecond * 60f * 60f)/1000f);
    }

    void OnDrawGizmosSelected()
    {
        Vector3 Origin = this.transform.position;

        if (Closest != Vector3.zero)
        {
            Gizmos.color = Color.white;
            Gizmos.DrawSphere(Closest, 0.25f);
            Origin = Closest;
        }

        if(mSv_hour > Black_mSv)
        {
            Gizmos.color = Color.black;
            Gizmos.DrawWireSphere(Origin, Mathf.Sqrt((float) mSv_hour/Black_mSv));
        }
        
        if(mSv_hour > Red_mSv)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(Origin, Mathf.Sqrt((float) mSv_hour/Red_mSv));
        }
        
        if(mSv_hour > Yellow_mSv)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(Origin, Mathf.Sqrt((float) mSv_hour/Yellow_mSv));
        }
        
        if(mSv_hour > Green_mSv)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(Origin, Mathf.Sqrt((float) mSv_hour/Green_mSv));
        }
    }
}
