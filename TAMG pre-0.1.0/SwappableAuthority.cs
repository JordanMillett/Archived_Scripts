using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.Events;

public class SwappableAuthority : NetworkBehaviour
{
    public UnityEvent OnGranted;
    public UnityEvent OnRevoked;

    public bool CurrentAuthority = false;

    NetworkIdentity ID;

    void Start()
    {
        ID = GetComponent<NetworkIdentity>();
    }

    [Command(ignoreAuthority = true)]
    public void AskForAuthority(NetworkConnectionToClient sender = null)
    {
        //Debug.Log(ID.connectionToClient);
        //Debug.Log(sender);
        //ClearAuthority();
        //sender.identity.AssignClientAuthority();
        ID.RemoveClientAuthority();
        ID.AssignClientAuthority(sender);                   //Assign authority to who ever send the message
        
    }
    /*
    [ClientRpc(excludeOwner = true)]    //Everyone but the owner is revoked
    void ClearAuthority()  //TargetRPC ClearAllAuthorities was given a null connection, make sure the object has an owner or you pass in the target connection
    {
        ID.RemoveClientAuthority();
    }
    */
    public override void OnStartAuthority()
    {
        //Debug.Log("AUTHORITY GRANTED ON CAR");
        OnGranted.Invoke();
    }

    public override void OnStopAuthority()
    {
        //Debug.Log("AUTHORITY REVOKED ON CAR");
        OnRevoked.Invoke();
    }
}
