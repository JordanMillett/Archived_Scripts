using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PositionVisualizer : MonoBehaviour
{
    void OnDrawGizmos()
    {
        Gizmos.color = new Color(1f, 0f, 0f, 0.5f);
        Gizmos.DrawSphere(this.transform.position, 0.5f);
    }
}