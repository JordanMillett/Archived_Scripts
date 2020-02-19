using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class CursorChanger : MonoBehaviour
{
    
    public Transform RayStart;

    public List<Texture> Icons;
    RawImage Icon;
    int CurrentCursor = 1;

    void Start()
    {
        Icon = GetComponent<RawImage>();
        Icon.texture = Icons[1];
    }    

    void Update()
    {

        SendDetectRay();

    }

    public void UpdateCursor(int Index)
    {
        if(Index != CurrentCursor)
        {   
            CurrentCursor = Index;
            Icon.texture = Icons[Index];
        }
    }

    void SendDetectRay()
    {

        RaycastHit hit;

		if(Physics.Raycast(RayStart.position,RayStart.forward, out hit, 5f))
		{
			try 
			{
						
				Interactable I = hit.collider.transform.gameObject.GetComponent<Interactable>();

				if(I != null)
					UpdateCursor(2);
                else
                    UpdateCursor(1);
				
			}
			catch(NullReferenceException){}
       }

    }

    
}
