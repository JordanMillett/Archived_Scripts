using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using Random = UnityEngine.Random;

public class StanderAgent : Agent
{
    public bool Playmode = false;
    public bool FellOver = false;

    public Transform Target;


    public List<Transform> Parts = new List<Transform>();

    JointDriveController Controller;

    public override void Initialize()
    {
        Controller = GetComponent<JointDriveController>();

        for (int i = 0; i < Parts.Count; i++)
            Controller.SetupBodyPart(Parts[i]);
    }

    public override void OnEpisodeBegin()
    {
        FellOver = false;
        Target.localPosition = new Vector3((Random.value * 10f) - 5f, 1f, (Random.value * 10f) - 5f);

        foreach (var bodyPart in Controller.bodyPartsDict.Values)
        {
            bodyPart.Reset(bodyPart);
        }
    }

    public void CollectObservationBodyPart(BodyPart bp, VectorSensor sensor)
    {
        if (bp.rb.transform != Parts[0])
        {
            sensor.AddObservation(bp.currentStrength / Controller.maxJointForceLimit);
        }
    }

    /// <summary>
    /// Loop over body parts to add them to observation.
    /// </summary>
    public override void CollectObservations(VectorSensor sensor)
    {
        RaycastHit hit;
        float maxRaycastDist = 10;
        if (Physics.Raycast(Parts[0].position, Vector3.down, out hit, maxRaycastDist))
        {
            sensor.AddObservation(hit.distance / maxRaycastDist);
        }
        else
            sensor.AddObservation(1);

        foreach (var bodyPart in Controller.bodyPartsList)
        {
            CollectObservationBodyPart(bodyPart, sensor);
        }
    }

    public override void OnActionReceived(ActionBuffers actionBuffers)
    {
        // The dictionary with all the body parts in it are in the jdController
        var bpDict = Controller.bodyPartsDict;

        var continuousActions = actionBuffers.ContinuousActions;
        var action = -1;

        for (int i = 1; i < Parts.Count; i++)
            bpDict[Parts[i]].SetJointTargetRotation(continuousActions[++action], continuousActions[++action], continuousActions[++action]);

        // Update joint strength
        for (int i = 1; i < Parts.Count; i++)  
            bpDict[Parts[i]].SetJointStrength(continuousActions[++action]);
    }

    void FixedUpdate()
    {
        if(!Playmode)
        {
            float Dist = Vector3.Distance(Parts[0].position, Target.position);

            if(Dist < 7.5f)
            {
                SetReward(0.25f);
            }

            if(Dist < 0.1f)
            {
                FellOver = true;
                SetReward(0.75f);
            }

            if(Dist > 15f)
            {
                FellOver = true;
                SetReward(Dist/30f);
            }

            if(FellOver)
            {
                EndEpisode();
            }
        }
    }

    Vector3 GetAvgVelocity()
    {
        Vector3 velSum = Vector3.zero;
        Vector3 avgVel = Vector3.zero;

        //ALL RBS
        int numOfRb = 0;
        foreach (var item in Controller.bodyPartsList)
        {
            numOfRb++;
            velSum += item.rb.velocity;
        }

        avgVel = velSum / numOfRb;
        return avgVel;
    }

    public override void Heuristic(in ActionBuffers actionsOut)  //CONTROL OVERRIDE
    {

    }
}