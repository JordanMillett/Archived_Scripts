using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class Manager : MonoBehaviour
{
    [HideInInspector]
    public static Manager M;

    [HideInInspector]
    public Player P;

    public GameObject EquipmentPrefab;
    public GameObject ArtilleryPrefab;
    public GameObject BombPrefab;
    public GameObject FlarePrefab;

    public List<FactionInfo> Factions;

    List<List<string>> FactionNames = new List<List<string>>();

    public List<Unit> TeamOne;
    public List<Unit> TeamTwo;

    public InfantryKit RandomKit;
    public Material DestroyedMaterial;

    public GameObject HitNormal;
    public GameObject HitPerson;
    public GameObject HitMetal;
    public GameObject HitExplode;
    public GameObject HitExplodeAir;

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
            
        P.Href.Choose(Game.GameMode);
        
        Game.StartGame();

        if(Game.GameMode == GameModes.Defense)
        {
            InvokeRepeating("CreateAttackerCargoPlane", 0f, Game.Defense_EnemyCargoSpawnRate);
            InvokeRepeating("CreateAttackerFighterPlane", Game.Defense_EnemyFighterSpawnRate, Game.Defense_EnemyFighterSpawnRate);
        }else if(Game.GameMode == GameModes.Conquest)
        {
            Game.Conquest_TeamOneTickets = Game.Conquest_StartingTickets;
            Game.Conquest_TeamTwoTickets = Game.Conquest_StartingTickets;
            InvokeRepeating("DrainTickets", 6f, 6f);
        }else if(Game.GameMode == GameModes.Hill)
        {
            Game.Conquest_TeamOneTickets = Game.Conquest_StartingTickets;
            Game.Conquest_TeamTwoTickets = Game.Conquest_StartingTickets;
            InvokeRepeating("DrainTickets", 3f, 3f);
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

    public string GetName(Faction Team)
    {
        return FactionNames[(int) Team][Random.Range(0, FactionNames[(int) Team].Count)];
    }

    public List<Flag> GetFlagsOfTeam(Faction Team, bool IncludeSpawns) //for infantry respawns
    {
        int teamNum = Team == Game.TeamOne ? 1 : -1;

        List<Flag> Flags = new List<Flag>();

        if(IncludeSpawns)
        {
            if(!Game.FlipSpawns)
                Flags.Add(Team == Game.TeamOne ? Logic.L.TeamOneSpawn : Logic.L.TeamTwoSpawn);
            else
                Flags.Add(Team == Game.TeamOne ? Logic.L.TeamTwoSpawn : Logic.L.TeamOneSpawn);
        }

        for(int i = 0; i < Logic.L.Conquest.Count; i++)
            if(Logic.L.Conquest[i].Owner == teamNum && !Logic.L.Conquest[i].Contested)
                Flags.Add(Logic.L.Conquest[i]);
                
        return Flags;
    }

    public Flag GetConquestFlag(Vector3 Pos, Faction Team)
    {
        int teamNum = Team == Game.TeamOne ? 1 : -1;
        List<Flag> Flags = new List<Flag>();

        for(int i = 0; i < Logic.L.Conquest.Count; i++)
            if(Logic.L.Conquest[i].Owner != teamNum)
                Flags.Add(Logic.L.Conquest[i]);

        return Flags[Random.Range(0, Flags.Count)];
            
    }
    
    public void DelaySpawnInfantry(Flag F, Faction Team, bool Close)
    {
        StartCoroutine(SpawnInfantryRoutine(F, Team, Close));
    }
    
    IEnumerator SpawnInfantryRoutine(Flag F, Faction Team, bool Close)
    {
        yield return new WaitForSeconds(Game.Conquest_InfantryRespawnTime);
        SpawnInfantry(F, Team, Close);
    }

    public void SpawnInfantry(Flag F, Faction Team, bool Close)
    {
        Vector2 CirclePoint = Random.insideUnitCircle * (Game.DefenderStayDistance/2f);
        if(Close)
            CirclePoint = Random.insideUnitCircle * (Game.DefenderStayDistance/4f);
            
        CirclePoint += new Vector2(F.transform.position.x, F.transform.position.z);
        Vector3 RayStart = new Vector3(CirclePoint.x, 200f, CirclePoint.y);

        RaycastHit hit;
        if(Physics.Raycast(RayStart, -Vector3.up, out hit, 400f, Game.IgnoreSelectMask))
        {
            if(hit.transform.gameObject.CompareTag("Landable"))
            {
                GameObject Defender = GameObject.Instantiate(Factions[(int) Team].InfantryPrefab, hit.point, Quaternion.LookRotation(new Vector3(-hit.point.x, 0f, -hit.point.z), Vector3.up));
                Unit U = Defender.GetComponent<Unit>();
                U.Team = Team;
                Defender.GetComponent<Infantry>().Kit = GetKit(Team);
            }else
            {
                SpawnInfantry(F, Team, Close);
            }
        }else
        {
            SpawnInfantry(F, Team, Close);
        }
    }

    public void DelaySpawnTank(Faction Team, bool Heavy)
    {
        StartCoroutine(SpawnTankRoutine(Team, Heavy));
    }
    
    IEnumerator SpawnTankRoutine(Faction Team, bool Heavy)
    {
        yield return new WaitForSeconds(Heavy ? Game.Conquest_HeavyTankRespawnTime : Game.Conquest_LightTankRespawnTime);
        SpawnTank(Team, Heavy);
    }

    public void SpawnTank(Faction Team, bool Heavy)
    {
        Vector3 Spawn = Vector3.zero;

        if(Game.FlipSpawns)
        {
            Spawn = Team != Game.TeamOne ? Logic.L.TeamOneVehicleSpawn.transform.position : Logic.L.TeamTwoVehicleSpawn.transform.position;
        }else
        {
            Spawn = Team == Game.TeamOne ? Logic.L.TeamOneVehicleSpawn.transform.position : Logic.L.TeamTwoVehicleSpawn.transform.position;
        }

        Vector2 CirclePoint = Random.insideUnitCircle * (Game.DefenderStayDistance);
        CirclePoint += new Vector2(Spawn.x, Spawn.z);
        Vector3 RayStart = new Vector3(CirclePoint.x, 200f, CirclePoint.y);

        RaycastHit hit;
        if(Physics.Raycast(RayStart, -Vector3.up, out hit, 400f, Game.IgnoreSelectMask))
        {
            if(hit.transform.gameObject.CompareTag("Landable"))
            {
                GameObject Defender = GameObject.Instantiate(Heavy ? Factions[(int) Team].HeavyTankPrefab : Factions[(int) Team].LightTankPrefab, hit.point, Quaternion.LookRotation(-Spawn.normalized, Vector3.up));
                Unit U = Defender.GetComponent<Unit>();
                U.Team = Team;
            }else
            {
                SpawnTank(Team, Heavy);
            }
        }else
        {
            SpawnTank(Team, Heavy);
        }
    }
    
    public void DelaySpawnFighterPlane(Faction Team)
    {
        StartCoroutine(SpawnFighterPlaneRoutine(Team));
    }
    
    IEnumerator SpawnFighterPlaneRoutine(Faction Team)
    {
        yield return new WaitForSeconds(Game.Conquest_PlaneRespawnTime);
        SpawnFighterPlane(Team);
    }
    
    public void SpawnFighterPlane(Faction Team)
    {
        Vector2 CirclePoint = Random.insideUnitCircle.normalized * Game.PlaneSpawnDistance;
        CirclePoint += new Vector2(Logic.L.Defense.transform.position.x, Logic.L.Defense.transform.position.z);

        float FlyHeight = Random.Range(Game.DropHeightMin, Game.DropHeightMax);
        Vector3 SpawnPoint = new Vector3(CirclePoint.x, FlyHeight, CirclePoint.y);

        Vector3 FlyDirection = (Logic.L.Defense.transform.position - SpawnPoint).normalized;

        GameObject FighterPlane = GameObject.Instantiate(Factions[(int) Team].FighterPlanePrefab, SpawnPoint, Quaternion.identity);

        FighterPlane.transform.LookAt(new Vector3(0f, FlyHeight, 0f) + (FlyDirection * Game.PlaneSpawnDistance));
        FighterPlane.GetComponent<Unit>().Team = Team;
    }

    void SpawnAttackerCargoPlane()
    {
        SpawnCargoPlane(Game.TeamTwo, Logic.L.Defense.transform.position, Random.Range(Game.FarCargoDropStart, Game.FarCargoDropEnd), 100f, Mathf.FloorToInt((Time.time - Game.Defense_StartTime)/Game.Defense_EnemyIncreaseInterval) + Game.Defense_EnemiesPerDrop);
    }

    void SpawnCargoPlane(Faction Team, Vector3 Pos, float DropDistance, float OffDistance, int SpawnAmount)
    {
        Vector2 CirclePoint = (Random.insideUnitCircle.normalized * Game.PlaneSpawnDistance) + new Vector2(Pos.x, Pos.z);

        Vector2 Offset = Random.insideUnitCircle.normalized * OffDistance;

        Vector3 FlyDirection = ((Pos + new Vector3(Offset.x, 0f, Offset.y)) - new Vector3(CirclePoint.x, 0f, CirclePoint.y)).normalized;
        float FlyHeight = Random.Range(Game.DropHeightMin, Game.DropHeightMax);

        GameObject CargoPlane = GameObject.Instantiate(Factions[(int) Team].CargoPlanePrefab, new Vector3(CirclePoint.x, FlyHeight, CirclePoint.y), Quaternion.identity);
        CargoPlane.GetComponent<Plane>().DropTarget = CargoPlane.transform.position + (FlyDirection * (Game.PlaneSpawnDistance - DropDistance));

        CargoPlane.GetComponent<Plane>().HeadTarget = CargoPlane.transform.position + (FlyDirection * (Game.PlaneSpawnDistance * 2f));
        CargoPlane.transform.LookAt(CargoPlane.transform.position + (FlyDirection * (Game.PlaneSpawnDistance * 2f)));
        CargoPlane.GetComponent<Plane>().SpawnAmount = SpawnAmount;

        CargoPlane.GetComponent<Unit>().Team = Team;

        if(Game.DEBUG_ShowDropPoint)
            Debug.DrawLine(new Vector3(CirclePoint.x, FlyHeight, CirclePoint.y),  CargoPlane.transform.position + (FlyDirection * (Game.PlaneSpawnDistance - DropDistance)), Color.red, 60f);
    }

    public void BuyFighterPlane(Faction Team)
    {
        if(Game.Defense_Money >= Game.Defense_FighterPlaneCost)
        {
            Game.Defense_Money -= Game.Defense_FighterPlaneCost;
            SpawnFighterPlane(Team);
        }
    }

    void SpawnAttackerFighterPlane()
    {
        SpawnFighterPlane(Game.TeamTwo);
    }

    public void BuyCargoPlane(Vector3 Pos)
    {
        if(Game.Defense_Money >= Game.Defense_CargoPlaneCost)
        {
            SpawnFlare(Pos, false, 30f);
            Game.Defense_Money -= Game.Defense_CargoPlaneCost;
            SpawnCargoPlane(Game.TeamOne, Pos, Random.Range(Game.CloseCargoDropStart, Game.CloseCargoDropEnd), 5f, Game.Defense_AlliesPerDrop);
        }
    }

    public void BuyEquipment(Vector3 Pos)
    {
        return;
        /*
        if(Game.Defense_Money >= Game.Defense_EquipmentCost)
        {
            Game.Defense_Money -= Game.Defense_EquipmentCost;
            CreateEquipment(Pos);
        }*/
    }

    void CreateEquipment(Vector3 Pos)
    {
        GameObject Equipment = GameObject.Instantiate(EquipmentPrefab, Pos, Quaternion.identity);
    }

    public void BuyArtillery(Vector3 Pos)
    {
        if(Game.Defense_Money >= Game.Defense_ArtilleryCost)
        {
            SpawnFlare(Pos, true, 10f);
            Game.Defense_Money -= Game.Defense_ArtilleryCost;
            SpawnArtillery(Pos);
        }
    }

    void SpawnArtillery(Vector3 Pos)
    {
        GameObject Artillery = GameObject.Instantiate(ArtilleryPrefab, Pos + new Vector3(0f, 500f, 0f), Quaternion.identity);
    }
    
    void SpawnFlare(Vector3 Pos, bool Danger, float Offtime)
    {
        GameObject Flare = GameObject.Instantiate(FlarePrefab, Pos, Quaternion.identity);
        Flare.GetComponent<VisualEffect>().SetBool("Danger", Danger);
        Flare.GetComponent<VisualEffect>().SetFloat("Offtime", Offtime);
    }

    public InfantryKit GetKit(Faction Team)
    {
        if(Game.GameMode != GameModes.Defense && Game.RandomizeLoadouts)
            return RandomKit;

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
}
