using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Interact : MonoBehaviour
{
    public float InteractRange;

    void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            Use();
        }

        Highlight();
    }

    void Highlight()
    {
        int layerMask = LayerMask.GetMask("Interact");
        RaycastHit hit;

		if(Physics.Raycast(transform.position,transform.forward, out hit, layerMask))
		{
			try 
			{
						
				HighLightable HL = hit.collider.transform.gameObject.GetComponent<HighLightable>();

				if(HL != null)
                {
					HL.Over = true;
                }
				
			}
			catch{}
        }
    }

    void Use()
    {
        int layerMask = LayerMask.GetMask("Interact");
        RaycastHit hit;

		if(Physics.Raycast(transform.position,transform.forward, out hit, layerMask))
		{
			try 
			{
						
				Interactable I = hit.collider.transform.gameObject.GetComponent<Interactable>();

				if(I != null)
                {
					I.Activate();
                }
				
			}
			catch{}
        }
    }
    
}