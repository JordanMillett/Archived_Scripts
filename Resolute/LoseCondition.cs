using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoseCondition : MonoBehaviour
{
    public StanderAgent Agent;

    void OnCollisionEnter(Collision Col)
    {
        if(Col.transform.gameObject.CompareTag("Lose"))
        {
            if(Agent.FellOver == false)
                Agent.FellOver = true;
        }
    }
}