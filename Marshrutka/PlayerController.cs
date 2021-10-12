using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public Transform ToPitch;
    public Transform ToYaw;
    public Camera Cam;
    public float yaw = 0f;
    public float pitch = 0f;
    public BusAnimator BA;

    

    public bool BusStopped = false;

    float camLimits = 85f;

    public Transform Mass;

    Rigidbody r;

    float MaxSpeed = 40f;

    public Transform Door;
    public Transform Stand;

    public Transform[] Seats;

    public List<Passenger> CurrentPassengers = new List<Passenger>();
    public Passenger TalkingTo;

    public bool isUnloading = false;
    public bool Frozen = false;
    public bool NoLook = false;
    public int UnLoadedAmount = 0;

    bool Offroading = false;

    float DefaultFOV = 80f;
    float ZoomFOV = 40f;
    float ZoomAlpha = 0f;

    public int MaxFuel = 500;
    public int Fuel = 500;
    bool UsingFuel = false;
    bool Brake = false;

    public Dial FuelDial;
    public Dial SpeedDial;

    public Phone P;
    public Beeper B;

    public GameObject PromptPrefab;

    public bool Unconscious = false; 

    public Transform WalletLocation;

    float RigidbodyDeathForceThreshold = 4000f;
    float SolidDeathForceThreshold = 5000f;

    public bool Brights = false;
    float LowLight = 15f;
    float LowRange = 50f;
    float BrightLight = 50f;
    float BrightRange = 75f;
    public Light Headlight;

    Manager M;

    //SOUNDS
    public AudioSourceController ASC_Misc;
    public AudioSourceController ASC_Engine;
    public AudioSourceController ASC_Tires;
    public AudioSourceController ASC_OffRoad;
    public AudioSourceController ASC_Wind;
    public AudioSourceController ASC_Road;
    public AudioSourceController ASC_Radio;

    public AudioClip BrightsSound;
    public AudioClip StaticSound;

    public float PitchLow = 0.5f;
    public float PitchHigh = 1f;
    float SkidAmount = 0f;

    ///VEHICLE VARS
    float CurrentSteeringAngle = 0;
    float MaxSteeringAngle = 40;
    public float MotorForce = 300f; //200
    float BrakeForce = 300f;

    public Transform FL_Model;
    public Transform FR_Model;
    public Transform BL_Model;
    public Transform BR_Model;

    public WheelCollider FL_Collider;
    public WheelCollider FR_Collider;
    public WheelCollider BL_Collider;
    public WheelCollider BR_Collider;

    ///RADIO VARS
    public List<Station> Stations;
    bool RadioSwapping = false;
    IEnumerator RadioCoroutine;
    int CurrentStation = 1;
    int DefaultStation = 1;
    List<int> StationSongIndices = new List<int>();
    List<float> StationSongTimes = new List<float>();

    GameObject PromptReference;

    bool RadioTurnedOff = false;


    void Start()
    {
        M = GameObject.FindWithTag("Manager").GetComponent<Manager>();

        InitRadio();

        Cam = ToPitch.GetComponent<Camera>();
        r = GetComponent<Rigidbody>();
        r.centerOfMass = Mass.localPosition;

        MaxFuel = GameSettings.NightDuration;

        FuelDial.Max = MaxFuel;
        FuelDial.Value = Fuel;

        SpeedDial.Max = MaxSpeed;
        SpeedDial.Value = r.velocity.magnitude;

        InvokeRepeating("CheckFuel", 1f, 1f);
    }

    void InitRadio()
    {

        CurrentStation = DefaultStation;

        for(int i = 0; i < Stations.Count; i++)
        {
            StationSongIndices.Add(0);      //init blank
            StationSongTimes.Add(0f);        //init blank  
        }

        for(int i = 0; i < Stations.Count; i++)
        {
            StartCoroutine(StationManager(i));
        }
    }

    IEnumerator StationManager(int Index)
    {
        while(Index != 0)   //if radio station has more than 0 songs
        {
            StationSongIndices[Index] = Random.Range(0, Stations[Index].Songs.Count);
            StationSongTimes[Index] = Time.time;

            yield return new WaitForSeconds(Stations[Index].Songs[StationSongIndices[Index]].length);
            yield return new WaitForSeconds(2f);//time between songs
        }

        yield break;
    }

    public void ClearPassengers()
    {
        for(int i = 0; i < CurrentPassengers.Count; i++)
        {
            Destroy(CurrentPassengers[i].gameObject);
        }
        CurrentPassengers.Clear();
    }

    void CheckFuel()
    {
        if(UsingFuel && Fuel > 0)
            Fuel--;

        FuelDial.Value = Fuel;
    }

    void FixedUpdate()
    {
        if(!M.Paused)
        {
            CarControls();
            UpdateWheelModels();
        }
    }

    void ZoomControls()
    {
        if(Input.GetKey("z"))
        {
            if(ZoomAlpha < 1f)
                ZoomAlpha += 0.1f;
        }
        else
        {
            if(ZoomAlpha > 0f)
                ZoomAlpha -= 0.1f;
        }

        Cam.fieldOfView = Mathf.Lerp(DefaultFOV, ZoomFOV, ZoomAlpha);
    }

    public void SetFrozen(bool State)
    {
        Frozen = State;
        if(Frozen)
        {
            ASC_Engine.Stop();
            SetStation(0, false);
        }else
        {
            ASC_Engine.Play();
            BA.FixGlass();
            if(!RadioTurnedOff)
                SetStation(DefaultStation, false);
        }
    }

    void Update()
    {
        RefreshLoopingAudioLevels();

        if(!M.Paused)
        {
            if(!Frozen)
            {
                CameraControls();
                if(!M.C.Active)
                {
                    ZoomControls();

                    if(!RadioSwapping && CurrentStation != 0)
                    {
                        if(Input.mouseScrollDelta.y > 0f)
                        {
                            ChangeStation(true);
                        }

                        if(Input.mouseScrollDelta.y < 0f)
                        {
                            ChangeStation(false);
                        }
                    }
                }
            }

            if(Input.GetKeyDown("q") && !M.C.Active)
            {
                ASC_Misc.Sound = BrightsSound;
                ASC_Misc.SetVolume(1f);
                ASC_Misc.Play();
                Brights = !Brights;
            }

            if(Brights)
            {
                Headlight.intensity = BrightLight;
                Headlight.range = BrightRange;
                BrightEffect();
            }else
            {
                Headlight.intensity = LowLight;
                Headlight.range = LowRange;
            }

            float ShakeAmount = Offroading ? 0.025f * (r.velocity.magnitude/MaxSpeed) : 0.005f * (r.velocity.magnitude/MaxSpeed);
            Vector3 ShakeVec = new Vector3(Random.Range(-ShakeAmount, ShakeAmount),Random.Range(-ShakeAmount, ShakeAmount),Random.Range(-ShakeAmount, ShakeAmount));
            ToPitch.transform.localPosition = ShakeVec;

            SpeedDial.Value = r.velocity.magnitude;

            float SpeedAlpha = r.velocity.magnitude/MaxSpeed;
            if(SpeedAlpha > 1f)
                SpeedAlpha = 1f;

            ASC_Road.SetVolume(Offroading ? Mathf.Lerp(0f, 1f, SpeedAlpha) : Mathf.Lerp(0f, 0.25f, SpeedAlpha));
            ASC_OffRoad.SetVolume(Offroading ? Mathf.Lerp(0f, 1f, SpeedAlpha) : Mathf.Lerp(0f, 0.5f, SpeedAlpha));
            
            //ASC_Engine.SetVolume(Mathf.Lerp(0.5f, 1f, SpeedAlpha));
            ASC_Engine.SetPitch(Mathf.Lerp(PitchLow, PitchHigh, SpeedAlpha));

            SpeedAlpha -= 0.4f;
            if(SpeedAlpha < 0f)
                SpeedAlpha = 0f;
            ASC_Wind.SetPitch(SpeedAlpha);


            if(!ASC_Radio.AS.isPlaying && CurrentStation != 0 && !RadioSwapping)
                PlaySong();
            

            SkidSounds();
        }
    }

    void RefreshLoopingAudioLevels()
    {
        ASC_Engine.Refresh();
        ASC_Tires.Refresh();
        ASC_OffRoad.Refresh();
        ASC_Wind.Refresh();
        ASC_Road.Refresh();
        ASC_Radio.Refresh();
    }

    void BrightEffect()
    {
        Collider[] Nearby = Physics.OverlapSphere(Headlight.transform.position + (Headlight.transform.forward * (BrightRange/4f)), (BrightRange/4f));
        foreach(Collider Col in Nearby)
        {
            try 
            {   
                Driver AI = Col.transform.gameObject.GetComponent<Driver>();

                if(AI != null)
                {
                    AI.Blinded = true;
                }
            }catch{}
        } 
    }

    public void ToggleRadio()
    {
        if(CurrentStation == 0)
        {
            RadioTurnedOff = false;
            SetStation(DefaultStation, false);
        }
        else
        {
            RadioTurnedOff = true;
            DefaultStation = CurrentStation;
            SetStation(0, false);
        }
    }

    public void SetStation(int Index, bool Static)
    {
        CurrentStation = Index;

        if(RadioSwapping)
            StopCoroutine(RadioCoroutine);

        RadioCoroutine = ChangeChannel(Index, Static);
        StartCoroutine(RadioCoroutine);
    }

    void ChangeStation(bool Forward)
    {
        int Index = CurrentStation;

        if(Forward && (Index + 1) == Stations.Count)
        {
            Index = 1;
        }else if(!Forward && (Index - 1) == 0)
        {
            Index = Stations.Count - 1;
        }else
        {
            Index += Forward ? 1 : -1;
        }

        SetStation(Index, true);
    }

    IEnumerator ChangeChannel(int Index, bool Static)
    {
        RadioSwapping = true;
        
        if(Static)
        {
            ASC_Radio.SetVolume(0.5f);
            ASC_Radio.Sound = StaticSound;
            ASC_Radio.Play();
            yield return new WaitForSeconds(1f);
            ASC_Radio.Stop();
        }

        PlaySong();

        yield return new WaitForSeconds(0.25f);

        RadioSwapping = false;
    }

    void PlaySong()
    {

        if(CurrentStation != 0)
        {
            if(Time.time - StationSongTimes[CurrentStation] < Stations[CurrentStation].Songs[StationSongIndices[CurrentStation]].length)
            {
                ASC_Radio.SetVolume(0.25f);
                ASC_Radio.Sound = Stations[CurrentStation].Songs[StationSongIndices[CurrentStation]];
                ASC_Radio.PlayAtTime(Time.time - StationSongTimes[CurrentStation]);
            }
        }else
        {
            ASC_Radio.Stop();
        }
    }

    void SkidSounds()
    {
        float maxSkid = (MaxSpeed/2f);

        SkidAmount = Mathf.Abs(transform.InverseTransformDirection(r.velocity).x);
        SkidAmount -= (MaxSpeed/8f);        //making it less sensitive

        if(SkidAmount > maxSkid)
            SkidAmount = maxSkid;
        
        float alpha = Mathf.Lerp(0f, 1f, SkidAmount/maxSkid);

        float SpeedAlpha = r.velocity.magnitude/MaxSpeed;
        if(SpeedAlpha > 1f)
            SpeedAlpha = 1f;

        if(Brake)
        {
            ASC_Tires.SetVolume(Mathf.Lerp(0f, 1f, SpeedAlpha));
        }else
        {
            ASC_Tires.SetVolume(alpha);
        }

        
    }


    public void StartUnload(BusStop BS)
    {
        StartCoroutine(Unload(BS));
    }

    IEnumerator Unload(BusStop BS)
    {
        Debug.Log("Now UnLoading");

        int StartCount = 0;

        if(CurrentPassengers.Count > 0)
            StartCount = Random.Range(1, CurrentPassengers.Count);

        for(int i = 0; i < StartCount; i++)
        {
            CurrentPassengers[CurrentPassengers.Count - i - 1].UnLoad(BS, this);
            yield return new WaitForSeconds(1f);
        }

        while(UnLoadedAmount != StartCount)
        {
            yield return null;
        }
        UnLoadedAmount = 0;

        isUnloading = false;
    }

    void CarControls()  
    {
        if(!BusStopped && !Frozen && !M.C.Active)   //if not console active 
        {
            int Turn = 0;

            if(Input.GetKey("d"))
            {
                Turn += 1;
            }

            if(Input.GetKey("a"))
            {
                Turn -= 1;
            }

            CurrentSteeringAngle = MaxSteeringAngle * Turn;
            FR_Collider.steerAngle = CurrentSteeringAngle;
            FL_Collider.steerAngle = CurrentSteeringAngle;

            int Gas = 0;
            Brake = false;

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

            if(Gas != 0)   
                UsingFuel = true;
            else
                UsingFuel = false;

            //ANIMATIONS

            BA.Gas.Alpha = Mathf.Abs(Gas);
            BA.Brake.Alpha = Brake ? 1f : 0f;
            BA.Steering.Alpha = ((Turn + 1f)/2f);
            BA.Gear.Alpha = Gas >= 0f ? 0f : 1f;
            BA.Lights.Alpha = Brights ? 1f : 0f;



            //ACTIONS

            if(!Brake)
            {
                if(Fuel > 0)
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
            
        }else
        {
            UsingFuel = false;

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

    void CameraControls()  
    {
        float mouseSens = M.C.Active ? 0f : Settings._mouseSensitivity/100f;

        if(!NoLook)
        {
            yaw += mouseSens * Input.GetAxis("Mouse X");

            if(!Settings._invertedLook)
                pitch -= mouseSens * Input.GetAxis("Mouse Y");
            else
                pitch += mouseSens * Input.GetAxis("Mouse Y");
        }

        ToYaw.transform.localEulerAngles = new Vector3(0f, yaw, 0f);
            
        if(pitch >= camLimits)
            pitch = camLimits;
                
        if(pitch <= -camLimits)
            pitch = -camLimits;

        float y = ToPitch.transform.localEulerAngles.y;
        float z = ToPitch.transform.localEulerAngles.z;

        ToPitch.transform.localEulerAngles = new Vector3(pitch, 0f, 0f);
    }

    void OnCollisionEnter(Collision Col)
    {
        //Debug.Log(Col.impulse.magnitude);

        if(BusStopped)
            return;
        
        float DeathForce = 0f;

        if(Col.gameObject.GetComponent<Rigidbody>() != null)
            DeathForce = RigidbodyDeathForceThreshold;
        else
            DeathForce = SolidDeathForceThreshold;

        if(Col.impulse.magnitude > DeathForce)
        {
            if(!Frozen && !Unconscious)
            {
                Unconscious = true;
                Invoke("Passout", 0.1f);
            }
        }

        if(Col.impulse.magnitude > DeathForce - 1000f)
        {
            BA.BreakGlass();
        }
    }

    void Passout()
    {
        M.MUI.ShowResultsMenu();
        BA.BreakGlass();
    }

    public void TogglePrompt()
    {
        Debug.Log("Called");

        if(PromptReference == null)
        {
            PromptReference = Instantiate(PromptPrefab, Vector3.zero, Quaternion.identity);

            PromptReference.transform.SetParent(GameObject.FindWithTag("Camera").transform);
            PromptReference.transform.localPosition = new Vector3(0f, 0f, 0.2f);   //OFFSET
            PromptReference.transform.localRotation = Quaternion.Euler(0f, 0f, 0f);

            PromptReference.GetComponent<Prompt>().P = TalkingTo;
        }else
        {
            Destroy(PromptReference);
        }
    }

    void OnTriggerEnter(Collider Col)
    {
        if(Col.gameObject.CompareTag("Land"))
            Offroading = true;
    }

    void OnTriggerExit(Collider Col)
    {
        if(Col.gameObject.CompareTag("Land"))
            Offroading = false;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.magenta;
        Gizmos.DrawSphere(Headlight.transform.position + (Headlight.transform.forward * (BrightRange/4f)), (BrightRange/4f));
        
    }
    
}
