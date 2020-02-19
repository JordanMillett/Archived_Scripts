using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="New Animation Clip", menuName = "Animation Clip")]
public class ClipData : ScriptableObject
{
    //public float AlphaSpeed; public list<float> AlphaSpeed;
    //public float EndPauseTime;
    public string Name;
    public List<Vector3> Positions;
    public List<Quaternion> Rotations;
}
