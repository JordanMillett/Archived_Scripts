using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CarMenu : MonoBehaviour
{
    [System.Serializable]
    public struct Menu
    {
	    public GameObject Screen;
        //public transform camera location
    }

    List<Material[]> Mats = new List<Material[]>();
    List<MeshRenderer> Meshes = new List<MeshRenderer>();

    public AutoShop AS = null;

    public int CurrentScreen = 0;

    public List<Menu> MenuScreens;

    public TuneMenu TM;
    public HornMenu HM;
    public PartMenu PM;

    public VehicleController VC;

    public List<EngineKit> Kits;

    public List<Horn> Horns;

    public int PreviewHornIndex = 0;
    public int HornIndex = 0;
    
    public int ColorPartIndex = 0;
    public int ColorIndex = 0;
    public int SubColorIndex = 0;

    public List<VehiclePart> NewParts = new List<VehiclePart>();
    List<VehiclePart> PreviewParts = new List<VehiclePart>();
    
    public TextMeshProUGUI SellLabel;
    int CarValue = 0;

    AudioSource ASS;

    Color[] PreviewColors = new Color[5]
    {
        new Color(239f/255f,240f/255f,215f/255f,1), //Paint
        new Color(94f/255f,107f/255f,130f/255f,1),  //Body
        new Color(248f/255f,229f/255f,116f/255f,1), //Seat
        new Color(55f/255f,50f/255f,84f/255f,1),    //Tire
        new Color(155f/255f,156f/255f,130f/255f,1) //Wheel
    };

    Color[] Colors = new Color[5]
    {
        new Color(239f/255f,240f/255f,215f/255f,1), //Paint
        new Color(94f/255f,107f/255f,130f/255f,1),  //Body
        new Color(248f/255f,229f/255f,116f/255f,1), //Seat
        new Color(55f/255f,50f/255f,84f/255f,1),    //Tire
        new Color(155f/255f,156f/255f,130f/255f,1) //Wheel
    };
    

    public void HoverColor(int Index, bool Sub)
    {
        Color C;

        if(!Sub)
        {
            ColorUtility.TryParseHtmlString("#" + ColorLookup.CustomColors[Index].Hex, out C);
        }else
        {
            if(Index == -1)
            {
                ColorUtility.TryParseHtmlString("#" + ColorLookup.CustomColors[ColorIndex].Hex, out C);
            }else
            {
                ColorUtility.TryParseHtmlString("#" + ColorLookup.CustomColors[ColorIndex].SubShades[Index].Hex, out C);
            }  
        }

        PreviewColors[ColorPartIndex] = C;

        ApplyColors(PreviewColors);
    }

    public void SetColorMenu(int Index)
    {
        ColorIndex = Index;
    }

    public void SetColor(int Index)
    {
        SubColorIndex = Index;
        //ColorIndex = Index;
        //SubColorIndex = 0;

        Color C;

        if(SubColorIndex == -1)
        {
            if(PlayerInfo.Balance >= ColorLookup.CustomColors[ColorIndex].Cost)
            {
                ColorUtility.TryParseHtmlString("#" + ColorLookup.CustomColors[ColorIndex].Hex, out C);
                PlayerInfo.Balance -= ColorLookup.CustomColors[ColorIndex].Cost;
            }else
            {
                return;
            }
        }else
        {   
            if(PlayerInfo.Balance >= ColorLookup.CustomColors[ColorIndex].SubShades[SubColorIndex].Cost)
            {
                ColorUtility.TryParseHtmlString("#" + ColorLookup.CustomColors[ColorIndex].SubShades[SubColorIndex].Hex, out C);
                PlayerInfo.Balance -= ColorLookup.CustomColors[ColorIndex].SubShades[SubColorIndex].Cost;
            }else
            {
                return;
            }
        }

        Colors[ColorPartIndex] = C;

        //for(int i = 0; i < Colors.Length; i++)
            //PreviewColors[i] = Colors[i];
        

        SetScreen(1);
        //ApplyColors(Colors);
    }

    public void PlayHorn(int Index)
    {
        ASS = GetComponent<AudioSource>();
        ASS.volume = (1f * (Settings._sfxVolume/100f)) * (Settings._masterVolume/100f);
        ASS.clip = Horns[Index].Sound;
        ASS.Play();
    }

    public void SetHorn(int Index)
    {
        PlayHorn(Index);
        HornIndex = Index;
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            SetScreen(0);
        }
    }

    void ApplyColors(Color[] C)
    {
        //Debug.Log(Mats.Count);

        VC.RecalcMeshes();

        Meshes.Clear();
        Mats.Clear();

        for(int i = 0; i < VC.Meshes.Count; i++)
        {
            Meshes.Add(VC.Meshes[i]);
            Mats.Add(Meshes[i].materials);
        }

        for(int i = 0; i < Mats.Count; i++)
        {
            foreach(Material M in Mats[i])
            {
                switch (M.name)
                {
                    case "Paint (Instance)" :
                        M.SetColor("_albedo", C[0]);
                    break;
                    case "Body (Instance)" :
                        M.SetColor("_albedo", C[1]);
                    break;
                    case "Seat (Instance)" :
                        M.SetColor("_albedo", C[2]);
                    break;
                    case "Tire (Instance)" :
                        M.SetColor("_albedo", C[3]);
                    break;
                    case "Wheel (Instance)" :
                        M.SetColor("_albedo", C[4]);
                    break;
                }
            }
        }

        //Paint
        //Body
        //Seat
        //Tire
        //Wheel


        /*
        BodyMats[0].SetColor("_albedo", C[0]); //Paint 

        BodyMats[1].SetColor("_albedo", C[1]); //Body 

        WheelMats[0][2].SetColor("_albedo", C[1]); //Wheel Body 
        WheelMats[1][2].SetColor("_albedo", C[1]); 
        WheelMats[2][2].SetColor("_albedo", C[1]); 
        WheelMats[3][2].SetColor("_albedo", C[1]); 

        WheelMats[4][0].SetColor("_albedo", C[1]); //Steering Wheel

        BodyMats[2].SetColor("_albedo", C[2]); //Seat 

        WheelMats[0][0].SetColor("_albedo", C[3]); //Wheel 
        WheelMats[1][0].SetColor("_albedo", C[3]);
        WheelMats[2][0].SetColor("_albedo", C[3]);
        WheelMats[3][0].SetColor("_albedo", C[3]);

        WheelMats[0][1].SetColor("_albedo", C[4]); //Tire 
        WheelMats[1][1].SetColor("_albedo", C[4]);
        WheelMats[2][1].SetColor("_albedo", C[4]);
        WheelMats[3][1].SetColor("_albedo", C[4]);
        */
    }

    public void TogglePicker(int index)
    {
        ColorPartIndex = index;
        SetScreen(4);
    }

    void OnEnable()
    {
        VC = GameObject.FindWithTag("Player").GetComponent<Player>().CurrentVehicle;
        AS = VC.CurrentShop;

        TM.Init(this, Kits, GetUpgradeIndex());
        HM.Init(this, Horns);
        PM.Init(this);

        for(int i = 0; i < 3; i++)      //FRONT MIDDLE END
        {
            NewParts.Add(VC.CurrentParts[i]);
            PreviewParts.Add(VC.CurrentParts[i]);
        }
        //SubPartIndices

        /*
        WheelMats.Add(Wheels[0].materials);
        WheelMats.Add(Wheels[1].materials);
        WheelMats.Add(Wheels[2].materials);
        WheelMats.Add(Wheels[3].materials);

        WheelMats.Add(Wheels[4].materials);
        */

        for(int i = 0; i < Colors.Length; i++)
            PreviewColors[i] = Colors[i];

        ApplyColors(Colors);
    }

    int GetUpgradeIndex()
    {
        for(int i = 0; i < Kits.Count; i++)
        {
            if(VC.EK == Kits[i])
                return i;
        }

        return 0;
    }

    public void ChangeActiveView(int Index)
    {
        AS.SetActiveView(Index);
    }

    void Start()
    {
        SetScreen(0);
    }

    public void Exit()
    {
        AS.Set(false);
    }

    void OnDisable()
    {
        ApplyColors(Colors);
        ApplyEngine();
        ApplyHorn();
        ApplyParts();
        SetScreen(0);

        SendInfo();
    }

    public void ApplyEngine()
    {
        VC.EK = Kits[TM.UpgradeIndex];
    }

    public void ApplyHorn()
    {
        VC.Horn = Horns[HornIndex].Sound;
    }

    void SendInfo()
    {
        if(VC != null)
        {
            VC.GetComponent<CarVarSync>().UpdateInfo(Colors, Kits[TM.UpgradeIndex], HornIndex);
        }
    }

    public void SetScreen(int Index)
    {
        ApplyColors(Colors);
        for(int i = 0; i < Colors.Length; i++)
            PreviewColors[i] = Colors[i];
        
        RefreshValue();

        CurrentScreen = Index;

        if(Index == 0)
            ChangeActiveView(1);

        //if(Index == 6)
            //VC.SetParts(VC.CurrentParts);

        for(int i = 0; i < MenuScreens.Count; i++)
        {
            if(i == Index)
            {
                MenuScreens[i].Screen.SetActive(true);
            }
            else
            {
                MenuScreens[i].Screen.SetActive(false);
            }
        }
    }

    void ApplyParts()
    {
        for(int i = 0; i < 3; i++)
        {
            ApplyPart((VehiclePart.Location) i, NewParts[i], true);
        }
    }

    public void ApplyPart(VehiclePart.Location Loc, VehiclePart VP, bool PaidFor)    //Loc = Which tab
    {
        if(PaidFor)
        {
            NewParts[(int)Loc] = VP;
            VC.SetParts(NewParts);
            for(int i = 0; i < VC.CurrentParts.Count; i++)
            {
                VC.CurrentParts[i] = NewParts[i];
            }
            PM.SetScreen(0);
            SetScreen(6);
        }else if(PreviewParts[(int)Loc] != VP)
        {
            PreviewParts.Clear();
            PreviewParts.Add(NewParts[0]);
            PreviewParts.Add(NewParts[1]);
            PreviewParts.Add(NewParts[2]);
            PreviewParts[(int)Loc] = VP;
            VC.SetParts(PreviewParts);
        }
        
        ApplyColors(Colors);

        //VC.InitialzeParts

        //NewParts.Add(VC.Config.InstallableParts[])

        



        //do the thing
    }

    public void RefreshValue()
    {   
        CarValue = Mathf.RoundToInt(VC.Config.VehicleBaseValue * 0.75f);
        
        if(TM.UpgradeIndex != 0)
            CarValue += Mathf.RoundToInt(Kits[TM.UpgradeIndex].Cost * 0.75f);
        if(HornIndex != 0)
            CarValue += Mathf.RoundToInt(Horns[HornIndex].Cost * 0.75f);

        SellLabel.text = "+$" + CarValue.ToString();
    }

    public void Sell()
    {
        PlayerInfo.Balance += CarValue;
        GameObject.FindWithTag("Player").GetComponent<Player>().LeaveVehicle(false);
        Destroy(VC.gameObject);
        AS.ExitNoCar();
    }
}
