using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="New Caller", menuName = "Caller")]
public class Caller : ScriptableObject
{
    public string CallerID;
    public Voice CallerVoice;
    public List<string> Lines;
}
