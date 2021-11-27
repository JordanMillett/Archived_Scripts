using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class VehicleController : NetworkBehaviour
{
    /*
    public VehicleConfig Config;
    public bool InShop = false;
    public Vector2 SavedLook = Vector2.zero;
    public BoxCollider CarryBounds;
    public AutoShop CurrentShop;
    public NetworkIdentity ID;
    public EngineKit EK;
    public AudioClip CrashSound;
    public AudioClip Horn;
    public AudioSource AS_Engine;
    public AudioSource AS_Horn;
    public AudioSource AS_Tire;
    public AudioSource AS_Radio;
    public Transform Model;
    public Transform FRWheel;
    public Transform FLWheel;
    public Transform SteeringWheel;
    public Transform Seat;
    public Transform ExitLocation;
    public Transform Mass;
    public GameObject Exhaust;
    public Transform Addons;
    public Transform Lights;

    public List<MeshRenderer> Meshes;
    
    public List<VehiclePart> CurrentParts;

    PurchaseAble PA = null;
    Rigidbody r;
    MenuManager MM;
    List<GameObject> Carried;
    
    float WheelTurnDeg = 25f;
    float SteeringTurnDeg = 55f;

    //bool SwappingGears = false;
    //int TotalGears = 3;
    //int CurrentGear = 1;
    //float IdleRPM = 1000f;
    //float SwapDifferenceRPM = 1000f;
    //float MaxRPM = 4000f;
    //float RPMIncrease = 10f;
    //float RPMDecrease = 50f;
    //float CurrentRPM = 1000f;

    Color[] DefaultColors = new Color[5]
    {
        new Color(239f/255f,240f/255f,215f/255f,1), //Paint
        new Color(94f/255f,107f/255f,130f/255f,1),  //Body
        new Color(248f/255f,229f/255f,116f/255f,1), //Seat
        new Color(55f/255f,50f/255f,84f/255f,1),    //Tire
        new Color(155f/255f,156f/255f,130f/255f,1) //Wheel
    };

    [SyncVar]
    public bool Occupied = false;

    [SyncVar]
    public float EngineAlpha = 0f;

    [SyncVar]
    public float SkidAmount = 0f;

    [SyncVar]
    public bool Locked = false;

    [SyncVar]
    public bool LightsOn = false;
    
    bool Initialized = false;

    public HUD RadioHUD;
    */
    /*

    void Start()
    {
        if(this.gameObject.tag == "EODD")
        {
            CmdToggleLocked();
        }

        ID = GetComponent<NetworkIdentity>();
        r = GetComponent<Rigidbody>();
        r.centerOfMass = Mass.localPosition;
        MM = GameObject.FindWithTag("Camera").GetComponent<MenuManager>();
        RadioHUD = GameObject.FindWithTag("HUD").GetComponent<HUD>();
        if(ID.hasAuthority)
        {
            SetParts(CurrentParts);
            GetComponent<CarVarSync>().UpdateInfo(DefaultColors, GetComponent<VehicleController>().EK, 0);
        }

        Initialized = true;
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
                if(!Occupied)
                {
                    GetComponent<SwappableAuthority>().AskForAuthority();

                    if(Locked)
                    {
                        GameObject.FindWithTag("Camera").GetComponent<Interact>().Clear();
                        if(PlayerInfo.Balance >= Config.VehicleBaseValue)
                        {
                            PlayerInfo.Balance -= Config.VehicleBaseValue;
                            CmdToggleLocked();
                        }
                        //GameObject.FindWithTag("Camera").GetComponent<Interact>().Clear();
                    }else
                    {
                        P.CurrentVehicle = this;
                        P.ChangeEquipped(0);
                        CmdSetOccupied(true);
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
                        //P.An.SetFloat("Sitting_Direction", 0f);
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
        if(ID.hasAuthority && Occupied)
        {
            if(!InShop && MM.CurrentScreen == "HUD" && !MenuManager.MMInstance.ConsoleOpen)
            {
                Movement();
            }
            else
            {
                Decelerate();
            }
        }
	}

    void Update()
    {
        if(!Locked)
        {
            EngineSounds();
            SkidSounds();
        }

        if(Occupied && MM.CurrentScreen == "HUD" && !MenuManager.MMInstance.ConsoleOpen)
        {
            if(Input.GetKeyDown("e"))
            {
                if(ID.hasAuthority)
                {
                    CmdPlayHorn();
                }
            }

            if(Input.GetKeyDown("r"))
            {
                if(ID.hasAuthority)
                {
                    CmdToggleLights();
                }
            }
        }
    }

    public void TurnWheels(float alpha)
    {
        FRWheel.localEulerAngles = new Vector3(0f, alpha * WheelTurnDeg, 0f);
        FLWheel.localEulerAngles = new Vector3(0f, alpha * WheelTurnDeg, 0f);
        SteeringWheel.localEulerAngles = new Vector3(SteeringWheel.localEulerAngles.x, SteeringWheel.localEulerAngles.y, alpha * SteeringTurnDeg);
    }

    [Command]
    public void CmdSetOccupied(bool status)
    {
        RpcSetOccupied(status);
    }

    [ClientRpc]
    void RpcSetOccupied(bool status)
    {
        Occupied = status;
    }

    [Command]
    void CmdPlayHorn()
    {
        RpcPlayHorn();
    }

    [ClientRpc]
    void RpcPlayHorn()
    {
        AS_Horn.volume = (.4f * (Settings._sfxVolume/100f)) * (Settings._masterVolume/100f);
        AS_Horn.clip = Horn;
        AS_Horn.Play();
    }

    [Command]
    void CmdToggleLights()
    {
        RpcToggleLights();
    }

    [ClientRpc]
    void RpcToggleLights()
    {
        LightsOn = !LightsOn;
        Lights.gameObject.SetActive(LightsOn);
    }

    [Command]
    void CmdToggleLocked()
    {
        RpcToggleLocked();
    }

    [ClientRpc]
    void RpcToggleLocked()
    {
        Locked = !Locked;

        if(Locked == false)
        {
            Destroy(PA);
            this.gameObject.tag = "Untagged";
            Exhaust.SetActive(true);
        }else
        {
            PA = this.gameObject.AddComponent(typeof(PurchaseAble)) as PurchaseAble;
            PA.Name = Config.VehicleName;
            PA.Cost = Config.VehicleBaseValue;
            Exhaust.SetActive(false);
        }
    }

    [Command]
    void CmdPlayCrash(Vector3 CrashPos)
    {
        RpcPlayCrash(CrashPos);
    }

    [ClientRpc]
    void RpcPlayCrash(Vector3 CrashPos)
    {
        GameObject Sound = new GameObject();
        Sound.transform.position = CrashPos;
        Sound.transform.SetParent(this.transform);
        Despawn DSpawn = Sound.AddComponent(typeof(Despawn)) as Despawn;

        AudioClip Clip = CrashSound;

        DSpawn.DespawnTime = Clip.length + 0.1f;
        AudioSource AS = Sound.AddComponent(typeof(AudioSource)) as AudioSource;
        AS.spatialBlend = 1f;
        AS.volume = (.1f * (Settings._sfxVolume/100f)) * (Settings._masterVolume/100f);
        AS.maxDistance = 1000f;
        AS.clip = Clip;
        AS.Play();
    }

    void SkidSounds()
    {
        float maxSkid = (EK.TopSpeed/2f);

        if(ID.hasAuthority)
        {
            SkidAmount = Mathf.Abs(transform.InverseTransformDirection(r.velocity).x);
            SkidAmount -= 5f;

            if(SkidAmount > maxSkid)
                SkidAmount = maxSkid;

            CmdSendSkidInfo(SkidAmount);
        }
        
        float tirealpha = Mathf.Lerp(0f, .15f, SkidAmount/maxSkid);

        AS_Tire.volume = (tirealpha * (Settings._sfxVolume/100f)) * (Settings._masterVolume/100f);
    }

    [Command]
    public void CmdSendSkidInfo(float _skidAmount)
    {
        SkidAmount = _skidAmount;
    }

    void EngineSounds()
    {
        if(ID.hasAuthority)
        {
            //if(EngineAlpha % (float)1f/Gears > )
            
            //if(EngineAlpha % (1f/Gears))


            EngineAlpha = r.velocity.magnitude/EK.TopSpeed;
            //EngineAlpha = (CurrentRPM - IdleRPM)/(MaxRPM - IdleRPM);

            if(EngineAlpha > 1f)
                EngineAlpha = 1f;

            //Debug.Log(EngineAlpha % (1f/Gears));

            CmdSendEngineInfo(EngineAlpha);
        }
        
        AS_Engine.pitch = Mathf.Lerp(Config.PitchLow, Config.PitchHigh, EngineAlpha);
        AS_Engine.volume = (.5f * (Settings._sfxVolume/100f)) * (Settings._masterVolume/100f);
    }

    [Command]
    public void CmdSendEngineInfo(float _EngineAlpha)
    {
        EngineAlpha = _EngineAlpha;
    }

    public void AuthorityGranted()
    {
        //Debug.Log("KINEMATIC FALSE");
        r = GetComponent<Rigidbody>();
        r.isKinematic = false;

        if(Initialized)
            SetParts(CurrentParts); //used to re invoke parts to apply changes 
    }

    public void AuthorityRevoked()
    {
        //Debug.Log("KINEMATIC TRUE");
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
    */
    /*
    void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(1, 0, 0, 0.5f);
        Gizmos.DrawCube(CarryBounds.transform.position, CarryBounds.size);
    }*/
    /*
    void Movement()
    {
		if(Input.GetKey("w") || Input.GetKey("a") || Input.GetKey("s") || Input.GetKey("d"))
        {
            
            if(Input.GetKey(KeyCode.LeftShift))
            {
                CurrentBoost = 2f;
            }else
            {
                CurrentBoost = 1f;
            }

            Vector3 MoveDirection = Vector3.zero;
            float TurnDirection = 0f;

            if (Input.GetKey("w"))
            {
                MoveDirection += transform.forward;
                //Accelerate();
            }

            if (Input.GetKey("a")) 
                TurnDirection += -1f;

            if (Input.GetKey("s")) 
            {
                MoveDirection += -transform.forward;
                //Decelerate();
            }

            if (Input.GetKey("d")) 
                TurnDirection += 1f;

            
            if(r.velocity.magnitude < Maxspeed)
            {
                
            }

            if(isGrounded())
            {
                if(r.velocity.magnitude < EK.TopSpeed)
                {
                    r.AddForce(MoveDirection * EK.Acceleration);  //USE THIS ONE
                    //r.AddForce((MoveDirection * EK.Acceleration) * ((CurrentRPM/MaxRPM) * CurrentGear));  //USE THIS ONE
                }
                //r.velocity += (MoveDirection * Acceleration * CurrentBoost) * Time.fixedDeltaTime;
                r.AddTorque((transform.up * Config.Turn * TurnDirection * r.mass) * Time.fixedDeltaTime);
            }

            

            
            //r.AddForce(MoveDirection * AccelerationForce);
            //r.AddTorque(transform.up * Torque * Turn);

            //this.transform.localEulerAngles += new Vector3(0f, Torque, 0f);

        }else
        {
            Decelerate();
        }

        float Alpha = (r.velocity.magnitude - 25f)/750f;
        //float Alpha = r.velocity.magnitude/1000f;

        if(Alpha > 1f)
            Alpha = 1f;
        
        if(Alpha < 0f)
            Alpha = 0f;

        float ShakeAmount = Mathf.Lerp(0f, 0.15f, Alpha);

        Vector3 ShakeVec = new Vector3(Random.Range(-ShakeAmount, ShakeAmount),Random.Range(-ShakeAmount, ShakeAmount),Random.Range(-ShakeAmount, ShakeAmount));

        Model.transform.localPosition = ShakeVec;
        Addons.transform.localPosition = ShakeVec;
    }
    */
    /*

    void OnCollisionEnter(Collision col)    //STORE REFERENCE TO PLAYER ON OWN LATER YOU STUPID IDIOT GOD DAMN
    {
        if(Occupied && ID.hasAuthority)
        {
            if(col.impulse.magnitude > 1000f || col.gameObject.CompareTag("Hazard"))
            {
                GameObject.FindWithTag("Player").GetComponent<Player>().Die("Crashed");

                CmdPlayCrash(col.contacts[0].point);

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

    public void SetParts(List<VehiclePart> NewParts)
    {
        //Clear old children gameobjects
        foreach (Transform Child in Addons)
        {
            Destroy(Child.gameObject);
        }

        for(int i = 0; i < NewParts.Count; i++)
        {
            if(NewParts[i].PartPrefab != null)
            {
                //Debug.Log(NewParts[i].PartName);
                GameObject PartObject = Instantiate(NewParts[i].PartPrefab, Vector3.zero, Quaternion.identity);
                PartObject.transform.parent = Addons;
                PartObject.transform.localPosition = Vector3.zero;
                PartObject.transform.localEulerAngles = Vector3.zero;
                PartObject.transform.localScale = Vector3.one;

                if(PartObject.GetComponent<PartFunction>() != null)
                {
                    PartObject.GetComponent<PartFunction>().Activate(this);
                    //Meshes.Add(PartObject.GetComponent<PartFunction>().PartMesh);
                }
                //Debug.Log("Called");
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
    */
    /*
    void Accelerate()
    {
        if(!SwappingGears)
        {
            if(CurrentRPM < MaxRPM - RPMIncrease)
            {
                CurrentRPM += RPMIncrease;
            }else
            {
                if(CurrentGear == TotalGears)
                    return;
                StartCoroutine(ChangeGears(true));
            }
        }
    }

    void Decelerate()
    {
        if(!SwappingGears)
        {
            if(CurrentRPM > IdleRPM + RPMDecrease)
            {
                CurrentRPM -= RPMDecrease;
            }else
            {
                if(CurrentGear == 1)
                    return;
                StartCoroutine(ChangeGears(false));
            }
        }
    }

    IEnumerator ChangeGears(bool Up)
    {
        CurrentGear += Up ? 1 : -1;
        SwappingGears = true;
        CurrentRPM = IdleRPM;
        yield return new WaitForSeconds(0.5f);  //Gear Swap time
        if(Up)
            CurrentRPM = IdleRPM + SwapDifferenceRPM;
        else
            CurrentRPM = MaxRPM - SwapDifferenceRPM;

        SwappingGears = false;
    }*/


}
