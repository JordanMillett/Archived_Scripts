using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapSettings
{
    public int PlayerCount = 0;
    public List<KartConfig> PlayerKarts;
    public int AICount = 0;
    public List<KartConfig> AIKarts;
    public float SpeedModifier = 1;
    public float Laps = 3;
    public bool Items = true;
}
