using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Driver : MonoBehaviour
{
    bool Frozen = false;

    public CarConfig Tune;

    public Transform Mass;
    Rigidbody r;

    public Transform Goal;
    public Road CurrentRoad;
    public Road.Node CurrentNode;
    public float SteerAngleGoal = 5f;
    public float PositionDistanceGoal = 2f;
    public float DetectStart = 2.5f;

    public Vector2 MaxSpeedRandom;
    public Vector2 TurnDeadZoneRandom;
    float VehicleMaxSpeed = 25f;
    float DriverMaxSpeed = 0f;
    float TurnDeadZone = 0f;
    public bool OuterLane = false;
    public bool isInPinkLane = false;
    public bool willTurn = false;

    float StartSearchRadius = 5f;
    float SearchRadius = 5f;
    float ForgetDistance = 50f;
    float DetectDistance = 20f;
    Vector3 DetectDirection = Vector3.zero;
    float DetectAlpha = 0f;
    bool Waiting = false;
    int DespawnCounter = 0;
    Vector3 DespawnPosition = Vector3.zero;
    bool Angry = false;
    IEnumerator AngerCoroutine;
    CollisionSound CS;

    public bool Blinded = false;

    //SOUNDS
    public AudioSourceController ASC_Engine;
    public AudioSourceController ASC_Wind;
    public AudioSourceController ASC_Road;
    public AudioSourceController ASC_Horn;

    [HideInInspector]
    public float PitchLow = 0.5f;
    [HideInInspector]
    public float PitchHigh = 1f;
    
    ///VEHICLE VARS
    float CurrentSteeringAngle = 0;
    float MaxSteeringAngle = 40;
    float MotorForce = 200;
    float BrakeForce = 500;

    public Transform FL_Model;
    public Transform FR_Model;
    public Transform BL_Model;
    public Transform BR_Model;

    public WheelCollider FL_Collider;
    public WheelCollider FR_Collider;
    public WheelCollider BL_Collider;
    public WheelCollider BR_Collider;

    void Start()
    {
        DriverMaxSpeed = Random.Range(MaxSpeedRandom.x, MaxSpeedRandom.y);
        TurnDeadZone = Random.Range(TurnDeadZoneRandom.x, TurnDeadZoneRandom.y);
        OuterLane = Random.Range(0f, 1f) > 0.5;
        
        CS = GetComponent<CollisionSound>();
        r = GetComponent<Rigidbody>();
        r.centerOfMass = Mass.localPosition;
        Goal.SetParent(null);
        CurrentNode = new Road.Node();
        CurrentNode.Index = -1;

        InitTune();

        GetRoad();
        InvokeRepeating("CheckWaiting", 10f, 10f);
        InvokeRepeating("ChangeLanes", Random.Range(0f, 25f), Random.Range(10f, 25f));
        ASC_Engine.Play();
    }

    void InitTune()
    {
        r.mass = Tune.CarWeight;
        PitchLow = Tune.PitchLow;
        PitchHigh = Tune.PitchHigh;
        MotorForce = Tune.MotorForce;
        BrakeForce = Tune.BrakeForce;
    }

    void ChangeLanes()
    {
        if(!willTurn)
            OuterLane = !OuterLane;
    }

    void CheckWaiting()
    {
        if(DespawnCounter == 2)
        {
            StartCoroutine(MarkForDeletion());
        }

        

        if(Waiting)
        {
            if(Random.Range(0f, 1f) > 0.5f)
            {
                OuterLane = !OuterLane;
            }else
            {
                CurrentNode = new Road.Node();
                CurrentNode.Index = -2;
                GetRoad();
            }
        }

        

        if(Vector3.Distance(this.transform.position, DespawnPosition) < 2f)
        {
            DespawnCounter++;
        }else
        {
            DespawnCounter = 0;
        }

        DespawnPosition = this.transform.position;
    }

    IEnumerator MarkForDeletion()
    {
        GameObject Player = GameObject.FindWithTag("Player");

        while(Vector3.Distance(this.transform.position, Player.transform.position) < 75f)
        {
            yield return null;
            DespawnCounter = 0;
        }

        Destroy(this.gameObject);
        
    }

    void Update()
    {
        RefreshLoopingAudioLevels();

        float SpeedAlpha = r.velocity.magnitude/VehicleMaxSpeed;
        if(SpeedAlpha > 1f)
            SpeedAlpha = 1f;

        ASC_Engine.SetPitch(Mathf.Lerp(PitchLow, PitchHigh, SpeedAlpha));
        ASC_Engine.SetVolume(1f);

        ASC_Road.SetVolume(Mathf.Lerp(0f, 0.25f, SpeedAlpha));

        ASC_Wind.SetPitch(SpeedAlpha);
        ASC_Wind.SetVolume(SpeedAlpha);
    }

    void RefreshLoopingAudioLevels()
    {
        ASC_Engine.Refresh();
        ASC_Wind.Refresh();
        ASC_Road.Refresh();
        ASC_Horn.Refresh();
    }

    void FixedUpdate()
    {
        CarControls();
        UpdateWheelModels();
    }

    void GetRoad()
    {
        if(CurrentRoad == null)
        {
            bool Found = false;
            Collider[] Nearby = Physics.OverlapSphere(this.transform.position, SearchRadius);
            foreach(Collider Col in Nearby)
            {
                try 
                {   
                    Road R = Col.transform.gameObject.GetComponent<Road_Mesh>().Associated;

                    if(R != null)
                    {
                        CurrentRoad = R;
                        Found = true;
                        SearchRadius = StartSearchRadius;

                        UpdateGoal();
                    }
                }
                catch{}
            }

            if(!Found)
            {
                SearchRadius += SearchRadius;
                //Debug.Log("Expanding to " + SearchRadius);
                GetRoad();
            }
        }else
        {
            UpdateGoal();
        }
    }

    void UpdateGoal()
    {

        if(CurrentNode.Index == -2)//GET NEW NODE TURNING AROUND
        {
            CurrentNode = CurrentRoad.GetFirstNode(this.transform, true);
        }

        if(CurrentNode.Index == -1)
        {
            CurrentNode = CurrentRoad.GetFirstNode(this.transform, false);
        }


        bool Changed = false;
        
        
        if(CurrentNode.ExitRoad != null)
        {
            if(CurrentNode.ExitChance == 100 && (isInPinkLane == CurrentNode.LeaveFromPink))    //road ends into another
            {
                CurrentRoad = CurrentNode.ExitRoad;
                CurrentNode = CurrentRoad.GetFirstNode(this.transform, false);
                //isInPinkLane = Random.value > 0.5f;
                //isInPinkLane = true;
                Changed = true;
                OuterLane = true;
                willTurn = false;
            }else if(CurrentNode.LeaveFromPink == isInPinkLane)                                 //if can turn
            {
                if(willTurn)                                                                    //if will turn
                {
                    CurrentRoad = CurrentNode.ExitRoad;  
                    CurrentNode = CurrentRoad.GetFirstNode(this.transform, false);
                    isInPinkLane = !CurrentNode.LeaveFromPink;
                    Changed = true;
                    willTurn = false;
                }
            }
        }

        if(!Changed)
        {
            isInPinkLane = CurrentRoad.isInPinkLane(CurrentNode.Index, this.transform);
            CurrentNode = CurrentRoad.GetNextNode(CurrentNode.Index, isInPinkLane); 

            if(CurrentNode.LeaveFromPink && isInPinkLane)
            {
                if(CurrentNode.ExitRoad != null)
                {
                    int Chance = Random.Range(1, 101);
                    if(Chance <= CurrentNode.ExitChance)
                    {
                        willTurn = true;
                        OuterLane = true;
                    }
                }
            }

            if(CurrentNode.ExitChance == 100)
                willTurn = true;
        }
   
                        
        if(!CurrentRoad.FourLanes)
        {
            if(isInPinkLane)
            {
                Goal.transform.position = CurrentNode.Pos + ((Quaternion.Euler(0, CurrentNode.CorrectionAngle, 0) * CurrentNode.PerpDir) * CurrentRoad.LaneSpacing);
            }else
            {
                Goal.transform.position = CurrentNode.Pos + ((Quaternion.Euler(0, CurrentNode.CorrectionAngle, 0) * CurrentNode.PerpDir) * -CurrentRoad.LaneSpacing);
            }
        }else
        {
            if(isInPinkLane)
            {
                if(OuterLane)
                    Goal.transform.position = CurrentNode.Pos + ((Quaternion.Euler(0, CurrentNode.CorrectionAngle, 0) * CurrentNode.PerpDir) * CurrentRoad.LaneSpacing) + (Quaternion.Euler(0, CurrentNode.CorrectionAngle, 0) * CurrentNode.PerpDir) * CurrentRoad.CenterLaneSpacing;
                else
                    Goal.transform.position = CurrentNode.Pos + ((Quaternion.Euler(0, CurrentNode.CorrectionAngle, 0) * CurrentNode.PerpDir) * CurrentRoad.LaneSpacing) + (Quaternion.Euler(0, CurrentNode.CorrectionAngle, 0) * CurrentNode.PerpDir) * -CurrentRoad.CenterLaneSpacing;
                    
            }else
            {
                if(OuterLane)
                    Goal.transform.position = CurrentNode.Pos + ((Quaternion.Euler(0, CurrentNode.CorrectionAngle, 0) * CurrentNode.PerpDir) * -CurrentRoad.LaneSpacing) + (Quaternion.Euler(0, CurrentNode.CorrectionAngle, 0) * CurrentNode.PerpDir) * CurrentRoad.CenterLaneSpacing;
                else
                    Goal.transform.position = CurrentNode.Pos + ((Quaternion.Euler(0, CurrentNode.CorrectionAngle, 0) * CurrentNode.PerpDir) * -CurrentRoad.LaneSpacing) + (Quaternion.Euler(0, CurrentNode.CorrectionAngle, 0) * CurrentNode.PerpDir) * -CurrentRoad.CenterLaneSpacing;
            }
        }
    }

    void CarControls()  
    {
        if(!Frozen)
        {
            float Turn = 0f;
            float Gas = 0f;
            bool Brake = false;

            Vector3 LookDir = Goal.position - this.transform.position;
            float Angle = Vector3.SignedAngle(LookDir, this.transform.forward, this.transform.up);

            float BlindDeadZone = 0f;
            if(Blinded)
            {
                BlindDeadZone = 10f;
                Blinded = false;
                Enrage();
            }

            if(Mathf.Abs(Angle) > TurnDeadZone + BlindDeadZone)
            {
                if(Angle < -SteerAngleGoal) //Full Turn Right
                {
                    Turn = 1f;
                }else if(Angle > SteerAngleGoal) //Full Turn Left
                {
                    Turn = -1f;
                }

                if(Angle < 0f && Angle > -SteerAngleGoal)   //Adjust Turn Right
                {
                    Turn = Mathf.Lerp(0f, 1f, Angle/-SteerAngleGoal);
                }else if(Angle > 0f && Angle < SteerAngleGoal)  //Adjust Turn Left
                {
                    Turn = Mathf.Lerp(0f, -1f, Angle/SteerAngleGoal);
                }
            }
            
            
            float Distance = Vector3.Distance(this.transform.position, Goal.position);

            //Debug.Log(Distance);

            if(Distance < PositionDistanceGoal)
            {
                GetRoad();
            }

            if(Distance > ForgetDistance)
            {
                CurrentNode = new Road.Node();
                CurrentNode.Index = -1;
                GetRoad();
            }


            //DetectDirection = (Goal.transform.position - this.transform.position).normalized;
            DetectDirection = Quaternion.Euler(0f, Turn * SteerAngleGoal, 0f) * transform.forward;
            Vector3 RayOrigin = this.transform.position + (this.transform.up * 0.75f) + (this.transform.forward * DetectStart);

            RaycastHit hit;
            if(Physics.Raycast(RayOrigin + (this.transform.right * .6f), DetectDirection, out hit, DetectDistance))
            {
                DetectAlpha = 1f;

                try
                {

                    if(hit.transform.GetComponent<Rigidbody>() != null)
                    {
                        DetectAlpha = Vector3.Distance(this.transform.position + (this.transform.up * 1f) + (this.transform.forward * DetectStart), hit.point)/DetectDistance;
                    }

                }catch{}
            }else
            {
                DetectAlpha = 1f;
            }

            float SecondDetect = 1f;

            if(Physics.Raycast(RayOrigin + (this.transform.right * -.6f), DetectDirection, out hit, DetectDistance))
            {

                try
                {

                    if(hit.transform.GetComponent<Rigidbody>() != null)
                    {
                        SecondDetect = Vector3.Distance(this.transform.position + (this.transform.up * 1f) + (this.transform.forward * DetectStart), hit.point)/DetectDistance;
                    }

                }catch{}
            }else
            {
                SecondDetect = 1f;
            }

            if(SecondDetect < DetectAlpha)
                DetectAlpha = SecondDetect;

        
            
            

            if(Mathf.Abs(Angle) > 90f)      //to turn around
            {
                //Debug.Log(Mathf.Abs(Angle));
                if(Mathf.Abs(Angle) > (180f - 25f))
                {
                    Turn = 0f;
                    Gas = -1f;
                }else
                {
                    Turn = -Turn;
                    Gas = -1f;
                }
            }else
            {
                float SpeedGoal = 
                (
                    CurrentRoad.Speedlimit + 
                    DriverMaxSpeed + 
                    (willTurn ? 5f - CurrentRoad.Speedlimit - DriverMaxSpeed : 0f) + 
                    (Angry ? 5f : 0f)
                );

                if(r.velocity.magnitude < SpeedGoal && Distance > PositionDistanceGoal) //stay at a decent speed
                {
                    Gas = Mathf.Lerp(0f, 1f, DetectAlpha/1f);
                }

                if(r.velocity.magnitude > SpeedGoal)        //reverse if driving too fast
                    Gas = -1f;
                /*
                if(DetectAlpha < 0.15f && DetectAlpha > 0.1f)
                {
                    Brake = true;
                }*/
                if(DetectAlpha < 0.5f && r.velocity.magnitude > SpeedGoal/2f)   //brake if going fast next to another car
                {
                    Brake = true;
                    ASC_Horn.SetVolume(1f);
                }else
                {
                    ASC_Horn.SetVolume(0f);
                }

                if(Angry)
                {
                    ASC_Horn.SetVolume(1f);
                }

                if(DetectAlpha < 0.25f && DetectAlpha > 0.1f && r.velocity.magnitude > 0f)   //brake to a stop
                {
                    Brake = true;
                }

                if(DetectAlpha > 0.1f && DetectAlpha < 0.25f)
                    Waiting = true;
                else
                    Waiting = false;

                if(willTurn && r.velocity.magnitude > (CurrentRoad.Speedlimit + DriverMaxSpeed))     //brake if speeding before turn
                {
                    Brake = true;
                }

                if(DetectAlpha < 0.1f)     //reverse if stuck
                {
                    Gas = -1f;
                }
            }

            CurrentSteeringAngle = MaxSteeringAngle * Turn;
            FR_Collider.steerAngle = CurrentSteeringAngle;
            FL_Collider.steerAngle = CurrentSteeringAngle;

            if(!Brake)
            {
                FR_Collider.motorTorque = MotorForce * Gas;
                FL_Collider.motorTorque = MotorForce * Gas;

                FR_Collider.brakeTorque = 0f;
                FL_Collider.brakeTorque = 0f;
                BR_Collider.brakeTorque = 0f;
                BL_Collider.brakeTorque = 0f;
                    
                
            }else
            {
                FR_Collider.motorTorque = 0f;
                FL_Collider.motorTorque = 0f;

                FR_Collider.brakeTorque = BrakeForce;
                FL_Collider.brakeTorque = BrakeForce;
                BR_Collider.brakeTorque = BrakeForce;
                BL_Collider.brakeTorque = BrakeForce;
            }
            
        }else
        {
            FR_Collider.motorTorque = 0f;
            FL_Collider.motorTorque = 0f;

            FR_Collider.brakeTorque = BrakeForce;
            FL_Collider.brakeTorque = BrakeForce;
            BR_Collider.brakeTorque = BrakeForce;
            BL_Collider.brakeTorque = BrakeForce;
        }
    }

    void UpdateWheelModels()
    {
        UpdateWheelModel(FR_Collider, FR_Model);
        UpdateWheelModel(FL_Collider, FL_Model);
        UpdateWheelModel(BR_Collider, BR_Model);
        UpdateWheelModel(BL_Collider, BL_Model);
    }

    void UpdateWheelModel(WheelCollider Wheel, Transform Model)
    {
        Vector3 Pos = Model.position;
        Quaternion Rot = Model.rotation;

        Wheel.GetWorldPose(out Pos, out Rot);

        Model.position = Pos;
        Model.rotation = Rot;
    }

    void OnCollisionEnter(Collision Col)
    {
        if(Col.impulse.magnitude >= CS.Threshold)
        {
            Enrage();
        }
    }

    void Enrage()
    {
        if(Angry)
            StopCoroutine(AngerCoroutine);

        AngerCoroutine = BecomeAngry();
        StartCoroutine(AngerCoroutine);
    }

    IEnumerator BecomeAngry()
    {
        Angry = true;
        yield return new WaitForSeconds(1f);
        Angry = false;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.magenta;
        Vector3 RayOrigin = this.transform.position + (this.transform.up * .75f) + (this.transform.forward * DetectStart);
        Gizmos.DrawLine(RayOrigin + (this.transform.right * .6f), RayOrigin + (this.transform.right * .6f) + (DetectDirection * (DetectDistance*DetectAlpha)));
        Gizmos.DrawLine(RayOrigin + (this.transform.right * -.6f), RayOrigin + (this.transform.right * -.6f) + (DetectDirection * (DetectDistance*DetectAlpha)));

        Gizmos.color = Color.blue;
        Gizmos.color = new Color(Gizmos.color.r, Gizmos.color.g, Gizmos.color.b, 0.25f);
        Gizmos.DrawSphere(Goal.position + (Goal.transform.up * 1f), PositionDistanceGoal/2f);

        Gizmos.color = Color.red;
        Gizmos.DrawLine(this.transform.position + (this.transform.up * 1f), this.transform.position + (Quaternion.Euler(0, SteerAngleGoal, 0) * this.transform.forward * 15f) + (this.transform.up * 1f));
        Gizmos.DrawLine(this.transform.position + (this.transform.up * 1f), this.transform.position + (Quaternion.Euler(0, -SteerAngleGoal, 0) * this.transform.forward * 15f) + (this.transform.up * 1f));

        Gizmos.color = Color.green;
        Gizmos.DrawLine(this.transform.position + (this.transform.up * 1f), Goal.transform.position + (this.transform.up * 1f));
        
    }
}
