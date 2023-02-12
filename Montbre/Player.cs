using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.Rendering;
using UnityEngine.VFX;

public class Player : MonoBehaviour
{
    public static HUD H;
    public HUD Href;

    public static Unit Controlled;

    public Ragdoll Body;

    Camera Cam;

    public Transform ConquestOverview;
    public Transform KothOverview;
    public Transform DefenseOverview;
    public Transform SpectatorTransform;

    public bool Spectating = false;
    float FOValpha = 1f;
    float Timealpha = 1f;
    float FixedTime = 0.02f;
    float VFXTime = 0.02f;

    public float CurrentFOV = 70f;

    ChromaticAberration Distort;
    LensDistortion Bend;
    ColorAdjustments Tinter;
    Vignette Supression;

    float CurrentSupression = 0f;
    float CurrentMuffle = 22000f;
    float LastMuffled = -100f;

    AudioLowPassFilter Muffle;
    AudioReverbFilter Reverb;

    Vector3 CallInAt;
    Unit ToCommand;

    float NormalSaturation = 10f; //used to be 10f

    void Awake()
    {
        H = Href;
        FixedTime = Time.fixedDeltaTime;
        VFXTime = VFXManager.fixedTimeStep;
    }

    void Start()
    {
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = 60;

        Cam = GetComponent<Camera>();
        Muffle = GetComponent<AudioLowPassFilter>();
        Reverb = GetComponent<AudioReverbFilter>();

        Volume V = GameObject.FindWithTag("Volume").GetComponent<Volume>();
        if(V.profile.TryGet<ChromaticAberration>(out ChromaticAberration tmpDistort) )
            Distort = tmpDistort;

        if(V.profile.TryGet<LensDistortion>(out LensDistortion tmpBend) )
            Bend = tmpBend;

        if(V.profile.TryGet<ColorAdjustments>(out ColorAdjustments tmpTinter) )
            Tinter = tmpTinter;

        if(V.profile.TryGet<Vignette>(out Vignette tmpSupression) )
            Supression = tmpSupression;
        
        StartCoroutine(WaitForGame());
        
    }

    IEnumerator WaitForGame()
    {
        while(!Game.Started)
            yield return null;

        MenuManager.MM.SetScreen("Loading");

        //FindNewBody(false, false); 
        
        //while(!Controlled)
            //yield return null;
            
        while(!Game.Started)
            yield return null;
            
        //MenuManager.MM.SetScreen("Loading");

        StartCoroutine(ReturnToOverview(true));
    }
    
    public void Leave()//instant
    {
        StartCoroutine(ReturnToOverview(true));
    }
    
    public void Died()//wait for death animation thing
    {
        Controlled = null;
        StartCoroutine(ReturnToOverview(false));
    }
    
    IEnumerator ReturnToOverview(bool Quick)
    {
        if(!Quick)
            yield return new WaitForSeconds(2f);
        
        float startTime = 0f;
        float Tspeed = 0.4f;
      
        startTime = Time.time;
        while(Time.time < startTime + Tspeed)
        {   
            Bend.intensity.Override(Mathf.Lerp(0f, -0.4f, (Time.time - startTime)/Tspeed));
            Distort.intensity.Override(Mathf.Lerp(0f, 1f, (Time.time - startTime)/Tspeed));
            yield return null;
        } 

        Bend.intensity.Override(-0.4f);
        Distort.intensity.Override(1f);
        
        if(Controlled != null)
        {
            Controlled.Controller = null;
            if(Controlled.Type == Unit.Types.Infantry)
            {
                //Controlled.inf.SetState(Infantry.States.Idle);
                Controlled.inf.ChangeBehavior();
                for (int i = 0; i < Controlled.inf.HatLocation.childCount; i++)
                    Controlled.inf.HatLocation.GetChild(i).GetChild(0).gameObject.layer = 0;
            }
            Controlled = null;
        }
        if(Body != null)
        {
            for (int i = 0; i < Body.HatLocation.childCount; i++)
                Body.HatLocation.GetChild(i).GetChild(0).gameObject.layer = 0;
            Body = null;
        }
        
        switch(Game.GameMode)
        {
            case GameModes.Conquest : this.transform.SetParent(ConquestOverview); break;
            case GameModes.Defense : this.transform.SetParent(DefenseOverview); break;
            case GameModes.Hill : this.transform.SetParent(KothOverview); break;
        }
        this.transform.localPosition = Vector3.zero;
        this.transform.localEulerAngles = Vector3.zero;
        
        MenuManager.MM.SetScreen("Overview");

        ClearEffects();
        CurrentFOV = Game.BaseFOV;
        Cam.fieldOfView = CurrentFOV;
        
        Logic.L.Show(Game.GameMode);
        if(Game.GameMode == GameModes.Defense)
            Logic.L.Defense.HideRing();
        if(Game.GameMode == GameModes.Hill)
            Logic.L.Hill.HideRing();

        startTime = Time.time;
        while(Time.time < startTime + Tspeed)
        {   
            Bend.intensity.Override(Mathf.Lerp(-0.4f, 0f, (Time.time - startTime)/Tspeed));
            Distort.intensity.Override(Mathf.Lerp(1f, 0f, (Time.time - startTime)/Tspeed));
            yield return null;

            Bend.intensity.Override(0f);
            Distort.intensity.Override(0f);
        }
    }

    public void Supress(float passed)
    {
        if(Controlled)
        {
            CurrentSupression = Mathf.Max(CurrentSupression, Mathf.Lerp(0.4f, 0.1f, passed));
            float newmuffled = Mathf.Lerp(0f, 14000f, passed);
            if(newmuffled < CurrentMuffle)
            {
                LastMuffled = Time.time;
                CurrentMuffle = newmuffled;
            }
        }
    }

    void ClearEffects()
    {  
        CurrentSupression = 0f;
        CurrentMuffle = 22000f;

        Muffle.cutoffFrequency = CurrentMuffle;
        LastMuffled = -100f;

        Tinter.saturation.Override(NormalSaturation);
        Supression.intensity.Override(0f);
    }

    void Update()
    {
        if(Game.Started)
        {
            if(Controlled)
                Reverb.enabled = Controlled.Type == Unit.Types.Tank;

            if (Input.GetKeyDown(KeyCode.Tab) || (Spectating && Input.GetKeyDown(KeyCode.Escape)))
            {
                if(!Spectating)
                {
                    Spectating = true;
                    Logic.L.HideAll();
                    if(Controlled != null)
                    {
                        Controlled.Controller = null;
                        if(Controlled.Type == Unit.Types.Infantry)
                        {
                            //Controlled.inf.SetState(Infantry.States.Idle);
                            Controlled.inf.ChangeBehavior();
                            for (int i = 0; i < Controlled.inf.HatLocation.childCount; i++)
                                Controlled.inf.HatLocation.GetChild(i).GetChild(0).gameObject.layer = 0;
                        }
                        Controlled = null;
                    }
                    if(Body != null)
                    {
                        for (int i = 0; i < Body.HatLocation.childCount; i++)
                            Body.HatLocation.GetChild(i).GetChild(0).gameObject.layer = 0;
                        Body = null;
                    }
                    
                    MenuManager.MM.SetScreen("Blank");
                    SpectatorTransform.position = this.transform.position;
                    SpectatorTransform.rotation = this.transform.rotation;
                    this.transform.SetParent(SpectatorTransform);
                    this.transform.localPosition = Vector3.zero;
                    this.transform.localEulerAngles = Vector3.zero;
                    return;
                }else
                {
                    Spectating = false;
                    Time.timeScale = 1f;
                    Time.fixedDeltaTime = FixedTime;
                    VFXManager.fixedTimeStep = VFXTime;
                    FOValpha = 1f;
                    Leave();
                    return;
                }
                
            }

            if(Spectating)
            { 
                Vector3 MoveDirection = Vector3.zero;
                if (Input.GetKey("w"))
                    MoveDirection += SpectatorTransform.transform.forward;
                if (Input.GetKey("a")) 
                    MoveDirection += -SpectatorTransform.transform.right;
                if (Input.GetKey("s")) 
                    MoveDirection += -SpectatorTransform.transform.forward;
                if (Input.GetKey("d")) 
                    MoveDirection += SpectatorTransform.transform.right;
                if (Input.GetKey(KeyCode.LeftControl)) 
                    MoveDirection += -Vector3.up;
                if (Input.GetKey(KeyCode.Space)) 
                    MoveDirection += Vector3.up;
                float SpeedMult = 2f;
                if(Input.GetKey(KeyCode.LeftShift))
                {
                    SpectatorTransform.transform.Translate(MoveDirection * 0.5f * SpeedMult * FOValpha, Space.World);
                }else
                {
                    SpectatorTransform.transform.Translate(MoveDirection * 0.5f * FOValpha, Space.World);
                }

                float yaw = SpectatorTransform.transform.localEulerAngles.y;
                if (!Input.GetKey("f"))
                    yaw += (Settings._mouseSensitivity/100f) * Input.GetAxis("Mouse X") * FOValpha;
                float pitch = SpectatorTransform.transform.localEulerAngles.x;
                float roll = SpectatorTransform.transform.localEulerAngles.z;
                if (Input.GetKeyDown("q"))
                    roll += 15f;
                if (Input.GetKeyDown("e"))
                    roll += -15f;

                if (!Input.GetKey("r"))
                {
                    if (Settings._invertedLook)
                    {
                        pitch += (Settings._mouseSensitivity / 100f) * Input.GetAxis("Mouse Y") * FOValpha;
                    }
                    else
                    {
                        pitch -= (Settings._mouseSensitivity / 100f) * Input.GetAxis("Mouse Y") * FOValpha;
                    }
                }

                SpectatorTransform.transform.localEulerAngles = new Vector3(pitch, yaw, roll);
                
                if(Input.GetMouseButton(1))
                {
                    if(Timealpha == 1f)
                    {
                        if(Input.mouseScrollDelta.y < 0f)
                            Timealpha = 0.05f;
                    }else if(Timealpha == 0.05f)
                    {
                        if(Input.mouseScrollDelta.y > 0f)
                            Timealpha = 1f;
                        if(Input.mouseScrollDelta.y < 0f)
                            Timealpha = 0.01f;
                    }else if(Timealpha == 0.01f)
                    {
                        if(Input.mouseScrollDelta.y > 0f)
                            Timealpha = 0.05f;
                        if(Input.mouseScrollDelta.y < 0f)
                            Timealpha = 0f;
                    }else if(Timealpha == 0f)
                    {
                        if(Input.mouseScrollDelta.y > 0f)
                            Timealpha = 0.01f;
                    }

                    
                    Time.timeScale = Timealpha;
                    Time.fixedDeltaTime = FixedTime * Timealpha;
                    VFXManager.fixedTimeStep = VFXTime * Timealpha;
                }else
                {
                    if(Input.mouseScrollDelta.y > 0f)
                        FOValpha -= 0.05f;
                    if(Input.mouseScrollDelta.y < 0f)
                        FOValpha += 0.05f;
                        
                    FOValpha = Mathf.Clamp(FOValpha, 0.1f, 1f);

                }

                ClearEffects();
                CurrentFOV = Mathf.Lerp(1f, 70f, FOValpha);
                Cam.fieldOfView = CurrentFOV;

                return;
            }
        
            WheelControls();
        
            if(Controlled)                     //PLAYER IS CONTROLLING
            {
                if(Input.GetKeyDown(KeyCode.Escape))
                {
                    if (MenuManager.MM.CurrentScreen == "HUD" || MenuManager.MM.CurrentScreen == "Overview")
                        MenuManager.MM.SetScreen("Pause");
                    else
                        Resume();
                }
                
                if(Input.GetMouseButton(0))
                    Controlled.Fire();
                
                if(Input.GetKeyDown("k"))
                    Controlled.Kill();
                    
                CurrentSupression = Mathf.Lerp(CurrentSupression, 0f, Time.deltaTime * 0.15f);
                if(Time.time > LastMuffled + 3f)
                    CurrentMuffle = Mathf.Lerp(CurrentMuffle, 22000f, Time.deltaTime * 0.5f);
                Muffle.cutoffFrequency = CurrentMuffle;
                Supression.intensity.Override(CurrentSupression);

                try
                {
                    if (Controlled.Type == Unit.Types.Infantry)
                        Tinter.saturation.Override(Controlled.inf.Dead ? -100f : Mathf.Lerp(-100f, NormalSaturation, Controlled.inf.HealthAlpha));
                }catch
                {
                    Tinter.saturation.Override(-100f);
                }
            }else                          
            {
                if (MenuManager.MM.CurrentScreen == "HUD")   //PLAYER IS DEAD
                {
                    Muffle.cutoffFrequency = 1000f;
                    Supression.intensity.Override(0.4f);
                    Tinter.saturation.Override(-100f);
                }else if(MenuManager.MM.CurrentScreen == "Overview")  //PLAYER IS ON OVERVIEW
                {
                    if(Input.GetKeyDown(KeyCode.Escape))
                        MenuManager.MM.SetScreen("Pause");
                    
                    ClearEffects();
                    CurrentFOV = Game.BaseFOV;
                    Cam.fieldOfView = CurrentFOV;
                }else
                {
                    if(Input.GetKeyDown(KeyCode.Escape))
                        Resume();
                }
            }
            
            
        }  
    }
    
    public void Resume()
    {
        if(Spectating)
            MenuManager.MM.SetScreen("Blank");
        else if(Controlled)
            MenuManager.MM.SetScreen("HUD");
        else
            MenuManager.MM.SetScreen("Overview");
    }

    void WheelControls()
    {
        //CONTROL MENU
        if(Input.GetKey("q") && Controlled)
        {
            if(Controlled.Type == Unit.Types.Infantry)
            {
                if(!ToCommand)
                {
                    RaycastHit hit;
                    if(Physics.Raycast(this.transform.position, this.transform.forward, out hit, Game.InteractRange,  LayerMask.GetMask("Select")))
                    {
                        if(hit.transform.root.gameObject.GetComponent<Unit>())
                            if(hit.transform.root.gameObject.GetComponent<Unit>() != Controlled)
                                if(hit.transform.root.gameObject.GetComponent<Unit>().Targetable)
                                    if(hit.transform.root.gameObject.GetComponent<Unit>().Controllable)
                                        if(hit.transform.root.gameObject.GetComponent<Unit>().Team == Game.TeamOne)
                                            ToCommand = hit.transform.root.gameObject.GetComponent<Unit>();
                    }else
                    {
                        ToCommand = null;
                    }
                }

                if(ToCommand)
                {
                    if(ToCommand.Type == Unit.Types.Infantry)
                        H.Wheel.SetMenu(1);
                    if(ToCommand.Type == Unit.Types.Plane)
                        H.Wheel.SetMenu(2);
                    if(ToCommand.Type == Unit.Types.Tank)
                        H.Wheel.SetMenu(5);
                    
                    H.Wheel.Show = true;
                }
            }else if(Controlled.Type == Unit.Types.Plane)
            {
                H.Wheel.SetMenu(3);
                H.Wheel.Show = true;
            }else if(Controlled.Type == Unit.Types.Tank)
            {
                H.Wheel.SetMenu(4);
                H.Wheel.Show = true;
            }
        }else if(Input.GetKey("e") && Game.Started && Game.GameMode == GameModes.Defense)
        {
            RaycastHit hit;
            if(Physics.Raycast(this.transform.position, this.transform.forward, out hit, Game.InteractRange, Game.IgnoreSelectMask))
            {
                CallInAt = hit.point;
            }else
            {
                CallInAt = this.transform.position;
            }

            H.Wheel.SetMenu(0);
            H.Wheel.Show = true;
        }
        else    //HANDLE INPUT FROM WHEEL
        {
            H.Wheel.Show = false;
            if(H.Wheel.Selected != "")
            {
                if(ToCommand)
                {
                    if(ToCommand.Type == Unit.Types.Infantry)
                    {
                        switch(H.Wheel.Selected)
                        {
                            case "Control" : StartCoroutine(TakeControl(ToCommand)); break;
                            case "Clear" : ToCommand.inf.Order = Infantry.Orders.None; ToCommand.inf.ChangeBehavior(); break;
                            case "Follow" : ToCommand.inf.Order = Infantry.Orders.Follow; break;
                            case "Stay" :  ToCommand.inf.Order = Infantry.Orders.DefendPosition; break;
                        }
                    }else if(ToCommand.Type == Unit.Types.Plane)
                    {
                        switch(H.Wheel.Selected)
                        {
                            case "Control" : StartCoroutine(TakeControl(ToCommand)); break;
                        }
                    }else if(ToCommand.Type == Unit.Types.Tank)
                    {
                        switch(H.Wheel.Selected)
                        {
                            case "Control" : StartCoroutine(TakeControl(ToCommand)); break;
                        }
                    }
                    
                }else if(Controlled.Type == Unit.Types.Plane)
                {
                    switch(H.Wheel.Selected)
                    {
                        case "Return" : Controlled.pla.ChangeBehavior(); Leave(); break;
                        case "Drop" : Controlled.pla.DropBomb(); break;
                    }
                }else if(Controlled.Type == Unit.Types.Tank)
                {
                    switch(H.Wheel.Selected)
                    {
                        case "Return" : Controlled.tan.ChangeBehavior(); Leave(); break;
                    }
                }
                
                switch(H.Wheel.Selected)
                {
                    case "Men" : Manager.M.BuyCargoPlane(CallInAt); break;
                    case "Air" : Manager.M.BuyFighterPlane(Game.TeamOne); break;
                    case "Equipment" : Manager.M.BuyEquipment(CallInAt); break;
                    case "Artillery" : Manager.M.BuyArtillery(CallInAt); break;
                }
                
                

                H.Wheel.Selected = "";
            }
            ToCommand = null;
        }
    }

    public void ChangeFOV(float Goal, float Speed)
    {
        CurrentFOV = Mathf.Lerp(CurrentFOV, Goal, Time.deltaTime * 10f * Speed);
        Cam.fieldOfView = CurrentFOV;
    }
    
    public void TakeControlBruh(Unit U)
    {
        StartCoroutine(TakeControl(U));
    }
    
    IEnumerator TakeControl(Unit U)
    {
        float startTime = 0f;
        float Tspeed = 0.4f;
      
        startTime = Time.time;
        while(Time.time < startTime + Tspeed)
        {   
            Bend.intensity.Override(Mathf.Lerp(0f, -0.4f, (Time.time - startTime)/Tspeed));
            Distort.intensity.Override(Mathf.Lerp(0f, 1f, (Time.time - startTime)/Tspeed));
            yield return null;
        } 

        Bend.intensity.Override(-0.4f);
        Distort.intensity.Override(1f);
        
        
        if(U)
            SwapTo(U);

        ClearEffects();

        startTime = Time.time;
        while(Time.time < startTime + Tspeed)
        {   
            Bend.intensity.Override(Mathf.Lerp(-0.4f, 0f, (Time.time - startTime)/Tspeed));
            Distort.intensity.Override(Mathf.Lerp(1f, 0f, (Time.time - startTime)/Tspeed));
            yield return null;

            Bend.intensity.Override(0f);
            Distort.intensity.Override(0f);
        }
    }

    public void SwapTo(Unit U)
    {
        if(Controlled != null)
        {
            Controlled.Controller = null;
            if(Controlled.Type == Unit.Types.Infantry)
            {
                //Controlled.inf.SetState(Infantry.States.Idle);
                Controlled.inf.ChangeBehavior();
                for (int i = 0; i < Controlled.inf.HatLocation.childCount; i++)
                    Controlled.inf.HatLocation.GetChild(i).GetChild(0).gameObject.layer = 0;
            }
        }
        if(Body != null)
        {
            for (int i = 0; i < Body.HatLocation.childCount; i++)
                Body.HatLocation.GetChild(i).GetChild(0).gameObject.layer = 0;
            Body = null;
        }
        Controlled = U;
        
        this.transform.SetParent(Controlled.CameraParent);
        this.transform.localPosition = Vector3.zero;
        this.transform.localEulerAngles = Vector3.zero;
        
        U.Controller = this;
        U.gameObject.name = "Player";

        if(Controlled.Type == Unit.Types.Infantry)
        {
            for (int i = 0; i < Controlled.inf.HatLocation.childCount; i++)
                Controlled.inf.HatLocation.GetChild(i).GetChild(0).gameObject.layer = 8;
        }

        if(Controlled.Type == Unit.Types.Tank)
        {
            Controlled.tan.PlayerUsingPrimary = true;
        }

        if(Controlled.Type == Unit.Types.Plane)
        {
            Controlled.pla.PlayerUsingPrimary = true;
        }
        
        Controlled.SetGunImages();
        
        MenuManager.MM.SetScreen("HUD");

        //Debug.Log("SWAPPED");
    }
    /*
    public void FindNewBody(bool WithoutPause, bool GetNonIdeal)
    {
        StartCoroutine(SearchForNewBody(WithoutPause, GetNonIdeal));
    }

    IEnumerator SearchForNewBody(bool WithoutPause, bool GetNonIdeal)
    {
        if(!WithoutPause)
            yield return new WaitForSeconds(2f);

        List<Unit> Possible = new List<Unit>();
        for(int i = 0; i < Manager.M.TeamOne.Count; i++)
        {
            if(Manager.M.TeamOne[i].Type == Unit.Types.Infantry)
            {
                if(Manager.M.TeamOne[i].Targetable)
                {
                    Possible.Add(Manager.M.TeamOne[i]);
                }
                else if(GetNonIdeal)
                {
                    Possible.Add(Manager.M.TeamOne[i]);
                }
            }
        }
        
        if(Possible.Count > 0)
            StartCoroutine(TakeControl(Possible[Random.Range(0, Possible.Count)], false));
        
        if(Possible.Count == 0 && !GetNonIdeal)
        {
            FindNewBody(true, true);
        }
        
        if(Possible.Count == 0 && GetNonIdeal)
        {
            Debug.Log("YOU LOSE");
            this.transform.SetParent(null);
        }
    }*/
}
