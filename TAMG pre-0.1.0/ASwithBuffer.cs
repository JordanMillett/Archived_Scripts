using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ASwithBuffer : MonoBehaviour
{
    public float[] VoiceBuffer = new float[2048];
    public bool Transmitting = false;
    public AudioSource AS;

    //bool ReadyToReceive = false;
    /*
    void Update()
    {
        Transmitting = false;
    }*/

    void Start()
    {
        VoiceBuffer = new float[2048];
    }

    void OnAudioFilterRead(float[] data, int channels)
    {
        if(Transmitting)
        {   
            //int shortMax = 30000; //32767
            for(int i = 0; i < data.Length; i++)    //(((VoiceBuffer[Count] + 1f)/2f) * (shortMax * 2)) - shortMax))
            {
                data[i] = 3f * (VoiceBuffer[i] * (Settings._voiceVolume/100f)) * (Settings._masterVolume/100f);
                
                if(channels == 2)
                {
                    data[i + 1] = data[i];
                    i++;
                }

                //Transmitting = false;
                //float voiceData = (((VoiceBuffer[i] + shortMax)/(shortMax * 2f)) * 2f) - 1f;
                //data[i] = voiceData * Settings._voiceVolume * 0.25f;
                //data[i] = VoiceBuffer[(VoiceBuffer.Length - data.Length) + i];
                //data[i] = ran;
            }
            //Transmitting = false;

            /*
            short[] voiceShort = new short[VoiceBuffer.Length/2];   //4096

            int byteIndex = 0;

            for(int i = 0; i < voiceShort.Length; i++)
            {
                byte[] single = new byte[2]{VoiceBuffer[byteIndex], VoiceBuffer[byteIndex + 1]};
                voiceShort[i] = BitConverter.ToInt16(single, 0);
                byteIndex += 2;
            }

            Buffer.BlockCopy(VoiceBuffer, 0, voiceShort, 0, VoiceBuffer.Length);

            int shortMax = 32767;

            for(int i = 0; i < data.Length; i++)    //(((VoiceBuffer[Count] + 1f)/2f) * (shortMax * 2)) - shortMax))
            {
                data[i] = (((voiceShort[i] + 32767)/(shortMax * 2f)) * 2f) - 1f;
                //data[i] = VoiceBuffer[(VoiceBuffer.Length - data.Length) + i];
                //data[i] = ran;
            }
            //data = VoiceBuffer;
            */
        }
    }
}

/*
public AudioSource emitterSend;
private AudioSource audioSource;    
public AudioClip streamedClip;

public bool debugSampleData;
private float[] sampleDataArray;
public float sampleSize;
public float sampleFreq;

private int outputSampleRate;
public bool _bufferReady;                         


void Start () {

    audioSource = GetComponent<AudioSource>();
    sampleSize = emitterSend.clip.samples;
    sampleFreq = emitterSend.clip.frequency;
    sampleDataArray = new float[2048];
    streamedClip = AudioClip.Create("audiostream", (int)sampleSize, 1, (int)sampleFreq, false);
    audioSource.clip = streamedClip;
    audioSource.Play();
    _bufferReady = false;
}

 private void FixedUpdate()
{

    if (emitterSend.isPlaying && _bufferReady == false)
    {
        FillAudioBuffer();           

    }
    else if (!emitterSend.isPlaying)
    {
        Debug.Log("Emitter is not playing!");
    }

    if (debugSampleData && sampleDataArray != null && Input.GetKeyDown("p"))
    {
        for (int i = 0; i < sampleDataArray.Length; i++)
        {
        Debug.Log(sampleDataArray[i]);
        }

    }
    else if (sampleDataArray == null)
    {
        Debug.Log("No data in array!");
    }


}    

void FillAudioBuffer()
{

    emitterSend.GetOutputData(sampleDataArray, 0);             
    streamedClip.SetData(sampleDataArray, 0);
    _bufferReady = true;
}

void OnAudioFilterRead(float[] data, int channels)
{
    if (_bufferReady)
    {

        for (int i = 0; i < data.Length; i++)
        {
            data[i] = (float)sampleDataArray[i];           

        }

        _bufferReady = false;
        }
    }        

*/
