using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class KinematicGun : MonoBehaviour
{
    public float InteractRange;

    public float Radius;
    public float Magnitude;

    public GameObject Wall;

    Vector3 LastPos = Vector3.zero;

    void Update()
    {
        if(Input.GetMouseButtonDown(0))
            SpawnExplosion(GetHitPos());

        if(Input.GetMouseButtonDown(1))
            SpawnWall(GetHitPos());
    
    }
    
    void OnDrawGizmos()
    {
        //Gizmos.color = Color.red;
        //Gizmos.DrawSphere(LastPos, Radius);
    }

    void SpawnWall(Vector3 Start)
    {
        Vector3 LookOffset = new Vector3(this.transform.position.x, Start.y, this.transform.position.z);
        GameObject W = Instantiate(Wall, Start, Quaternion.LookRotation(Start - LookOffset, Vector3.up) * Quaternion.Euler(0, 90f, 0));
    }

    void SpawnExplosion(Vector3 Start)
    {
        
        LastPos = Start;

        if(Start != Vector3.zero)
        {
            Collider[] hitColliders = Physics.OverlapSphere(Start, Radius);

            int i = 0;
            while (i < hitColliders.Length)
            {
                try
                {

                    BluePart Part = hitColliders[i].GetComponent<BluePart>();

                    if(Part != null)
                    {
                        Part.Break();
                    }

                }catch{}

                try
                {
                    Rigidbody R = hitColliders[i].GetComponent<Rigidbody>();

                    if(R != null)
                    {
                        

                        Vector3 Direction = hitColliders[i].transform.position - Start;
                        float percent = (Vector3.Distance(hitColliders[i].transform.position, Start))/(Radius);
                        if(percent > 1)
                            percent = 1;
                        percent = Magnitude * (1 - percent);

                        R.AddForce(Direction.normalized * percent);
                    }

                }catch{}

                i++;
            }
        }
    }

    Vector3 GetHitPos()
    {

        RaycastHit hit;

        int layerMask =~ LayerMask.GetMask("BluePhysics");

		if(Physics.Raycast(transform.position,transform.forward, out hit, InteractRange, layerMask))
		{
            /*
			try 
			{
						
				Rigidbody R = hit.collider.transform.gameObject.GetComponent<Rigidbody>();

				if(R != null)
					R.isKinematic = false;
                    R.AddForce(transform.forward * Force);
                    R.gameObject.layer = 8;
				
			}
			catch{}
            */

            return hit.point;
       }

       return Vector3.zero;

    }

}
