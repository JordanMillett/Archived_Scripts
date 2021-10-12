using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="New Station", menuName = "Station")]
public class Station : ScriptableObject
{
    public List<AudioClip> Songs;
}