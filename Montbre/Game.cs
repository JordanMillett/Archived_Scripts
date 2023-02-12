using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    public static LayerMask IgnoreSelectMask = ~LayerMask.GetMask("Select", "MapView");
    public static LayerMask UnitOnlyMask = LayerMask.GetMask("Unit");
    public static LayerMask DamageOnlyMask = LayerMask.GetMask("Damage", "Default");

    public static Color32 FriendlyColor = new Color32(199, 245, 142, 255);
    public static Color32 NeutralColor = new Color32(180, 180, 180, 255);
    public static Color32 EnemyColor = new Color32(255, 0, 0, 255);

    public static Faction TeamOne = Faction.SovietUnion;
    public static Faction TeamTwo = Faction.Germany;

    //Gamemodes
    public static GameModes GameMode = GameModes.Conquest;
    public static bool Setup = false;
    public static bool Started = false;
    //public static bool PlayerReady = false;

    public static bool FlipSpawns = false;
    public static bool RandomizeLoadouts = false;

    public static int   Defense_Allies = 0;
    public static int   Defense_Enemies = 0;
    public static int   Defense_AllyPlanes = 0;
    public static int   Defense_EnemyPlanes = 0;

    public static int   Defense_StartingAllies = 16;
    public static int   Defense_AlliesPerDrop = 5;

    public static int   Defense_Money = 400;//normally 0
    public static int   Defense_InfantryKillBonus = 10;
    public static int   Defense_PlaneKillBonus = 40;
    public static int   Defense_TankKillBonus = 60;

    public static int   Defense_CargoPlaneCost = 100;
    public static int   Defense_FighterPlaneCost = 200;
    public static int   Defense_EquipmentCost = 30;
    public static int   Defense_ArtilleryCost = 40;

    public static int   Defense_EnemiesPerDrop = 3;      
    public static float Defense_EnemyFighterSpawnRate = 160f;
    public static float Defense_EnemyCargoSpawnRate = 20f;   
    public static float Defense_EnemyIncreaseInterval = 120f;

    public static float Defense_UnitFollowDistance = 8f;
    public static float Defense_StartTime = 0;
    public static float Defense_EndTime = 0;

    public static int   Conquest_TeamSize = 40;
    public static int   Conquest_PlanesPerTeam = 4;
    public static int   Conquest_HeavyTanksPerTeam = 2;
    public static int   Conquest_LightTanksPerTeam = 2;
    public static int   Conquest_StartingTickets = 500;
    
    public static int   Conquest_InfantryRespawnTime = 10;
    public static int   Conquest_HeavyTankRespawnTime = 25;
    public static int   Conquest_LightTankRespawnTime = 15;
    public static int   Conquest_PlaneRespawnTime = 10;

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

    public static float AttackerPushDistance = 10f;
    public static float DefenderStayDistance = 50f;

    public static float DropHeightMin = 100f;
    public static float DropHeightMax = 300f;
    public static float CloseCargoDropStart = 5f; 
    public static float CloseCargoDropEnd = 5f;
    public static float FarCargoDropStart = 400f; 
    public static float FarCargoDropEnd = 300f;

    public static void StartGame()
    {
        if(GameMode == GameModes.Defense)
            Defense_StartTime = Time.time;

        Game.Started = true;
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
        Defense_Money = 400;

        Defense_EnemiesPerDrop = 4;

        Defense_StartTime = 0;
        Defense_EndTime = 0;

        Setup = false;
        Started = false;
        //PlayerReady = false;
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
