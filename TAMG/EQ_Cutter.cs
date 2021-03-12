using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EQ_Cutter : MonoBehaviour
{
    bool isOn = false;

    public void ToggleOn()
    {
        isOn = true;
    }

    void Update()
    {
        if(isOn)
        {
            if(Input.GetMouseButtonDown(0))
            {
                Transform Cam = GameObject.FindWithTag("Camera").transform;
                RaycastHit hit;

                if(Physics.Raycast(Cam.transform.position, Cam.transform.forward, out hit, 5f))
                {
                    try 
                    {
                                
                        Package P = hit.collider.transform.root.gameObject.GetComponent<Package>();

                        if(P == null)
                            P = hit.collider.transform.gameObject.GetComponent<Package>();

                        if(P == null)
                            P = hit.collider.transform.parent.gameObject.GetComponent<Package>();

                        if(P != null)
                        {
                            P.Open();
                        }
                        
                    }
                    catch{}
                }
            }
        }
    }
}
