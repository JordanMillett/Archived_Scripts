using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Mirror;
using UnityEngine.SceneManagement;

public class MirrorVehicle : NetworkBehaviour
{
    [SyncVar]
    public float EngineAlpha = 0f;

    [SyncVar]
    public float SkidAmount = 0f;

    [SyncVar]
    public float TurnAmount = 0f;

    [SyncVar]
    public bool LightsOn = false;

    [HideInInspector]
    public Vehicle MRD;

    void Awake()
    {
        MRD = GetComponent<Vehicle>();
    }

    [ClientRpc]
    void RpcPlayHorn()
    {
        MRD.AS_Horn.volume = (.4f * (Settings._sfxVolume/100f)) * (Settings._masterVolume/100f);
        MRD.AS_Horn.clip = MRD.Config.InstallableHorns.Horns[0].Sound;
        MRD.AS_Horn.Play();
    }

    [ClientRpc]
    void RpcToggleLights()
    {
        LightsOn = !LightsOn;
        MRD.Lights.gameObject.SetActive(LightsOn);
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

    [Command]
    public void CmdSendEngineInfo(float _EngineAlpha)
    {
        EngineAlpha = _EngineAlpha;
    }
}
