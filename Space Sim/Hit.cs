using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hit : MonoBehaviour
{
    public float DamageAmount;
    public bool StopsShip;
    public float SpeedThreshold;
    bool Cooling = false;
    Ship S;

    void Start()
    {
        S = GameObject.FindWithTag("Ship").GetComponent<Ship>();
    }

    void OnCollisionEnter(Collision col) 
    {
        if(!Cooling)
        {
            try{
                if(col.gameObject.transform.root.gameObject == S.gameObject)
                {
                    S.Crash(DamageAmount,StopsShip,SpeedThreshold);
                    Cooling = true;
                    StartCoroutine(Cooling_Timer());
                    
                }
            }catch{}
        }
    }

    IEnumerator Cooling_Timer()
    {

        yield return new WaitForSeconds(3f);
        Cooling = false;

    }

 
}
