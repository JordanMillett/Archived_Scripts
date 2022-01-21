using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;

public class RollerAgent : Agent
{
    public bool PlayMode = false;

    public Transform Target;

    public float forceMultiplier = 10f;

    Rigidbody r;

    bool Fell = false;
    
    void Start() 
    {
        r = GetComponent<Rigidbody>();
    }

    public override void OnEpisodeBegin()   //RESET WHEN WIN/LOSE
    {
        if(!PlayMode)
        {
            // If the Agent fell, zero its momentum
            if(Fell)
            {
                Fell = false;
                this.r.angularVelocity = Vector3.zero;
                this.r.velocity = Vector3.zero;
                this.transform.localPosition = new Vector3( 0f, 0f, 0f);
            }else
            {
                // Move the target to a new spot
                Target.localPosition = new Vector3(Random.value * 5f - 2.5f, 0f, Random.value * 5f - 2.5f);
            }
        }
        
        
    }

    public override void CollectObservations(VectorSensor sensor)   //SEND INFO
    {
        // Target and Agent positions
        sensor.AddObservation(Target.position);    //3
        sensor.AddObservation(this.transform.position);    //3

        // Agent velocity
        sensor.AddObservation(r.velocity);  //3
    }

    
    public override void OnActionReceived(ActionBuffers actionBuffers)  //UPDATE
    {
        // Actions, size = 2
        Vector3 controlSignal = Vector3.zero;
        controlSignal.x = actionBuffers.ContinuousActions[0];
        controlSignal.z = actionBuffers.ContinuousActions[1];
        r.AddForce(controlSignal * forceMultiplier);

        if(!PlayMode)
        {
            // Rewards
            float distanceToTarget = Vector3.Distance(this.transform.localPosition, Target.localPosition);
            
            // Reached target
            if(distanceToTarget < 1f)
            {
                SetReward(1.0f);
                EndEpisode();
            }

            // Fell off platform
            if(distanceToTarget > 5f)
            {
                Fell = true;
                EndEpisode();
            }
        }
    }

    public override void Heuristic(in ActionBuffers actionsOut)  //CONTROL OVERRIDE
    {
        var continuousActionsOut = actionsOut.ContinuousActions;
        continuousActionsOut[0] = Input.GetAxis("Horizontal");
        continuousActionsOut[1] = Input.GetAxis("Vertical");
    }
}