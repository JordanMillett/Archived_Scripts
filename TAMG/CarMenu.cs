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

    public Vehicle VC;

    public List<Horn> Horns;

    public int ColorPartIndex = 0;
    public int ColorIndex = 0;
    public int SubColorIndex = 0;

    public int PaidHornIndex = 0;
    public int PaidKitIndex = 0;
    List<int> PaidPartsIndices = new List<int>();
    
    public TextMeshProUGUI SellLabel;
    int CarValue = 0;

    AudioSource ASS;

    Color[] PaidColors = new Color[5];

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

        List<Color> PreviewColors = new List<Color>();      //Make new list of colors and change one color to be the preview
        foreach(Color Col in PaidColors)
        {
            PreviewColors.Add(Col);
        }
        PreviewColors[ColorPartIndex] = C;

        ApplyColors(PreviewColors.ToArray());
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

        PaidColors[ColorPartIndex] = C;

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
        VC.MRD.HornIndex = Index;
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
        for(int i = 0; i < VC.MRD.Colors.Count; i++)
        {
            VC.MRD.Colors[i] = C[i];
        }

        VC.MRD.RpcRefreshVehicle();

        //Debug.Log(Mats.Count);
        /*
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

        TM.Init(this, VC.Config.InstallableKits, VC.MRD.KitIndex);
        HM.Init(this, Horns);
        PM.Init(this);

        PaidHornIndex = VC.MRD.HornIndex;
        PaidKitIndex = VC.MRD.KitIndex;

        PaidPartsIndices.Clear();
        for(int i = 0; i < 3; i++)      //FRONT MIDDLE END
        {
            PaidPartsIndices.Add(VC.MRD.CurrentPartsIndices[i]);
        }

        for(int i = 0; i < VC.MRD.Colors.Count; i++)      //FRONT MIDDLE END
        {
            PaidColors[i] = VC.MRD.Colors[i];
        }
        //SubPartIndices

        /*
        WheelMats.Add(Wheels[0].materials);
        WheelMats.Add(Wheels[1].materials);
        WheelMats.Add(Wheels[2].materials);
        WheelMats.Add(Wheels[3].materials);

        WheelMats.Add(Wheels[4].materials);
        */

        /*
        for(int i = 0; i < Colors.Length; i++)
            PreviewColors[i] = Colors[i];

        ApplyColors(Colors);
        */
    }
    /*
    int GetUpgradeIndex()
    {
        for(int i = 0; i < Kits.Count; i++)
        {
            if(VC.MRD.EK == Kits[i])
                return i;
        }

        return 0;
    }
    */
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
        ApplyColors(PaidColors);
        ApplyEngine(PaidKitIndex);
        ApplyHorn(PaidHornIndex);
        ApplyParts(PaidPartsIndices, true);
        SetScreen(0);

        VC.MRD.CmdRefreshVehicle();
    }

    public void ApplyEngine(int _KitIndex)
    {
        VC.MRD.KitIndex = _KitIndex;
    }

    public void ApplyHorn(int _HornIndex)
    {
        VC.MRD.HornIndex = _HornIndex;
    }

    public void SetScreen(int Index)
    {
        ApplyColors(PaidColors);
        //for(int i = 0; i < Colors.Length; i++)
            //PreviewColors[i] = Colors[i];
        
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

    public void ApplyParts(List<int> _paidPartsIndices, bool PaidFor)
    {
        //Debug.Log(_paidPartsIndices);

        for(int i = 0; i < 3; i++)
        {
            VC.MRD.CurrentPartsIndices[i] = _paidPartsIndices[i];
            //ApplyPart((VehiclePart.Location) i, _paidPartsIndices[i]);
        }
        
        VC.MRD.CmdRefreshVehicle();

        if(PaidFor)
        {
            for(int i = 0; i < 3; i++)
            {
                PaidPartsIndices[i] = _paidPartsIndices[i];
            }
            PM.SetScreen(0);
            SetScreen(6);
        }
    }
    /*
    void ApplyPart(VehiclePart.Location Loc, int PartIndex)    //Loc = Which tab
    {
        VC.MRD.CurrentPartsIndices[(int)Loc] = PartIndex;

        
            
            //NewPartsIndices[(int)Loc] = PartIndex;
            //VC.SetParts(NewPartsIndices);
            /*
            for(int i = 0; i < 3; i++)
            {
                VC.MRD.CurrentPartsIndices[i] = NewParts[i];
            }*/
            
        //}/*else if(PreviewParts[(int)Loc] != PartIndex)
        //{
            //VC.MRD.CurrentPartsIndices[i] = PartIndex;
            /*
            PreviewParts.Clear();
            PreviewParts.Add(NewParts[0]);
            PreviewParts.Add(NewParts[1]);
            PreviewParts.Add(NewParts[2]);
            PreviewParts[(int)Loc] = VP;
            VC.SetParts(PreviewParts);*/
        //}
        

        
        //ApplyColors(Colors);

        //VC.InitialzeParts

        //NewParts.Add(VC.Config.InstallableParts[])

        



        //do the thing
    //}

    public void RefreshValue()
    {   
        CarValue = Mathf.RoundToInt(VC.Config.VehicleBaseValue * 0.75f);
        
        if(VC.MRD.KitIndex != 0)
            CarValue += Mathf.RoundToInt(VC.Config.InstallableKits[VC.MRD.KitIndex].Cost * 0.75f);
        if(VC.MRD.HornIndex != 0)
            CarValue += Mathf.RoundToInt(VC.Config.InstallableHorns.Horns[VC.MRD.HornIndex].Cost * 0.75f);

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
