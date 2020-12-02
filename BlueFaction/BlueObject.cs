using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlueObject : MonoBehaviour
{
    public BluePart[] Parts;
    public List<int> GroundIndices;

    public void GenerateDistances()
    {
        Parts = GetComponentsInChildren<BluePart>();

        int Roots = 0;
        foreach (BluePart BP in Parts)
            if(BP.GroundPart)
            {
                Roots++;
            }

        for (int i = 0; i < Parts.Length;i++)
        {
            Parts[i].Init(Roots, i);
        }

        int GroundIndex = 0;
        for (int i = 0; i < Parts.Length;i++)
        {
            if(Parts[i].GroundPart)
            {
                GroundIndices.Add(i);
                Parts[i].SetDistance(GroundIndex, 0);
                GroundIndex++;
            }
        }
    }

    public void RootDestroyed(int Index)        //3
    {
        int Value = 0;
        for (int i = 0; i < GroundIndices.Count;i++)  //List of each ground part
        {
            if(Index == GroundIndices[i])              
            {
                Value = i;
                foreach (BluePart BP in Parts)        
                {
                    BP.RootDestroyed(Value);      
                }
            }
        }
    }
}
