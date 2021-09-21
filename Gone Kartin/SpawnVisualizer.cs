using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnVisualizer : MonoBehaviour
{
    void OnDrawGizmos()
    {
        Gizmos.color = new Color(0f, 0f, 1f, 0.5f);
        Gizmos.DrawSphere(this.transform.position, 1f);
    }
}
