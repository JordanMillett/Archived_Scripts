using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KartController : MonoBehaviour
{
    public enum Driver
	{
        AI,
		Player1,
		Player2,
		Player3,
        Player4
	};

    public Driver InControl;

    public bool Frozen = false;

    public Transform CameraEmpty;

    Rigidbody r;

    float CurrentSteeringAngle = 0;
    float MaxSteeringAngle = 20;
    public float MotorForce = 100f;
    float BrakeForce = 10f;
    float MaxSpeed = 20f;

    public Transform Mass;
    public Transform ItemSlot;

    Transform FL_Model;
    Transform FR_Model;
    Transform BL_Model;
    Transform BR_Model;

    public WheelCollider FL_Collider;
    public WheelCollider FR_Collider;
    public WheelCollider BL_Collider;
    public WheelCollider BR_Collider;

    //public Map.LapTime LT;
    public float LapOffset = 0f;
    public int LapCount = 0;
    public int CheckpointIndex = 0;
    public int Place = 0;
    public int Score = 0;

    public bool Respawning = false;

    public List<MeshRenderer> KartMeshes;

    public GameObject ItemReference;

    float WheelDirection = 0f;

    public float LastLapTime = 0f;

    //AI
    public Transform Goal;
    public Road CurrentRoad;
    public Road.Node CurrentNode;
    public float SteerAngleGoal = 5f;
    public float PositionDistanceGoal = 2f;
    public Vector2 TurnDeadZoneRandom;
    float TurnDeadZone = 0f;

    void Start()
    {
        r = GetComponent<Rigidbody>();
        r.centerOfMass = Mass.localPosition;

        TurnDeadZone = Random.Range(TurnDeadZoneRandom.x, TurnDeadZoneRandom.y);
        

        if(InControl == Driver.AI)
            InvokeRepeating("UseItem", Random.Range(5f, 15f),Random.Range(5f, 15f));
    }

    void FixedUpdate()
    {
        if(InControl == Driver.AI)
        {
            AICarControls();
        }else
        {
            CarControls();
        }

        UpdateWheelModels();
        CameraFollow();
    }

    public void OutOfBounds()
    {
        if(!Respawning)
        {
            Respawning = true;
            StartCoroutine(Respawn());
        }
    }

    public void UseItem()
    {
        if(ItemReference != null)
        {
            ItemReference.GetComponent<ItemInvoker>().Activate();
        }
    }

    IEnumerator Respawn()//make transparent
    {
        Frozen = true;
        r.velocity = Vector3.zero;

        /*
        foreach(MeshRenderer MR in KartMeshes)
        {
            MR.material.SetColor("Tint", Color.gray);
        }
        */
        GameObject Empty = new GameObject();
        Empty.AddComponent<Rigidbody>();
        Empty.GetComponent<Rigidbody>().isKinematic = true;
        Empty.transform.position = this.transform.position + new Vector3(0f, 1f, 0f);

        this.gameObject.layer = LayerMask.NameToLayer("NoClip");
        FL_Collider.gameObject.layer = LayerMask.NameToLayer("NoClip");
        FR_Collider.gameObject.layer = LayerMask.NameToLayer("NoClip");
        BL_Collider.gameObject.layer = LayerMask.NameToLayer("NoClip");
        BR_Collider.gameObject.layer = LayerMask.NameToLayer("NoClip");

        r.drag = 5f;
        r.angularDrag = 5f;

        SpringJoint SJ = this.gameObject.AddComponent<SpringJoint>();
        SJ.spring = 2000;
        SJ.damper = 0;
        SJ.connectedBody = Empty.GetComponent<Rigidbody>();


        yield return new WaitForSeconds(1f);

        //if(InControl != Driver.AI)
            //Debug.Log(CheckpointIndex + " " + GameObject.FindWithTag("Map").GetComponent<Map>().Checkpoints[CheckpointIndex].transform.position.y);

        Vector3 PickupPos = new Vector3(transform.position.x, GameObject.FindWithTag("Map").GetComponent<Map>().Checkpoints[CheckpointIndex].transform.position.y + 10f, transform.position.z);
        float Distance = Vector3.Distance(Empty.transform.position, PickupPos);
        while(Distance > 1f)
        {
            Distance = Vector3.Distance(Empty.transform.position, PickupPos);
            Empty.transform.position = Vector3.Lerp(Empty.transform.position, PickupPos, Time.deltaTime * 2f);
            yield return null;
        }

        Vector3 Randomness = new Vector3(Random.Range(-3f, 3f), 0f, Random.Range(-3f, 3f));
        Vector3 RespawnPos = GameObject.FindWithTag("Map").GetComponent<Map>().Checkpoints[CheckpointIndex].transform.position + new Vector3(0f, 5f, 0f) + Randomness;
        Quaternion RespawnRot = GameObject.FindWithTag("Map").GetComponent<Map>().Checkpoints[CheckpointIndex].transform.rotation;
        Distance = Vector3.Distance(Empty.transform.position, RespawnPos);
        while(Distance > 0.5f)
        {
            Distance = Vector3.Distance(Empty.transform.position, RespawnPos);
            Empty.transform.position = Vector3.Lerp(Empty.transform.position, RespawnPos, Time.deltaTime * 2f);

            transform.rotation = Quaternion.Lerp(transform.rotation, RespawnRot, Time.deltaTime * 2f);

            yield return null;
        }

        yield return new WaitForSeconds(1f);

        this.gameObject.layer = LayerMask.NameToLayer("Kart");
        FL_Collider.gameObject.layer = LayerMask.NameToLayer("Kart");
        FR_Collider.gameObject.layer = LayerMask.NameToLayer("Kart");
        BL_Collider.gameObject.layer = LayerMask.NameToLayer("Kart");
        BR_Collider.gameObject.layer = LayerMask.NameToLayer("Kart");

        r.velocity = Vector3.zero;
        r.drag = 0f;
        r.angularDrag = 0f;
        Destroy(Empty.gameObject);
        Destroy(SJ);
        
        if(InControl == Driver.AI)
        {
            CurrentNode = CurrentRoad.GetFirstNode(this.transform); ///AIII
            Goal.SetParent(null);
            Goal.transform.position = CurrentNode.Pos;
        }

        Frozen = false;
        Respawning = false;
        /*
        foreach(MeshRenderer MR in KartMeshes)
        {
            MR.material.SetColor("Tint", Color.white);
        }*/
    }

    public void Load(KartConfig KC)
    {
        GameObject NewBody = Instantiate(KC.KB.Prefab, Vector3.zero, Quaternion.identity);
        NewBody.transform.parent = transform.GetChild(0);
        NewBody.transform.localPosition = new Vector3(0f, KC.KW.RideHeightOffset, 0f);
        NewBody.transform.localPosition = Vector3.zero;
        NewBody.transform.localEulerAngles = Vector3.zero;
        KartMeshes.Add(NewBody.transform.GetChild(0).GetComponent<MeshRenderer>());

        r = GetComponent<Rigidbody>();

        MaxSpeed = KC.KB.MaxSpeed;
        r.mass = KC.KB.Weight;
        MotorForce = KC.KW.Acceleration;

        

        for(int i = 0; i < 4; i++)
        {
            GameObject NewWheel = Instantiate(KC.KW.Prefab, Vector3.zero, Quaternion.identity);
            NewWheel.transform.parent = NewBody.transform.GetChild(1).GetChild(i);
            NewWheel.transform.localPosition = Vector3.zero;
            NewWheel.transform.localEulerAngles = Vector3.zero;
            KartMeshes.Add(NewWheel.transform.GetChild(0).GetComponent<MeshRenderer>());
        }

        FL_Model = NewBody.transform.GetChild(1).GetChild(0);
        FR_Model = NewBody.transform.GetChild(1).GetChild(1);
        BL_Model = NewBody.transform.GetChild(1).GetChild(2);
        BR_Model = NewBody.transform.GetChild(1).GetChild(3);

        if(KC.KB.HideWheels)
        {
            FL_Model.GetChild(0).gameObject.SetActive(false);
            FR_Model.GetChild(0).gameObject.SetActive(false);
            BL_Model.GetChild(0).gameObject.SetActive(false);
            BR_Model.GetChild(0).gameObject.SetActive(false);
        }
        

        /*
        FL_Collider.transform.localPosition = FL_Model.transform.localPosition;
        FR_Collider.transform.localPosition = FR_Model.transform.localPosition;
        BL_Collider.transform.localPosition = BL_Model.transform.localPosition;
        BR_Collider.transform.localPosition = BR_Model.transform.localPosition;

        FL_Collider.radius = KC.KW.Radius;
        FR_Collider.radius = KC.KW.Radius;
        BL_Collider.radius = KC.KW.Radius;
        BR_Collider.radius = KC.KW.Radius;
        */
    }

    void CameraFollow()
    {
        if(CameraEmpty)
        {
            Vector3 FollowPosition = (FL_Model.position + FR_Model.position + BL_Model.position + BR_Model.position)/4f;
            CameraEmpty.transform.position = FollowPosition + new Vector3(0f, -Mass.localPosition.y, 0f);

            CameraEmpty.transform.rotation = Quaternion.Slerp(CameraEmpty.transform.rotation, this.transform.rotation, Time.fixedDeltaTime * 4f);
        }
    }

    void CarControls()  
    {
        if(!Frozen)
        {   
            float Turn = 0f;
            float Gas = 0f;

            if(InControl == Driver.Player1)
            {
                if(Input.GetKey("d"))
                    Turn += 1f;

                if(Input.GetKey("a"))
                    Turn -= 1f;

                if(Input.GetKey("w"))
                    Gas += 1f;

                if(Input.GetKey("s"))
                    Gas -= 1f;

                if(Input.GetKey(KeyCode.Space))
                    UseItem();
            }

            if(InControl == Driver.Player2)
            {
                if(Input.GetKey(KeyCode.RightArrow))
                    Turn += 1f;

                if(Input.GetKey(KeyCode.LeftArrow))
                    Turn -= 1f;

                if(Input.GetKey(KeyCode.UpArrow))
                    Gas += 1f;

                if(Input.GetKey(KeyCode.DownArrow))
                    Gas -= 1f;

                if(Input.GetKey(KeyCode.RightControl))
                    UseItem();
            }

            if(InControl == Driver.Player3)
            {
                if(Input.GetKey("l"))
                    Turn += 1f;

                if(Input.GetKey("j"))
                    Turn -= 1f;

                if(Input.GetKey("i"))
                    Gas += 1f;

                if(Input.GetKey("k"))
                    Gas -= 1f;

                if(Input.GetKey(KeyCode.Return))
                    UseItem();
            }

            if(InControl == Driver.Player4)
            {
                if(Input.GetKey(KeyCode.Keypad6))
                    Turn += 1f;

                if(Input.GetKey(KeyCode.Keypad4))
                    Turn -= 1f;

                if(Input.GetKey(KeyCode.Keypad8))
                    Gas += 1f;

                if(Input.GetKey(KeyCode.Keypad5))
                    Gas -= 1f;

                if(Input.GetKey(KeyCode.Keypad0))
                    UseItem();
            }

            WheelDirection = Mathf.Lerp(WheelDirection, Turn, Time.deltaTime * 4f);

            CurrentSteeringAngle = MaxSteeringAngle * WheelDirection;
            FR_Collider.steerAngle = CurrentSteeringAngle;
            FL_Collider.steerAngle = CurrentSteeringAngle;

            

            
            if(r.velocity.magnitude/MaxSpeed > 0.8f)//past 80% max speed start slowing (25 max conceivable speed?)
                Gas = (1f - (r.velocity.magnitude/MaxSpeed)) * Gas;

            //Debug.Log(r.velocity.magnitude/MaxSpeed); make speedometer

            if(r.velocity.magnitude < MaxSpeed)
            {
                FR_Collider.motorTorque = MotorForce * Gas;
                FL_Collider.motorTorque = MotorForce * Gas;
            }

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
    }

    void AICarControls()  
    {
        if(!Frozen)
        {   
            float Turn = 0f;
            float Gas = 0f;

            Vector3 LookDir = Goal.position - this.transform.position;
            float Angle = Vector3.SignedAngle(LookDir, this.transform.forward, this.transform.up);

            if(Mathf.Abs(Angle) > TurnDeadZone)
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
            //GET ROAD ONLY ONCE

            if(Distance < PositionDistanceGoal)
            {
                GetRoad();
            }

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
                if(r.velocity.magnitude < MaxSpeed && Distance > PositionDistanceGoal) //stay at a decent speed
                {
                    Gas = 1f;
                }

                if(r.velocity.magnitude > MaxSpeed)        //reverse if driving too fast
                    Gas = -1f;

            }

            CurrentSteeringAngle = MaxSteeringAngle * Turn;
            FR_Collider.steerAngle = CurrentSteeringAngle;
            FL_Collider.steerAngle = CurrentSteeringAngle;

     
            
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

    void GetRoad()
    {
        if(CurrentRoad != null)
        {
            CurrentNode = CurrentRoad.GetNextNode(CurrentNode.Index); 
            Goal.SetParent(null);
            Goal.transform.position = CurrentNode.Pos + (CurrentNode.PerpDir * (Random.Range(-5f, 5f)));
        }else
        {
            CurrentRoad = GameObject.FindWithTag("Map").GetComponent<Map>().R;
            CurrentNode = CurrentRoad.GetFirstNode(this.transform);
            Goal.SetParent(null);
            Goal.transform.position = CurrentNode.Pos + (CurrentNode.PerpDir * (Random.Range(-5f, 5f)));
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(0f, 0f, 1f, 0.5f);
        Gizmos.DrawSphere(Goal.position, PositionDistanceGoal/2f);

        Gizmos.color = Color.green;
        Gizmos.DrawLine(this.transform.position, Goal.transform.position );
    }

    void OnTriggerEnter(Collider Col)
    {
        if(Col.GetComponent<OutOfBounds>())
        {
            Respawn();
        }
    }

}
