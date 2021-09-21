using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="New Track", menuName = "Track")]
public class Track : ScriptableObject
{
    public string BuildName;
    public string TrackName;
    public string TrackDescription;
    public int SpawnCount;
    public int LapCount;
    public Texture2D Icon; 
}
