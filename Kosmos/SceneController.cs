using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneController : MonoBehaviour
{
    public GameObject Overview;
    public GameObject Battle;
    public GameObject Interior;

    void Start()
    {
        Application.targetFrameRate = 60;
    }

    public void StartBattle(FleetInfo Player, FleetInfo Enemy, Mission M)
    {
        Overview.SetActive(false);
        Battle.SetActive(true);
        Battle.GetComponent<BattleMaker>().MakeBattle(Player, Enemy, M);
    }

    public void GoBack()
    {
        Overview.SetActive(true);
        Battle.SetActive(false);
    }
}
