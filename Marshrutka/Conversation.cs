using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="New Conversation", menuName = "Conversation")]
public class Conversation : ScriptableObject
{
    public List<string> IntroLines;
    public List<string> AcceptedLines;
    public List<string> DenialLines;
}
