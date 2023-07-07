using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Vehicle : MonoBehaviour
{
    [HideInInspector]
    public Vector2 SavedLook = Vector2.zero;
    public BoxCollider CarryBounds;
    [HideInInspector]
    public NetworkIdentity ID;
    [HideInInspector]
    public AudioSource AS_Engine;
    [HideInInspector]
    public AudioSource AS_Horn;
    [HideInInspector]
    public AudioSource AS_Tire;
    [HideInInspector]
    public Transform Seat;
    [HideInInspector]
    public GameObject Effects;
    [HideInInspector]
    public Transform Lights;
    [HideInInspector]
    public Transform ExitLocation;

    Transform Model;
    Transform SteeringWheel;

    public Transform FL_Model;
    public Transform FR_Model;
    public Transform BL_Model;
    public Transform BR_Model;

    public WheelCollider FL_Collider;
    public WheelCollider FR_Collider;
    public WheelCollider BL_Collider;
    public WheelCollider BR_Collider;

    public VehicleConfig Config;
    
    [HideInInspector]
    public List<MeshRenderer> Meshes;

    Rigidbody r;
    List<GameObject> Carried;
    
    float WheelTurnDeg = 25f;
    float SteeringTurnDeg = 55f;

    public bool Driving = false;

    [HideInInspector]
    public MirrorVehicle MRD;

    float BreakForce = 100f;

    void Start()
    {
        MRD = GetComponent<MirrorVehicle>();
        ID = GetComponent<NetworkIdentity>();
        r = GetComponent<Rigidbody>();
        r.centerOfMass = Config.CenterOfMass;

        if(ID.hasAuthority)    
        {
            r.isKinematic = false;
        }

        Initialize();
    }

    void Initialize()
    {
        Model = transform.GetChild(0);
        SteeringWheel = Model.transform.GetChild(1);

        Seat = transform.GetChild(2).GetChild(0);

        Effects = transform.GetChild(4).gameObject;

        Lights = transform.GetChild(5);
        Lights.gameObject.SetActive(false);

        ExitLocation = transform.GetChild(6);

        AS_Engine = transform.GetChild(3).GetChild(0).GetComponent<AudioSource>();
        AS_Horn = transform.GetChild(3).GetChild(1).GetComponent<AudioSource>();
        AS_Tire = transform.GetChild(3).GetChild(2).GetComponent<AudioSource>();
    }

    public void Trigger()
    {
        if(ID.hasAuthority)
        {
            Player P = GameObject.FindWithTag("Player").GetComponent<Player>();

            if(P.CurrentVehicle != null)
            {
                P.LeaveVehicle(false);

            }else
            {
                Driving = true;
                P.CurrentVehicle = this;
                P.ChangeEquipped(0);
                ToggleCarried(true);

                P.r.collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;
                P.r.isKinematic = true;

                P.ToggleColliders();
                
                ExitLocation.position = P.transform.position;
                ExitLocation.rotation = P.transform.rotation;
                SavedLook = new Vector2(ExitLocation.localEulerAngles.y, P.pitch);
                P.transform.position = Seat.position;
                
                P.An.SetInteger("Sitting", Config.SittingAnimationIndex);
                P.TurnWheel(0f, true);
                P.transform.SetParent(this.transform);
                P.transform.localEulerAngles = Vector3.zero;
                P.SetLook(0f, 10f);
                
            }
            
        }
    }

    void FixedUpdate()
	{   
        if(ID.hasAuthority)
        {
            UpdateWheelModels();

            if(Driving)
            {
                if(!UI.Instance.Busy())
                {
                    Drive();
                }
            }else
            {
                FR_Collider.brakeTorque = BreakForce;
                FL_Collider.brakeTorque = BreakForce;
                BR_Collider.brakeTorque = BreakForce;
                BL_Collider.brakeTorque = BreakForce;
            }
        }
	}

    void Update()
    {
        EngineSounds();
        SkidSounds();
        SteerAnimation();
        
        if(Driving && !UI.Instance.Busy())
        {
            if(ID.hasAuthority)
            {
                if(Input.GetKeyDown("e"))
                    MRD.CmdPlayHorn();
                    
                if(Input.GetKeyDown("r"))
                    MRD.CmdToggleLights();
            }
        }
    }

    void SteerAnimation()
    {
        SteeringWheel.localEulerAngles = new Vector3(SteeringWheel.localEulerAngles.x, SteeringWheel.localEulerAngles.y, MRD.TurnAmount * SteeringTurnDeg);
    }

    void SkidSounds() //Perfect example of command usage without RPC calls
    {
        float maxSkid = 15f;

        if(ID.hasAuthority)
        {
            MRD.SkidAmount = Mathf.Abs(transform.InverseTransformDirection(r.velocity).x);
            MRD.SkidAmount -= 5f;

            if(MRD.SkidAmount > maxSkid)
                MRD.SkidAmount = maxSkid;

            MRD.CmdSendSkidInfo(MRD.SkidAmount);
        }
        
        float tirealpha = Mathf.Lerp(0f, .15f, MRD.SkidAmount/maxSkid);
        AS_Tire.volume = (tirealpha * (Settings._sfxVolume/100f)) * (Settings._masterVolume/100f);
    }

    void EngineSounds() //Perfect example of command usage without RPC calls
    {
        if(ID.hasAuthority)
        {
            MRD.EngineAlpha = r.velocity.magnitude/25f;

            if(MRD.EngineAlpha > 1f)
                MRD.EngineAlpha = 1f;

            MRD.CmdSendEngineInfo(MRD.EngineAlpha);
        }
        
        AS_Engine.pitch = Mathf.Lerp(Config.PitchLow, Config.PitchHigh, MRD.EngineAlpha);
        AS_Engine.volume = (.5f * (Settings._sfxVolume/100f)) * (Settings._masterVolume/100f);
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
        float zOff = Model.localPosition.x > 0f ? 0.25f : -0.25f;
        Model.localPosition = new Vector3(zOff, Model.localPosition.y, Model.localPosition.z);
        Model.rotation = Rot;
    }

    public void ToggleCarried(bool Freeze)
    {
        if(CarryBounds != null)
        {
            if(Freeze)
            {
                Collider[] DetectedItems = Physics.OverlapBox(CarryBounds.transform.position, CarryBounds.size/2f, CarryBounds.transform.rotation);
                Carried = new List<GameObject>();

                foreach (Collider C in DetectedItems)
                {
                    if(C.GetComponent<PickupAble>())
                    {
                        Carried.Add(C.gameObject);
                    }
                }

                for(int i = 0; i < Carried.Count; i++)
                {
                    Carried[i].transform.gameObject.GetComponent<Rigidbody>().collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;
                    Carried[i].transform.gameObject.GetComponent<Rigidbody>().isKinematic = true;
                    Carried[i].transform.gameObject.GetComponent<Collider>().enabled = false;
                    Carried[i].transform.SetParent(this.transform);
                }

            }else
            {
                for(int i = 0; i < Carried.Count; i++)
                {
                    Carried[i].transform.gameObject.GetComponent<Rigidbody>().isKinematic = false;
                    Carried[i].transform.gameObject.GetComponent<Rigidbody>().collisionDetectionMode = CollisionDetectionMode.Continuous;
                    Carried[i].transform.gameObject.GetComponent<Collider>().enabled = true;
                    Carried[i].transform.SetParent(null);
                }
            }
        }
    }

    void Drive()
    {
        FR_Collider.steerAngle = MRD.TurnAmount * WheelTurnDeg;
        FL_Collider.steerAngle = MRD.TurnAmount * WheelTurnDeg;

		int Gas = 0;
        bool Brake = false;

        if(Input.GetKey("w"))
        {
            Gas += 1;
        }

        if(Input.GetKey("s"))
        {
            Gas -= 1;
        }

        if(Input.GetKey(KeyCode.Space))
        {
            Brake = true;
        }

        if(!Brake)
        {
            FR_Collider.motorTorque = Config.InstallableKits[0].Acceleration * Gas;
            FL_Collider.motorTorque = Config.InstallableKits[0].Acceleration * Gas;

            FR_Collider.brakeTorque = 0f;
            FL_Collider.brakeTorque = 0f;
            BR_Collider.brakeTorque = 0f;
            BL_Collider.brakeTorque = 0f;
                
        }else
        {
            FR_Collider.motorTorque = 0f;
            FL_Collider.motorTorque = 0f;

            FR_Collider.brakeTorque = BreakForce;
            FL_Collider.brakeTorque = BreakForce;
            BR_Collider.brakeTorque = BreakForce;
            BL_Collider.brakeTorque = BreakForce;
        }

        //CAR SHAKE STUFF
        float Alpha = (r.velocity.magnitude - 25f)/750f;

        if(Alpha > 1f)
            Alpha = 1f;
        
        if(Alpha < 0f)
            Alpha = 0f;

        float ShakeAmount = Mathf.Lerp(0f, 0.15f, Alpha);

        Vector3 ShakeVec = new Vector3(Random.Range(-ShakeAmount, ShakeAmount),Random.Range(-ShakeAmount, ShakeAmount),Random.Range(-ShakeAmount, ShakeAmount));

        Model.transform.localPosition = ShakeVec;
    }

    void OnCollisionEnter(Collision col)    //STORE REFERENCE TO PLAYER ON OWN LATER YOU STUPID IDIOT GOD DAMN
    {
        if(Driving && ID.hasAuthority)
        {
            if(col.impulse.magnitude > 2000f || col.gameObject.CompareTag("Hazard"))
            {
                GameObject.FindWithTag("Player").GetComponent<Player>().Die("Crashed");

                MRD.CmdPlayCrash(col.contacts[0].point);

            }
        }
    }

    bool isGrounded()
    {
        RaycastHit hit;
        if(Physics.Raycast(this.transform.position + new Vector3(0f, 0.2f, 0f), -Vector3.up, out hit, .4f))
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}
