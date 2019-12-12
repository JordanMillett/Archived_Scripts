using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Interact : MonoBehaviour
{
    public float InteractRange;

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.F))
            Use();
    }

    void Use()
    {

        RaycastHit hit;

		if(Physics.Raycast(transform.position,transform.forward, out hit, InteractRange, 1 << 9))
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