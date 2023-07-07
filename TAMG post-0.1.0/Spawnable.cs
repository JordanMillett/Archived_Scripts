using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="New Spawnable", menuName = "Spawnable")]
public class Spawnable : ScriptableObject
{
    public GameObject Prefab;
    public bool HideInConsole;
    public bool PopSound;
    public float ConsoleOffset;
}