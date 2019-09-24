using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stack : MonoBehaviour
{

    //create new empty if stack is existing
    //store gameobjects in list
    //picking up removes only the top
    //placing goes to only the top



    /*

    float threshhold = .20f;
    public bool canStack = true;
    public bool Top = false;
    public bool Bottom = false;
    public bool Plate = false;

    void OnCollisionEnter(Collision collision)
    {
        try{

            Stack S = collision.transform.gameObject.GetComponent<Stack>();

            if(S != null && collision.transform.position.y > this.transform.position.y)
            {   
                if(InRange(collision.transform) && !isBottom(S) && canStack)
                {
                    canStack = false;
                    collision.transform.position = new Vector3(this.transform.position.x,collision.transform.position.y,this.transform.position.z);
                    collision.transform.gameObject.GetComponent<Rigidbody>().isKinematic = true;

                }

                /* 
                if(InRange(collision.transform) && !isBottom(S) && S.gameObject.GetComponent<Stack>().canStack)
                {
                    //collision.transform.gameObject.GetComponent<Rigidbody>().isKinematic = true;
                    canStack = false;
                    collision.transform.SetParent(this.transform);
                    collision.transform.position = new Vector3(this.transform.position.x,collision.transform.position.y,this.transform.position.z);
                    //collision.transform.rotation = this.transform.rotation;
                }
                
            }

        }catch{}
    }

    bool isBottom(Stack S)
    {

        if(S.Bottom && !Plate)
            return true;
        else
            return false;

    }

    void OnCollisionExit(Collision collision)
    {
        try{

            Stack S = collision.transform.gameObject.GetComponent<Stack>();

            if(S != null)
            {

                canStack = true;

            }

        }catch{}
    }
    

    bool InRange(Transform t)
    {

        Vector3 xy = new Vector3(t.transform.position.x,this.transform.position.y,t.transform.position.z);

        float dist = Vector3.Distance(xy, this.transform.position);

        if(dist < threshhold && !Top)
            return true;
        else
            return false;

    }

    */
}
