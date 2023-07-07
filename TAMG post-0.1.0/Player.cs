using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Mirror;
using TMPro;

public class Player : MonoBehaviour
{
    [System.Serializable]
    public struct Equippable
    {
        public string Name;
        public GameObject Prefab;
        public Texture2D Crosshair;
        public int HoldAnimationIndex;
    }

    float Maxspeed = 4f;
    float SmoothCamSpeed = 10f;
    float SprintMultiplier = 1.5f;
    float DeathForceThreshold = 100f;

    float camLimits = 85f;
    float JumpForce = 2500f;
    float CurrentSprint = 1f;
    float SittingcamLimits = 60f;
    float MovementForce = 300f;
    float speedAlpha = 0f;

    float WheelAlpha = 0f;

    float FOVGoal = 90f;
    float CurrentFOV = 90f;

    public bool CollidersOn = true;
    bool RespawnInBed = false;

    [Header("Controller Vars")]
    public GameObject ToPitch;
    public GameObject CamEmpty;
    [HideInInspector]
    public Transform CamOverride = null;
    public Animator An;
    public BodyPositions BodyInfo;
    [HideInInspector]
    public Vehicle CurrentVehicle;
    public GameObject RagdollPrefab;
    public bool RagdollEnabled = false;
    [HideInInspector]
    public float yaw = 0f;
    [HideInInspector]
    public float pitch = 0f;
    [HideInInspector]
    public Rigidbody r;
    public List<Equippable> EQs;
    [HideInInspector]
    public int EquippedIndex = 0;
    public Transform EquipSlot;
    public GameObject EquippedInstance;

    PickupAble CurrentPicked;
    Camera Cam;
    VolumeController VC;

    public Collider Col;

    NetworkIdentity NetID;

    [Header("Network Vars")]
    public SkinnedMeshRenderer Model;
    [HideInInspector]
    public MirrorPlayer MRD;

    public Material[] Mats = new Material[4];

    void Start()
    {
        //------------------------------------------------------- CLIENT AND SERVER
        Col = this.transform.GetComponent<Collider>();

        NetID = GetComponent<NetworkIdentity>();
        MRD = GetComponent<MirrorPlayer>();

        if (NetID.hasAuthority)//--------------------------------- CLIENT
        {
            if (!Client.Instance.ServerInstance)
            {
                Client.Instance.ServerInstance = GameObject.FindWithTag("Manager").GetComponent<Server>();
                Client.Instance.ServerInstance.SetupPlayer();
            }

            this.gameObject.tag = "Player";

            Cam = GameObject.FindWithTag("Camera").GetComponent<Camera>();
            VC = Cam.GetComponent<VolumeController>();
            r = GetComponent<Rigidbody>();
            r.isKinematic = false;

            pitch = ToPitch.transform.localEulerAngles.x;
            
            UI.Instance.GetScreen("PlayerCustomization").GetComponent<CustomizationMenu>().Init();

        }
        else//-------------------------------------------------- SERVER
        {
            ToggleColliders();
        }

        //Voice.AS.pitch = MRD.VoicePitch; called on rpc
    }

    void Update()
    {
        if (NetID.hasAuthority)
        {
            if (UI.Instance.CurrentScreen == "HUD" && !UI.Instance.ConsoleOpen)
            {
                if (Input.GetKeyDown(KeyCode.Space))
                    if (isGrounded())
                        r.AddForce(Vector3.up * JumpForce);

                An.SetFloat("Arms_Direction", pitch);

                if (Input.GetKeyDown("k"))
                    Die("Suicide :(");

                if (CurrentVehicle == null)
                {
                    if (Input.GetKeyDown(KeyCode.Alpha1))
                        ChangeEquipped(0);
                    if (Input.GetKeyDown(KeyCode.Alpha2))
                        ChangeEquipped(1);
                    if (Input.GetKeyDown(KeyCode.Alpha3))
                        ChangeEquipped(2);
                    if (Input.GetKeyDown(KeyCode.Alpha4))
                        ChangeEquipped(3);
                }
                else
                {
                    if (Input.GetKey("a") || Input.GetKey("d"))
                    {
                        if (Input.GetKey("a"))
                        {
                            TurnWheel(-1f, false);
                        }

                        if (Input.GetKey("d"))
                        {
                            TurnWheel(1f, false);
                        }
                    }
                    else
                    {
                        TurnWheel(0f, false);
                    }
                }

                if(Input.GetMouseButtonDown(0))
                {
                    if(EquippedInstance != null)
                    {
                        MRD.CmdUseEquipped(EquippedIndex);
                    }
                }
                
                if(Input.GetMouseButton(1))
                    FOVGoal = 40f;
                else
                    FOVGoal = 90f;

                CurrentFOV = Mathf.Lerp(CurrentFOV, FOVGoal, Time.deltaTime * 10f);
                Cam.fieldOfView = CurrentFOV;

            }
        }
    }

    void FixedUpdate()
    {
        if (NetID.hasAuthority)
        {
            if (!RagdollEnabled && !UI.Instance.Busy())
            {
                Movement();
                CameraControls();
            }

            Vector3 CurrentVelocity = new Vector3(r.velocity.x, 0f, r.velocity.z);

            speedAlpha = Smooth((CurrentVelocity.magnitude) / (Maxspeed * SprintMultiplier));

            An.SetFloat("Move_Speed", speedAlpha);
        }
    }

    void LateUpdate()
    {
        if (NetID.hasAuthority)
        {
            if (CamOverride != null)
            {
                Cam.transform.position = CamOverride.transform.position;
                Cam.transform.rotation = CamOverride.transform.rotation;
            }
            else
            {
                Cam.transform.position = CamEmpty.transform.position;
                Cam.transform.rotation = CamEmpty.transform.rotation;
            }
        }
    }

    void OnCollisionEnter(Collision col)
    {
        if (NetID.hasAuthority)
        {
            if (col.impulse.magnitude > DeathForceThreshold || col.gameObject.CompareTag("Hazard"))
            {
                if (col.gameObject.CompareTag("Hazard"))
                    Die("Hit a Hazard");
                else
                    Die("Hit something too hard");
            }
        }
    }

    void OnTriggerEnter(Collider col)
    {
        if (NetID.hasAuthority)
        {
            if (col.gameObject.CompareTag("Hazard"))
            {
                Die("Hit a Hazard");
            }
        }
    }

    public void ChangeEquipped(int Index)
    {
        if(Index != EquippedIndex)
        {
            Cam.GetComponent<Interact>().Clear();
            ClearHands(false);

            if(EquippedInstance != null)
            {
                Destroy(EquippedInstance);
            }

            if(EQs[Index].Prefab != null)
            {
                EquippedInstance = Instantiate(EQs[Index].Prefab, Vector3.zero, Quaternion.identity);
                EquippedInstance.transform.SetParent(EquipSlot.transform);
                EquippedInstance.transform.localPosition = Vector3.zero;
                EquippedInstance.transform.localEulerAngles = Vector3.zero;
                EquippedInstance.GetComponent<Invoker>().Activate();
            }

            An.SetInteger("Arms_Index", EQs[Index].HoldAnimationIndex);

            EquippedIndex = Index;

            MRD.CmdChangeEquipped(EquippedIndex);
        }
    }

    public void TurnWheel(float Goal, bool Override)
    {
        if(!Override)
        {
            WheelAlpha = Mathf.Lerp(WheelAlpha, Goal, Time.deltaTime * 5f);
            An.SetFloat("Sitting_Direction", WheelAlpha);
            CurrentVehicle.MRD.CmdSendWheelInfo(WheelAlpha);

        }else
        {
            An.SetFloat("Sitting_Direction", Goal);
            CurrentVehicle.MRD.CmdSendWheelInfo(Goal);
        }
    }

    void Movement()
    {
		if(Input.GetKey("w") || Input.GetKey("a") || Input.GetKey("s") || Input.GetKey("d"))
        {
            if(Input.GetKey(KeyCode.LeftShift))
            {
                CurrentSprint = SprintMultiplier;
            }else
            {
                CurrentSprint = 1f;
            }

            Vector3 MoveDirection = Vector3.zero;

            if (Input.GetKey("w"))
                MoveDirection += transform.forward;

            if (Input.GetKey("a")) 
                MoveDirection += -transform.right * 0.75f;

            if (Input.GetKey("s")) 
                MoveDirection += -transform.forward;

            if (Input.GetKey("d")) 
                MoveDirection += transform.right * 0.75f;

            if(r.velocity.magnitude < Maxspeed * CurrentSprint)
            {
                r.AddForce(MoveDirection * CurrentSprint * MovementForce);  //USE THIS ONE
            }
        }
    }

    void CameraControls()  
    {
        if(CurrentVehicle == null)
        {
            yaw += (Settings._mouseSensitivity/100f) * Input.GetAxis("Mouse X");
            this.transform.localEulerAngles = Vector3.Lerp(new Vector3(0f, yaw - (Settings._mouseSensitivity/100f) * Input.GetAxis("Mouse X"), 0f),new Vector3(0f, yaw, 0f),Time.fixedDeltaTime * SmoothCamSpeed);

            if(Settings._invertedLook)
            {
                pitch += (Settings._mouseSensitivity/100f) * Input.GetAxis("Mouse Y");
            }else
            {
                pitch -= (Settings._mouseSensitivity/100f) * Input.GetAxis("Mouse Y");
            }
            
            if(pitch >= camLimits)
                pitch = camLimits;
                
            if(pitch <= -camLimits)
                pitch = -camLimits;
            
            float y = ToPitch.transform.localEulerAngles.y;
            float z = ToPitch.transform.localEulerAngles.z;

            ToPitch.transform.localEulerAngles = new Vector3(pitch, 0f, 0f);
        }else
        {
            An.SetFloat("Move_Speed", 0);

            this.transform.localEulerAngles = new Vector3(0f, 0f, 0f);
            float SittingLookLimits = 80f;

            yaw += (Settings._mouseSensitivity/100f) * Input.GetAxis("Mouse X");

            if(Settings._invertedLook)
            {
                pitch += (Settings._mouseSensitivity/100f) * Input.GetAxis("Mouse Y");
            }else
            {
                pitch -= (Settings._mouseSensitivity/100f) * Input.GetAxis("Mouse Y");
            }
            
            if(pitch >= SittingcamLimits)
                pitch = SittingcamLimits;
                
            if(pitch <= -SittingcamLimits)
                pitch = -SittingcamLimits;

            if(yaw >= SittingLookLimits)
                yaw = SittingLookLimits;
                
            if(yaw <= -SittingLookLimits)
                yaw = -SittingLookLimits;

            float z = ToPitch.transform.localEulerAngles.z;

            ToPitch.transform.localEulerAngles = new Vector3(pitch, yaw, z);
        }
    }

    List<float> speedAverages = new List<float>(){0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f};
    float Smooth(float x)
    {
        speedAverages.RemoveAt(0);
        speedAverages.Add(x);

        float Average = 0f;

        for(int i = 0; i < speedAverages.Count; i++)
        {
            Average += speedAverages[i];
        }

        Average = (Average/speedAverages.Count);
        if(Average > 1f)
            Average = 1f;

        if(Average < 0.005f)
            Average = 0f;

        return Average;
    }

    public void SetLook(float y, float p)
    {
        yaw = y;
        pitch = p;

        if(y == 0f)
        {
            float z = ToPitch.transform.localEulerAngles.z;
            ToPitch.transform.localEulerAngles = new Vector3(pitch, yaw, z);
        }else
        {
            this.transform.localEulerAngles = new Vector3(0f, yaw, 0f);
            ToPitch.transform.localEulerAngles = new Vector3(pitch, 0f, 0f);
        }
    }
    
    public void ToggleColliders()       //used to transition in and out of vehicles
    {
        ClearHands(true);
        CollidersOn = !CollidersOn;
        Col.enabled = CollidersOn;
    }

    public void Die(string message)
    {
        if(!RagdollEnabled && NetID.hasAuthority)
        {
            Cam.GetComponent<Interact>().Clear();
            Debug.Log(message);
            LeaveVehicle(true);
            ClearHands(false);
            RagdollEnabled = true;

            Model.gameObject.SetActive(false); 

            if(EquippedInstance != null)
            {
                Destroy(EquippedInstance);
            }

            GameObject NewRagdoll = Instantiate(RagdollPrefab, this.transform.position, this.transform.rotation);
            NewRagdoll.GetComponent<Ragdoll>().Init(this, r.velocity, r.angularVelocity);

            MRD.CmdDie(r.velocity, r.angularVelocity);

            r.collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;
            r.isKinematic = true;
            
            ToggleColliders();
            UI.Instance.SetScreen("Death");

            StartCoroutine(Respawn());        
        }
    }

    IEnumerator Respawn()
    {
        int fullBlackTime = 1;  //must be less than respawn time
        int respawnTime = Mathf.RoundToInt(Client.Instance.ServerInstance.ServerRules.PlayerRespawnTime);
        int increment = 60;     //60 increments per second
        int i = 0;              //starts at zero
        int totalWait = increment * (respawnTime - fullBlackTime);   //total increments before finished
        while(i < totalWait)
        {
            if(Settings._quickBlackout)
            {
                VC.DeathFade(1f);         //set alpha value
            }else
            {
                VC.DeathFade((float) i/(float) totalWait);         //set alpha value
            }

            
            yield return new WaitForSeconds(1f/increment);          //wait for fraction of second (frame)
            i++;                                               //
        }

        VC.DeathFade(1f);
        yield return new WaitForSeconds(fullBlackTime);


        Model.gameObject.SetActive(true);
        r.isKinematic = false;
        r.collisionDetectionMode = CollisionDetectionMode.Continuous;
        
        CamOverride = null;
        RagdollEnabled = false;
        ToggleColliders();
        if(!RespawnInBed)
        {
            this.transform.position = Client.Instance.NearbyPoint(this.transform.position, 40f);
            //this.transform.position = OfflineManager.Offline.GetNearestSpawn(this.transform.position).position;
            //this.transform.rotation = OfflineManager.Offline.GetNearestSpawn(this.transform.position).rotation;

        }else
        {
            this.transform.position = Vector3.zero; //To bed bed location to be;
        }

        Cam.GetComponent<AudioReverbFilter>().reverbPreset = AudioReverbPreset.Off;
        yield return null;
        UI.Instance.SetScreen("HUD");
        yield return null;
        VC.DeathFade(0f);

        RespawnInBed = false;

        yield return new WaitForSeconds(1f);
        MRD.CmdRespawn();
    }

    public void Passout()
    {
        RespawnInBed = true;
        Die("Fell Asleep");
    }

    public void ObjectPicked(PickupAble P)
    {
        if(!RagdollEnabled && CurrentVehicle == null && EquippedIndex == 0)    //can't pickup if dead or in a vehicle
        {
            if(CurrentPicked == P)      //if picked up is same as in hands
            {
                ClearHands(true);           //drop it

            }else if(CurrentPicked == null)     //if hands empty
            {
                P.GetComponent<SwappableAuthority>().AskForAuthority();
                CurrentPicked = P;              //fill hands
                CurrentPicked.Pickup();
                An.SetFloat("Arms_Direction", pitch);
                An.SetInteger("Arms_Index", 1);
            }else                               //if different object is picked up when hands full
            {
                CurrentPicked.Drop();           //drop old 
                CurrentPicked = P;              //assign new
                CurrentPicked.Pickup();
                An.SetFloat("Arms_Direction", pitch);
                An.SetInteger("Arms_Index", 1);
            }
        }
    }

    public void LeaveVehicle(bool dontMove)
    {
        if(CurrentVehicle != null)
        {
            CurrentVehicle.Driving = false;
            CurrentVehicle.ToggleCarried(false);
            

            r.isKinematic = false;
            r.collisionDetectionMode = CollisionDetectionMode.Continuous;

            Vector3 CarVelocity = CurrentVehicle.GetComponent<Rigidbody>().velocity;
            r.velocity = CarVelocity;

            ToggleColliders();

            An.SetInteger("Sitting", 0);
            TurnWheel(0f, true);
            //An.SetFloat("Sitting_Direction", 0f);
            this.transform.SetParent(null);

            if(!dontMove)
            {
                this.transform.position = CurrentVehicle.ExitLocation.position;
                this.transform.localEulerAngles = new Vector3(0f, CurrentVehicle.ExitLocation.eulerAngles.y, 0f);
                SetLook(CurrentVehicle.ExitLocation.eulerAngles.y, CurrentVehicle.SavedLook.y);
            }

            CurrentVehicle = null;

            if(CarVelocity.magnitude > 10f)     //kill if exit vehicle when going to quick
                StartCoroutine(TimedDeath(0.1f, "Left a moving vehicle"));
        }
    }
    
    IEnumerator TimedDeath(float t, string message)
    {
        yield return new WaitForSeconds(t);
        Die(message);
    }

    public void ClearHands(bool PutArmsDown)
    {
        if(CurrentPicked != null)
            CurrentPicked.Drop();
        CurrentPicked = null;
        if(PutArmsDown)
            An.SetInteger("Arms_Index", 0);
    }

    public bool isGrounded()
    {
        RaycastHit hit;
        if(Physics.Raycast(this.transform.position + new Vector3(0f, 0.05f, 0f), -Vector3.up, out hit, 0.2f))
            return true;
        else
            return false;
    }
}
