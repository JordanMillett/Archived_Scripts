using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.Rendering;

public class Player : MonoBehaviour
{
    public static HUD H;
    public HUD Href;

    public static Unit Controlled;

    public Ragdoll Body;

    Camera Cam;

    public float CurrentFOV = 70f;

    ChromaticAberration Distort;
    LensDistortion Bend;
    ColorAdjustments Tinter;
    Vignette Supression;

    float CurrentSupression = 0f;
    float CurrentMuffle = 22000f;
    float LastMuffled = -100f;

    AudioLowPassFilter Muffle;

    Vector3 CallInAt;
    Unit ToCommand;

    void Awake()
    {
        H = Href;
    }

    void Start()
    {
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = 60;

        Cam = GetComponent<Camera>();
        Muffle = GetComponent<AudioLowPassFilter>();

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
        while(!Game.Setup)
            yield return null;

        MenuManager.MM.SetScreen("Loading");

        FindNewBody(false, false);
        
        while(!Controlled)
            yield return null;

        MenuManager.MM.SetScreen("HUD");
        Game.PlayerReady = true;
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

        Tinter.saturation.Override(20f);
        Supression.intensity.Override(0f);
    }

    void Update()
    {
        if(Controlled)
        {
            if(Input.GetKeyDown("k"))
                Controlled.Kill();
            
            //this.transform.position = Vector3.Lerp(this.transform.position, Controlled.CameraParent.position, Time.deltaTime * 5f);
            //this.transform.position = Controlled.CameraParent.position;
            //this.transform.rotation = Quaternion.Lerp(this.transform.rotation, Controlled.CameraParent.rotation, Time.deltaTime * 15f);
            //this.transform.rotation = Controlled.CameraParent.rotation;
        }else
        {
            ChangeFOV(Game.BaseFOV, 0.1f);
        }

        if(Input.GetMouseButton(0) && Controlled != null)
            Controlled.Fire();

        if(Controlled && Controlled.Type == Unit.Types.Infantry || Body)
        {
            CurrentSupression = Mathf.Lerp(CurrentSupression, 0f, Time.deltaTime * 0.15f);
            if(Time.time > LastMuffled + 3f)
                CurrentMuffle = Mathf.Lerp(CurrentMuffle, 22000f, Time.deltaTime * 0.5f);

            Muffle.cutoffFrequency = CurrentMuffle;
            
            Tinter.saturation.Override(Controlled.inf.Dead ? -100f : Mathf.Lerp(-100f, 20f, Controlled.inf.HealthAlpha));
            Supression.intensity.Override(CurrentSupression);
        }else
        {
            ClearEffects();
        }
        
        if(Controlled != null)
            WheelControls();

        if(Game.Setup && Game.PlayerReady)
        {
            if(Input.GetKeyDown(KeyCode.Escape))
            {
                if(MenuManager.MM.CurrentScreen == "HUD")
                    MenuManager.MM.SetScreen("Pause");
                else
                    MenuManager.MM.SetScreen("HUD");
            }
        }
                
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
        }else if(Input.GetKey("e") && Game.PlayerReady && Game.GameMode == GameModes.Defense)
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
                            case "Control" : StartCoroutine(TakeControl(ToCommand, false)); break;
                            case "Clear" : ToCommand.inf.Order = Infantry.Orders.None; break;
                            case "Follow" : ToCommand.inf.Order = Infantry.Orders.Follow; break;
                            case "Stay" :  ToCommand.inf.Order = Infantry.Orders.DefendPosition; break;
                        }
                    }else if(ToCommand.Type == Unit.Types.Plane)
                    {
                        switch(H.Wheel.Selected)
                        {
                            case "Control" : StartCoroutine(TakeControl(ToCommand, false)); break;
                        }
                    }else if(ToCommand.Type == Unit.Types.Tank)
                    {
                        switch(H.Wheel.Selected)
                        {
                            case "Control" : StartCoroutine(TakeControl(ToCommand, false)); break;
                        }
                    }
                    
                }else if(Controlled.Type == Unit.Types.Plane)
                {
                    switch(H.Wheel.Selected)
                    {
                        case "Return" : FindNewBody(true, false); break;
                        case "Drop" : Controlled.pla.DropBomb(); break;
                    }
                }else if(Controlled.Type == Unit.Types.Tank)
                {
                    switch(H.Wheel.Selected)
                    {
                        case "Return" : FindNewBody(true, false); break;
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

    IEnumerator TakeControl(Unit U, bool Instant)
    {
        float startTime = 0f;
        float Tspeed = 0.4f;
        if(!Instant)
        {
            startTime = Time.time;
            while(Time.time < startTime + Tspeed)
            {   
                Bend.intensity.Override(Mathf.Lerp(0f, -0.4f, (Time.time - startTime)/Tspeed));
                Distort.intensity.Override(Mathf.Lerp(0f, 1f, (Time.time - startTime)/Tspeed));
                yield return null;
            } 

            Bend.intensity.Override(-0.4f);
            Distort.intensity.Override(1f);
        }
        
        if(U)
        {
            SwapTo(U);
        }else
        {
            FindNewBody(true, false);
        }

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

    void SwapTo(Unit U)
    {
        if(Controlled != null)
        {
            Controlled.Controller = null;
            if(Controlled.Type == Unit.Types.Infantry)
            {
                Controlled.inf.UpdateState(Infantry.States.Idle);
                for (int i = 0; i < Controlled.inf.HatLocation.childCount; i++)
                    Controlled.inf.HatLocation.GetChild(i).GetChild(0).gameObject.layer = 0;
            }
        }
        if(Body != null)
        {
            for (int i = 0; i < Body.HatLocation.childCount; i++)
                Body.HatLocation.GetChild(i).GetChild(0).gameObject.layer = 0;
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
    }

    public void FindNewBody(bool WithoutPause, bool GetNonIdeal)
    {
        StartCoroutine(SearchForNewBody(WithoutPause, GetNonIdeal));
    }

    IEnumerator SearchForNewBody(bool WithoutPause, bool GetNonIdeal)
    {
        if(!WithoutPause)
            yield return new WaitForSeconds(2f);

        bool Found = false;

        List<Unit> Possible = new List<Unit>();
        for(int i = 0; i < Manager.M.TeamOne.Count; i++)
            Possible.Add(Manager.M.TeamOne[i]);
        
        while(Possible.Count > 1 || Found)
        {
            Unit U = Possible[Random.Range(0, Possible.Count)];
            Possible.Remove(U);
            if(U.Type == Unit.Types.Infantry)
            {
                if(U.Targetable)
                {
                    Found = true;
                    StartCoroutine(TakeControl(U, false));
                }else if(GetNonIdeal)
                {
                    Found = true;
                    StartCoroutine(TakeControl(U, false));
                }
            }
        }

        if(!Found && !GetNonIdeal)
        {
            FindNewBody(true, true);
        }else
        {
            Debug.Log("YOU LOSE");
        }
        
        //ON LOSE
        /*
        if(!Found && GetNonIdeal)
        {
            this.transform.SetParent(null);
        }*/
    }
}
