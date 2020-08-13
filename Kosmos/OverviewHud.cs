using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class OverviewHud : MonoBehaviour
{
    public TextMeshProUGUI ShipRoster;
    public TextMeshProUGUI Credits;
    public TextMeshProUGUI MissionInfo;
    public FleetInfo FI;

    List<string> Names = new List<string>();
    List<int> Counts = new List<int>();

    public GameObject GameScreen;
    public GameObject StoreScreen;

    MissionController MC;

    void Start()
    {
        UpdateRoster();
    }

    void OnEnable()
    {
        UpdateRoster();
    }

    public void OpenShop(Planet P)
    {
        FI.GetComponent<ShipController>().Busy = true;
        GameScreen.SetActive(false);
        StoreScreen.GetComponent<ShopScreen>().InitializeVendor(P);
        StoreScreen.SetActive(true);
    }

    public void CloseShop()
    {
        UpdateRoster();
        FI.GetComponent<CameraControls>().ShopClosed();
        FI.GetComponent<ShipController>().Busy = false;
        GameScreen.SetActive(true);
        StoreScreen.SetActive(false);
    }

    public void UpdateRoster()
    {
        MC = FI.GetComponent<MissionController>();

        if(MC.CurrentMission == null)
        {
            MissionInfo.text = "No Current Mission";
        }else
        {
            MissionInfo.text = "";

            if(MC.CurrentMission.Bounty)
            {
                MissionInfo.text += "Bounty" + "\n";
                MissionInfo.text += "\n";
                MissionInfo.text += "Ship Count : " + "\n";
                MissionInfo.text += MC.CurrentMission.GetComponent<FleetInfo>().Ships.Count + "\n";
                MissionInfo.text += "\n";
                MissionInfo.text += "Reward : " + "\n";
                MissionInfo.text += MC.CurrentMission.CreditGain + "c" + "\n";
            }else
            {
                MissionInfo.text += "Delivery" + "\n";
                MissionInfo.text += "\n";
                MissionInfo.text += "Planet Name : " + "\n"; 
                MissionInfo.text += MC.CurrentMission.GetComponent<Planet>().PlanetName + "\n";
                MissionInfo.text += "\n";
                MissionInfo.text += "Reward : " + "\n";
                MissionInfo.text += MC.CurrentMission.CreditGain + "c" + "\n";
            }
        }

        Credits.text = FI.GetComponent<PlayerInformation>().Credits.ToString() + " Credits";

        Names = new List<string>();
        Counts = new List<int>();
        

        for(int i = 0; i < FI.Ships.Count; i++)
        {
            if(NameIndex(FI.Ships[i].name) == -1)
            {
                Names.Add(FI.Ships[i].name);
                Counts.Add(1);
            }else
            {
                Counts[NameIndex(FI.Ships[i].name)]++;
            }
        }

        string RosterText = "";

        for(int i = 0; i < Names.Count; i++)
        {
            RosterText += Counts[i] + "x " + Names[i] + "\n";
        }

        ShipRoster.text = RosterText;

    }

    int NameIndex(string Name)
    {
        if(Names.Count == 0)
        {
            return -1;
        }else
        {
            for(int i = 0; i < Names.Count; i++)
            {
                if(Name == Names[i])
                {
                    return i;
                }
            }
        }

        return -1;
    }
}
