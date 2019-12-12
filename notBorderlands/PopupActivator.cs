using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PopupActivator : MonoBehaviour
{
    public float Range;

    void Update()
    {
        RaycastHit hit;

		if(Physics.Raycast(transform.position, transform.forward, out hit, Range, 1 << 8))
		{
            if(hit.collider.isTrigger)
            {
                try 
                {
                
                    Interactable I = hit.collider.transform.gameObject.GetComponent<Interactable>();

                    if(I != null)
                        I.Activate();
                    
                }
                catch(NullReferenceException){}
            }
       }
    }
}
