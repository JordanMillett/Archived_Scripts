using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Prompt : MonoBehaviour
{
    //DATATYPES

    //PUBLIC COMPONENTS
    public Passenger P;
    public Canvas C;

    //PUBLIC VARS

    //PUBLIC LISTS

    //COMPONENTS

    //VARS

    //LISTS

    void Start()
    {
        C.worldCamera = GameObject.FindWithTag("Camera").GetComponent<Camera>();
    }

    public void Approve()
    {
        P.CurrentStatus = Passenger.Status.Approved;
        //Debug.Log("Approved");
        Destroy(this.gameObject);
    }

    public void Deny()
    {
        P.CurrentStatus = Passenger.Status.Denied;
        //Debug.Log("Denied");
        Destroy(this.gameObject);
    }
}