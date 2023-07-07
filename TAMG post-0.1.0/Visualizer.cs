using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Visualizer : MonoBehaviour
{
    public enum VisualizerType
    {
        PlayerSpawn,
        VehicleSpawn,
        ItemSpawner,
        DeliveryPoint
    }

    struct VisualizerData
    {
	    public VisualizerType Type;
        public Color Hue;
        public float Scale;
    }

    List<VisualizerData> VisualizerList = new List<VisualizerData>()
    {
        new VisualizerData{Type = VisualizerType.PlayerSpawn, Hue = new Color(0f, 0f, 1f, 0.75f), Scale = 1f},
        new VisualizerData{Type = VisualizerType.VehicleSpawn, Hue = new Color(1f, 0f, 0f, 0.75f), Scale = 2f},
        new VisualizerData{Type = VisualizerType.ItemSpawner, Hue = new Color(0f, 1f, 0f, 0.75f), Scale = 3f},
        new VisualizerData{Type = VisualizerType.DeliveryPoint, Hue = new Color(1f, 0f, 1f, 0.75f), Scale = 0.5f}
    };

    public VisualizerType Selected;

    void OnDrawGizmos()
    {
        Gizmos.color = VisualizerList[(int) Selected].Hue;
        Gizmos.DrawSphere(this.transform.position, VisualizerList[(int) Selected].Scale);
    }
}
