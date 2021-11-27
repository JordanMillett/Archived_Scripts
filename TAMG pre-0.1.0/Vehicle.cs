using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Vehicle : MonoBehaviour
{
    [HideInInspector]
    public bool InShop = false;
    [HideInInspector]
    public Vector2 SavedLook = Vector2.zero;
    [HideInInspector]
    public BoxCollider CarryBounds;
    [HideInInspector]
    public AutoShop CurrentShop;
    [HideInInspector]
    public NetworkIdentity ID;
    [HideInInspector]
    public AudioSource AS_Engine;
    [HideInInspector]
    public AudioSource AS_Horn;
    [HideInInspector]
    public AudioSource AS_Tire;
    [HideInInspector]
    public AudioSource AS_Radio;
    [HideInInspector]
    public Transform Seat;
    [HideInInspector]
    public GameObject Effects;
    [HideInInspector]
    public Transform Lights;
    [HideInInspector]
    public Transform ExitLocation;

    Transform Model;
    Transform FRWheel;
    Transform FLWheel;
    Transform SteeringWheel;
    Transform Addons;

    public VehicleConfig Config;
    
    [HideInInspector]
    public List<MeshRenderer> Meshes;

    [HideInInspector]
    public PurchaseAble PA = null;
    Rigidbody r;
    MenuManager MM;
    List<GameObject> Carried;
    
    float WheelTurnDeg = 25f;
    float SteeringTurnDeg = 55f;

    [HideInInspector]
    public HUD RadioHUD;

    [HideInInspector]
    public MirrorVehicle MRD;

    void Start()
    {
        MRD = GetComponent<MirrorVehicle>();
        MRD.Initialize();

        Initialize();
        
        if(this.gameObject.tag == "EODD")
        {
            MRD.CmdToggleLocked();
        }

        ID = GetComponent<NetworkIdentity>();
        r = GetComponent<Rigidbody>();
        r.centerOfMass = Config.CenterOfMass;
        MM = GameObject.FindWithTag("Camera").GetComponent<MenuManager>();
        RadioHUD = GameObject.FindWithTag("HUD").GetComponent<HUD>();
        if(ID.hasAuthority)
        {
            GetComponent<MirrorVehicle>().CmdRefreshVehicle();
        }
    }

    void Initialize()
    {
        Model = transform.GetChild(0);
        FRWheel = Model.transform.GetChild(1);
        FLWheel = Model.transform.GetChild(2);
        SteeringWheel = Model.transform.GetChild(3);
        Addons = Model.transform.GetChild(4);

        Seat = transform.GetChild(2).GetChild(0);

        Effects = transform.GetChild(4).gameObject;

        Lights = transform.GetChild(5);
        Lights.gameObject.SetActive(false);

        ExitLocation = transform.GetChild(6);

        AS_Engine = transform.GetChild(3).GetChild(0).GetComponent<AudioSource>();
        AS_Horn = transform.GetChild(3).GetChild(1).GetComponent<AudioSource>();
        AS_Tire = transform.GetChild(3).GetChild(2).GetComponent<AudioSource>();
        AS_Radio = transform.GetChild(3).GetChild(3).GetComponent<AudioSource>();
    }

    public void Trigger()
    {
        if(!InShop)
        {
            Model.transform.localPosition = Vector3.zero;
            VehicleToggle(GameObject.FindWithTag("Player").GetComponent<Player>());
        }
    }

    void VehicleToggle(Player P)
    {
        if(!P.RagdollEnabled)
        {
            if(P.CurrentVehicle == this)    //if the player is already in the vehicle
            {
                P.LeaveVehicle(false);

            }else if(P.CurrentVehicle == null)    //if the player isn't in a vehicle
            {
                if(!MRD.Occupied)
                {
                    GetComponent<SwappableAuthority>().AskForAuthority();

                    if(MRD.Locked)
                    {
                        GameObject.FindWithTag("Camera").GetComponent<Interact>().Clear();
                        if(PlayerInfo.Balance >= Config.VehicleBaseValue)
                        {
                            PlayerInfo.Balance -= Config.VehicleBaseValue;
                            MRD.CmdToggleLocked();
                        }
                        //GameObject.FindWithTag("Camera").GetComponent<Interact>().Clear();
                    }else
                    {
                        P.CurrentVehicle = this;
                        P.ChangeEquipped(0);
                        MRD.CmdSetOccupied(true);
                        ToggleCarried(true);
                        MenuManager.MMInstance.fovOffset = 0f;
                        RadioHUD.VC = this;
                        RadioHUD.InCar = true;
                        RadioHUD.SetStation(RadioHUD.CurrentStation);

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
        }
    }

    void FixedUpdate()
	{   
        if(ID.hasAuthority && MRD.Occupied)
        {
            if(!InShop && MM.CurrentScreen == "HUD" && !MenuManager.MMInstance.ConsoleOpen)
            {
                Movement();
            }
        }
	}

    void Update()
    {
        if(!MRD.Locked)
        {
            EngineSounds();
            SkidSounds();
            TurnWheels();
        }

        if(MRD.Occupied && MM.CurrentScreen == "HUD" && !MenuManager.MMInstance.ConsoleOpen)
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

    void TurnWheels()
    {
        FRWheel.localEulerAngles = new Vector3(0f, MRD.TurnAmount * WheelTurnDeg, 0f);
        FLWheel.localEulerAngles = new Vector3(0f, MRD.TurnAmount * WheelTurnDeg, 0f);
        SteeringWheel.localEulerAngles = new Vector3(SteeringWheel.localEulerAngles.x, SteeringWheel.localEulerAngles.y, MRD.TurnAmount * SteeringTurnDeg);
    }

    void SkidSounds() //Perfect example of command usage without RPC calls
    {
        float maxSkid = (Config.InstallableKits[MRD.KitIndex].TopSpeed/2f);

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
            MRD.EngineAlpha = r.velocity.magnitude/Config.InstallableKits[MRD.KitIndex].TopSpeed;

            if(MRD.EngineAlpha > 1f)
                MRD.EngineAlpha = 1f;

            MRD.CmdSendEngineInfo(MRD.EngineAlpha);
        }
        
        AS_Engine.pitch = Mathf.Lerp(Config.PitchLow, Config.PitchHigh, MRD.EngineAlpha);
        AS_Engine.volume = (.5f * (Settings._sfxVolume/100f)) * (Settings._masterVolume/100f);
    }

    public void AuthorityGranted()
    {
        r = GetComponent<Rigidbody>();
        r.isKinematic = false;
    }

    public void AuthorityRevoked()
    {
        r = GetComponent<Rigidbody>();
        r.isKinematic = true;
    }

    public void ToggleCarried(bool Freeze)
    {
        if(CarryBounds != null)
        {
            if(Freeze)
            {
                Collider[] DetectedItems = Physics.OverlapBox(CarryBounds.transform.position, CarryBounds.size, CarryBounds.transform.rotation);
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

    void Movement()
    {
		if(Input.GetKey("w") || Input.GetKey("a") || Input.GetKey("s") || Input.GetKey("d"))
        {
            Vector3 MoveDirection = Vector3.zero;
            float TurnDirection = 0f;

            if (Input.GetKey("w"))
                MoveDirection += transform.forward;
            

            if (Input.GetKey("a")) 
                TurnDirection += -1f;

            if (Input.GetKey("s")) 
                MoveDirection += -transform.forward;
            

            if (Input.GetKey("d")) 
                TurnDirection += 1f;

            if(isGrounded())
            {
                if(r.velocity.magnitude < Config.InstallableKits[MRD.KitIndex].TopSpeed)
                {
                    r.AddForce(MoveDirection * Config.InstallableKits[MRD.KitIndex].Acceleration);  //USE THIS ONE
                }
                r.AddTorque((transform.up * Config.Turn * TurnDirection * r.mass) * Time.fixedDeltaTime);
            }

        }

        float Alpha = (r.velocity.magnitude - 25f)/750f;

        if(Alpha > 1f)
            Alpha = 1f;
        
        if(Alpha < 0f)
            Alpha = 0f;

        float ShakeAmount = Mathf.Lerp(0f, 0.15f, Alpha);

        Vector3 ShakeVec = new Vector3(Random.Range(-ShakeAmount, ShakeAmount),Random.Range(-ShakeAmount, ShakeAmount),Random.Range(-ShakeAmount, ShakeAmount));

        Model.transform.localPosition = ShakeVec;
        //Addons.transform.localPosition = ShakeVec;
    }

    void OnCollisionEnter(Collision col)    //STORE REFERENCE TO PLAYER ON OWN LATER YOU STUPID IDIOT GOD DAMN
    {
        if(MRD.Occupied && ID.hasAuthority)
        {
            if(col.impulse.magnitude > 1000f || col.gameObject.CompareTag("Hazard"))
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

    public void SetParts(List<int> NewPartsIndices)
    {
        //Clear old children gameobjects
        foreach (Transform Child in Addons)
        {
            Destroy(Child.gameObject);
        }

        for(int i = 0; i < NewPartsIndices.Count; i++)
        {
            if(Config.InstallableParts[NewPartsIndices[i]].PartPrefab != null)
            {
                //Debug.Log(NewParts[i].PartName);
                GameObject PartObject = Instantiate(Config.InstallableParts[NewPartsIndices[i]].PartPrefab, Vector3.zero, Quaternion.identity);
                PartObject.transform.parent = Addons;
                PartObject.transform.localPosition = Vector3.zero;
                PartObject.transform.localEulerAngles = Vector3.zero;
                PartObject.transform.localScale = Vector3.one;

                if(PartObject.GetComponent<PartFunction>() != null)
                {
                    PartObject.GetComponent<PartFunction>().Activate(this);
                }
                NetworkServer.Spawn(PartObject, ID.connectionToClient);
            }
        }

        RecalcMeshes();

    }

    public void RecalcMeshes()
    {
        Meshes.Clear();

        foreach (Transform T in Addons)
            Meshes.Add(T.GetChild(0).GetComponent<MeshRenderer>());

        Transform ModelLoc = transform.GetChild(0);
        MeshRenderer[] BodyMeshes = ModelLoc.GetChild(0).GetComponentsInChildren<MeshRenderer>();
        foreach (MeshRenderer MR in BodyMeshes)
            Meshes.Add(MR);

        Meshes.Add(ModelLoc.GetChild(1).GetChild(0).GetComponent<MeshRenderer>());
        Meshes.Add(ModelLoc.GetChild(2).GetChild(0).GetComponent<MeshRenderer>());
        Meshes.Add(ModelLoc.GetChild(3).GetChild(0).GetComponent<MeshRenderer>());
    }
}
