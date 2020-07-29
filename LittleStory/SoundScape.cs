using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundScape : MonoBehaviour
{
    public SoundPreset SP;

    AudioSource AC;

    void Start()
    {
        Invoke("PlaySounds", Random.Range(.5f, 5f));
    }
    
    void PlaySounds()
    {
        AC = GetComponent<AudioSource>();
        foreach (SoundPreset.ClipData CD in SP.SoundClips)
        {
            float Chance = Random.Range(0f, 100f);
            if(Chance < CD.ChanceToSpawn)
            {
                AC.clip = CD.Clip;
                AC.PlayDelayed(Random.Range(0f, CD.Clip.length));
            }
        }
    }
}
