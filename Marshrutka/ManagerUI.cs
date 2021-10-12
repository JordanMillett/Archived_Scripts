using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ManagerUI : MonoBehaviour
{
    //DATATYPES

    //PUBLIC COMPONENTS
    public GameObject TabStats;
    public GameObject TabMoney;
    public TextMeshProUGUI NewNightText;
    public TextMeshProUGUI NewNightDate;
    public TextMeshProUGUI ResultsNightText;
    public TextMeshProUGUI T_GasUsed;
    public TextMeshProUGUI T_TopSpeed;
    public TextMeshProUGUI T_Savings;
    public TextMeshProUGUI T_FineCost;
    public TextMeshProUGUI T_Earnings;
    public TextMeshProUGUI T_GasCost;
    public TextMeshProUGUI T_Bills;
    public TextMeshProUGUI T_Total;
    public AudioClip MenuSound;

    //PUBLIC VARS
    public int Fines = 0;
    public int StartMoney = 0;
    public float TopSpeed = 0f;

    //PUBLIC LISTS

    //COMPONENTS
    PlayerController Bus;
    Manager M;
    MenuManager MM;
    AudioSourceController ASC;

    //VARS
    bool ContinuePressed = false;

    //LISTS

    void Start()
    {
        ASC = GetComponent<AudioSourceController>();
    }    

    public void Initialize()
    {
        M = GameObject.FindWithTag("Manager").GetComponent<Manager>();
        MM = GameObject.FindWithTag("MenuManager").GetComponent<MenuManager>();
        Bus = GameObject.FindWithTag("Player").GetComponent<PlayerController>();
    }

    public void ShowResultsMenu()          //THE NIGHT ENDS AND RESULTS ARE SHOWN
    {
        StartCoroutine(ResultsMenuLoop());
    }

    IEnumerator ResultsMenuLoop()
    {
        Bus.SetFrozen(true);

        SaveController.Stats NewData = new SaveController.Stats();
        NewData.MoneyGained = GameSettings.SC.GetMoney() - StartMoney;
        NewData.GasUsed = Mathf.RoundToInt(((float)Bus.MaxFuel - (float)Bus.Fuel)/((float)Bus.MaxFuel) * 100f);
        NewData.TopSpeed = Mathf.RoundToInt(TopSpeed * 3f);
        NewData.Fines = Fines;
        GameSettings.SC.AddNightStats(NewData);

        ResultsNightText.text = "Night " + GameSettings.SC.GetNight() + " Results";
        
        MM.SetScreen("Results");

        TabMoney.SetActive(false);
        TabStats.SetActive(true);
        T_GasUsed.text = NewData.GasUsed + " Percent";
        T_TopSpeed.text = NewData.TopSpeed + " KPH";

        while(!ContinuePressed)
        {
            yield return null;
        }
        ContinuePressed = false;

        TabMoney.SetActive(true);
        TabStats.SetActive(false);
        //GAINS
        T_Savings.text = GameSettings.SC.GetMoney() - NewData.MoneyGained + ".00";
        T_Earnings.text = NewData.MoneyGained + ".00";
        //LOSSES
        int TotalLoss = 0;
        T_FineCost.text = NewData.Fines * GameSettings.FineCost + ".00";
        TotalLoss -= NewData.Fines * GameSettings.FineCost;
        T_GasCost.text = Mathf.RoundToInt((NewData.GasUsed/100f) * GameSettings.FullRefillCost) + ".00";
        TotalLoss -= Mathf.RoundToInt((NewData.GasUsed/100f) * GameSettings.FullRefillCost);
        T_Bills.text = GameSettings.Bills + (GameSettings.SC.GetNight() * GameSettings.BillMultiplier) - GameSettings.BillMultiplier+ ".00";
        TotalLoss -= GameSettings.Bills + (GameSettings.SC.GetNight() * GameSettings.BillMultiplier) - GameSettings.BillMultiplier;
        //TOTAL 
        GameSettings.SC.SetMoney(GameSettings.SC.GetMoney() + TotalLoss);
        T_Total.text = GameSettings.SC.GetMoney() + ".00";

        while(!ContinuePressed)
        {
            yield return null;
        }
        ContinuePressed = false;

        ShowStartMenu();
    }

    public void ShowStartMenu()     //Show start menu
    {
        StartCoroutine(StartMenuLoop());
    }

    IEnumerator StartMenuLoop()
    {
        if(GameSettings.SC.GetMoney() < 0)
        {
            MM.SetScreen("Lose");
        }else
        {
            GameSettings.SC.SetNew(false);
            GameSettings.SC.SetNight(GameSettings.SC.GetNight() + 1);
            Debug.Log("GAME SAVED IN SLOT " + GameSettings.SC.SaveIndex);
            
            GameSettings.SC.SaveGame();
            M.UpdateCalendar();

            NewNightText.text = "Night " + GameSettings.SC.GetNight();

            MM.SetScreen("Continue");
            while(!ContinuePressed)
            {
                yield return null;
            }
            ContinuePressed = false;
            MM.SetScreen("HUD");
            
            M.StartNewNight();
        }
    }

    public void Continue()
    {
        ContinuePressed = true;
    }

    public void MakeSound()
    {
        
        ASC.Sound = MenuSound;
        ASC.SetVolume(0.2f);
        ASC.Play();
        
    }
}