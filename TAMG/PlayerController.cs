using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PlayerController : MonoBehaviour
{
    /*
    [System.Serializable]
    public struct Equippable
    {
        public string Name;
        public GameObject Prefab;
        public Texture2D Crosshair;
        //public int HoldAnimationIndex;
    }

    public float Maxspeed = 4f;
    public float SmoothCamSpeed = 10f;
    public float SprintMultiplier = 1.5f;
    public float DeathForceThreshold = 100f;

    public float FreeCamSpeed = 5f;

    float yaw = 0f;
    float pitch = 0f;
    float camLimits = 85f;
    float JumpForce = 2500f;
    float CurrentSprint = 1f;
    float SittingcamLimits = 60f;
    //float MovementForce = 10000f;
    float MovementForce = 300f;
    float speedAlpha = 0f;

    bool CollidersOn = true;
    bool RagdollEnabled = false;
    bool RespawnInBed = false;

    public GameObject ToPitch;
    public GameObject CamEmpty;
    public GameObject RagdollCamEmpty;

    public Transform CamOverride = null;
    
    public Animator An;
    public BodyPositions Model;
    public BodyPositions Ragdoll;

    public Animator OnlineAn;
    
    Rigidbody r;
    PickupAble CurrentPicked;
    GameObject Cam;
    MenuManager MM;
    GameManager GM;
    VolumeController VC;
    public VehicleController CurrentVehicle;

    Collider[] AllColliders;

    public PlayerGhost PG;

    AudioClip BufferClip;
    float[] VoiceBuffer;
    bool VoiceInitialized = false;
    float[] FloatBuffer = new float[2048];  //2048 for single channel speaker, 1024 for stereo channel

    public bool ConsoleOpen = false;

    public List<Equippable> EQs;

    void Start()
    {
        Cam = GameObject.FindWithTag("Camera");
        MM = Cam.GetComponent<MenuManager>();
        GM = GameObject.FindWithTag("Manager").GetComponent<GameManager>();
        VC = GameObject.FindWithTag("Volume").GetComponent<VolumeController>();
        r = GetComponent<Rigidbody>();

        pitch = ToPitch.transform.localEulerAngles.x;
        AllColliders = this.transform.GetComponentsInChildren<Collider>();
    }
    
    void Update()
    {
        if(MM.CurrentScreen == "HUD" && !ConsoleOpen)
        {
            //CameraControls();
            if(Input.GetKeyDown(KeyCode.Tab))
            {
                MM.SetScreen("Map");
            }

            if(Input.GetKeyDown(KeyCode.Space))
                if(isGrounded())
                    r.AddForce(Vector3.up * JumpForce);

            
            An.SetFloat("Arms_Direction", pitch);
            if(OnlineAn != null)
                OnlineAn.SetFloat("Arms_Direction", pitch);

            if(Input.GetKeyDown(KeyCode.Escape))
                MM.SetScreen("Pause");

            if(Input.GetKeyDown("k"))
                Die(false, "Suicide :(");

            if(Input.GetKeyDown("o"))
                MM.SetScreen("Freecam");

            //if(Input.GetKeyDown("v"))
                //ClearVoiceBuffer();

            if(Input.GetKey("v"))
                TransmitVoice();
                
        }else if(!ConsoleOpen)
        {
            if(Input.GetKeyDown(KeyCode.Escape) && MM.CurrentScreen == "Pause")
                MM.SetScreen("HUD");

            if(Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.Tab))
            {
                if(MM.CurrentScreen == "Map")
                {
                    MM.SetScreen("HUD");
                }
            }

            if((Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown("o")) && MM.CurrentScreen == "Freecam")
                MM.SetScreen("HUD");
        }

    }

    void FixedUpdate()
	{   
        if(!RagdollEnabled && MM.CurrentScreen == "HUD" && !ConsoleOpen)
        {
            Movement();
            CameraControls();

            //yaw += (Settings._mouseSensitivity/100f) * Input.GetAxis("Mouse X");
            //this.transform.localEulerAngles = new Vector3(0f, yaw, 0f);
            //this.transform.localEulerAngles = Vector3.Lerp(new Vector3(0f, yaw - (Settings._mouseSensitivity/100f) * Input.GetAxis("Mouse X"), 0f),new Vector3(0f, yaw, 0f),Time.fixedDeltaTime * SmoothCamSpeed);
            //Turn();
        }

        if(MM.CurrentScreen == "Freecam" && !ConsoleOpen)
        {
            FreeCameraMovement();
            FreeCameraControls();
        }

        Vector3 CurrentVelocity = new Vector3(r.velocity.x, 0f, r.velocity.z);

        speedAlpha = Smooth((CurrentVelocity.magnitude)/(Maxspeed * SprintMultiplier));

        An.SetFloat("Move_Speed", speedAlpha);
        if(OnlineAn != null)
            OnlineAn.SetFloat("Move_Speed", speedAlpha);

        
        if(!GameInfo.Paused && !RagdollEnabled)
        {
            Quaternion deltaRotation = Quaternion.Euler(0f, (Settings._mouseSensitivity) * Input.GetAxis("Mouse X") * Time.deltaTime, 0f);
            r.MoveRotation(r.rotation * deltaRotation);
            //this.transform.localEulerAngles += new Vector3(0f, (Settings._mouseSensitivity/100f) * Input.GetAxis("Mouse X"), 0f);
        }



            //CameraControls();
	}
    
    
    void Turn()
    {
        //Quaternion deltaRotation = Quaternion.Euler(0f, (Settings._mouseSensitivity) * Input.GetAxis("Mouse X") * Time.fixedDeltaTime, 0f);
        //r.MoveRotation(r.rotation * deltaRotation);
        

        //Quaternion deltaRotation = Quaternion.Euler(m_EulerAngleVelocity * Time.deltaTime);
        //m_Rigidbody.MoveRotation(m_Rigidbody.rotation * deltaRotation);
        
        yaw += (Settings._mouseSensitivity/100f) * Input.GetAxis("Mouse X");

        if(Input.GetAxis("Mouse X") != 0)
            r.AddTorque((transform.up * (Settings._mouseSensitivity/100f) * Input.GetAxis("Mouse X")), ForceMode.Impulse);
        else
            r.angularVelocity = Vector3.zero;
        
    }

    void ClearVoiceBuffer()
    {
        //VoiceBuffer = new float[44100];
        //BufferClip = Microphone.Start(Microphone.devices[0], false, 1, 44100);
    }

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
            
            PG.CmdSetVoiceBuffer(VoiceBuffer);
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

        PG.CmdInitVoice(pitch);

        VoiceInitialized = true;
    }

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
        
        if(Input.GetKeyDown("r"))
        {
            string date = System.DateTime.Now.ToString();
            date = date.Replace("/","-");
            date = date.Replace(" ","_");
            date = date.Replace(":","-");
            ScreenCapture.CaptureScreenshot(Application.persistentDataPath + "/screenshots/" + date + ".png", 2);
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

    void LateUpdate()   //lateupdate
    {
        if(MM.CurrentScreen != "Freecam")
        {
            if(!RagdollEnabled)
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

                
                //Cam.transform.position = Vector3.Lerp(Cam.transform.position,CamEmpty.transform.position,Time.fixedDeltaTime * SmoothCamSpeed);
                
                //Cam.transform.rotation = Quaternion.Slerp(Cam.transform.rotation, CamEmpty.transform.rotation, Time.fixedDeltaTime * SmoothCamSpeed);
            }
            else
            {
                Cam.transform.position = RagdollCamEmpty.transform.position;
                Cam.transform.rotation = Quaternion.Slerp(Cam.transform.rotation, RagdollCamEmpty.transform.rotation, Time.fixedDeltaTime * SmoothCamSpeed);
            }
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
                //r.velocity += (MoveDirection * CurrentSprint * MovementForce) * Time.fixedDeltaTime;
                r.AddForce(MoveDirection * CurrentSprint * MovementForce);  //USE THIS ONE
            }

            //r.AddForce(MoveDirection * CurrentSprint * (MovementForce/2f));

            //r.velocity = MoveDirection * CurrentSprint * 10f;

            //this.transform.position = Vector3.Lerp(StartingPos, this.transform.position, Time.deltaTime * SmoothCamSpeed);

            //r.AddForce(MoveDirection * CurrentSprint * MovementForce * Time.fixedDeltaTime * 100);

            //r.MovePosition(transform.position + MoveDirection * Time.fixedDeltaTime * 5f);

            //r.velocity = new Vector3(MoveDirection.x * Time.fixedDeltaTime * MovementForce, r.velocity.y, MoveDirection.z * Time.fixedDeltaTime * MovementForce);

            //r.velocity = MoveDirection * CurrentSprint * Maxspeed;
            
            //r.velocity = MoveDirection * CurrentSprint * Maxspeed;
            //r.AddForce(MoveDirection * CurrentSprint * MovementForce * Time.fixedDeltaTime);
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

    void CameraControls()  
    {
        if(CurrentVehicle != null)
        {
            An.SetFloat("Move_Speed", 0);
            if(OnlineAn != null)
                OnlineAn.SetFloat("Move_Speed", 0);

            this.transform.localEulerAngles = new Vector3(0f, 0f, 0f);
            float SittingLookLimits = 80f;

            //Debug.Log((Settings._mouseSensitivity/100f));
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

            //Cam.transform.rotation = Quaternion.Slerp(Cam.transform.rotation, CamEmpty.transform.rotation, Time.fixedDeltaTime * SmoothCamSpeed);
            //Cam.transform.rotation = CamEmpty.transform.rotation;

        }else
        {

            yaw += (Settings._mouseSensitivity/100f) * Input.GetAxis("Mouse X");
            //this.transform.localEulerAngles = new Vector3(0f, yaw, 0f);
            this.transform.localEulerAngles = Vector3.Lerp(new Vector3(0f, yaw - (Settings._mouseSensitivity/100f) * Input.GetAxis("Mouse X"), 0f),new Vector3(0f, yaw, 0f),Time.fixedDeltaTime * SmoothCamSpeed);

            //r.AddTorque((transform.up * (Settings._mouseSensitivity/100f) * Input.GetAxis("Mouse X")));

            //r.AddTorque(transform.up * yaw);
            //Quaternion deltaRotation = Quaternion.Euler(0f, (Settings._mouseSensitivity) * Input.GetAxis("Mouse X") * Time.fixedDeltaTime, 0f);
            //r.MoveRotation(r.rotation * deltaRotation);

            //this.transform.localEulerAngles += new Vector3(0f, (Settings._mouseSensitivity/100f) * Input.GetAxis("Mouse X"), 0f);

            if(Settings._invertedLook)
            {
                pitch += (Settings._mouseSensitivity/100f) * Input.GetAxis("Mouse Y");
                //pitch = (Settings._mouseSensitivity/100f) * Input.GetAxis("Mouse Y");
            }else
            {
                pitch -= (Settings._mouseSensitivity/100f) * Input.GetAxis("Mouse Y");
                //pitch = -(Settings._mouseSensitivity/100f) * Input.GetAxis("Mouse Y");
            }
            
            if(pitch >= camLimits)
                pitch = camLimits;
                
            if(pitch <= -camLimits)
                pitch = -camLimits;
            
            
            if(ToPitch.transform.localEulerAngles.x >= camLimits)
                pitch = 0f;
                
            if( ToPitch.transform.localEulerAngles.x <= -camLimits)
                pitch = 0f;
            
            float y = ToPitch.transform.localEulerAngles.y;
            float z = ToPitch.transform.localEulerAngles.z;

            ToPitch.transform.localEulerAngles = new Vector3(pitch, 0f, 0f);
            //ToPitch.transform.localEulerAngles += new Vector3(pitch, 0f, 0f);

            //Cam.transform.rotation = Quaternion.Slerp(Cam.transform.rotation, CamEmpty.transform.rotation, Time.fixedDeltaTime * SmoothCamSpeed);
            //Cam.transform.rotation = CamEmpty.transform.rotation;
        }
        
    }

    void SetLook(float y, float p)
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

    void OnCollisionEnter(Collision col)
    {
        //Debug.Log(col.impulse.magnitude);

        if(col.impulse.magnitude > DeathForceThreshold || col.gameObject.CompareTag("Hazard"))
        {
            if(col.gameObject.CompareTag("Hazard"))
                Die(false, "Hit a Hazard");
            else
                Die(false, "Hit something too hard");
        }
    }

    void OnTriggerEnter(Collider col)
    {
        if(col.gameObject.CompareTag("Hazard"))
        {
            Die(false, "Went out of bounds");
        }
    }

    public void ToggleColliders()       //used to transition in and out of vehicles
    {
        ClearHands();
        CollidersOn = !CollidersOn;

        AllColliders[0].enabled = CollidersOn;
    }

    void SyncRagdoll()
    {
        for(int i = 0; i < Ragdoll.Syncs.Count; i++)
        {
            Ragdoll.Syncs[i].position = Model.Syncs[i].position;
            Ragdoll.Syncs[i].rotation = Model.Syncs[i].rotation;
            if(Ragdoll.Syncs[i].GetComponent<Rigidbody>() != null)
            {
                Ragdoll.Syncs[i].GetComponent<Rigidbody>().velocity = r.velocity;
                Ragdoll.Syncs[i].GetComponent<Rigidbody>().angularVelocity = r.angularVelocity;
            }
        }
    }

    public void Passout()
    {
        RespawnInBed = true;
        Die(true, "Fell Asleep");
    }

    public void Die(bool newDay, string message)
    {
        if(!RagdollEnabled)
        {
            Cam.GetComponent<Interact>().Clear();
            Debug.Log(message);
            LeaveVehicle(true);
            ClearHands();
            RagdollEnabled = true;

            Model.gameObject.SetActive(false);

            SyncRagdoll();

            PG.CmdDie(r.velocity, r.angularVelocity);

            r.collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;
            r.isKinematic = true;
            
            ToggleColliders();
            Ragdoll.gameObject.SetActive(true);
            MM.SetScreen("Death");

            StartCoroutine(Respawn(newDay));        
        }
    }

    public void ObjectPicked(PickupAble P)
    {
        if(!RagdollEnabled && CurrentVehicle == null)    //can't pickup if dead or in a vehicle
        {
            if(CurrentPicked == P)      //if picked up is same as in hands
            {
                ClearHands();           //drop it

            }else if(CurrentPicked == null)     //if hands empty
            {
                P.GetComponent<SwappableAuthority>().AskForAuthority();
                CurrentPicked = P;              //fill hands
                CurrentPicked.Pickup();
                An.SetFloat("Arms_Direction", pitch);
                An.SetBool("Arms_Active", true);

                if(OnlineAn != null)
                {
                    OnlineAn.SetFloat("Arms_Direction", pitch);
                    OnlineAn.SetBool("Arms_Active", true);
                }
            }else                               //if different object is picked up when hands full
            {
                CurrentPicked.Drop();           //drop old 
                CurrentPicked = P;              //assign new
                CurrentPicked.Pickup();
                An.SetFloat("Arms_Direction", pitch);
                An.SetBool("Arms_Active", true);

                if(OnlineAn != null)
                {
                    OnlineAn.SetFloat("Arms_Direction", pitch);
                    OnlineAn.SetBool("Arms_Active", true);
                }
            }
        }
    }

    public void VehicleToggle(VehicleController VC)
    {
        if(!RagdollEnabled)
        {
            if(CurrentVehicle == VC)    //if the player is already in the vehicle
            {
                LeaveVehicle(false);

            }else if(CurrentVehicle == null)    //if the player isn't in a vehicle
            {
                VC.GetComponent<SwappableAuthority>().AskForAuthority();
                CurrentVehicle = VC;
                VC.inUse = true;
                VC.ToggleCarried();

                MM.fovOffset = 0f;

                r.collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;
                r.isKinematic = true;

                ToggleColliders();
                VC.ExitLocation.position = this.transform.position;
                VC.ExitLocation.rotation = this.transform.rotation;
                VC.SavedLook = new Vector2(VC.ExitLocation.localEulerAngles.y, pitch);
                //VC.ExitLocation.rotation =  this.transform.rotation;
                this.transform.position = VC.Seat.position;
                
                An.SetInteger("Sitting", VC.SittingAnimationIndex);
                if(OnlineAn != null)
                    OnlineAn.SetInteger("Sitting", VC.SittingAnimationIndex);
                this.transform.SetParent(VC.transform);
                this.transform.localEulerAngles = Vector3.zero;
                SetLook(0f, 10f);
            }
        }
    }

    public void LeaveVehicle(bool dontMove)
    {
        if(CurrentVehicle != null)
        {
            CurrentVehicle.inUse = false;
            CurrentVehicle.ToggleCarried();

            r.isKinematic = false;
            r.collisionDetectionMode = CollisionDetectionMode.Continuous;

            MM.fovOffset = 0f;

            Vector3 CarVelocity = CurrentVehicle.GetComponent<Rigidbody>().velocity;
            r.velocity = CarVelocity;

            ToggleColliders();

            An.SetInteger("Sitting", 0);
            if(OnlineAn != null)
                OnlineAn.SetInteger("Sitting", 0);
            this.transform.SetParent(null);

            if(!dontMove)
            {
                this.transform.position = CurrentVehicle.ExitLocation.position;
                this.transform.localEulerAngles = new Vector3(0f, CurrentVehicle.ExitLocation.eulerAngles.y, 0f);
                SetLook(CurrentVehicle.ExitLocation.eulerAngles.y, CurrentVehicle.SavedLook.y);
                //this.transform.rotation = CurrentVehicle.ExitLocation.rotation;
            }
            //this.transform.position = CurrentVehicle.transform.position + CurrentVehicle.ExitLocation.localPosition;

            CurrentVehicle = null;

            if(CarVelocity.magnitude > 10f)     //kill if exit vehicle when going to quick
                StartCoroutine(TimedDeath(0.1f, false, "Left a moving vehicle"));
        }
    }
    
    IEnumerator TimedDeath(float t, bool newDay, string message)
    {
        yield return new WaitForSeconds(t);
        Die(newDay, message);
    }

    public void ClearHands()
    {
        if(CurrentPicked != null)
            CurrentPicked.Drop();
        CurrentPicked = null;
        An.SetBool("Arms_Active", false);
        if(OnlineAn != null)
            OnlineAn.SetBool("Arms_Active", false);
    }

    IEnumerator Respawn(bool newDay)
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

        if(newDay)
            GameServer.GS.GameManagerInstance.NewDay();

        VC.DeathFade(1f);
        yield return new WaitForSeconds(fullBlackTime);


        Model.gameObject.SetActive(true);
        r.isKinematic = false;
        r.collisionDetectionMode = CollisionDetectionMode.Continuous;
        
        Ragdoll.gameObject.SetActive(false);
        RagdollEnabled = false;
        ToggleColliders();
        if(!RespawnInBed)
        {
            this.transform.position = GameServer.GS.GetNearestSpawn(this.transform.position);

        }else
        {
            this.transform.position = Vector3.zero; //To bed bed location to be;
        }

        MM.SetScreen("HUD");
        VC.DeathFade(0f);

        RespawnInBed = false;

        Invoke("GhostAppear", 1f);
    }

    void GhostAppear()
    {
        PG.CmdRespawn();
    }

    public bool isGrounded()
    {
        RaycastHit hit;
        if(Physics.Raycast(this.transform.position + new Vector3(0f, 0.05f, 0f), -Vector3.up, out hit, 0.2f))
            return true;
        else
            return false;
    }
    */
}