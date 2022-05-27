using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Manager : MonoBehaviour
{
    [HideInInspector]
    public static Manager M;

    [HideInInspector]
    public Player P;

    public GameObject CargoPlanePrefab;
    public GameObject FighterPlanePrefab;
    public GameObject InfantryPrefab;
    public GameObject TankPrefab;
    public GameObject EquipmentPrefab;
    public GameObject ArtilleryPrefab;
    public GameObject BombPrefab;

    public List<FactionInfo> Factions;

    List<List<string>> FactionNames = new List<List<string>>();

    public List<Unit> TeamOne;
    public List<Unit> TeamTwo;

    void Awake()
    {
        M = this;
    }

    void Start()
    {
        P = GameObject.FindWithTag("Camera").GetComponent<Player>();
        LoadNames();
        StartCoroutine(WaitForGame());
    }

    IEnumerator WaitForGame()
    {
        while(MenuManager.MM.CurrentScreen != "Setup")
            yield return null;

        while(!Game.Setup) 
            yield return null;

        GameModes Chosen = SetupUI.S.Choice;

        while(!Game.PlayerReady)
            yield return null;

        Player.H.Choose(Chosen);
        Game.StartGame(Chosen);

        if(Chosen == GameModes.Defense)
        {
            InvokeRepeating("CreateAttackerCargoPlane", 0f, Game.Defense_EnemyCargoSpawnRate);
            InvokeRepeating("CreateAttackerFighterPlane", Game.Defense_EnemyFighterSpawnRate, Game.Defense_EnemyFighterSpawnRate);
        }else if(Chosen == GameModes.Conquest)
        {
            Game.Conquest_TeamOneTickets = Game.Conquest_StartingTickets;
            Game.Conquest_TeamTwoTickets = Game.Conquest_StartingTickets;
            InvokeRepeating("DrainTickets", 3f, 3f);
        }else if(Chosen == GameModes.Hill)
        {
            Game.Conquest_TeamOneTickets = Game.Conquest_StartingTickets;
            Game.Conquest_TeamTwoTickets = Game.Conquest_StartingTickets;
            InvokeRepeating("DrainTickets", 2f, 2f);
        }
    }

    void DrainTickets()
    {
        if(Game.Conquest_TeamOneTickets > 0 && Game.Conquest_TeamTwoTickets > 0)
        {
            if(Game.GameMode == GameModes.Hill)
            {
                if(Logic.L.Hill.Owner == 1)
                Game.Conquest_TeamTwoTickets--;
                if(Logic.L.Hill.Owner == -1)
                    Game.Conquest_TeamOneTickets--;
            }else
            {
                foreach(Flag F in Logic.L.Conquest)
                {
                    if(F.Owner == 1)
                        Game.Conquest_TeamTwoTickets--;
                    if(F.Owner == -1)
                        Game.Conquest_TeamOneTickets--;
                }
            }
        }
    }

    void LoadNames()
    {
        for(int i = 0; i < Factions.Count; i++)
        {
            TextAsset file = (TextAsset) Resources.Load("Factions/" + Factions[i].Prefix + "/names");
            string[] lines = file.text.Split("\n"[0]);

            List<string> names = new List<string>();
            for(int j = 0; j < lines.Length; j++)
                names.Add(lines[j]);

            FactionNames.Add(names);
        }
    }

    public string GetName(Faction F)
    {
        return FactionNames[(int) F][Random.Range(0, FactionNames[(int) F].Count)];
    }

    public List<Flag> GetFlagsOfTeam(Faction T, bool IncludeSpawns)
    {
        int teamNum = T == Game.TeamOne ? 1 : -1;

        List<Flag> Flags = new List<Flag>();

        if(IncludeSpawns)
            Flags.Add(T == Game.TeamOne ? Logic.L.TeamOneSpawn : Logic.L.TeamTwoSpawn);

        for(int i = 0; i < Logic.L.Conquest.Count; i++)
            if(Logic.L.Conquest[i].Owner == teamNum)
                Flags.Add(Logic.L.Conquest[i]);
                
        return Flags;
    }

    public Vector3 GetConquestObjective(Vector3 Pos, Faction T)
    {
        int teamNum = T == Game.TeamOne ? 1 : -1;
        List<Flag> Flags = new List<Flag>();

        for(int i = 0; i < Logic.L.Conquest.Count; i++)
            if(Logic.L.Conquest[i].Owner != teamNum)
                Flags.Add(Logic.L.Conquest[i]);
            
        return Flags[Random.Range(0, Flags.Count)].transform.position;
            
    }

    public void SpawnAtPoint(Flag F, Faction T)
    {
        Vector2 CirclePoint = Random.insideUnitCircle * (Game.DefenderStayDistance/2f);
        CirclePoint += new Vector2(F.transform.position.x, F.transform.position.z);
        Vector3 RayStart = new Vector3(CirclePoint.x, 200f, CirclePoint.y);

        RaycastHit hit;
        if(Physics.Raycast(RayStart, -Vector3.up, out hit, 400f, Game.IgnoreSelectMask))
        {
            if(hit.transform.gameObject.CompareTag("Landable"))
            {
                GameObject Defender = GameObject.Instantiate(Manager.M.InfantryPrefab, hit.point, Quaternion.identity);
                Unit U = Defender.GetComponent<Unit>();
                U.Team = T;
                Defender.GetComponent<Infantry>().Kit = GetKit(T);
            }else
            {
                SpawnAtPoint(F, T);
            }
        }else
        {
            SpawnAtPoint(F, T);
        }
    }

    public void CreateTank(Faction F)
    {
        Vector3 Spawn = F == Game.TeamOne ? Logic.L.TeamOneVehicleSpawn.transform.position : Logic.L.TeamTwoVehicleSpawn.transform.position;

        Vector2 CirclePoint = Random.insideUnitCircle * (Game.DefenderStayDistance);
        CirclePoint += new Vector2(Spawn.x, Spawn.z);
        Vector3 RayStart = new Vector3(CirclePoint.x, 200f, CirclePoint.y);

        RaycastHit hit;
        if(Physics.Raycast(RayStart, -Vector3.up, out hit, 400f, Game.IgnoreSelectMask))
        {
            if(hit.transform.gameObject.CompareTag("Landable"))
            {
                GameObject Defender = GameObject.Instantiate(Manager.M.TankPrefab, hit.point, Quaternion.LookRotation(-Spawn.normalized, Vector3.up));
                Unit U = Defender.GetComponent<Unit>();
                U.Team = F;
            }else
            {
                CreateTank(F);
            }
        }else
        {
            CreateTank(F);
        }
    }

    void CreateAttackerCargoPlane()
    {
        CreateCargoPlane(Game.TeamTwo, Logic.L.Defense.transform.position, Random.Range(Game.FarCargoDropStart, Game.FarCargoDropEnd), 100f, Mathf.FloorToInt((Time.time - Game.Defense_StartTime)/Game.Defense_EnemyIncreaseInterval) + Game.Defense_EnemiesPerDrop);
    }

    void CreateCargoPlane(Faction F, Vector3 Pos, float DropDistance, float OffDistance, int SpawnAmount)
    {
        Vector2 CirclePoint = (Random.insideUnitCircle.normalized * Game.PlaneSpawnDistance) + new Vector2(Pos.x, Pos.z);

        Vector2 Offset = Random.insideUnitCircle.normalized * OffDistance;

        Vector3 FlyDirection = ((Pos + new Vector3(Offset.x, 0f, Offset.y)) - new Vector3(CirclePoint.x, 0f, CirclePoint.y)).normalized;
        float FlyHeight = Random.Range(Game.DropHeightMin, Game.DropHeightMax);

        GameObject CargoPlane = GameObject.Instantiate(CargoPlanePrefab, new Vector3(CirclePoint.x, FlyHeight, CirclePoint.y), Quaternion.identity);
        CargoPlane.GetComponent<Plane>().DropTarget = CargoPlane.transform.position + (FlyDirection * (Game.PlaneSpawnDistance - DropDistance));

        CargoPlane.GetComponent<Plane>().HeadTarget = CargoPlane.transform.position + (FlyDirection * (Game.PlaneSpawnDistance * 2f));
        CargoPlane.transform.LookAt(CargoPlane.transform.position + (FlyDirection * (Game.PlaneSpawnDistance * 2f)));
        CargoPlane.GetComponent<Plane>().SpawnAmount = SpawnAmount;

        CargoPlane.GetComponent<Unit>().Team = F;

        if(Game.DEBUG_ShowDropPoint)
            Debug.DrawLine(new Vector3(CirclePoint.x, FlyHeight, CirclePoint.y),  CargoPlane.transform.position + (FlyDirection * (Game.PlaneSpawnDistance - DropDistance)), Color.red, 60f);
    }

    public void BuyFighterPlane(Faction F)
    {
        if(Game.Defense_Money >= Game.Defense_FighterPlaneCost)
        {
            Game.Defense_Money -= Game.Defense_FighterPlaneCost;
            CreateFighterPlane(F);
        }
    }

    void CreateAttackerFighterPlane()
    {
        CreateFighterPlane(Game.TeamTwo);
    }

    public void CreateFighterPlane(Faction F)
    {
        Vector2 CirclePoint = Random.insideUnitCircle.normalized * Game.PlaneSpawnDistance;
        CirclePoint += new Vector2(Logic.L.Defense.transform.position.x, Logic.L.Defense.transform.position.z);

        float FlyHeight = Random.Range(Game.DropHeightMin, Game.DropHeightMax);
        Vector3 SpawnPoint = new Vector3(CirclePoint.x, FlyHeight, CirclePoint.y);

        Vector3 FlyDirection = (Logic.L.Defense.transform.position - SpawnPoint).normalized;

        GameObject FighterPlane = GameObject.Instantiate(FighterPlanePrefab, SpawnPoint, Quaternion.identity);

        FighterPlane.transform.LookAt(new Vector3(0f, FlyHeight, 0f) + (FlyDirection * Game.PlaneSpawnDistance));
        FighterPlane.GetComponent<Unit>().Team = F;
    }

    public void BuyCargoPlane(Vector3 Pos)
    {
        if(Game.Defense_Money >= Game.Defense_CargoPlaneCost)
        {
            Game.Defense_Money -= Game.Defense_CargoPlaneCost;
            CreateCargoPlane(Game.TeamOne, Pos, Random.Range(Game.CloseCargoDropStart, Game.CloseCargoDropEnd), 5f, Game.Defense_AlliesPerDrop);
        }
    }

    public void BuyEquipment(Vector3 Pos)
    {
        if(Game.Defense_Money >= Game.Defense_EquipmentCost)
        {
            Game.Defense_Money -= Game.Defense_EquipmentCost;
            CreateEquipment(Pos);
        }
    }

    void CreateEquipment(Vector3 Pos)
    {
        GameObject Equipment = GameObject.Instantiate(EquipmentPrefab, Pos, Quaternion.identity);
    }

    public void BuyArtillery(Vector3 Pos)
    {
        if(Game.Defense_Money >= Game.Defense_ArtilleryCost)
        {
            Game.Defense_Money -= Game.Defense_ArtilleryCost;
            CreateArtillery(Pos);
        }
    }

    void CreateArtillery(Vector3 Pos)
    {
        GameObject Artillery = GameObject.Instantiate(ArtilleryPrefab, Pos + new Vector3(0f, 500f, 0f), Quaternion.identity);
    }

    public InfantryKit GetKit(Faction Team)
    {
        float Chance = Random.value;

        if(Chance < 0.5f)                               //Rifle     50%
            return Factions[(int) Team].PossibleKits[0];
        if(Chance < 0.7f)                               //Auto      20% 
            return Factions[(int) Team].PossibleKits[1];
        if(Chance < 0.8f)                               //Gunner    10%
            return Factions[(int) Team].PossibleKits[2];
        if(Chance < 0.9f)                               //Sniper    10%
            return Factions[(int) Team].PossibleKits[3];
        if(Chance < 1.0f)                               //Anti      10%
            return Factions[(int) Team].PossibleKits[4];
        
        return Factions[(int) Team].PossibleKits[0];
    }

    /*

    void Update()
    {
        for(int i = 0; i < 5; i++)
            VisualizePath();
    }

    void VisualizePath()
    {
        Vector2 CirclePoint = Random.insideUnitCircle.normalized * Game.PlaneSpawnDistance;
        CirclePoint += new Vector2(Base.position.x, Base.position.z);

        Vector2 Offset = Random.insideUnitCircle.normalized * 100f;

        Offset = Random.insideUnitCircle.normalized * 5f;

        Vector3 DropPoint = Base.position + new Vector3(Offset.x, 0f, Offset.y);

        Vector3 InDirection = (DropPoint - new Vector3(CirclePoint.x, 0f, CirclePoint.y)).normalized;

        float DropAt = Random.Range(Game.DropHeightMin, Game.DropHeightMax);

        float DropDistance = Random.Range(Game.EnemyDropStart, Game.EnemyDropEnd);

        DropDistance = 5f;

        Debug.DrawRay(new Vector3(CirclePoint.x, DropAt, CirclePoint.y),  InDirection * (Game.PlaneSpawnDistance - DropDistance), Color.red, 2f);
    }

    */
}
