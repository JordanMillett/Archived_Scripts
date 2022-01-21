using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PositionVisualizer : MonoBehaviour
{
    public Color DisplayColor = new Color(1f, 0f, 0f, 0.5f); 
    public float DisplaySize = 0.5f;

    void OnDrawGizmos()
    {
        Gizmos.color = DisplayColor;
        Gizmos.DrawSphere(this.transform.position, DisplaySize);
    }
}