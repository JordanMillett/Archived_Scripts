using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="New Sound Material", menuName = "Sound Material")]
public class SoundMaterial : ScriptableObject
{
    public List<AudioClip> ImpactSounds;
}