using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;

public class WalkerAgent : Agent
{
    //DATATYPES

    //PUBLIC COMPONENTS
    public Transform BodyCenter;
    public Transform Target;

    public Transform AlignObject;

    public List<JointController> JCS = new List<JointController>();

    //PUBLIC VARS
    [HideInInspector]
    public List<Vector3> DefaultPositions = new List<Vector3>();
    [HideInInspector]
    public List<Vector3> DefaultAngles = new List<Vector3>();
    public bool FellOver = false;

    //PUBLIC LISTS

    //COMPONENTS

    //VARS

    //LISTS
    const float maxDistanceToTarget = 4f;
    float distanceToTarget = maxDistanceToTarget;
    float SpawnDist = 0f;

    float TotalRew = 0f;

    bool PerformMode = false;
    
    float StartTime = 0f;
    float RunningTime = 0f;

    float Duration = 20f;

    void Start() 
    {
        PerformMode = GetComponent<Unity.MLAgents.Policies.BehaviorParameters>().Model != null;

        for(int i = 0; i < JCS.Count; i++)
        {
            DefaultPositions.Add(JCS[i].transform.localPosition);
            DefaultAngles.Add(JCS[i].transform.localEulerAngles);
        }

        DefaultPositions.Add(BodyCenter.transform.localPosition);
        DefaultAngles.Add(BodyCenter.transform.localEulerAngles);
    }

    public override void OnEpisodeBegin()   //RESET WHEN WIN/LOSE
    {
        TotalRew = 0f;
        FellOver = false;

        for(int i = 0; i < JCS.Count; i++)
        {
            JCS[i].transform.localPosition = DefaultPositions[i];
            JCS[i].transform.localEulerAngles = DefaultAngles[i];
            JCS[i].r.velocity = Vector3.zero;
            JCS[i].r.angularVelocity = Vector3.zero;
        }
        
        BodyCenter.transform.localPosition = DefaultPositions[JCS.Count];
        BodyCenter.transform.localEulerAngles = DefaultAngles[JCS.Count];

        //Target.localPosition = new Vector3((Random.value * (maxDistanceToTarget * 2f)) - (maxDistanceToTarget), 1.175f, (Random.value * (maxDistanceToTarget * 2f)) - (maxDistanceToTarget));
        Target.localPosition = new Vector3((Random.value * 4) + 4, 1.175f, (Random.value * 4) + 4);
        //SpawnDist = Vector3.Distance(BodyCenter.position, Target.position);
        StartTime = Time.time;
        RunningTime = 0f;
    }

    public override void CollectObservations(VectorSensor sensor)   //SEND INFO
    {
        //Position info //SIZE 6
        sensor.AddObservation(Target.transform.position);               //3
        sensor.AddObservation(BodyCenter.transform.position);           //3

        //Rotation info //SIZE 39 (3 * 13)      //LIMITED TO 22
        for(int i = 0; i < JCS.Count; i++)
        {
            if(JCS[i].EnabledAxis.x == 1)
                sensor.AddObservation(JCS[i].transform.localEulerAngles.x);
            if(JCS[i].EnabledAxis.y == 1)
                sensor.AddObservation(JCS[i].transform.localEulerAngles.y);
            if(JCS[i].EnabledAxis.z == 1)
                sensor.AddObservation(JCS[i].transform.localEulerAngles.z);
          

            //sensor.AddObservation(JCS[i].transform.localEulerAngles);   //3
        }

        //TOTAL SIZE 45
    }

    public override void OnActionReceived(ActionBuffers actionBuffers)  //UPDATE
    {
        int ControlIndex = 0;
        for(int i = 0; i < JCS.Count; i++)  //TOTAL ACTIONS 39
        {
            Vector3 Torque = Vector3.zero;

            if(JCS[i].EnabledAxis.x == 1)
            {
                Torque.x = actionBuffers.ContinuousActions[ControlIndex];
                ControlIndex++;
            }
            if(JCS[i].EnabledAxis.y == 1)
            {
                Torque.y = actionBuffers.ContinuousActions[ControlIndex + 1];
                ControlIndex++;
            }
            if(JCS[i].EnabledAxis.z == 1)
            {
                Torque.z = actionBuffers.ContinuousActions[ControlIndex + 2];
                ControlIndex++;
            }


            JCS[i].TorqueVector = Torque;
            //JCS[i].TargetAngle = new Vector3(actionBuffers.ContinuousActions[i * 1], actionBuffers.ContinuousActions[i * 2], actionBuffers.ContinuousActions[i * 3]);
        }
        
        if(!PerformMode)
        {
            // Rewards
            //distanceToTarget = Vector3.Distance(BodyCenter.position, Target.position);

            //TotalRew += 0.05f;
            //SetReward(TotalRew);

            //SetReward(1f - (distanceToTarget/SpawnDist));
            /*
            if(distanceToTarget < 1.5f)
            {
                //SetReward(1f);
                EndEpisode();
            }*/

            
            

            /*
            RunningTime = Time.time - StartTime;

            SetReward(Mathf.Lerp(1f - (angle/60f), 1f));

            if(RunningTime > Duration)
            {
                SetReward(1f);
                EndEpisode();
            }  
            */

            
            float angle = Vector3.Angle(AlignObject.transform.up, Vector3.up);
            
            //RunningTime = Time.time - StartTime;

            //SetReward((1f - (angle/70f)) * RunningTime);
            SetReward(Mathf.Lerp(0f, 0.5f - (angle/70f), AlignObject.transform.position.y/1.4f));

            if(angle > 70f)
            {
                //SetReward(0f);
                EndEpisode();
            }
            if(FellOver)
            {
                //SetReward(0f);
                EndEpisode();
            }

            /*
            RunningTime = Time.time - StartTime;

            //SetReward(1f - (Vector3.Distance(AlignObject.transform.localPosition, new Vector3(0f, 1.4f, 0f))));

            if(RunningTime > Duration)
            {
                //SetReward(1f);
                EndEpisode();
            }

            if(FellOver)
            {
                float TimeAlpha = RunningTime/Duration;

                //SetReward(TimeAlpha);
                EndEpisode();
            }*/
            
        }

    }

    public override void Heuristic(in ActionBuffers actionsOut)  //CONTROL OVERRIDE
    {

    }

    

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(AlignObject.transform.position, AlignObject.transform.position + (AlignObject.transform.up * 2f));
        Gizmos.color = Color.green;
        Gizmos.DrawLine(AlignObject.transform.position, AlignObject.transform.position + (Vector3.up * 2f));
    }
    
}