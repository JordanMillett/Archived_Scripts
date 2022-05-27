using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class Plane : MonoBehaviour
{
    public enum Types
    {
        Fighter,
        Cargo
    }

    public enum States
    {
        Idle,
        FlyNear,
        FlyFar,
        Strafe,
        Attack,
        Passing,
        Falling
    };

    public States State;
    public Types Type;
    public PlaneInfo PI;

    Rigidbody r;

    Vector3 RandomFar = Vector3.zero;
    Vector3 RandomNear = Vector3.zero;

    public bool PlayerUsingPrimary = true;
    public List<Weapon> MGs;
    public List<Weapon> ATs;
    public Transform BailPosition;

    public Transform CameraParent;

    public Unit Enemy;

    public Vector3 HeadTarget = Vector3.zero;
    public Vector3 DropTarget = Vector3.zero;

    public GameObject FireEffect;

    bool Dropping = false;
    public int SpawnAmount = 0;

    bool Landed = false;

    public List<VisualEffect> Engines = new List<VisualEffect>();
    public MeshRenderer MR;
    public AudioSource AS;

    public float CurrentSpeed = 0f;

    bool HitInAir = false;

    Vector3 SpawnPosition;

    public bool Exploded = false;

    public Unit U;

    void Start()
    {
        Initialize();

        InvokeRepeating("ChangeBehavior", 1f, Random.Range(10f, 15f));
    }

    void Initialize()
    {
        r = GetComponent<Rigidbody>();
        U = GetComponent<Unit>();

        foreach(Weapon AT in ATs)   //GUN ROCK PLANE AROUND
            AT.Holder = U;
        foreach(Weapon MG in MGs)
            MG.Holder = U;

        if(Type == Types.Fighter)
        {
            U.Capabilities.AntiInfantry = true;
            U.Capabilities.AntiArmor = true;
            U.Capabilities.AntiAir = true;
        }

        SpawnPosition = this.transform.position;

        MR.materials[0].SetColor("Team", U.Team == Game.TeamOne ? Game.FriendlyColor : Game.EnemyColor);
        MR.materials[2].SetTexture("Icon", Manager.M.Factions[(int) U.Team].Symbol);
        
        if(State == States.Idle)
            UpdateState(States.Idle);

        if(Type == Types.Fighter)
        {
            if(U.Team == Game.TeamTwo)
            {
                Manager.M.TeamTwo.Add(U);
                Game.Defense_EnemyPlanes++;
            }
            else
            {
                Manager.M.TeamOne.Add(U);
                Game.Defense_AllyPlanes++;
            }
        }
    }

    void OnCollisionEnter(Collision Col)
    {
        if(Col.transform.GetComponent<Bomb>())
            return;

        if(State != States.Falling)
        {
            HitInAir = true;
            Explode();
        }

        if(Col.gameObject.isStatic)
        {
            Landed = true;
        }

        
    }

    public void Explode()
    {   
        if(!Exploded)
        {
            Exploded = true;

            if(U.Team == Game.TeamTwo)
            {
                Manager.M.TeamTwo.Remove(U);
            }
            else
            {
                Manager.M.TeamOne.Remove(U);
            }
            
            if(Game.GameMode == GameModes.Defense)
            {
                if(!HitInAir && U.Team == Game.TeamTwo)
                {
                    Game.Defense_Money += Game.Defense_PlaneKillBonus;
                }

                if(Type == Types.Fighter)
                {
                    if(U.Team == Game.TeamTwo)
                    {
                        Game.Defense_EnemyPlanes--;
                    }
                    else
                    {
                        Game.Defense_AllyPlanes--;
                    }
                }
            }else
            {
                Manager.M.CreateFighterPlane(U.Team);
                
                if(U.Team == Game.TeamOne)
                    Game.Conquest_TeamOneTickets--;
                else
                    Game.Conquest_TeamTwoTickets--;
            }

            FireEffect.SetActive(true);
            UpdateState(States.Falling);
            StartCoroutine(Fall());
        }
    }

    public void DropBomb()
    {
        GameObject Dropped = GameObject.Instantiate(Manager.M.BombPrefab, BailPosition.position, Quaternion.identity);
        Dropped.transform.rotation = this.transform.rotation;
        Dropped.GetComponent<Rigidbody>().velocity = r.velocity + (-this.transform.up * 2f);
        Dropped.GetComponent<Rigidbody>().angularVelocity = r.angularVelocity;
    }

    IEnumerator Fall()
    {
        MR.materials[1].SetInt("Broke", 1);

        while(!Landed)
        {
            yield return null;
        }

        if(U.Controller)
            GameObject.FindWithTag("Camera").GetComponent<Player>().FindNewBody(false, false);

        float Begin = Time.time;
        while(Time.time < Begin + 1f)
        {
            yield return null;
            if(r.velocity.magnitude > 10f)
                Begin = Time.time;
        }

        AS.enabled = false;

        Begin = Time.time;
        while(Time.time < Begin + 10f)
        {
            yield return null;
            foreach (VisualEffect V in Engines)
                V.SetFloat("Rate", Mathf.Lerp(0f, 1f, (Time.time - Begin)/10f));
        }

        Despawn D = this.gameObject.AddComponent(typeof(Despawn)) as Despawn;
        D.DespawnTime = 10f;
        Destroy(this);
    }

    void FixedUpdate()
    {
        if(State == States.Falling)
        {
            U.Targetable = false;
            return;
        }

        if(!U.Controller)
        {
            if(Type == Types.Cargo)
            {
                if(Vector3.Distance(this.transform.position, SpawnPosition) > Vector3.Distance(SpawnPosition, DropTarget) && !Dropping)
                {
                    //Debug.DrawRay(this.transform.position, -Vector3.up * 500f, Color.blue, 20f);
                    Dropping = true;
                    StartCoroutine(Drop());
                }
            }

            if(Type == Types.Fighter)
            {
                switch(State)
                {
                    case States.Idle :      S_Idle();        break;
                    case States.FlyNear :   S_FlyNear();     break;
                    case States.FlyFar :    S_FlyFar();    break;
                    case States.Strafe :    S_Strafe();    break;
                    case States.Attack :    S_Attack();    break;
                }
            }else
            {
                switch(State)
                {
                    case States.Idle :      S_Idle();        break;
                    case States.Passing :   S_Passing();     break;
                }
            }
        }
    }

    void S_Idle()
    {

    }

    void S_FlyFar()
    {
        FlyTo(RandomFar, Vector3.up, PI.Normal);
    }

    void S_FlyNear()
    {
        FlyTo(RandomNear, Vector3.up, PI.Strafe);
    }

    void S_Strafe()
    {
        if(!Enemy)
        {
            UpdateState(States.FlyFar);
            return;
        }
        
        if(AI.LinedUp(Enemy.transform.position, transform.position, transform.forward, 5f))
        {
            if(Enemy.Type == Unit.Types.Tank)
                FireAT();
            else
                FireMG();
        }

        if(this.transform.position.y > 150f)
        {
            FlyTo(Enemy.transform.position, Vector3.up, PI.Strafe);
        }
        else
        {
            UpdateState(States.FlyFar);
        }
    }

    void S_Attack()
    {
        if(Enemy)
            if(!Enemy.Targetable)
                Enemy = null;

        if(!Enemy)
        {
            UpdateState(States.FlyFar);
            return;
        }
        
        Vector3 Target = Enemy.Target.position + (Enemy.transform.forward * Enemy.pla.CurrentSpeed);

        if(Vector3.Distance(this.transform.position, Target) < 350f)
            if(AI.LinedUp(Target, transform.position, transform.forward, 7.5f))
                FireMG();

        if(Vector3.Distance(this.transform.position, Enemy.transform.position) > 150f)
        {
            FlyTo(Target, Vector3.up, PI.Dogfight);
        }
        else
        {
            UpdateState(States.FlyFar);
        }
    }

    void S_Passing()
    {
        FlyTo(HeadTarget, Vector3.up, PI.Normal);

        if(Vector3.Distance(this.transform.position, HeadTarget) < 10f)
            Destroy(this.gameObject);
    }

    public void FlyTo(Vector3 Pos, Vector3 TurnUp, PlaneInfo.Gear G)
    {
        CurrentSpeed = G.Speed;

        Quaternion newRotation = Quaternion.LookRotation((Pos - this.transform.position).normalized, TurnUp);
        /*
        float ForwardAngle = Vector3.Angle(this.transform.forward, Vector3.up);
        ForwardAngle /= 180f;
        r.velocity = this.transform.forward * Mathf.Lerp(0f, G.Speed * 2f, ForwardAngle);
        */
        r.velocity = this.transform.forward * G.Speed;

        this.transform.rotation = Quaternion.Slerp(this.transform.rotation, newRotation, Time.fixedDeltaTime * G.Turn);
    }

    void ChangeBehavior()
    {
        if(State == States.Falling)
            return;

        if(!U.Controller)
        {

            if(!Enemy)
                Enemy = AI.FindNewUnit(U, U.Capabilities, false);

            UpdateState(GetState());

            ChangeRandom();
        }
    }

    public void FireAT()
    {
        foreach(Weapon AT in ATs)
            AT.PullTrigger();
    }

    public void FireMG()
    {
        foreach(Weapon MG in MGs)
            MG.PullTrigger();
    }

    IEnumerator Drop()
    {
        while(SpawnAmount > 0)
        {
            if(CanBail())
            {
                SpawnAmount--;
                GameObject Dropped = GameObject.Instantiate(Manager.M.InfantryPrefab, BailPosition.position, Quaternion.identity);
                Dropped.GetComponent<Unit>().Team = U.Team;
                Dropped.GetComponent<Infantry>().Kit = Manager.M.GetKit(U.Team);
                Dropped.GetComponent<Infantry>().DroppingIn = true;
                yield return new WaitForSeconds(Random.Range(0.1f, 0.5f));
            }else
            {
                yield return null;
            }
        }
    }

    bool CanBail()
    {
        RaycastHit hit;
        if(Physics.Raycast(BailPosition.position, -Vector3.up, out hit, Game.DropHeightMax * 2f, Game.IgnoreSelectMask))
        {
            if(hit.transform.gameObject.CompareTag("Landable"))
                return true;
            else
                return false;
        }

        return false;
    }

    void ChangeRandom()
    {
        RandomNear = new Vector3(Random.Range(-250f, 250f), Random.Range(125f, 150f), Random.Range(-250f, 250f));

        RandomFar = RandomNear;
        RandomFar.x += RandomFar.x > 0 ? 500f : -500f;
        RandomFar.y += 250f;
        RandomFar.z += RandomFar.z > 0 ? 500f : -500f;
    }

    States GetState()
    {
        float choice = Random.value;
        States chosen = States.Idle;

        if(Type == Types.Fighter)
        {
            if(choice > 0.75f)
                chosen = States.FlyNear;
            else 
                chosen = States.FlyFar;

            choice = Random.value;

            if(State == States.FlyFar)  //only strafe if far away
            {
                if(Enemy)
                {
                    if(choice > 0.75f &&        Enemy.Type == Unit.Types.Infantry || Enemy.Type == Unit.Types.Tank)
                        chosen = States.Strafe;
                    if(choice > 0.15f &&        Enemy.Type == Unit.Types.Plane)
                        chosen = States.Attack;
                }
            }
        }else
        {
            chosen = States.Passing;
        }
            

        return chosen;
    }

    public void UpdateState(States S)
    {
        State = S;
        
        string letter = Manager.M.Factions[(int) U.Team].Prefix;
        string UnitName = Type == Types.Fighter ? "fighter" : "cargo";
        this.gameObject.name = letter + " - " + UnitName + " - " + State.ToString().ToLower();
    }
}
