using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CommandPost : MonoBehaviour
{

    public Material Good;
    public Material Bad;
    public Material Neutral;

    public int State = 0;

    public MeshRenderer FlagMesh;
    public MeshRenderer EffectMesh;

    public int CurrentGood = 0;
    public int CurrentBad = 0;

    void Start()
    {
        SetState(State);
    }

    public void Refresh()
    {

        //if(CurrentBad == CurrentGood)
            //SetState(0);

        if(CurrentBad < CurrentGood)
            SetState(1);

        if(CurrentBad > CurrentGood)
            SetState(2);

    }

    public void SetState(int Index)
    {

        if(Index == 0)
        {
            State = 0;
            EffectMesh.material.SetFloat("_team", 0f);
            EffectMesh.material.SetFloat("_uncaptured", 1f);
            FlagMesh.material = Neutral;
            
        }else if(Index == 1)
        {
            State = 1;
            EffectMesh.material.SetFloat("_team", 0f);
            EffectMesh.material.SetFloat("_uncaptured", 0f);
            FlagMesh.material = Good;

        }else if(Index == 2)
        {
            State = 2;
            EffectMesh.material.SetFloat("_team", 1f);
            EffectMesh.material.SetFloat("_uncaptured", 0f);
            FlagMesh.material = Bad;

        }

    }
}
