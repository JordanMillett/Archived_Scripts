using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.SceneManagement;

public class CarVarSync : NetworkBehaviour
{
    public SyncList<Color> Colors = new SyncList<Color>();

    [SyncVar]
    public int TimesUpdated = 0;

    [SyncVar]
    public EngineKit Kit;

    [SyncVar]
    public int HornIndex = 0;

    int PrevTimesUpdated = 0;

    GameManager GM;
    NetworkIdentity netID;

    List<Material[]> Mats = new List<Material[]>();
    List<MeshRenderer> Meshes = new List<MeshRenderer>();

    public List<AudioClip> Horns;

    void Awake()
    {
        netID = GetComponent<NetworkIdentity>();

        if(SceneManager.GetActiveScene().buildIndex != 0)
        {
            GM = GameObject.FindWithTag("Manager").GetComponent<GameManager>();
        }
    }

    void Update()
    {
        if(PrevTimesUpdated != TimesUpdated)
        {
            PrevTimesUpdated = TimesUpdated;

            if(!netID.hasAuthority)
            {
                ReceiveInfo();
            } 
        }
    }

    public void UpdateInfo(Color[] _colors, EngineKit EK, int _hornIndex)
    {
        TimesUpdated++;
        CmdSendInfo(_colors, EK, _hornIndex,TimesUpdated);
    }

    public void ReceiveInfo()
    {
        
        VehicleController VC = GetComponent<VehicleController>();

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
                        M.SetColor("_albedo", Colors[0]);
                    break;
                    case "Body (Instance)" :
                        M.SetColor("_albedo", Colors[1]);
                    break;
                    case "Seat (Instance)" :
                        M.SetColor("_albedo", Colors[2]);
                    break;
                    case "Tire (Instance)" :
                        M.SetColor("_albedo", Colors[3]);
                    break;
                    case "Wheel (Instance)" :
                        M.SetColor("_albedo", Colors[4]);
                    break;
                }
            }
        }
        /*
        BodyMats = Body.materials;
        WheelMats.Add(Wheels[0].materials);
        WheelMats.Add(Wheels[1].materials);
        WheelMats.Add(Wheels[2].materials);
        WheelMats.Add(Wheels[3].materials);

        WheelMats.Add(Wheels[4].materials);

        BodyMats[0].SetColor("_albedo", Colors[0]); //Paint 

        BodyMats[1].SetColor("_albedo", Colors[1]); //Body 

        WheelMats[0][2].SetColor("_albedo", Colors[1]); //Wheel Body 
        WheelMats[1][2].SetColor("_albedo", Colors[1]); 
        WheelMats[2][2].SetColor("_albedo", Colors[1]); 
        WheelMats[3][2].SetColor("_albedo", Colors[1]); 

        WheelMats[4][0].SetColor("_albedo", Colors[1]); //Steering Wheel

        BodyMats[2].SetColor("_albedo", Colors[2]); //Seat 

        WheelMats[0][0].SetColor("_albedo", Colors[3]); //Wheel 
        WheelMats[1][0].SetColor("_albedo", Colors[3]);
        WheelMats[2][0].SetColor("_albedo", Colors[3]);
        WheelMats[3][0].SetColor("_albedo", Colors[3]);

        WheelMats[0][1].SetColor("_albedo", Colors[4]); //Tire 
        WheelMats[1][1].SetColor("_albedo", Colors[4]);
        WheelMats[2][1].SetColor("_albedo", Colors[4]);
        WheelMats[3][1].SetColor("_albedo", Colors[4]);
        */
        VC.EK = Kit;

        VC.Horn = Horns[HornIndex];
    }

    [Command]
    public void CmdSendInfo(Color[] _colors, EngineKit _ek, int _hornIndex, int times)
    {
        //Debug.Log("COLORS Sent");
        Colors.Clear();
        foreach(Color C in _colors)
        {
            Colors.Add(C);
        }

        Kit = _ek;
        HornIndex = _hornIndex;

        TimesUpdated = times;
    }
}
