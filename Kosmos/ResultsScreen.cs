using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ResultsScreen : MonoBehaviour
{
    TextMeshProUGUI Friendly;
    TextMeshProUGUI Enemy;
    TextMeshProUGUI Cost;

    public BattleMaker BM;

    List<Ship> LostFriendlies;
    List<Ship> LostEnemies;

    void OnEnable()
    {
        Cost = transform.GetChild(2).GetComponent<TextMeshProUGUI>();
        Friendly = transform.GetChild(3).GetChild(1).GetComponent<TextMeshProUGUI>();
        Enemy = transform.GetChild(4).GetChild(1).GetComponent<TextMeshProUGUI>();

        LostFriendlies = GetLost(BM.OriginalFriendlyShips, ToShip(BM.FriendlyShips));
        LostEnemies = GetLost(BM.OriginalEnemyShips, ToShip(BM.EnemyShips));

        Friendly.text = ShipString(LostFriendlies);
        Enemy.text = ShipString(LostEnemies);

        Cost.text = CostCalculation();
    }

    string CostCalculation()
    {
        string text = "";
        int LostMoney = 0;

        for(int i = 0; i < LostFriendlies.Count; i++)
        {
            LostMoney += LostFriendlies[i].Cost;
        }

        text += "‒" + LostMoney.ToString() + "c \n";

        int GainedMoney = 0;
        GainedMoney = LostEnemies.Count * 25;

        text += "+" + GainedMoney.ToString() + "c \n";

        int MissionBonus = 0;
        if(BM.CurrentMission != null)
            MissionBonus = BM.CurrentMission.CreditGain;

        text += "+" + MissionBonus.ToString() + "c \n";

        text += "\n";

        int Total = 0;
        Total = GainedMoney + MissionBonus - LostMoney;

        if(Total >= 0)
        {
            text += "+" + Total.ToString() + "c \n";
        }else
        {
            text += "‒" + (-Total).ToString() + "c \n";
        }
        
        return text;

        
    }

    List<Ship> ToShip(List<ShipStats> Stats)
    {
        List<Ship> Ships = new List<Ship>();
        for(int i = 0; i < Stats.Count; i++)
        {
            Ships.Add(Stats[i].GetComponent<BattleShipController>().ShipInfo);
        }
        return Ships;
    }

    List<Ship> GetLost(List<Ship> Original, List<Ship> Current)
    {
        List<Ship> Difference = new List<Ship>();       //Blank list to contain missing ships

        for(int i = 0; i < Current.Count; i++)          //For every ship left
        {   
            int CheckValue = CheckShip(Original, Current[i]);
            if(CheckValue != -1)                        //If the original contains the ship
            {
                Original[CheckValue] = null;            //Delete it from the original
            }
        }

        for(int i = 0; i < Original.Count; i++)
        {
            if(Original[i] != null)
            {
                Difference.Add(Original[i]);
            }
        }

        return Difference;
    }

    int CheckShip(List<Ship> Original, Ship Value)
    {
        for(int i = 0; i < Original.Count; i++)
        {
            if(Original[i] != null)
            {
                if(Original[i] == Value)
                    return i;
            }
        }

        return -1;
    }

    string ShipString(List<Ship> Ships)
    {
        List<string> Names = new List<string>();
        List<int> Counts = new List<int>();

        for(int i = 0; i < Ships.Count; i++)
        {
            if(NameIndex(Ships[i].name, Names) == -1)
            {
                Names.Add(Ships[i].name);
                Counts.Add(1);
            }else
            {
                Counts[NameIndex(Ships[i].name, Names)]++;
            }
        }

        string ResultsText = "";

        if(Names.Count == 0)
        {
            ResultsText = "No Ships Lost";
        }

        for(int i = 0; i < Names.Count; i++)
        {
            ResultsText += Counts[i] + "x " + Names[i] + "\n";
        }

        return ResultsText;

    }

    int NameIndex(string Name, List<string> Names)
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
