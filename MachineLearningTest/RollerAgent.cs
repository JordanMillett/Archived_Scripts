using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;

public class RollerAgent : Agent
{
    //DATATYPES

    //PUBLIC COMPONENTS
    public Transform Target;

    //PUBLIC VARS
    public float forceMultiplier = 10f;

    //PUBLIC LISTS

    //COMPONENTS
    Rigidbody r;

    //VARS

    //LISTS
    float distanceToTarget = 2f;
    
    void Start() 
    {
        r = GetComponent<Rigidbody>();
    }

    public override void OnEpisodeBegin()   //RESET WHEN WIN/LOSE
    {
        // If the Agent fell, zero its momentum
        if(distanceToTarget > 5f)
        {
            this.r.angularVelocity = Vector3.zero;
            this.r.velocity = Vector3.zero;
            this.transform.localPosition = new Vector3( 0f, 0f, 0f);
        }else
        {
            // Move the target to a new spot
            Target.localPosition = new Vector3(Random.value * 8 - 4, Random.value * 8 - 4, Random.value * 8 - 4);
        }
        
        
    }

    public override void CollectObservations(VectorSensor sensor)   //SEND INFO
    {
        // Target and Agent positions
        sensor.AddObservation(Target.localPosition);    //3
        sensor.AddObservation(this.transform.localPosition);    //3

        // Agent velocity
        sensor.AddObservation(r.velocity.x);
        sensor.AddObservation(r.velocity.y);
        sensor.AddObservation(r.velocity.z);
    }

    
    public override void OnActionReceived(ActionBuffers actionBuffers)  //UPDATE
    {
        // Actions, size = 2
        Vector3 controlSignal = Vector3.zero;
        controlSignal.x = actionBuffers.ContinuousActions[0];
        controlSignal.y = actionBuffers.ContinuousActions[1];
        controlSignal.z = actionBuffers.ContinuousActions[2];
        r.AddForce(controlSignal * forceMultiplier);

        // Rewards
        distanceToTarget = Vector3.Distance(this.transform.localPosition, Target.localPosition);

        // Reached target
        if (distanceToTarget < 1.5f)
        {
            SetReward(1.0f);
            EndEpisode();
        }

        // Fell off platform
        /*
        if (distanceToTarget > 5f)
        {
            EndEpisode();
        }
        */
    }

    public override void Heuristic(in ActionBuffers actionsOut)  //CONTROL OVERRIDE
    {
        var continuousActionsOut = actionsOut.ContinuousActions;
        continuousActionsOut[0] = Input.GetAxis("Horizontal");
        continuousActionsOut[1] = Input.GetAxis("Vertical");
    }
}