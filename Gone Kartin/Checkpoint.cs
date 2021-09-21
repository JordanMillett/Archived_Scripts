using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    public int Index = -1;

    public Map M;

    void OnTriggerEnter(Collider Col)
    {
        if(Col.GetComponent<KartController>() != null)
        {
            if(Col.GetComponent<KartController>().Respawning)
                return;

            if(Index != 0)
            {
                if(Col.GetComponent<KartController>().CheckpointIndex == Index - 1)
                {
                    M.CheckpointHit(Index, Col.GetComponent<KartController>());
                }
            }else
            {
                if(Col.GetComponent<KartController>().CheckpointIndex == M.CheckpointCount - 1)
                {
                    M.CheckpointHit(Index, Col.GetComponent<KartController>());
                }
            }
            
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = new Color(1f, 0f, 0f, 0.5f);
        for(int i = 0; i < 5f; i++)
            Gizmos.DrawSphere(this.transform.position + (this.transform.forward * i), .5f);
    }
}
