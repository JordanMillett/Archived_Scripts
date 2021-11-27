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
    float FreeCamSpeed = 5f;

    float camLimits = 85f;
    float JumpForce = 2500f;
    float CurrentSprint = 1f;
    float SittingcamLimits = 60f;
    float MovementForce = 300f;
    float speedAlpha = 0f;

    float WheelAlpha = 0f;

    public bool CollidersOn = true;
    bool RespawnInBed = false;

    [Header("Controller Vars")]
    public GameObject ToPitch;
    public GameObject CamEmpty;
    public Transform CamOverride = null;
    public Animator An;
    public BodyPositions BodyInfo;
    public Vehicle CurrentVehicle;
    public GameObject RagdollPrefab;
    public bool RagdollEnabled = false;
    public float yaw = 0f;
    public float pitch = 0f;
    public Rigidbody r;
    public List<Equippable> EQs;
    public int EquippedIndex = 0;
    public Transform EquipSlot;
    public GameObject EquippedInstance;

    PickupAble CurrentPicked;
    GameObject Cam;
    VolumeController VC;
    
    public Collider[] AllColliders;

    AudioClip BufferClip;
    float[] VoiceBuffer;
    bool VoiceInitialized = false;
    float[] FloatBuffer = new float[2048];  //2048 for single channel speaker, 1024 for stereo channel

    NetworkIdentity NetID;

    [Header("Network Vars")]
    public SkinnedMeshRenderer Model;
    public Material[] Mats = new Material[4];

    public ASwithBuffer Voice;

    public MirrorPlayer MRD;

    #region Unity Functions

    void Start()
    {
        //------------------------------------------------------- CLIENT AND SERVER
        NetID = GetComponent<NetworkIdentity>();
        MRD = GetComponent<MirrorPlayer>();
        MRD.Initialize();
        
        if(NetID.hasAuthority)//--------------------------------- CLIENT
        {
            this.gameObject.tag = "Player";
            
            Cam = GameObject.FindWithTag("Camera");
            VC = GameObject.FindWithTag("Volume").GetComponent<VolumeController>();
            r = GetComponent<Rigidbody>();

            pitch = ToPitch.transform.localEulerAngles.x;
            AllColliders = this.transform.GetComponentsInChildren<Collider>();

            GameObject.FindWithTag("Camera").GetComponent<MenuManager>().CM.Init();

        }else//-------------------------------------------------- SERVER
        {
            MRD.MirrorStart();
        }

        //Voice.AS.pitch = MRD.VoicePitch; called on rpc
    }
    
    void Update()
    {
        if(NetID.hasAuthority)
        {
            if(MenuManager.MMInstance.CurrentScreen == "HUD" && !MenuManager.MMInstance.ConsoleOpen)
            {
                if(Input.GetKeyDown(KeyCode.Space))
                    if(isGrounded())
                        r.AddForce(Vector3.up * JumpForce);

                An.SetFloat("Arms_Direction", pitch);

                if(Input.GetKeyDown("k"))
                    Die("Suicide :(");

                if(Input.GetKey("v"))
                    TransmitVoice();

                if(CurrentVehicle == null)
                {
                    if(Input.GetKeyDown(KeyCode.Alpha1))
                        ChangeEquipped(0);
                    if(Input.GetKeyDown(KeyCode.Alpha2))
                        ChangeEquipped(1);
                    if(Input.GetKeyDown(KeyCode.Alpha3))
                        ChangeEquipped(2);
                    if(Input.GetKeyDown(KeyCode.Alpha4))
                        ChangeEquipped(3);
                }else
                {
                    if(Input.GetKey("a") || Input.GetKey("d"))
                    {
                        if(Input.GetKey("a"))
                        {
                            TurnWheel(-1f, false);
                        }

                        if(Input.GetKey("d"))
                        {
                            TurnWheel(1f, false);
                        }
                    }else
                    {
                        TurnWheel(0f, false);
                    }


                }
                
            }
        }
    }

    void FixedUpdate()
	{   
        if(NetID.hasAuthority)
        {
            if(!RagdollEnabled && MenuManager.MMInstance.CurrentScreen == "HUD" && !MenuManager.MMInstance.ConsoleOpen)
            {
                Movement();
                CameraControls();
            }

            if(MenuManager.MMInstance.CurrentScreen == "Freecam" && !MenuManager.MMInstance.ConsoleOpen)
            {
                FreeCameraMovement();
                FreeCameraControls();
            }

            Vector3 CurrentVelocity = new Vector3(r.velocity.x, 0f, r.velocity.z);

            speedAlpha = Smooth((CurrentVelocity.magnitude)/(Maxspeed * SprintMultiplier));

            An.SetFloat("Move_Speed", speedAlpha);
        }
	}

    void LateUpdate()
    {
        if(NetID.hasAuthority)
        {
            if(MenuManager.MMInstance.CurrentScreen != "Freecam")
            {
                if(CamOverride != null)
                {
                    Cam.transform.position = CamOverride.transform.position;
                    Cam.transform.rotation = CamOverride.transform.rotation;
                }else
                {
                    Cam.transform.position = CamEmpty.transform.position;
                    Cam.transform.rotation = CamEmpty.transform.rotation;
                }
            }
        }
    }

    void OnCollisionEnter(Collision col)
    {
        if(NetID.hasAuthority)
        {
            if(col.impulse.magnitude > DeathForceThreshold || col.gameObject.CompareTag("Hazard"))
            {
                if(col.gameObject.CompareTag("Hazard"))
                    Die("Hit a Hazard");
                else
                    Die("Hit something too hard");
            }
        }
    }

    void OnTriggerEnter(Collider col)
    {
        if(NetID.hasAuthority)
        {
            if(col.gameObject.CompareTag("Hazard"))
            {
                Die("Went out of bounds");
            }
        }
    }

    #endregion

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

    #region Voice Functions

    void TransmitVoice()
    {
        if(!VoiceInitialized)
        {
            InitializeVoice();
        }else
        {
            //int StartIndex = Microphone.GetPosition(Microphone.devices[0]);
            //BufferClip.GetData(VoiceBuffer, StartIndex);      
            //PG.CmdSetVoiceBuffer(VoiceBuffer);

            
            //int StartIndex = Microphone.GetPosition(Microphone.devices[0]);     //Get where microphone is currently writing to in loop
            //int StartIndex = 0;
            //BufferClip.GetData(VoiceBuffer, Microphone.GetPosition(Microphone.devices[0]));                        //get data of full loop in floats
            BufferClip.GetData(VoiceBuffer, Microphone.GetPosition(Microphone.devices[0]));     
            //Debug.Log(Microphone.GetPosition(Microphone.devices[0]));

            //new float buffer to send over network
            //short[] ShortBuffer = new short[2048];
            int Count = 0;
            //int shortMax = 30000; //32767

            float InputCutoff = 0.003f;

            //FloatBuffer = new float[2048];

            //Debug.Log(VoiceBuffer[VoiceBuffer.Length - 1]);

            while(Count < FloatBuffer.Length)   //while not bigger than 2048
            {   
                //ShortBuffer[Count] = Mathf.Abs(VoiceBuffer[Count]) > InputCutoff ? (short)Mathf.RoundToInt(((float)(((VoiceBuffer[Count] + 1f)/2f) * (shortMax * 2)) - shortMax)) : (short) 0;
                FloatBuffer[Count] = Mathf.Abs(VoiceBuffer[VoiceBuffer.Length - 1 - Count]) > InputCutoff ? VoiceBuffer[VoiceBuffer.Length - 1 - Count] : 0f;
                //FloatBuffer[Count + 1] = Mathf.Abs(VoiceBuffer[VoiceBuffer.Length - 1 - Count]) > InputCutoff ? VoiceBuffer[VoiceBuffer.Length - 1 - Count] : 0f;
                //Debug.Log(VoiceBuffer[VoiceBuffer.Length - 1 - Count]);
                //Buffer.BlockCopy(point, 16, dest, 22, 5);
                //byte[] temp = BitConverter.GetBytes(point);
                //SmallByteBuffer[Count] = temp[0];
                //SmallByteBuffer[Count + 1] = temp[1];
                //Debug.Log(SmallBuffer[Count]);

                //SmallBuffer[Count] = VoiceBuffer[Count];//SHRT_MAX
                //SmallBuffer[Count] = VoiceBuffer[VoiceBuffer.Length - Count - 1];
                Count++;
            }

            //Debug.Log(ShortBuffer[1024] + " - " + ((((VoiceBuffer[1024] + shortMax)/(shortMax * 2f)) * 2f) - 1f).ToString());

            //float --> int16 --> byte[]

            //BitConverter.ToSingle(SmallByteBuffer, i*4) / 0x80000000;
            
            MRD.CmdSetVoiceBuffer(VoiceBuffer);
        }
    }

    void InitializeVoice()
    {
        int minFreq = 0;
        int maxFreq = 0;
        Microphone.GetDeviceCaps(Microphone.devices[0], out minFreq, out maxFreq);
        int Samples = maxFreq; //48000 12288
        VoiceBuffer = new float[Samples];
        BufferClip = Microphone.Start(Microphone.devices[0], true, 1, Samples);

        float pitch = ((float) Samples/(float) maxFreq)/2f;

        MRD.CmdInitializeVoice(pitch);

        VoiceInitialized = true;
    }

    #endregion

    #region Controller

    void FreeCameraMovement()
    {
        Vector3 MoveDirection = Vector3.zero;

        if (Input.GetKey("w"))
            MoveDirection += Cam.transform.forward;

        if (Input.GetKey("a")) 
            MoveDirection += -Cam.transform.right;

        if (Input.GetKey("s")) 
            MoveDirection += -Cam.transform.forward;

        if (Input.GetKey("d")) 
            MoveDirection += Cam.transform.right;

        if (Input.GetKey("q")) 
            MoveDirection += -Vector3.up;

        if (Input.GetKey("e")) 
            MoveDirection += Vector3.up;

        float SpeedMult = 3f;

        if(Input.GetKey(KeyCode.LeftShift))
        {
            Cam.transform.Translate(MoveDirection * Time.deltaTime * FreeCamSpeed * SpeedMult, Space.World);
        }else
        {
            Cam.transform.Translate(MoveDirection * Time.deltaTime * FreeCamSpeed, Space.World);
        }
    }

    void FreeCameraControls()
    {
        yaw += (Settings._mouseSensitivity/100f) * Input.GetAxis("Mouse X");

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

        Cam.transform.localEulerAngles = Vector3.Lerp(new Vector3(pitch, yaw - (Settings._mouseSensitivity/100f) * Input.GetAxis("Mouse X"), 0f),new Vector3(0f, yaw, 0f),Time.fixedDeltaTime * SmoothCamSpeed);
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

    #endregion

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
        MRD.CmdToggleColliders();
    }

    #region Death Functions

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
            
            MRD.CmdToggleColliders();
            MenuManager.MMInstance.SetScreen("Death");

            StartCoroutine(Respawn());        
        }
    }

    IEnumerator Respawn()
    {
        int fullBlackTime = 2;  //must be less than respawn time
        int increment = 60;     //60 increments per second
        int i = 0;              //starts at zero
        int totalWait = increment * (GameInfo.PlayerRespawnTime - fullBlackTime);   //total increments before finished
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

        //if(newDay)
            //GameServer.GS.GameManagerInstance.NewDay();

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
            this.transform.position = GameServer.GS.GetNearestSpawn(this.transform.position);

        }else
        {
            this.transform.position = Vector3.zero; //To bed bed location to be;
        }

        MenuManager.MMInstance.SetScreen("HUD");
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

    #endregion

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
            CurrentVehicle.MRD.CmdSetOccupied(false);
            CurrentVehicle.ToggleCarried(false);

            r.isKinematic = false;
            r.collisionDetectionMode = CollisionDetectionMode.Continuous;

            MenuManager.MMInstance.fovOffset = 0f;

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

            CurrentVehicle.RadioHUD.InCar = false;
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

    public void ChangeColors(int faceIndex, Color skinColor, Color pantsColor, Color shirtColor, List<byte> customFaceData)
    {
        MRD.CmdChangeColors(faceIndex, skinColor, pantsColor, shirtColor, customFaceData);   
    }
}
