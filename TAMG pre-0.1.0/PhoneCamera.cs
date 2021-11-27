using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhoneCamera : MonoBehaviour
{
    Transform Cam;
    Phone ThePhone;
    public DeliveryInfo DI;

    public GameObject PhoneCam;

    void OnEnable()
    {
        PhoneCam.SetActive(true);
    }

    void OnDisable()
    {
        PhoneCam.SetActive(false);
    }

    void Start()
    {
        Cam = GameObject.FindWithTag("Camera").transform;
        ThePhone = GameObject.FindWithTag("Phone").GetComponent<Phone>();
    }

    void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            RaycastHit hit;

            if(Physics.Raycast(Cam.position, Cam.forward, out hit, 25f))
            {
                Collider[] Nearby = Physics.OverlapSphere(hit.point, 1f);
                foreach(Collider Col in Nearby)
                {
                    try 
                    {   
                        Package P = Col.transform.gameObject.GetComponent<Package>();

                        if(P != null)
                            if(P.Delivered())
                                DeliveryInfo(P);
                    }
                    catch{}
                }
            }
        }
    }

    void DeliveryInfo(Package P)
    {
        ThePhone.ExitApp();
        DI.Show(P);
    }
}
