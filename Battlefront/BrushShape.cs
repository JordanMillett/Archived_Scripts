using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="New Brush Shape", menuName = "Brush Shape")]
public class BrushShape : ScriptableObject
{
    public List<Vector2> vertices;
    public float normalizingScale;
    public string shapeName;
}
