using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class SellZone : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        try 
		{
			ItemObject I = other.transform.GetComponent<ItemObject>();

			if(I != null)
            {
                if (I.GetComponent<NetworkIdentity>().hasAuthority)
                {
                    Client.Instance.SessionSave.Balance += I.I.Value;
                    Client.Instance.SyncSessionSave();

                    I.CmdSell();
                }
            }
				
		}
		catch{}
    }
}
