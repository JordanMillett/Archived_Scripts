using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissionController : MonoBehaviour
{

    public ObjectiveArrow OA;

    public Mission CurrentMission;

    void Update()
    {  
        if(CurrentMission == null)
        {
            OA.Visible = false;
        }else
        {
            OA.Visible = true;
            OA.Destination = CurrentMission.transform.position;
        }
    }

    void OnDisable()
    {
        OA.Target.SetActive(false);
        OA.Visible = false;
    }

    public void Finish()
    {
        OA.Target.SetActive(false);
        OA.Visible = false;
        GetComponent<PlayerInformation>().Credits += CurrentMission.CreditGain;
        CurrentMission.P.PlayerReputation += CurrentMission.ReputationGain;
        Destroy(CurrentMission);
    }
}
