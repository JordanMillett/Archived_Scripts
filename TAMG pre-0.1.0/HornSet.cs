using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="New Horn Set", menuName = "Horn Set")]
public class HornSet : ScriptableObject
{
    public List<Horn> Horns;
}