using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="New Sound Preset", menuName = "Sound Preset")]
public class SoundPreset : ScriptableObject
{
    [System.Serializable]
    public struct ClipData
    {
        public AudioClip Clip;
        public float Volume;
        public float ChanceToSpawn;
    }

    public List<ClipData> SoundClips;

}
