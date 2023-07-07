using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class TransformSync : NetworkBehaviour
{
    NetworkIdentity netID;
    
    [SyncVar]
    Vector3 pos;
    [SyncVar]
    Quaternion rot;
    
    public Transform Selected = null;
    public bool Local = false;
    
    float posThreshold = 0.05f;
    float rotThreshold = 0.05f;
    float posLerpSpeed = 5f;
    float rotLerpSpeed = 5f;

    void Start()
    {
        if(Selected == null)
            Selected = this.transform;
        
        netID = GetComponent<NetworkIdentity>();
    }

    void Update()
    {
        if(netID.hasAuthority)
        {
            if (HasMoved())
                CmdSendPosition(Local ? Selected.localPosition : Selected.position);

            if (HasRotated())
                CmdSendRotation(Local ? Selected.localRotation : Selected.rotation);
        }
        else
        {
            if(pos == Vector3.zero)
                return;

            if(Local)
            {
                if (Selected.localPosition != pos)
                    Selected.localPosition = Vector3.Lerp(Selected.localPosition, pos, Time.deltaTime * posLerpSpeed);

                if (Selected.localRotation != rot)
                    Selected.localRotation = Quaternion.Lerp(Selected.localRotation, rot, Time.deltaTime * rotLerpSpeed);
            }else
            {
                if (Selected.position != pos)
                    Selected.position = Vector3.Lerp(Selected.position, pos, Time.deltaTime * posLerpSpeed);
                
                if (Selected.rotation != rot)
                    Selected.rotation = Quaternion.Lerp(Selected.rotation, rot, Time.deltaTime * rotLerpSpeed);
            }
        }
    }
    
    [Command]
    private void CmdSendPosition(Vector3 _pos)
    {
        pos = _pos;
    }

    [Command]
    private void CmdSendRotation(Quaternion _rot)
    {
        rot = _rot;
    }
    
    bool HasMoved()
    {
        if (posThreshold == 0)
            return true;
        
        if(Local)
            return Vector3.Distance(pos, Selected.localPosition) >= posThreshold;
        else
            return Vector3.Distance(pos, Selected.position) >= posThreshold;
    }

    bool HasRotated()
    {
        if (rotThreshold == 0)
            return true;
        
        if(Local)
            return Quaternion.Angle(rot, Selected.localRotation) >= rotThreshold;
        else
            return Quaternion.Angle(rot, Selected.rotation) >= rotThreshold;
    }
}
