using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Menu_World : Menu
{
    Texture2D[] StationMaps;

    public List<Element_Station> Choices;

    public TextMeshProUGUI Info;

    void Awake()
    {
        StationMaps = Resources.LoadAll<Texture2D>("Stations");
    }

    void OnEnable()
    {
        foreach(Element_Station Choice in Choices)
            Choice.Set((Station.Level) Random.Range(0, System.Enum.GetNames(typeof(Station.Level)).Length), StationMaps[Random.Range(0, StationMaps.Length)], this);

        Info.text = "";

        foreach(Raider R in Game.SaveData.Team)
        {
            Info.text += R.Name + "\t\tHired Day " + R.HireDay + "\t\tWins - " + R.SuccessfulRuns + "\n";
            Info.text += "Health - " + R.MaxHealth + "\tShields - " + R.MaxShields + "\tStamina - " + R.MaxStamina + "\n";
        }

        SetScreen("Main");

    }

    public void Launch(Element_Station Chosen)
    {
        Game.StationData = Game.StationLevels[Chosen.Level];

        Debug.Log("Difficulty - " + Chosen.Level);

        UIManager.UI.SetScreen("Loading");
        UIManager.UI.M_Loading.LoadStation(Chosen.Map);
    }
}
