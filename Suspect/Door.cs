using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Door : NetworkBehaviour
{
    [SyncVar]
    public bool Open = false;

    public float OpenDegrees = 120f;

    public Transform Hinge;
    public Collider DoorCollider;

    public void Interact()
    {
        CmdToggleUse();
    }
    
    void OnDrawGizmosSelected() 
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(Hinge.transform.position, Hinge.transform.position + (Quaternion.AngleAxis(this.transform.localScale.x == 1f ? OpenDegrees : -OpenDegrees, Vector3.up) * (-this.transform.right * (this.transform.localScale.x == 1f ? 2f : -2f))));
    }
    
    void Update()
    {
        if (isClient)
        {
            DoorCollider.enabled = !Open;
        }
        
        if (isServer)
        {
            Hinge.transform.localEulerAngles = new Vector3(0f, Mathf.Lerp(Hinge.transform.localEulerAngles.y, Open ? OpenDegrees : 0f, Time.deltaTime * 3f), 0f);
        }
    }
    
    [Command(requiresAuthority = false)] 
    void CmdToggleUse()
    {
        Open = !Open;
    }
    
    void OnTriggerEnter(Collider Col)
    {
        if(!isServer)
            return;

        Player P = Col.transform.gameObject.GetComponent<Player>();

        if(P)
        {
            if (P.AIControlled)
                Open = true;
        }
    }
}
