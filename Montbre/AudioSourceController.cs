using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioSourceController : MonoBehaviour
{
    bool VolumeInitialized = false;
    float Volume = 0f;
    public bool SyncWithTimeScale = true;
    public bool PlayOnStart = false;
    public bool PlayFromStart = true;
    public bool InitializeFromSource = false;
    public AudioClip Sound;
    AudioSource AS;
    public SoundGroup Sounds;

    void Start()
    {
        if(InitializeFromSource)
        {
            if(!AS)
                AS = GetComponent<AudioSource>();
            Volume = AS.volume;
            VolumeInitialized = true;
        }
        
        if(PlayOnStart)
            Play();
    }
    
    void Update()
    {
        Refresh();
    }

    public void Refresh()
    {
        if(!AS)
            AS = GetComponent<AudioSource>();

        AS.volume = (Volume * (Settings._sfxVolume/100f)) * (Settings._masterVolume/100f);

        if(SyncWithTimeScale && Time.timeScale == 0f)
            AS.Pause();
        else
            AS.UnPause();
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

    public void PlayRandom()
    {
        if(!AS)
            AS = GetComponent<AudioSource>();

        AS.clip = Sounds.GetRandom();
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