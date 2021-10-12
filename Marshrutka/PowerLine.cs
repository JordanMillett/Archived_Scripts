using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerLine : MonoBehaviour
{
    void Start()
    {
        List<PowerPole> Poles = new List<PowerPole>();

        foreach (Transform child in this.transform)
        {
            Poles.Add(child.GetComponent<PowerPole>());
        }

        int Connections = Poles[0].Connections.Count;

        for(int c = 0; c < Connections; c++)
        {
            for(int i = 0; i < Poles.Count - 1; i++) //for every pole but the last
            {
                Poles[i].Connections[c].Initialize
                (
                    Poles[i].Positions[c].position,     //start
                    Poles[i + 1].Positions[c].position  //end
                );

                /*
                Children[i].GetChild(0).GetChild(c).GetChild(0).GetComponent<Wire>().Initialize
                (
                    Children[i].GetChild(0).GetChild(c).GetChild(1).position,       //start 
                    Children[i + 1].GetChild(0).GetChild(c).GetChild(1).position    //end
                );*/
            }
        }
    }
}
