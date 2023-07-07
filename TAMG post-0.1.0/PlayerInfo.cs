using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerInfo
{
    public string Name = "NULL";
    public string SteamID = "NULL";

    public int Balance = 100;
    public int TotalDeliveries = 0;
    public int TotalScore = 0;

    public string CalculateGrade()    //use for scoreboard
    {
        if(TotalDeliveries == 0)
            return "A";

        float percent = (float) TotalScore / (float) TotalDeliveries;

        if(percent >= 0.90f)
            return "A";
        if(percent >= 0.80f)
            return "B";
        if(percent >= 0.70f)
            return "C";
        if(percent >= 0.60f)
            return "D";
        
        return "F";
    }
}
