using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class ItemObject : NetworkBehaviour
{
    public Item I;

    [Command]
    public void CmdCreate(string _itemFileName)
    {
        RpcCreate(_itemFileName);
    }

    [ClientRpc]
    public void RpcCreate(string _itemFileName)
    {
        I = GameServer.GS.GameManagerInstance.GetItemFromName(_itemFileName);
        transform.GetChild(0).GetComponent<MeshFilter>().sharedMesh = I.Model;
        GetComponent<MeshCollider>().sharedMesh = I.Model;
        GetComponent<CollisionSound>().SM = I.SM;
    }
}
