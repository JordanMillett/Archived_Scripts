using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class SCP : NetworkBehaviour
{
    public float MoveSpeed = 5f;
    public float CheckDistance = 200f;
    public float KillDistance = 1f;

    Rigidbody _rigidbody;
    NetworkIdentity netID;

    public AudioClip KillSound;

    public PlayerGhost Target;

    [SyncVar]
    int Observers = 0;

    void Start()
    {
        netID = GetComponent<NetworkIdentity>();
        _rigidbody = GetComponent<Rigidbody>();

        InvokeRepeating("TargetLoop", 0.25f, 0.25f);
    }

    [Command]
    public void CmdSeen()
    {
        RpcSeen();
    }

    [ClientRpc]
    void RpcSeen()
    {
        Observers++;
    }

    [Command]
    public void CmdUnSeen()
    {
        RpcUnSeen();
    }

    [ClientRpc]
    void RpcUnSeen()
    {
        Observers--;
    }

    void TargetLoop()
    {
        Target = FindTarget();
    }

    void Update()
    {
        if(netID.hasAuthority)
        {
            if(Observers == 0)
            {
                if(Target != null)
                {
                    if(Vector3.Distance(this.transform.position, Target.transform.position) < KillDistance)
                    {
                        Target.CmdKill("Killed by SCP-173");
                        CmdPlayKillSound(Target.transform.position + new Vector3(0f, 1f, 0f));
                        Target = null;
                    }else
                    {
                        _rigidbody.isKinematic = false;
                        LookAt(Target.transform.position);
                        _rigidbody.MovePosition(_rigidbody.position + (transform.forward * MoveSpeed) * Time.fixedDeltaTime);
                    }
                }
            }else
            {
                _rigidbody.isKinematic = true;
            }
        }
    }

    void LookAt(Vector3 Pos)
    {
        Vector3 yaw = Vector3.zero;

        yaw = this.transform.position - Pos;
        yaw = new Vector3(yaw.x, 0f, yaw.z);
        
        this.transform.eulerAngles = new Vector3(0f, -Vector3.SignedAngle(yaw, Vector3.forward, Vector3.up) + 180f, 0f);
    }

    [Command]
    void CmdPlayKillSound(Vector3 Location)
    {
        RpcPlayKillSound(Location);
    }

    [ClientRpc]
    void RpcPlayKillSound(Vector3 Location)
    {
        GameObject Sound = new GameObject();
        Sound.transform.position = Location;
        Despawn DSpawn = Sound.AddComponent(typeof(Despawn)) as Despawn;

        AudioClip Clip = KillSound;

        DSpawn.DespawnTime = Clip.length + 0.1f;
        AudioSource AS = Sound.AddComponent(typeof(AudioSource)) as AudioSource;
        AS.spatialBlend = 1f;
        AS.volume = (.5f * (Settings._sfxVolume/100f)) * (Settings._masterVolume/100f);
        AS.maxDistance = 250f;
        AS.clip = Clip;
        AS.Play();
    }


    //Send destination
    //recieve destination  
    

    PlayerGhost FindTarget()
    {
        List<PlayerGhost> PossibleTargets = new List<PlayerGhost>();

        int layerMask = LayerMask.GetMask("Waypoint");
        Collider[] hitColliders = Physics.OverlapSphere(this.transform.position, CheckDistance, layerMask);
        for(int i = 0; i < hitColliders.Length; i++)
        {
            try
            {
                PlayerGhost PossibleTarget = hitColliders[i].transform.parent.gameObject.GetComponent<PlayerGhost>();

                if(PossibleTarget != null)
                {
                    PossibleTargets.Add(PossibleTarget);
                    
                }
            }
            catch{}
        }

        int closestIndex = -1;

        float closestDistance = CheckDistance + 100f;

        for(int i = 0; i < PossibleTargets.Count; i++)
        {
            float Dist = Vector3.Distance(this.transform.position, PossibleTargets[i].transform.position);
            if(Dist < closestDistance)
            {
                closestDistance = Dist;
                closestIndex = i;
            }
        }

        if(closestIndex == -1)
            return null;

        return PossibleTargets[closestIndex];
    }
}
