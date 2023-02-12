using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="New Sound Group", menuName = "Sound Group")]
public class SoundGroup : ScriptableObject
{
    public float Volume = 1f;
    public List<AudioClip> Clips;
    
    public AudioClip GetRandom()
    {
        return Clips[Random.Range(0, Clips.Count)];
    }
}