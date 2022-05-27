using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="New Weapon Sound Group", menuName = "Weapon Sound Group")]
public class WeaponSoundGroup : ScriptableObject
{
    public List<AudioClip> Clips;
}