using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MissionOption : MonoBehaviour
{
    public int ReputationGain;
    public int CreditGain;
    public bool Bounty = false;
    GameObject TargetObject;

    TextMeshProUGUI NameText;
    TextMeshProUGUI RewardText;
    TextMeshProUGUI InfoText;

    bool Called = false;

    public Planet P;

    void Start()
    {
        RewardText = transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        NameText = transform.GetChild(1).GetComponent<TextMeshProUGUI>();
        InfoText = transform.GetChild(2).GetComponent<TextMeshProUGUI>();

        MakeMission();
    }

    void MakeMission()
    {
        Bounty = (Random.value < 0.5f);

        if(Bounty)
        {

            NameText.text = "Bounty";
            //InfoText = Distance, Enemy Count

            FollowPlayer[] AllEnemies = GameObject.FindWithTag("Overview").GetComponentsInChildren<FollowPlayer>();
            TargetObject = AllEnemies[Random.Range(0, AllEnemies.Length)].gameObject;
            FleetInfo TargetFleet = TargetObject.GetComponent<FleetInfo>();
            CreditGain = (TargetFleet.Ships.Count * UniversalConstants.BountyShipDefeatCredits);
            int Distance = Mathf.RoundToInt(Vector3.Distance(P.transform.position, TargetObject.transform.position));
            InfoText.text = Distance.ToString() + "ly " + TargetFleet.Ships.Count + " Ships";
            ReputationGain = TargetFleet.Ships.Count * 3;
            


        }else
        {
            NameText.text = "Delivery";

            Planet[] AllPlanets = GameObject.FindWithTag("Overview").GetComponentsInChildren<Planet>();
            TargetObject = AllPlanets[Random.Range(0, AllPlanets.Length)].gameObject;

            int Distance = Mathf.RoundToInt(Vector3.Distance(P.transform.position, TargetObject.transform.position));
            CreditGain = Distance * UniversalConstants.DeliveryCreditsMultiplier;
            ReputationGain = Distance;
            InfoText.text = Distance.ToString() + "ly " + TargetObject.GetComponent<Planet>().PlanetName;
        }

        RewardText.text = CreditGain.ToString() + "c";
    }

    public void Accept() 
    {
        MissionController MC = GameObject.FindWithTag("Player").GetComponent<MissionController>();

        if(MC.CurrentMission == null)
        {
            Mission M = TargetObject.AddComponent(typeof(Mission)) as Mission;

            M.P = P;
            M.ReputationGain = ReputationGain;
            M.CreditGain = CreditGain;
            M.Bounty = Bounty;

            MC.CurrentMission = M;
        }

    }

    void OnDisable()
    {
        if(Called)
            Destroy(this.gameObject);
        else
            Called = true;
    }
}
