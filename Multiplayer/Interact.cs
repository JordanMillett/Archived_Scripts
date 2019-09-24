using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Interact : MonoBehaviour
{

    GameObject cam;
    public bool canInteract = true;

    void Start()
    {

        cam = this.gameObject.transform.GetChild(0).gameObject;

    }

    void Update()
    {
        if(canInteract)
            if(Input.GetKeyDown(KeyCode.F))
               Use();
    }

    void Use()
    {

        RaycastHit hit;

		if(Physics.Raycast(cam.transform.position,cam.transform.forward, out hit,10))
		{
			try 
			{
						
				Interactable I = hit.transform.gameObject.GetComponent<Interactable>();

				if(I != null)
					I.Activate();
				
			}
			catch(NullReferenceException){}
       }

    }

}
