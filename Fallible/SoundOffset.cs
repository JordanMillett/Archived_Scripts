using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundOffset : MonoBehaviour
{
    void Start()
    {
        GetComponent<AudioSource>().time = Random.Range(0f, GetComponent<AudioSource>().clip.length);
    }
}