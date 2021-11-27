using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="New Radio Station", menuName = "Radio Station")]
public class RadioStation : ScriptableObject
{
    public string Name;
    public Texture2D Image;
    public List<AudioClip> Songs;
}