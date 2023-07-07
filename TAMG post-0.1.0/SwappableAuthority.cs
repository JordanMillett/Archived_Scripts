using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.Events;

public class SwappableAuthority : NetworkBehaviour
{
    NetworkIdentity netID;

    public UnityEvent OnGranted;
    public UnityEvent OnRevoked;

    public bool CurrentAuthority = false;

    void Start()
    {
        netID = GetComponent<NetworkIdentity>();
    }

    [Command(requiresAuthority = false)]
    public void AskForAuthority(NetworkConnectionToClient sender = null)
    {
        netID.RemoveClientAuthority();
        netID.AssignClientAuthority(sender);                
    }

    public override void OnStartAuthority()
    {
        OnGranted.Invoke();
    }

    public override void OnStopAuthority()
    {
        OnRevoked.Invoke();
    }
}
