using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="New Horn", menuName = "Horn")]
public class Horn : ScriptableObject
{
    public AudioClip Sound;

    public int Cost = 0;
}