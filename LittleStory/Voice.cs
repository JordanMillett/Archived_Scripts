using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="New Voice", menuName = "Voice")]
public class Voice : ScriptableObject
{
    public float Pitch = 1f;
    public int LetterReadRate = 1;
    public List<AudioClip> Letters;
}
