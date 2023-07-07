using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class ItemObject : NetworkBehaviour
{
    public AudioClip SellSound;
    public Item I;

    [Command(requiresAuthority = false)]
    public void CmdCreate(string _itemFileName)
    {
        RpcCreate(_itemFileName);
    }

    [ClientRpc]
    public void RpcCreate(string _itemFileName)
    {
        I = Client.Instance.ServerInstance.GetItemFromName(_itemFileName);
        transform.GetChild(0).GetComponent<MeshFilter>().sharedMesh = I.Model;
        transform.GetChild(0).GetComponent<MeshRenderer>().material.SetFloat("_Smoothness", I.Smoothness);
        GetComponent<MeshCollider>().sharedMesh = I.Model;
        GetComponent<CollisionSound>().SM = I.SM;
        GetComponent<Buoyancy>().SinkSpeed = I.SinkSpeed;
    }
    
    [Command]
    public void CmdSell()
    {
        RpcSell();
    }

    [ClientRpc]
    void RpcSell()
    {
        GameObject Sound = new GameObject();
        Sound.transform.position = this.transform.position;
        Despawn DSpawn = Sound.AddComponent(typeof(Despawn)) as Despawn;
        AudioClip Clip = SellSound;

        DSpawn.DespawnTime = Clip.length + 0.1f;
        AudioSource AS = Sound.AddComponent(typeof(AudioSource)) as AudioSource;
        AS.spatialBlend = 0f;
        AS.volume = (.2f * (Settings._sfxVolume / 100f)) * (Settings._masterVolume / 100f);
        AS.maxDistance = 50f;
        AS.clip = Clip;
        AS.Play();

        Destroy(this.gameObject);
    }
}
