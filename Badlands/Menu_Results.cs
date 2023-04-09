using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Menu_Results : MonoBehaviour
{
    public TextMeshProUGUI Status;
    public TextMeshProUGUI Duration;
    public TextMeshProUGUI BlueprintsCollected;
    public TextMeshProUGUI CreditsCollected;
    public TextMeshProUGUI TotalBlueprints;
    public TextMeshProUGUI TotalCredits;
    
    void OnEnable()
    {
        Status.text = Game.RunData.Escaped ? "You Escaped" : "You Died";
        Duration.text = Game.GetTimeFormatted(Game.RunData.EndTime - Game.RunData.StartTime);
        
        if(Game.RunData.Escaped)
        {
            BlueprintsCollected.text = Game.RunData.CollectedBlueprints.ToString() + " / " + Game.StationData.MaxBlueprints.ToString() + " Blueprints";
            TotalBlueprints.text = Game.SaveData.Blueprints.ToString() + " Total Blueprints";
            
            CreditsCollected.text = Game.RunData.CreditsDeposited.ToString() + " Collected Credits";
            TotalCredits.text = Game.SaveData.Credits.ToString() + " Total Credits";
        }else
        {
            BlueprintsCollected.text = "0 Blueprints";
            TotalBlueprints.text = Game.SaveData.Blueprints.ToString() + " Total Blueprints";
            
            CreditsCollected.text = "0 Credits";
            TotalCredits.text = Game.SaveData.Credits.ToString() + " Total Credits";
        }

        Game.RunData = null;
        Game.StationData = null;
    }
}
