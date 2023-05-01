using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Service : NetworkBehaviour
{
    [SyncVar]
    public bool InUse = false;

    public int Duration;

    public Transform Location;



    private Vector3 returnLocation;
    private Player user;
    
    public Game.Needs Functionality;

    [Server]
    public override void OnStartServer()
    {
        Server.instance.RegisterService(this);
    }

    public void Interact()
    {
        if(InUse)
            return;

        CmdToggleUse();

        user = Player.localPlayer;
        user.Animating = true;
        user.GetComponent<Rigidbody>().isKinematic = true;

        if (Location)
        {
            returnLocation = user.gameObject.transform.position;
            user.gameObject.transform.position = Location.position;
            user.gameObject.transform.rotation = Location.rotation;
        }

        if(Duration > 0)
            Invoke("Return", Duration);
    }
    
    [Command(requiresAuthority = false)] 
    void CmdToggleUse()
    {
        InUse = !InUse;
    }
    
    public void Return()
    {
        CmdToggleUse();
        
        if(Functionality == Game.Needs.None)
            return;
        switch(Functionality)
        {
            case Game.Needs.Solid :
            case Game.Needs.Liquid :
                user.Needs[Game.Needs.Solid].Current = 0;
                user.Needs[Game.Needs.Liquid].Current = 0;
                
                user.Needs[Game.Needs.Hygiene].Increase(50); //25%
                break;
            case Game.Needs.Hunger :
                user.Needs[Game.Needs.Hunger].Current = 0;
                
                user.Needs[Game.Needs.Solid].Increase(50); //25%
                break;
            case Game.Needs.Thirst :
                user.Needs[Game.Needs.Thirst].Current = 0;
                
                user.Needs[Game.Needs.Liquid].Increase(50); //25%
                break;
            default :
                user.Needs[Functionality].Current = 0;
                break;
        }

        user.GetComponent<Rigidbody>().isKinematic = false;
        user.Animating = false;
        user.gameObject.transform.position = returnLocation;
    }
}
