using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Plate : MonoBehaviour
{
    void OnCollisionEnter(Collision collision)
    {
        try{

            Cookable C = collision.transform.gameObject.GetComponent<Cookable>();

            if(C != null && collision.transform.position.y > this.transform.position.y)
            {

                collision.transform.SetParent(this.transform);
                collision.transform.position = new Vector3(this.transform.position.x,collision.transform.position.y,this.transform.position.z);
                
            }

        }catch{}
    }
}
