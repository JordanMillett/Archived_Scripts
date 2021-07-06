using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Mirror;
using UnityEngine.SceneManagement;

public class MirrorVehicle : NetworkBehaviour
{
    public SyncList<Color> Colors = new SyncList<Color>()
    {
        new Color(239f/255f,240f/255f,215f/255f,1), //Paint
        new Color(94f/255f,107f/255f,130f/255f,1),  //Body
        new Color(248f/255f,229f/255f,116f/255f,1), //Seat
        new Color(55f/255f,50f/255f,84f/255f,1),    //Tire
        new Color(155f/255f,156f/255f,130f/255f,1) //Wheel
    };

    public SyncList<int> CurrentPartsIndices = new SyncList<int>()
    {
        0,
        1,
        2
    };

    [SyncVar]
    public int KitIndex = 0;

    [SyncVar]
    public int HornIndex = 0;

    GameManager GM;

    List<Material[]> Mats = new List<Material[]>();
    List<MeshRenderer> Meshes = new List<MeshRenderer>();

    [SyncVar]
    public bool Occupied = false;

    [SyncVar]
    public float EngineAlpha = 0f;

    [SyncVar]
    public float SkidAmount = 0f;

    [SyncVar]
    public float TurnAmount = 0f;

    [SyncVar]
    public bool Locked = false;

    [SyncVar]
    public bool LightsOn = false;

    public Vehicle MRD;

    public void Initialize()
    {
        MRD = GetComponent<Vehicle>();
    }

    [ClientRpc]
    public void RpcRefreshVehicle() //Called to refresh every client with the new vehicle stats
    {
        List<int> NewPartsIndices = new List<int>();        //Set the parts
        foreach(int CPI in CurrentPartsIndices)
        {
            NewPartsIndices.Add(CPI);
        }

        MRD.SetParts(NewPartsIndices);

        /*
        Colors.Clear();
        foreach(Color C in _colors)
        {
            Colors.Add(C);
        }*/

        //HornIndex = _hornIndex;
        Mats.Clear();
        for(int i = 0; i < MRD.Meshes.Count; i++)
        {
            Mats.Add(MRD.Meshes[i].materials);
        }

        for(int i = 0; i < Mats.Count; i++)
        {
            foreach(Material M in Mats[i])
            {
                switch (M.name)
                {
                    case "Paint (Instance)" :
                        M.SetColor("_albedo", Colors[0]);
                    break;
                    case "Body (Instance)" :
                        M.SetColor("_albedo", Colors[1]);
                    break;
                    case "Seat (Instance)" :
                        M.SetColor("_albedo", Colors[2]);
                    break;
                    case "Tire (Instance)" :
                        M.SetColor("_albedo", Colors[3]);
                    break;
                    case "Wheel (Instance)" :
                        M.SetColor("_albedo", Colors[4]);
                    break;
                }
            }
        }

        //MRD.Horn = MRD.Config.InstallableHorns.Horns[HornIndex].Sound;
    }

    [ClientRpc]
    void RpcPlayHorn()
    {
        MRD.AS_Horn.volume = (.4f * (Settings._sfxVolume/100f)) * (Settings._masterVolume/100f);
        MRD.AS_Horn.clip = MRD.Config.InstallableHorns.Horns[HornIndex].Sound;
        MRD.AS_Horn.Play();
    }

    [ClientRpc]
    void RpcToggleLights()
    {
        LightsOn = !LightsOn;
        MRD.Lights.gameObject.SetActive(LightsOn);
    }

    [ClientRpc]
    void RpcToggleLocked()
    {
        Locked = !Locked;

        if(Locked == false)
        {
            Destroy(MRD.PA);
            this.gameObject.tag = "Untagged";
            MRD.Effects.SetActive(true);
        }else
        {
            MRD.PA = this.gameObject.AddComponent(typeof(PurchaseAble)) as PurchaseAble;
            MRD.PA.Name = MRD.Config.VehicleName;
            MRD.PA.Cost = MRD.Config.VehicleBaseValue;
            MRD.Effects.SetActive(false);
        }
    }

    [ClientRpc]
    void RpcPlayCrash(Vector3 CrashPos)
    {
        GameObject Sound = new GameObject();
        Sound.transform.position = CrashPos;
        Sound.transform.SetParent(this.transform);
        Despawn DSpawn = Sound.AddComponent(typeof(Despawn)) as Despawn;

        AudioClip Clip = MRD.Config.CrashSound;

        DSpawn.DespawnTime = Clip.length + 0.1f;
        AudioSource AS = Sound.AddComponent(typeof(AudioSource)) as AudioSource;
        AS.spatialBlend = 1f;
        AS.volume = (.1f * (Settings._sfxVolume/100f)) * (Settings._masterVolume/100f);
        AS.maxDistance = 1000f;
        AS.clip = Clip;
        AS.Play();
    }

    [Command]
    public void CmdRefreshVehicle()
    {
        RpcRefreshVehicle();
    }

    [Command]
    public void CmdSetOccupied(bool _occupied) //no rpc needed, sync var gets synced
    {
        Occupied = _occupied;
    }

    [Command]
    public void CmdPlayHorn()
    {
        RpcPlayHorn();
    }

    [Command]
    public void CmdToggleLights()
    {
        RpcToggleLights();
    }

    [Command]
    public void CmdToggleLocked()
    {
        RpcToggleLocked();
    }

    [Command]
    public void CmdPlayCrash(Vector3 CrashPos)
    {
        RpcPlayCrash(CrashPos);
    }

    [Command]
    public void CmdSendWheelInfo(float _turnAmount)
    {
        TurnAmount = _turnAmount;
    }

    [Command]
    public void CmdSendSkidInfo(float _skidAmount)
    {
        SkidAmount = _skidAmount;
    }

    [Command]       //Command called by owner changes a SyncVar, server then updates all client because SyncVar
    public void CmdSendEngineInfo(float _EngineAlpha)
    {
        EngineAlpha = _EngineAlpha;
    }
}
