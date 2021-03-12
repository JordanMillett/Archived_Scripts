using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SellZone : MonoBehaviour
{
    public AudioClip SellSound;

    void OnTriggerEnter(Collider other)
    {
        try 
		{
			ItemObject I = other.transform.GetComponent<ItemObject>();

			if(I != null)
            {
                PlayerInfo.Balance += I.I.Value;
                Destroy(I.gameObject);
            }
				
		}
		catch{}
    }
}
