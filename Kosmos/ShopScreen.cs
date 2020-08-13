using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ShopScreen : MonoBehaviour
{
    public GameObject ShipOptionTemplate;
    public GameObject MissionOptionTemplate;

    TextMeshProUGUI PlanetName;
    TextMeshProUGUI Credits;
    TextMeshProUGUI Reputation;

    GameObject Shipyard;
    GameObject Missions;

    PlayerInformation PI;

    bool SetUpDone = false;

    public Planet P;

    void Start()
    {
        if(!SetUpDone)
            SetUp();
    }

    void SetUp()
    {
        PI = GameObject.FindWithTag("Player").GetComponent<PlayerInformation>();

        PlanetName = transform.GetChild(1).GetComponent<TextMeshProUGUI>();
        Credits = transform.GetChild(2).GetComponent<TextMeshProUGUI>();
        Reputation = transform.GetChild(3).GetComponent<TextMeshProUGUI>();

        Shipyard = transform.GetChild(4).GetChild(1).gameObject;
        Missions = transform.GetChild(5).GetChild(1).gameObject;

        SetUpDone = true;
    }

    public void InitializeVendor(Planet Pl)
    {
        P = Pl;

        if(!SetUpDone)
            SetUp();

        PlanetName.text = P.PlanetName;
        Credits.text = PI.Credits.ToString() + " Credits";
        Reputation.text = P.PlayerReputation.ToString() + " Reputation";

        GenerateShips();
        GenerateMissions();
    }

    public void Refresh()
    {
        Credits.text = PI.Credits.ToString() + " Credits";
        Reputation.text = P.PlayerReputation.ToString() + " Reputation";
    }

    void GenerateShips()        //Up to 4 for now
    {
        float YLoc = 160f;
        float YInterval = 110f;

        for(int i = 0; i < P.Shipyard.Count; i++)
        {
            GameObject S = Instantiate(ShipOptionTemplate, Vector3.zero, Quaternion.identity);
            S.transform.SetParent(Shipyard.transform);
            S.GetComponent<RectTransform>().anchoredPosition = new Vector3(0f, YLoc - (YInterval * i) , 0f);
            S.GetComponent<RectTransform>().transform.localScale = Vector3.one;
            
            S.GetComponent<ShipyardOption>().S = P.Shipyard[i];
        }
    }

    void GenerateMissions()
    {
        float YLoc = 160f;
        float YInterval = 160f;

        int RanCount = Random.Range(1, 4);

        for(int i = 0; i < RanCount; i++)
        {
            GameObject M = Instantiate(MissionOptionTemplate, Vector3.zero, Quaternion.identity);
            M.transform.SetParent(Missions.transform);
            M.GetComponent<RectTransform>().anchoredPosition = new Vector3(0f, YLoc - (YInterval * i) , 0f);
            M.GetComponent<RectTransform>().transform.localScale = Vector3.one;
            
            M.GetComponent<MissionOption>().P = P;
        }
    }
}
