using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Mirror;

public class Interact : MonoBehaviour
{
    public float InteractRange;

    Interactable lastM = null;
    Interactable lastF = null;

    bool mPermission = false;
    bool fPermission = false;

    public void Clear()
    {
        lastM = null;
        lastF = null;
    }

    void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            if(lastM != null)
            {
                DropM();
            }else
            {
                Use('M');
            }
        }

        if(Input.GetKeyDown(KeyCode.F))
        {
            if(lastF != null)
            {
                DropF();
            }else
            {
                Use('F');
            }
        }
        /*
        if(Input.GetKeyDown("p"))
        {
            RaycastHit hit;

            if(Physics.Raycast(transform.position,transform.forward, out hit, 100f))
            {
                GameServer.GS.SpawnSCP(hit.point);
            }
        }*/

        NetworkDrop();
    }

    void NetworkDrop()
    {
        if(lastM != null)       //if package not null                                                
        {
            if(mPermission)     //if has network ownership already granted
            {
                if(!lastM.GetComponent<NetworkIdentity>().hasAuthority)      //and currently has no ownership anymore
                {
                    //Debug.Log("Taken");
                    DropM();
                }
            }else
            {
                mPermission = lastM.GetComponent<NetworkIdentity>().hasAuthority;
            }
        }

        if(lastF != null)
        {
            if(fPermission)
            {
                if(!lastF.GetComponent<NetworkIdentity>().hasAuthority)      
                {
                    DropF();
                }
            }else
            {
                fPermission = lastF.GetComponent<NetworkIdentity>().hasAuthority;
            }
        }
    }

    void DropM()
    {
        lastM.Activate('M');
        mPermission = false;
        lastM = null;
    }

    void DropF()
    {
        lastF.Activate('F');    
        fPermission = false;                           
        lastF = null;
    }

    void Use(char Filter)
    {
        LayerMask Ignore =~ LayerMask.GetMask("Waypoint");
        RaycastHit hit;

		if(Physics.Raycast(transform.position,transform.forward, out hit, InteractRange, Ignore))
		{
			try 
			{
						
				Interactable I = hit.collider.transform.root.gameObject.GetComponent<Interactable>();

                if(I == null)
                    I = hit.collider.transform.gameObject.GetComponent<Interactable>();

                if(I == null)
                    I = hit.collider.transform.parent.gameObject.GetComponent<Interactable>();

				if(I != null)
                {
                    if(Filter == 'M')
                        lastM = I;
                    else
                        lastF = I;

					I.Activate(Filter);
                    
                }
				
			}
			catch{}
       }

    }

}