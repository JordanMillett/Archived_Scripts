using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForceGun : MonoBehaviour
{
    public GameObject Cam;
    public float Force;
    public float Range;

    void Update()
    {
        if(Input.GetMouseButtonDown(0))
            Fire();
    }

    void Fire()
    {

        RaycastHit hit;

		if(Physics.Raycast(Cam.transform.position, Cam.transform.forward, out hit, Range))
		{
			try 
			{	
				DestructableSurface D = hit.collider.transform.gameObject.GetComponent<DestructableSurface>();

				if(D != null)
					D.Activate();
				
			}
			catch{}
       }

    }
}
