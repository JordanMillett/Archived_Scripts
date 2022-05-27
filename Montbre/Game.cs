using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class FactionInfo
{
    public Faction Name;
    public string Prefix;
    public GameObject Hat;

    public Texture2D Flag;
    public Texture2D Symbol;
    public Color BeltColor;
    public Color ButtonColor;
    public Texture2D Shirt;
    public List<InfantryKit> PossibleKits;
}

public enum Faction
{
    Germany,
    SovietUnion
};

public enum GameModes
{
    Defense,
    Conquest,
    Hill
};

public enum Stances
{
    Defend,
    Attack
};

public static class Game
{
    //DEBUG
    public static bool DEBUG_ShowExplosionSizes = false;
    public static bool DEBUG_ShowAimDirection = false;
    public static bool DEBUG_ShowDropPoint = false;
    public static bool DEBUG_ShowCoverPoint = false;

    public static LayerMask IgnoreSelectMask = ~LayerMask.GetMask("Select");
    public static LayerMask UnitOnlyMask = LayerMask.GetMask("Unit");
    public static LayerMask DamageOnlyMask = LayerMask.GetMask("Damage", "Default");

    public static Color32 FriendlyColor = new Color32(199, 245, 142, 255);
    public static Color32 NeutralColor = new Color32(180, 180, 180, 255);
    public static Color32 EnemyColor = new Color32(255, 0, 0, 255);

    public static Faction TeamOne = Faction.SovietUnion;
    public static Faction TeamTwo = Faction.Germany;

    //Gamemodes
    public static GameModes GameMode = GameModes.Defense;
    public static bool Setup = false;
    public static bool PlayerReady = false;

    public static int   Defense_Allies = 0;
    public static int   Defense_Enemies = 0;
    public static int   Defense_AllyPlanes = 0;
    public static int   Defense_EnemyPlanes = 0;

    public static int   Defense_StartingAllies = 20;
    public static int   Defense_AlliesPerDrop = 5;

    public static int   Defense_Money = 1000;//normally 0
    public static int   Defense_InfantryKillBonus = 10;
    public static int   Defense_PlaneKillBonus = 40;
    public static int   Defense_TankKillBonus = 60;

    public static int   Defense_CargoPlaneCost = 100;
    public static int   Defense_FighterPlaneCost = 200;
    public static int   Defense_EquipmentCost = 30;
    public static int   Defense_ArtilleryCost = 40;

    public static int   Defense_EnemiesPerDrop = 4;      
    public static float Defense_EnemyFighterSpawnRate = 160f;
    public static float Defense_EnemyCargoSpawnRate = 30f;   
    public static float Defense_EnemyIncreaseInterval = 180f;

    public static float Defense_UnitFollowDistance = 8f;
    public static float Defense_StartTime = 0;
    public static float Defense_EndTime = 0;

    public static int   Conquest_TeamSize = 50;
    public static int   Conquest_PlanesPerTeam = 3;
    public static int   Conquest_TanksPerTeam = 3;
    public static int   Conquest_StartingTickets = 500;

    public static int   Conquest_TeamOneTickets = 500;
    public static int   Conquest_TeamTwoTickets = 500;

    //MOVE TO OPTIONS OR SOMETHING LATER
    public static bool MouseInputLocked = false;
    public static float mouseModifier = 100f;

    public static float PossibleAccuracy = 97f;
    public static float DetectDistance = 250f;
    
    public static float PlaneSpawnDistance = 2000f;

    public static float BaseFOV = 70f;
    public static float SprintingFOVChange = 10f;
    public static float InteractRange = 1500f;

    public static float AttackerPushDistance = 20f;
    public static float DefenderStayDistance = 50f;

    public static float DropHeightMin = 100f;
    public static float DropHeightMax = 300f;
    public static float CloseCargoDropStart = 5f; 
    public static float CloseCargoDropEnd = 5f;
    public static float FarCargoDropStart = 400f; 
    public static float FarCargoDropEnd = 300f;

    public static void StartGame(GameModes GM)
    {
        GameMode = GM;

        if(GameMode == GameModes.Defense)
            Defense_StartTime = Time.time;
    }

    public static void EndGame()
    {
        if(GameMode == GameModes.Defense)
            Defense_EndTime = Time.time;
    }

    public static void ResetGame()
    {
        Defense_Allies = 0;
        Defense_Enemies = 0;
        Defense_AllyPlanes = 0;
        Defense_EnemyPlanes = 0;
        Defense_Money = 0;

        Defense_EnemiesPerDrop = 4;

        Defense_StartTime = 0;
        Defense_EndTime = 0;

        Setup = false;
        PlayerReady = false;
    }

    public static string GetTimeFormatted(float Time)
    {
        string Formatted = "0:00";

        int Minutes = (int)Mathf.Floor((Time)/60);

        int Seconds = (int)Mathf.Floor(((Time) % 60));
        string SecondsSpacer = "";
        if(Seconds < 10)
            SecondsSpacer = "0";

        Formatted = Minutes + ":" + SecondsSpacer + Seconds;

        return Formatted;
    }

    public static bool IsInaccurate(Faction F)
    {
        if(F == TeamOne)
            return false;
        else
            return true;
    }
}
