using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Plant : MonoBehaviour 
{

    public float GrowRate = 1f;
    public GameObject[] Stages;

    int CurrentStage = 0;
    int LastStage;
    bool Finished = false;

	void Start () 
    {
        LastStage = Stages.Length - 1;
		InvokeRepeating("Grow",GrowRate,GrowRate);
	}

    void Grow()
    {

        if(!Finished)
        {

            CurrentStage++;

            for(int i = 0; i <= LastStage;i++)
                if(i == CurrentStage)
                    Stages[i].SetActive(true);
                else
                    Stages[i].SetActive(false);
           
            if(CurrentStage == LastStage)
                Finished = true;
            

        
        }else
        {
            Destroy(this);
        }

    }
}
