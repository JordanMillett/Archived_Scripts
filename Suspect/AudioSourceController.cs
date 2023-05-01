using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioSourceController : MonoBehaviour
{
    public enum SoundTypes
    {
        SFX,
        Music,
        Voice
    }

    public SoundTypes SoundType = SoundTypes.SFX;

    bool VolumeInitialized = false;
    float Volume = 0f;
    public bool PlayOnStart = false;
    public bool PlayFromStart = true;
    public bool InitializeFromSource = false;
    public AudioClip Sound;
    AudioSource AS;
    AudioLowPassFilter Muffle;
    float cutoff = 22000f;

    void Start()
    {
        if(InitializeFromSource)
        {
            if(!AS)
                AS = GetComponent<AudioSource>();
            if(!Muffle)
                Muffle = GetComponent<AudioLowPassFilter>();
            Volume = AS.volume;
            VolumeInitialized = true;
        }
        
        if(PlayOnStart)
            Play();
    }
    
    void Update()
    {
        if(AS.loop)
            Refresh();
    }

    public void Refresh()
    {
        if(!AS)
            AS = GetComponent<AudioSource>();
        if(!Muffle)
            Muffle = GetComponent<AudioLowPassFilter>();

        switch(SoundType)
        {
            case SoundTypes.SFX : AS.volume = (Volume * (Game.SettingsData._sfxVolume/100f)) * (Game.SettingsData._masterVolume/100f); break;
            case SoundTypes.Music : AS.volume = (Volume * (Game.SettingsData._musicVolume/100f)) * (Game.SettingsData._masterVolume/100f); break;
            case SoundTypes.Voice : AS.volume = (Volume * (Game.SettingsData._voiceVolume/100f)) * (Game.SettingsData._masterVolume/100f); break;
        }
        
        
        RaycastHit[] hits;

        Vector3 dir = UIManager.instance.Cam.transform.position - this.transform.position;
        float dist = Vector3.Distance(this.transform.position, UIManager.instance.Cam.transform.position);
        dist = Mathf.Clamp(dist, 0f, AS.maxDistance);
        hits = Physics.RaycastAll(this.transform.position, dir.normalized, dist, LayerMask.GetMask("Blocker"));

        //Debug.Log("RAYCALL");

        cutoff = Mathf.Lerp(cutoff, hits.Length == 0f ? Mathf.Lerp(22000f, 0f, dist / AS.maxDistance) : Mathf.Lerp(6000f, 0f, dist / AS.maxDistance), Time.deltaTime * 4f);
        //Debug.Log(cutoff);
        Muffle.cutoffFrequency = cutoff;
        //Muffle.cutoffFrequency = hits.Length == 0f ? Mathf.Lerp(22000f, 0f, dist/AS.maxDistance) : Mathf.Lerp(6000f, 0f, dist/AS.maxDistance);
    }

    public void Play()
    {
        if (!PlayFromStart)
        {
            PlayAtTime(Random.Range(0f, Sound.length));
            return;
        }
        
        if(!AS)
            AS = GetComponent<AudioSource>();

        AS.clip = Sound;
        AS.time = 0f;
        Refresh();
        AS.Play();
        if(!VolumeInitialized)
            Debug.LogError(this.gameObject.name + " volume not initialized in " + this.transform.root.gameObject);
    }

    public void PlayAtTime(float _time)
    {
        if(!AS)
            AS = GetComponent<AudioSource>();
        
        AS.clip = Sound;
        AS.time = _time;
        Refresh();
        AS.Play();
        if(!VolumeInitialized)
            Debug.LogError(this.gameObject.name + " volume not initialized in " + this.transform.root.gameObject);
    }

    public void Stop()
    {
        if(!AS)
            AS = GetComponent<AudioSource>();
            
        AS.Stop();
    }

    public void SetVolume(float _volume)
    {
        if(!AS)
            AS = GetComponent<AudioSource>();
        
        Volume = _volume;
        VolumeInitialized = true;
        Refresh();
    }

    public void SetPitch(float _pitch)
    {
        if(!AS)
            AS = GetComponent<AudioSource>();

        AS.pitch = _pitch;
    }
}