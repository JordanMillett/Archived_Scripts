using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Jello;

public class UnitStats : MonoBehaviour
{
    public string Name;
    public int Kills;
    public int Deaths;

    void Start()
    {

        Name = Jello.Tools.RandomName();
        GameObject.FindGameObjectWithTag("Stats").GetComponent<StatsManager>().UnitData.Add(this);

    }
}
