using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class CollisionSound : NetworkBehaviour
{
    public SoundMaterial SM;

    public float LoudestForce = 1f;
    public float Threshold = 0f;

    NetworkIdentity netID;

    void Start()
    {
        netID = GetComponent<NetworkIdentity>();
    }

    void OnCollisionEnter(Collision Col)
    {
        if(!netID)
            netID = GetComponent<NetworkIdentity>();
        //Debug.Log(Col.impulse.magnitude);

        if(netID.hasAuthority)
        {
            if(Col.impulse.magnitude >= Threshold)
            {
                float Force = Col.impulse.magnitude - Threshold;
                float Alpha = Force/LoudestForce;

                if(Alpha > 1f)
                    Alpha = 1f;
                        
                float Volume = Mathf.Lerp(0f, 0.6f, Alpha);

                Volume = (Volume * (Settings._sfxVolume/100f)) * (Settings._masterVolume/100f);

                CmdSpawnSound(Col.contacts[0].point, Volume);
            }
        }
    }

    [Command]
    void CmdSpawnSound(Vector3 _position, float _volume)
    {
        RpcSpawnSound(_position, _volume);
    }

    [ClientRpc]
    void RpcSpawnSound(Vector3 _position, float _volume)
    {
        GameObject Sound = new GameObject();
        Sound.transform.position = _position;
        Despawn DSpawn = Sound.AddComponent(typeof(Despawn)) as Despawn;

        AudioClip Clip = SM.ImpactSounds[Random.Range(0, SM.ImpactSounds.Count)];

        DSpawn.DespawnTime = Clip.length + 0.1f;
        AudioSource AS = Sound.AddComponent(typeof(AudioSource)) as AudioSource;
        AS.spatialBlend = 1f;
        AS.maxDistance = 250f;
        AS.clip = Clip;

     

      

        AS.volume = (_volume * (Settings._sfxVolume/100f)) * (Settings._masterVolume/100f);
                
        AS.Play();
    }
}
