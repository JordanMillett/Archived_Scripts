using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretTest : MonoBehaviour
{
    public GameObject Target;

    //public transform SightLocation;

    Weapon W;

    void Start()
    {
        W = this.transform.GetChild(0).GetComponent<Weapon>();
    }
    
    void Update()
    {
        Vector3 relativePos = Target.transform.position - this.transform.position;
        this.transform.rotation = Quaternion.LookRotation(relativePos, Vector3.up);
        if(canSee())
            W.Fire();
    }

    bool canSee()
    {
        //Debug.DrawRay(this.transform.position, this.transform.forward * 100f, Color.green);

        RaycastHit hit;             
        if(Physics.Raycast(this.transform.position, this.transform.forward, out hit,100f))
            if(hit.transform.gameObject == Target.transform.root.gameObject)
                return true;
        
        return false;
                
    }
}
