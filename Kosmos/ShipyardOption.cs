using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ShipyardOption : MonoBehaviour
{
    public Ship S;

    PlayerInformation PI;

    TextMeshProUGUI NameText;
    TextMeshProUGUI CostText;

    bool Called = false;

    void Start()
    {
        CostText = transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        NameText = transform.GetChild(1).GetComponent<TextMeshProUGUI>();

        NameText.text = S.Name;
        CostText.text = S.Cost.ToString() + "c";
    }

    public void Purchase()      //Delete option after purchase
    {
        PI = GameObject.FindWithTag("Player").GetComponent<PlayerInformation>();

        if(PI.Credits >= S.Cost)
        {
            PI.Credits -= S.Cost;
            GameObject.FindWithTag("ShopPanel").GetComponent<ShopScreen>().P.PlayerReputation += Mathf.RoundToInt(S.Cost/10f);
            PI.GetComponent<FleetInfo>().Ships.Add(S);
            GameObject.FindWithTag("ShopPanel").GetComponent<ShopScreen>().Refresh();
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
