using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Landmine : MonoBehaviour
{
    public float ForceMount;

    void OnCollisionEnter(Collision col)
    {
        try 
		{
            Player PC = col.transform.root.gameObject.GetComponent<Player>();

			if(PC != null)
            {
                Vector3 Direction = col.transform.position - transform.position;
				col.transform.root.gameObject.GetComponent<Rigidbody>().AddForce(Direction.normalized * ForceMount);
                
                StartCoroutine(Kill(PC));
                
            }
				
		}
		catch{}

        
    }

    IEnumerator Kill(Player PC)
    {
        yield return new WaitForSeconds(0.1f);
        PC.Die("Blew Up");
    }
}
