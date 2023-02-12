using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class Plane : AI
{
    public enum PropellerSetup
	{
		Body,
        Wings,
        Both
	};
    
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
        Dead
    };

    public States State;
    public Types Type;
    public PlaneInfo PI;

    public PropellerSetup PropellerType;

    Vector3 RandomFar = Vector3.zero;
    Vector3 RandomNear = Vector3.zero;

    public bool PlayerUsingPrimary = true;
    public List<Weapon> MGs;
    public List<Weapon> ATs;
    public Transform BailPosition;

    public Vector3 HeadTarget = Vector3.zero;
    public Vector3 DropTarget = Vector3.zero;

    bool Dropping = false;
    public int SpawnAmount = 0;

    bool Landed = false;

    public List<VisualEffect> Engines = new List<VisualEffect>();
    public MeshRenderer MR;
    public MeshRenderer MR_Left;
    public MeshRenderer MR_Right;

    public float CurrentSpeed = 0f;

    bool HitInAir = false;

    Vector3 SpawnPosition;
    
    public GameObject CrashPrefab;
    public float EffectSize = 4f;
    public float ExplosionSize = 2f;
    public int ExplosionDamage = 100;
    public float SoundVolume = 1f;

    void Start()
    {
        Initialize();

        InvokeRepeating("ChangeBehavior", 1f, Random.Range(10f, 15f));
        
        InvokeRepeating("Bomb", 1f, Random.Range(15f, 20f));
    }

    void Bomb()
    {
        if(Type == Types.Fighter)
        {
            if(!U.Controller && State == States.Strafe)
            {
                DropBomb();
            }
        }
    }

    public override void Initialize()
    {
        base.Initialize();

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

        //MR.materials[Type == Types.Fighter ? 2 : 1].SetTexture("Icon", Manager.M.Factions[(int) U.Team].Symbol);
        //MR_Left.materials[Type == Types.Fighter ? 1 : 2].SetTexture("Icon", Manager.M.Factions[(int) U.Team].Symbol);
        //MR_Right.materials[Type == Types.Fighter ? 1 : 2].SetTexture("Icon", Manager.M.Factions[(int) U.Team].Symbol);

        if(State == States.Idle)
            SetState(States.Idle);

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

        if(State != States.Dead)
        {
            HitInAir = true;
            if(Col.GetContact(0).thisCollider.transform.root == this.transform)
                GetComponent<Damage>().LastDamagedPart = Col.GetContact(0).thisCollider.GetComponent<Damage_Part>().Part;
            else
                GetComponent<Damage>().LastDamagedPart = Col.GetContact(0).otherCollider.GetComponent<Damage_Part>().Part;
                
            Die();
        }
        
        if(Col.gameObject.CompareTag("Landable") || Col.gameObject.CompareTag("Unmoving"))
        {
            if(!Landed)
            {
                GameObject Dec = Instantiate(CrashPrefab, Col.contacts[0].point, Quaternion.identity);
                if(Col.contacts[0].normal != Vector3.zero)
                    Dec.transform.rotation = Quaternion.LookRotation(Col.contacts[0].normal, Vector3.up);

                Dec.GetComponent<Explosion>().Explode(EffectSize, ExplosionSize, ExplosionDamage, SoundVolume, U.Controller);
            }
            Landed = true;
        }
        

        

        
    }

    public void Die()
    {   
        if(!Dead)
        {
            Dead = true;
            if(U.Controller)
                GameObject.FindWithTag("Camera").GetComponent<Player>().Died();

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
                Manager.M.DelaySpawnFighterPlane(U.Team);
                
                if(U.Team == Game.TeamOne)
                    Game.Conquest_TeamOneTickets--;
                else
                    Game.Conquest_TeamTwoTickets--;
            }

            for (int i = 0; i < Engines.Count; i++)
                Engines[i].enabled = true;
            SetState(States.Dead);
            StartCoroutine(Fall());
        }
    }

    public void DropBomb()
    {
        GameObject Dropped = GameObject.Instantiate(Manager.M.BombPrefab, BailPosition.position, Quaternion.identity);
        Dropped.transform.rotation = this.transform.rotation;
        Dropped.GetComponent<Rigidbody>().velocity = r.velocity + (-this.transform.up * 2f);
        Dropped.GetComponent<Rigidbody>().angularVelocity = r.angularVelocity;
        Dropped.GetComponent<Bomb>().PlayerOwned = U.Controller;
    }

    IEnumerator Fall()
    {
        if (PropellerType == PropellerSetup.Body)
        {
            MR.materials[1].SetInt("Broke", 1);
        }else if (PropellerType == PropellerSetup.Wings)
        {
            MR_Left.materials[1].SetInt("Broke", 1);
            MR_Right.materials[1].SetInt("Broke", 1);
        }else
        {
            MR.materials[1].SetInt("Broke", 1);
            MR_Left.materials[1].SetInt("Broke", 1);
            MR_Right.materials[1].SetInt("Broke", 1);
        }

        string LastHit = GetComponent<Damage>().LastDamagedPart;
        List<string> Names = new List<string> { "Body", "Left", "Right" };

        if(LastHit == "")
            LastHit = Names[Random.Range(0, Names.Count)];

        if(LastHit == "Left")
        {
            Rigidbody Wing = MR_Left.gameObject.AddComponent(typeof(Rigidbody)) as Rigidbody;
            Wing.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
            Despawn Remove = MR_Left.gameObject.AddComponent(typeof(Despawn)) as Despawn;
            Remove.DespawnTime = 15f;

            MR_Left.transform.SetParent(GameObject.FindWithTag("Trash").transform);
            
            Rigidbody Lift = MR_Right.gameObject.AddComponent(typeof(Rigidbody)) as Rigidbody;
            Lift.mass = r.mass * 2f;
            Lift.drag = 0f;
            r.centerOfMass = Lift.transform.localPosition + (this.transform.up * 2f);
            FixedJoint Anchor = MR_Right.gameObject.AddComponent(typeof(FixedJoint)) as FixedJoint;
            Anchor.connectedBody = r;
        }else if (LastHit == "Right")
        {
            Rigidbody Wing = MR_Right.gameObject.AddComponent(typeof(Rigidbody)) as Rigidbody;
            Wing.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
            Despawn Remove = MR_Right.gameObject.AddComponent(typeof(Despawn)) as Despawn;
            Remove.DespawnTime = 15f;

            MR_Right.transform.SetParent(GameObject.FindWithTag("Trash").transform);
            
            Rigidbody Lift = MR_Left.gameObject.AddComponent(typeof(Rigidbody)) as Rigidbody;
            Lift.mass = r.mass * 2f;
            Lift.drag = 0f;
            r.centerOfMass = Lift.transform.localPosition + (this.transform.up * 2f);
            FixedJoint Anchor = MR_Left.gameObject.AddComponent(typeof(FixedJoint)) as FixedJoint;
            Anchor.connectedBody = r;
        }

        while (!Landed)
            yield return null;

        Material[] MR_Destroyed = MR.materials;
        Material[] MR_Left_Destroyed = MR_Left.materials;
        Material[] MR_Right_Destroyed = MR_Right.materials;

        MR_Destroyed[0] = Manager.M.DestroyedMaterial;
        if(LastHit != "Left")
            MR_Left_Destroyed[0] = Manager.M.DestroyedMaterial;
        if(LastHit != "Right")
            MR_Right_Destroyed[0] = Manager.M.DestroyedMaterial;

        MR.materials = MR_Destroyed;
        MR_Left.materials = MR_Left_Destroyed;
        MR_Right.materials = MR_Right_Destroyed;

        foreach (VisualEffect V in Engines)
            if(V)
                V.SetBool("Landed", true);
            
        for (int i = 0; i < Engines.Count; i++)
            if(Engines[i])
                Engines[i].gameObject.GetComponent<AudioSourceController>().Stop();

        float Begin = Time.time;
        while(Time.time < Begin + 1f)
        {
            yield return null;
            if(r.velocity.magnitude > 10f)
                Begin = Time.time;
        }

        Begin = Time.time;
        while(Time.time < Begin + 10f)
        {
            yield return null;
            foreach (VisualEffect V in Engines)
                if(V)
                    V.SetFloat("Rate", Mathf.Lerp(0f, 1f, (Time.time - Begin)/10f));
        }

        Despawn D = this.gameObject.AddComponent(typeof(Despawn)) as Despawn;
        D.DespawnTime = 10f;
        Destroy(this);
    }

    void FixedUpdate()
    {
        if(State == States.Dead)
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
            SetState(States.FlyFar);
            return;
        }
        
        if(LinedUp(Enemy.transform.position, transform.position, transform.forward, 6f))
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
            SetState(States.FlyFar);
        }
    }

    void S_Attack()
    {
        if(Enemy)
            if(!Enemy.Targetable)
                Enemy = null;

        if(!Enemy)
        {
            SetState(States.FlyFar);
            return;
        }

        //Vector3 Target = Enemy.Target.position + (Enemy.transform.forward * Enemy.pla.CurrentSpeed);
        //Vector3 Target = Enemy.Target.position + (Enemy.transform.forward * (Enemy.pla.CurrentSpeed));

        Vector3 Target = Enemy.Target.position;

        //Debug.DrawRay(this.transform.position, this.transform.forward * 400f, Color.red);
        //Debug.DrawLine(this.transform.position, Target, Color.green);

        if(Vector3.Distance(this.transform.position, Target) < 400f)
            if(LinedUp(Target, transform.position, transform.forward, 5f)) //7.5
                FireAT();

        if(Vector3.Distance(this.transform.position, Enemy.transform.position) > 100f)
        {
            FlyTo(Target, Vector3.up, PI.Dogfight);
        }
        else
        {
            SetState(States.FlyFar);
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

    public void ChangeBehavior()
    {
        if(State == States.Dead)
            return;

        if(!U.Controller)
        {
            SearchForEnemy();

            SetState(GetState());

            ChangeRandom();
        }
    }
    
    States GetState()
    {
        if(Type == Types.Fighter)             
        {
            if (Enemy)                                
            {
                if (Enemy.Type == Unit.Types.Plane)       
                    return States.Attack;               

                if (Random.value > 0.5f)
                {
                    if (State == States.FlyFar)          
                        return States.Strafe;        
                    else                                      
                        return States.FlyFar;               
                }else
                {
                    if(Random.value > 0.5f)                 
                        return States.FlyNear;
                    else 
                        return States.FlyFar;
                }
            }else                                       
            {
                if(Random.value > 0.5f)                  
                    return States.FlyNear;
                else 
                    return States.FlyFar;
            }
        }else
        {
            return States.Passing;
        }
    }
    
    void SearchForEnemy()
    {
        if (Type == Types.Fighter)
        {
            if (!Enemy)
            {
                Enemy = FindNewUnit();
            }
            else
            {
                if (Enemy.Type == Unit.Types.Infantry || Enemy.Type == Unit.Types.Tank)
                    Enemy = FindNewUnit();
            }
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
                GameObject Dropped = GameObject.Instantiate(Manager.M.Factions[(int) U.Team].InfantryPrefab , BailPosition.position, Quaternion.identity);
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

    public void SetState(States S)
    {
        State = S;
        
        string letter = Manager.M.Factions[(int) U.Team].Prefix;
        string UnitName = Type == Types.Fighter ? "fighter" : "cargo";
        this.gameObject.name = letter + " - " + UnitName + " - " + State.ToString().ToLower();
    }
}
