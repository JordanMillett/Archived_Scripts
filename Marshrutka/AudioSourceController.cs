using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioSourceController : MonoBehaviour
{
    public enum AudioType
    {
        SFX,
        Music
    }

    public AudioType Type;
    public float Volume = 1f;
    public bool SyncWithTimeScale = true;
    public bool PlayOnStart = false;
    public bool Offsetted = false;
    //public bool IsPlaying = false;
    public AudioClip Sound;
    public AudioSource AS;

    public bool Culled = false;
    Transform Hear;
    bool Inited = false;


    void Start()
    {


        if(PlayOnStart)
        {
            if(Offsetted)
            {
                Invoke("Play", Random.Range(0f, 0.25f));
            }else
            {
                Play();
            }
        }
    }

    void Init()
    {
        AS = GetComponent<AudioSource>();
        Hear = GameObject.FindWithTag("Manager").GetComponent<Manager>().GetListenerTransform();
        Inited = true;
    }

    public void Refresh()
    {
        if(!Inited)
            Init();

        if(Type == AudioType.SFX)
            AS.volume = (Volume * (Settings._sfxVolume/100f)) * (Settings._masterVolume/100f);
        else
            AS.volume = (Volume * (Settings._musicVolume/100f)) * (Settings._masterVolume/100f);

        if(SyncWithTimeScale && Time.timeScale == 0f)
        {
            AS.Pause();
        }else
        {
            AS.UnPause();
        }

        if(AS.spatialBlend > 0f)
        {
            if(Culled && AS.loop == true)
            {
                if(Vector3.Distance(Hear.position, AS.transform.position) < AS.maxDistance)
                {
                    Culled = false;
                    AS.Play();
                }
            }else if(!Culled && AS.loop == true)
            {
                if(Vector3.Distance(Hear.position, AS.transform.position) > AS.maxDistance)
                {
                    Culled = true;
                    AS.Stop();
                }
            }
        }
    }

    public void Play()
    {
        if(!Inited)
            Init();

        AS.clip = Sound;
        AS.time = 0f;
        Refresh();
        if(AS.spatialBlend > 0f)
        {
            if(Vector3.Distance(Hear.position, AS.transform.position) < (AS.maxDistance * 1.25f))
            {
                Culled = false;
                AS.Play();
            }
            else
            {
                Culled = true;
                AS.Stop();
            }
        }else
        {
            AS.Play();
        }
    }

    public void PlayAtTime(float _time)
    {
        //Debug.Log("Time : " + _time);

        if(!Inited)
            Init();
        
        AS.clip = Sound;
        AS.time = _time;
        Refresh();
        if(AS.spatialBlend > 0f)
        {
            if(Vector3.Distance(Hear.position, AS.transform.position) < (AS.maxDistance * 1.25f))
            {
                Culled = false;
                AS.Play();
            }
            else
            {
                Culled = true;
                AS.Stop();
            }
        }else
        {
            AS.Play();
        }
    }

    public void Stop()
    {
        if(!Inited)
            Init();
            
        AS.Stop();
    }

    public void SetVolume(float _volume)
    {
        if(!Inited)
            Init();
        
        Volume = _volume;
        Refresh();
    }

    public void SetPitch(float _pitch)
    {
        if(!Inited)
            Init();

        AS.pitch = _pitch;
    }
}
