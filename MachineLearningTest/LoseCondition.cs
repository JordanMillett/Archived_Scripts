using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoseCondition : MonoBehaviour
{
    public WalkerAgent Agent;

    void OnCollisionEnter(Collision Col)
    {
        if(!Col.transform.gameObject.CompareTag("Lower"))
        {
            if(Agent.FellOver == false)
                Agent.FellOver = true;
        }
    }
}