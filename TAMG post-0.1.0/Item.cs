using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="New Item", menuName = "Item")]
public class Item : ScriptableObject
{
    public string FileName;
    public string Name;
    public int Value;
    public float SinkSpeed;
    public Mesh Model;
    public float Smoothness;
    public SoundMaterial SM;
}
