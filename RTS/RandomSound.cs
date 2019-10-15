using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomSound : MonoBehaviour
{
    
    public AudioClip[] Sounds;
    AudioSource Source;

    void Start()
    {
        Source = GetComponent<AudioSource>();
        Source.clip = Sounds[Random.Range(0, Sounds.Length)];
        Source.enabled = true;
    }

}
