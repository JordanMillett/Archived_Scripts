using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SetupUI : MonoBehaviour
{
    [System.Serializable]
    public struct Menu
    {
	    public GameObject Screen;
    }

    [HideInInspector]
    public static SetupUI S;

    public List<Menu> MenuScreens;
    public int CurrentScreen = 0;
    
    GameModes Choice = GameModes.Conquest;

    public TextMeshProUGUI Chosen;
    public TextMeshProUGUI Info;

    public DropDown YourTeam;
    public DropDown EnemyTeam;

    public DropDown Defenders;
    public DropDown Attackers;

    public IntSlider StartingDefenders;

    public IntSlider StartingTickets;
    public IntSlider NumInfantry;
    public IntSlider NumPlanes;
    public IntSlider NumHeavyTanks;
    public IntSlider NumLightTanks;

    public Checkbox FlipSpawns;
    public Checkbox RandomizeLoadouts;

    int ModeIndex = 1;

    void Awake()
    {
        S = this;
    }

    void LoadConfig()
    {
        YourTeam.Initialize(System.Enum.GetNames(typeof(Faction)), (int) Game.TeamOne);
        EnemyTeam.Initialize(System.Enum.GetNames(typeof(Faction)), (int) Game.TeamTwo);
        Defenders.Initialize(System.Enum.GetNames(typeof(Faction)), (int) Game.TeamOne);
        Attackers.Initialize(System.Enum.GetNames(typeof(Faction)), (int) Game.TeamTwo);

        StartingDefenders.Initialize(Game.Defense_StartingAllies);

        StartingTickets.Initialize(Game.Conquest_StartingTickets);
        NumInfantry.Initialize(Game.Conquest_TeamSize);
        NumPlanes.Initialize(Game.Conquest_PlanesPerTeam);
        NumHeavyTanks.Initialize(Game.Conquest_HeavyTanksPerTeam);
        NumLightTanks.Initialize(Game.Conquest_LightTanksPerTeam);
        
        FlipSpawns.Initialize(Game.FlipSpawns);
        RandomizeLoadouts.Initialize(Game.RandomizeLoadouts);
    }

    void ApplyConfig()
    {
        if(Game.GameMode != GameModes.Defense)
        {
            Game.TeamOne = (Faction) YourTeam.CurrentIndex;
            Game.TeamTwo = (Faction) EnemyTeam.CurrentIndex;
        }else
        {
            Game.TeamOne = (Faction) Defenders.CurrentIndex;
            Game.TeamTwo = (Faction) Attackers.CurrentIndex;
        }

        Game.Defense_StartingAllies = StartingDefenders.Value;

        Game.Conquest_StartingTickets = StartingTickets.Value;
        Game.Conquest_TeamSize = NumInfantry.Value;
        Game.Conquest_PlanesPerTeam = NumPlanes.Value;
        Game.Conquest_HeavyTanksPerTeam = NumHeavyTanks.Value;
        Game.Conquest_LightTanksPerTeam = NumLightTanks.Value;
        
        Game.FlipSpawns = FlipSpawns.isOn;
        Game.RandomizeLoadouts = RandomizeLoadouts.isOn;
    }

    void Start()
    {
        LoadConfig();
    }

    void OnEnable()
    {
        SetScreen(0);
        SetChoice(Choice);
    }

    void OnDisable()
    {
        if(!Game.Setup)
            Logic.L.HideAll();
    }

    void Update()
    {
        if(Input.GetMouseButtonDown(1))
        {
            if(Choice == GameModes.Defense)
                Logic.L.Preview_Defense();
            else if(Choice == GameModes.Hill)
                Logic.L.Preview_Hill();
        }
        
        if(YourTeam.CurrentIndex == EnemyTeam.CurrentIndex)
            for(int i = 0; i < System.Enum.GetValues(typeof(Faction)).Length; i++)
                if(YourTeam.CurrentIndex != i)
                    EnemyTeam.Set(i);
        
        if(Defenders.CurrentIndex == Attackers.CurrentIndex)
            for(int i = 0; i < System.Enum.GetValues(typeof(Faction)).Length; i++)
                if(Defenders.CurrentIndex != i)
                    Attackers.Set(i);
    }

    public void SetChoice(GameModes GM)
    {
        Choice = GM;
        Logic.L.Show(Choice);
        Show(Choice);
    }

    public void Continue()
    {
        SetScreen((int) Choice + 1);
    }

    public void Play()
    {
        Game.GameMode = Choice;
        ApplyConfig();
        Logic.L.Confirm();
    }

    public void Show(GameModes GM)
    {
        switch(GM)
        {
            case GameModes.Defense : Chosen.text = "Defense"; 
            Info.text = "Survive increasingly difficult waves of infantry.\n(Right click to change defend location)";
            break;
            case GameModes.Conquest : Chosen.text = "Conquest";
            Info.text = "Infantry, Tanks, and Planes all fighting over flags. Each controlled flag and enemy kill will drain the other team's tickets.";
            break;
            case GameModes.Hill : Chosen.text = "King of the Hill"; 
            Info.text = "Infantry, Tanks, and Planes all fighting over one flag. The owner of the hill will drain the other team's tickets and so will getting kills.\n(Right click to change hill location)";
            break;
        }
    }

    public void SetScreen(int Index)
    {
        CurrentScreen = Index;

        for(int i = 0; i < MenuScreens.Count; i++)
            MenuScreens[i].Screen.SetActive(false);

        for(int i = 0; i < MenuScreens.Count; i++)
        {
            if(i == Index)
                MenuScreens[i].Screen.SetActive(true);
        }
    }

    public void NextMode()
    {
        if(ModeIndex < System.Enum.GetValues(typeof(GameModes)).Length - 1)
            ModeIndex++;
        else
            ModeIndex = 0;

        SetChoice((GameModes) ModeIndex);
    }

    public void LastMode()
    {
        if(ModeIndex > 0)
            ModeIndex--;
        else
            ModeIndex = System.Enum.GetValues(typeof(GameModes)).Length - 1;

        SetChoice((GameModes) ModeIndex);
    }
}
