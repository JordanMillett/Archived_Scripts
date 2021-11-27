using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="New Item", menuName = "Item")]
public class Item : ScriptableObject
{
    public enum ItemSize
	{
		Small,
        Medium,
        Large
	};
    
    public string FileName;
    public string Name;
    public int Value;
    public Mesh Model;
    public ItemSize Size;
    public SoundMaterial SM;
}
