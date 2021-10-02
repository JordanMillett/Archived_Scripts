using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomSound : MonoBehaviour
{
    public List<AudioClip> Sounds;

    AudioSource AS;

    void Start()
    {
        AS = GetComponent<AudioSource>();
        AS.clip = Sounds[Random.Range(0, Sounds.Count)];
        AS.volume = 0.3f;
        AS.Play();
    }
}