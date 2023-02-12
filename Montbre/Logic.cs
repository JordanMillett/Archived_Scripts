using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Logic : MonoBehaviour
{
    [HideInInspector]
    public static Logic L;

    [HideInInspector]
    public Camera C;
    
    [HideInInspector]
    public Unit LastKnownTarget;

    public Flag TeamOneSpawn;
    public Flag TeamTwoSpawn;
    public Transform TeamOneVehicleSpawn;
    public Transform TeamTwoVehicleSpawn;
    public Flag Hill;
    public Flag Defense;
    public List<Flag> Conquest;

    NavMeshPath PathToGoal;
    NavMeshQueryFilter NavInfo;
    Vector3 LastTried = Vector3.zero;

    void Awake()
    {
        L = this;

        PathToGoal = new NavMeshPath();
        NavInfo = new NavMeshQueryFilter();
        NavInfo.areaMask = 1;
        NavInfo.agentTypeID = 0;
    }

    void Start()
    {
        C = GameObject.FindWithTag("Camera").GetComponent<Camera>();
        HideAll();
    }

    public void HideAll()
    {
        TeamOneSpawn.HideEffect();
        TeamTwoSpawn.HideEffect();
        Hill.HideEffect();
        Defense.HideEffect();

        foreach(Flag F in Conquest)
            F.HideEffect();
    }

    public void Show(GameModes GM)
    {
        HideAll();

        if(GM == GameModes.Defense)
        {
            Defense.ShowEffect();
        }else if(GM == GameModes.Conquest)
        {
            TeamOneSpawn.ShowEffect();
            TeamTwoSpawn.ShowEffect();

            foreach(Flag F in Conquest)
                F.ShowEffect();
        }else if(GM == GameModes.Hill)
        {
            TeamOneSpawn.ShowEffect();
            TeamTwoSpawn.ShowEffect();
            Hill.ShowEffect();
        }
    }

    public void Confirm()
    {
        switch(Game.GameMode)
        {
            case GameModes.Defense : 
                Defense.HideRing(); 
                Defense.UpdateFlag(1); 
                StartCoroutine(Load_Defense()); 
            break;
            case GameModes.Conquest :
                TeamOneSpawn.Influence = Game.FlipSpawns ? -100 : 100;
                TeamTwoSpawn.Influence = Game.FlipSpawns ? 100 : -100;
                TeamOneSpawn.Change();
                TeamTwoSpawn.Change();
                TeamOneSpawn.UpdateFlag(Game.FlipSpawns ? -1 : 1); 
                TeamTwoSpawn.UpdateFlag(Game.FlipSpawns ? -1 : 1); 
                StartCoroutine(Load_Conquest()); 
            break;
            case GameModes.Hill : 
                Hill.HideRing(); 
                TeamOneSpawn.Influence = Game.FlipSpawns ? -100 : 100;
                TeamTwoSpawn.Influence = Game.FlipSpawns ? 100 : -100;
                TeamOneSpawn.Change();
                TeamTwoSpawn.Change();
                TeamOneSpawn.UpdateFlag(Game.FlipSpawns ? -1 : 1); 
                TeamTwoSpawn.UpdateFlag(Game.FlipSpawns ? -1 : 1); 
                StartCoroutine(Load_Conquest()); 
            break;
        }
        Game.Setup = true;
    }

    public void Preview_Defense()
    {
        RaycastHit hit;
        if(Physics.Raycast(C.ScreenPointToRay(Input.mousePosition), out hit, 2000f, Game.IgnoreSelectMask))
        {
            if(hit.transform.gameObject.CompareTag("Landable"))
            {
                LastTried = hit.point;
                NavMesh.CalculatePath(Vector3.zero, LastTried, NavInfo, PathToGoal);
            }
        }
        if(PathToGoal.status == NavMeshPathStatus.PathComplete)
            Defense.transform.position = LastTried;
    }

    public void Preview_Hill()
    {
        RaycastHit hit;
        if(Physics.Raycast(C.ScreenPointToRay(Input.mousePosition), out hit, 2000f, Game.IgnoreSelectMask))
        {
            if(hit.transform.gameObject.CompareTag("Landable"))
            {
                LastTried = hit.point;
                NavMesh.CalculatePath(Vector3.zero, LastTried, NavInfo, PathToGoal);
            }
        }
        if(PathToGoal.status == NavMeshPathStatus.PathComplete)
            Hill.transform.position = LastTried;
    }

    IEnumerator Load_Defense()
    {
        int spawned = 0;
        while(spawned < Game.Defense_StartingAllies)
        {
            Manager.M.SpawnInfantry(Defense, Game.TeamOne, false);
            spawned++;
            yield return null;
        }
    }

    IEnumerator Load_Conquest()
    {
        int spawned = 0;
        while(spawned < Game.Conquest_HeavyTanksPerTeam)
        {
            Manager.M.SpawnTank(Game.TeamOne, true);
            Manager.M.SpawnTank(Game.TeamTwo, true);
            spawned++;
            yield return null;
        }
        
        spawned = 0;
        while(spawned < Game.Conquest_LightTanksPerTeam)
        {
            Manager.M.SpawnTank(Game.TeamOne, false);
            Manager.M.SpawnTank(Game.TeamTwo, false);
            spawned++;
            yield return null;
        }

        spawned = 0;
        while(spawned < Game.Conquest_TeamSize)
        {
            Manager.M.SpawnInfantry(Game.FlipSpawns ? TeamTwoSpawn : TeamOneSpawn, Game.TeamOne, false);
            Manager.M.SpawnInfantry(Game.FlipSpawns ? TeamOneSpawn : TeamTwoSpawn, Game.TeamTwo, false);
            spawned++;
            yield return null;
        }

        spawned = 0;
        while(spawned < Game.Conquest_PlanesPerTeam)
        {
            Manager.M.SpawnFighterPlane(Game.TeamOne);
            Manager.M.SpawnFighterPlane(Game.TeamTwo);
            spawned++;
            yield return null;
        }
    }
}
