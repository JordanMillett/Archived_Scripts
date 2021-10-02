using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;

public class Commander : Agent
{
    //DATATYPES
    public enum Team
    {
        Blue,
        Red
    };

    const float FarthestDistance = 50f;

    //PUBLIC COMPONENTS
    public Commander Enemy;
    public Transform EnemyFlag;
    public Team _team;
    public Transform Spawn;
    public Transform Flag;
    public GameObject UnitPrefab;

    //PUBLIC VARS
    public bool Lost = false;
    public float ClosestDistance = FarthestDistance;

    //PUBLIC LISTS
    public List<Unit> Units;

    //COMPONENTS

    //VARS

    //LISTS
    
    void Start() 
    {
        for(int i = 0; i < Units.Count; i++)
            Units[i].Initialize(this);
    }

    public override void OnEpisodeBegin()   //RESET WHEN WIN/LOSE
    {
        for(int i = 0; i < Units.Count; i++)
            Units[i].Reset();
            
        Lost = false;
        ClosestDistance = FarthestDistance;
    }

    public override void CollectObservations(VectorSensor sensor)   //SEND INFO
    {
        sensor.AddObservation(Flag.localPosition);
        sensor.AddObservation(EnemyFlag.localPosition); //2 times

        for(int i = 0; i < Units.Count; i++)    //8 times
        {
            sensor.AddObservation(Units[i].transform.localPosition);
        }

        for(int i = 0; i < Enemy.Units.Count; i++)    //8 times
        {
            sensor.AddObservation(Enemy.Units[i].transform.localPosition);
        }
    }

    public override void OnActionReceived(ActionBuffers actionBuffers)  //UPDATE
    {
        bool DoNothing = actionBuffers.ContinuousActions[0] < 0f;

        if(!DoNothing)
        {
            int Index = Mathf.FloorToInt(actionBuffers.ContinuousActions[1] * Units.Count);
            if(Index > Units.Count - 1)
                Index = Units.Count - 1;
            if(Index < 0)
                Index = 0;

            Units[Index].GoTo(actionBuffers.ContinuousActions[2], actionBuffers.ContinuousActions[3]);
        }

        SetReward(ClosestDistance/FarthestDistance);

        if(Lost || Enemy.Lost)
        {
            EndEpisode();
        }
    }
}