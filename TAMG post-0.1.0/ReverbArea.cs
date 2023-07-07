using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReverbArea : MonoBehaviour
{
    public AudioReverbPreset SoundType;

    AudioReverbFilter A;
    
    void Start()
    {
        A = GameObject.FindWithTag("Camera").GetComponent<AudioReverbFilter>();
    }

    void OnTriggerEnter(Collider col) 
    {
        bool Yes = false;

        if(col.transform.root.gameObject.CompareTag("Player"))
            Yes = true;
        if(col.transform.root.GetComponent<Vehicle>())
            if(col.transform.root.GetComponent<Vehicle>().Driving)
                Yes = true;

        if(Yes)
            A.reverbPreset = SoundType;

    }

    void OnTriggerExit(Collider col)
    {
        bool Yes = false;

        if(col.transform.root.gameObject.CompareTag("Player"))
            Yes = true;
        if(col.transform.root.GetComponent<Vehicle>())
            if(col.transform.root.GetComponent<Vehicle>().Driving)
                Yes = true;

        if(Yes)
            A.reverbPreset = AudioReverbPreset.Off;
    }
}
