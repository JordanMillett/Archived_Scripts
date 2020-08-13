using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowPlayer : MonoBehaviour
{

    GameObject Player;
    MapGenerator MG;
    ShipController SC;

    float UpdateTime;
    float Distance = 0f;
    float VisionRadius = 8f;

    Vector3 CurrentRandomDestination = Vector3.zero;

    void Start()
    {
        Player = GameObject.FindWithTag("Player").gameObject;
        SC = GetComponent<ShipController>();
        MG = transform.parent.GetComponent<MapGenerator>();

        UpdateTime = Random.Range(15f, 30f);
        InvokeRepeating("UpdateRanPos", 0f, UpdateTime);
    }

    void Update()
    {
        Distance = Vector3.Distance(Player.transform.position, this.transform.position);

        if(Distance < VisionRadius && !Player.GetComponent<ShipController>().Busy)
            SC.GoTo(Player.transform.position);
        else
            SC.GoTo(CurrentRandomDestination);
    }

    void UpdateRanPos()
    {
        CurrentRandomDestination = new Vector3(Random.Range(-MG.MapSize.x/2f, MG.MapSize.x/2f), 0f, Random.Range(-MG.MapSize.y/2f, MG.MapSize.y/2f));
    }
}
