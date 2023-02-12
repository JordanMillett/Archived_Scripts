using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Infantry : AI
{   
    public enum States
    {
        Idle,
        Wandering,
        LookingAround,
        Fighting,
        GoingToObjective,
        Dead
    };
    
    public enum Orders
    {
        None,
        DefendPosition,
        Follow
    };

    public InfantryKit Kit;

    public States State;
    public Orders Order;

    public Animator An;

    public string Name;

    public Transform Chest;
    
    public Transform PrimarySlot;
    public Transform SecondSlot;
    public Transform s1;
    public Transform s2;
    public Transform HatLocation;
    
    public Weapon Equipped;
    public SkinnedMeshRenderer SMR;
    public GameObject RagdollPrefab;
    public BodyPositions BodyInfo;
    
    public float yaw = 0f;
    public float pitch = 0f;
    public float headUpperLimit = 50f;
    public float headLowerLimit = 60f;
    public Vector3 HitDirection;
    public float HealthAlpha = 1f;
    public bool Aiming = false;
    public int MoveStance = 0;
    public GameObject ParachutePrefab;
    public bool DroppingIn = false;
    
    public Weapon Primary;
    public Weapon Secondary;
    Vector3 HeadHeight = new Vector3(0f, 1.75f, 0f);
    Vector3 CoverLocation = Vector3.zero;
    Vector3 CoverDirection = Vector3.zero;
    Vector3 s1Default;
    Vector3 s2Default;
    Vector3 randomOffset = Vector3.zero;
    Vector3 randomLook = Vector3.zero;
    Vector3 Sway = Vector3.zero;
    Vector3 SwaySeed = Vector3.zero;
    IEnumerator CallOutRoutine;
    GameObject ParachuteReference;
    NavMeshQueryFilter NavInfo;
    NavMeshPath PathToGoal;

    float LastAlerted = -100f;
    float AlertTime = 10f;
    float RelaxTime = 30f;
    float LastHurt = -100f;
    float HealTime = 10f;
    Vector3 LastPos = Vector3.zero;
    float Stamina = 100f;
    float WalkSpeed = 4f;
    float MovementForce = 200f;
    float SprintMultiplier = 1.5f;
    float CurrentSprint = 1f;
    bool Exhausted = false;
    bool CoverTall = false;
    bool SwappingWeapons = false;
    int PathIndex = 0;
    bool Lost = false;

    void Start()
    {

        Initialize();

        if(!DroppingIn)
        {
            StartCoroutine(WaitForGameStart());
        }
        else
        {
            PrepareToDrop();
        }
    }
    
    public override void Initialize()
    {
        base.Initialize();
        
        PathToGoal = new NavMeshPath();

        Name = Manager.M.GetName(U.Team);

        NavInfo = new NavMeshQueryFilter();
        NavInfo.areaMask = 1;
        NavInfo.agentTypeID = 0;

        LastPos = this.transform.position;
        InvokeRepeating("CheckStuck", 3f, 3f);

        ChangeRandom();

        InvokeRepeating("CheckStamina", 0.1f, 0.1f);
        InvokeRepeating("CheckFall", 0.5f, 0.5f);
        InvokeRepeating("CheckHeal", 1f, 1f);
        
        /*
        for(int i = 0; i < SMR.materials.Length; i++)
        {
            SMR.materials[i].SetColor("Team", U.Team == Game.TeamOne ? Game.FriendlyColor : Game.EnemyColor);
        }*/

        if(Kit)
        {
            Primary = Kit.CreatePrimary(PrimarySlot, U.Team);
            Secondary = Kit.CreateSecondary(SecondSlot, U.Team);

            U.Capabilities.AntiInfantry = Primary.Info.UseCase.AntiInfantry || Secondary.Info.UseCase.AntiInfantry ? true : false;
            U.Capabilities.AntiArmor = Primary.Info.UseCase.AntiArmor || Secondary.Info.UseCase.AntiArmor ? true : false;
            U.Capabilities.AntiAir = Primary.Info.UseCase.AntiAir || Secondary.Info.UseCase.AntiAir ? true : false;

            //HatLocation.transform.GetChild(0).GetChild(0).GetComponent<MeshRenderer>().material.SetColor("Team", U.Team == Game.TeamOne ? Game.FriendlyColor : Game.EnemyColor);
        }

        s1Default = s1.localPosition;
        s2Default = s2.localPosition;

        SwaySeed = new Vector3(Random.Range(0f, 1000f), Random.Range(0f, 1000f), Random.Range(0f, 1000f));

        //MOVE TO MANAGER SCRIPT TO REG NEW AND DELETE OLD UNITS
        if(U.Team == Game.TeamTwo)
        {
            Manager.M.TeamTwo.Add(U);
            Game.Defense_Enemies++;
        }
        else
        {
            Manager.M.TeamOne.Add(U);
            Game.Defense_Allies++;
        }

        if(State == States.Idle)
            SetState(States.Idle);
    }
    
    IEnumerator WaitForGameStart()
    {
        while(!Game.Started)
            yield return null;
            
        UpdateObjective();
        NavMesh.CalculatePath(this.transform.position + new Vector3(0f, 0.25f, 0f), Objective, NavInfo, PathToGoal);
        PathIndex = 0;
        
        InvokeRepeating("ChangeBehavior", 1f, Random.Range(2f, 3f));
    }

    void CheckStuck()
    {
        if(State != States.GoingToObjective)
            return;
            
        if(Vector3.Distance(this.transform.position, LastPos) < 0.10f)
        {
            UpdateObjective();
            NavMesh.CalculatePath(this.transform.position + new Vector3(0f, 0.25f, 0f), Objective, NavInfo, PathToGoal);
            PathIndex = 0;
        }
        
        LastPos = this.transform.position;
        
    }

    void PrepareToDrop()
    {
        yaw = Random.Range(0f, 360f);
        this.transform.localEulerAngles = new Vector3(0f, yaw, 0f);

        r.drag = 1f;
        ParachuteReference = GameObject.Instantiate(ParachutePrefab, this.transform.position, Quaternion.identity);
        ParachuteReference.transform.SetParent(this.transform);
        ParachuteReference.transform.localPosition = Vector3.zero;
        ParachuteReference.transform.localEulerAngles = Vector3.zero;
        ParachuteReference.GetComponent<Parachute>().Team = U.Team;
    }

    void ChangeRandom()
    {
        randomOffset = GetRandomOffset();

        randomLook = new Vector3(Random.Range(-2f, 2f), Random.Range(-0.5f, 0.3f), Random.Range(-2f, 2f));
        randomLook.x += randomLook.x > 0 ? 1f : -1f;
        randomLook.z += randomLook.z > 0 ? 1f : -1f;
        randomLook += (Chest.transform.position + (Chest.transform.up * 0.58f));
    }

    void OnCollisionEnter(Collision Col)
    {
        if(ParachuteReference)
        {
            r.drag = 2f;
            Destroy(ParachuteReference);
            
            //Mini.gameObject.SetActive(true);

            SetState(States.Idle);

            StartCoroutine(WaitForGameStart());
        }

        if(Col.impulse.magnitude > 65f)
        {
            HitDirection = Col.relativeVelocity.normalized * 10f;
            Die();
        }
    }

    void FixedUpdate()
    {
        if(State == States.Dead)
        {
            U.Targetable = false;
            return;
        }
        
        U.Targetable = !ParachuteReference;
        
        if(U.Targetable)
        {
            if(!U.Controller)
            {
                switch(Order)
                {
                    case Orders.Follow :         O_Follow();          break;
                    case Orders.DefendPosition : O_DefendPosition();  break;
                    case Orders.None :           O_None();            break;
                }
            }
        }
        
        if(Equipped)
        {
            Vector3 Swizz = new Vector3(Sway.y, Sway.x, Sway.x * 4f);
            Swizz *= 0.5f;

            if(!Aiming)
            {
                s1.localPosition = Vector3.Lerp(s1Default + new Vector3(0f, 0f, Equipped.CurrentBackDistance), s1Default + Sway, Time.fixedDeltaTime * 10f);
                s2.localPosition = Vector3.Lerp(s2Default + new Vector3(0f, 0f, Equipped.CurrentBackDistance), s2Default + Sway, Time.fixedDeltaTime * 10f);

                Equipped.transform.localPosition = Vector3.Lerp(Equipped.HoldOffset, Sway + Equipped.HoldOffset, Time.fixedDeltaTime * 10f);
                Equipped.transform.localEulerAngles = Vector3.zero;
            }
            else
            {
                s1.localPosition = Vector3.Lerp(s1Default + new Vector3(0f, 0f, Equipped.CurrentBackDistance), s1Default + (Sway * 0.3f), Time.fixedDeltaTime * 10f);
                s2.localPosition = Vector3.Lerp(s2Default + new Vector3(0f, 0f, Equipped.CurrentBackDistance), s2Default + (Sway * 0.3f), Time.fixedDeltaTime * 10f);

                Equipped.transform.localPosition = Vector3.zero;
                Equipped.transform.localEulerAngles = Vector3.Lerp(Vector3.zero, Swizz * 100f, Time.fixedDeltaTime * 10f);
            }
        }
    }

    void Update()
    {
        if(State == States.Dead)
            return;
        
        if(!ParachuteReference)
        {
            //STAMINA SYSTEM
            if(MoveStance == 2) // if has stamina run if not regen
            {
                if(Stamina > 0f)
                    CurrentSprint = SprintMultiplier;
                else
                    CurrentSprint = 1f;
            }else
            {
                CurrentSprint = 1f;
            }

            //ANIMATIONS
            float Smod = Mathf.Lerp(3f, 0.5f, Stamina/100f);

            float mult = Mathf.Lerp(1f * Smod, 5f, r.velocity.magnitude/(WalkSpeed * SprintMultiplier));

            float s = Aiming ? 0.75f : 0.5f;

            Sway = new Vector3(
                Mathf.PerlinNoise(Time.time * s + SwaySeed.x, Time.time * s + SwaySeed.z) - 0.5f, 
                Mathf.PerlinNoise(Time.time * s + SwaySeed.y, Time.time * s + SwaySeed.x) - 0.5f,
                Mathf.PerlinNoise(Time.time * s + SwaySeed.z, Time.time * s + SwaySeed.y) - 0.5f
            );

            Sway.y -= 0.5f;
            Sway.y *= 0.5f;

            if(Aiming)
            {
                Sway *= (0.03f * mult);
            }else
            {
                Sway *= (0.1f * mult);
            }
            
            if(An.GetCurrentAnimatorStateInfo(1).IsName("Gun_None"))
                An.SetLayerWeight(1, Mathf.Lerp(An.GetLayerWeight(1), 0f, Time.deltaTime * 10f));
            else
                An.SetLayerWeight(1, Mathf.Lerp(An.GetLayerWeight(1), 1f, Time.deltaTime * 10f));
            
            if(!U.Controller)
            {
                if(Time.time < RelaxTime + LastAlerted)
                {
                    if(Time.time < AlertTime + LastAlerted)
                    {
                        if(Enemy)
                            Equip(GetRightGun(Enemy.Type));
                        else
                            Equip(Primary);
                            
                        if(!Aiming)
                            An.SetInteger("AttackStance", 2); //Held
                    }else
                    {
                        An.SetInteger("AttackStance", 1);   //Down
                    }
                }else
                {
                    Equip(null); //Away
                }
            }else
            {
                if(Equipped)
                {
                    if(!Aiming)
                        An.SetInteger("AttackStance", 2); //Held
                    
                }
            }

            An.SetInteger("MoveStance", MoveStance); 

            An.SetFloat("ReloadSpeed", Equipped ? 2f/Equipped.Info.ReloadTime : 1f);
            An.SetBool("Reloading", Equipped ? Equipped.Reloading : false);

            if(Equipped)
                An.SetFloat("Primary", Equipped == Primary ? 0f : 1f);
        }else
        {
            An.SetInteger("MoveStance", -5);    //dropping in
        }
    }

    void CheckStamina()
    {
        if(MoveStance == 2)
        {
            Stamina -= 1f;
            if(Stamina < 0f)
            {
                Stamina = 0f;
                Exhausted = true;
            }
        }else
        {
            Stamina += MoveStance == -1 ? 2f : 1f;
            if(Stamina > 100f)
                Stamina = 100f;
            if(Stamina > 40f)
                Exhausted = false;
        }
    }

    void CheckFall()
    {
        if(!ParachuteReference)
        {
            RaycastHit hit;
            if(!Physics.Raycast(this.transform.position + new Vector3(0f, 0.5f, 0f), -Vector3.up, out hit, 5f, Game.IgnoreSelectMask))
            {
                Lost = true;
                Die();
            }
        }
    }

    void CheckHeal()
    {
        if(Time.time > HealTime + LastHurt)
            GetComponent<Damage>().Heal(5);
    }

    public void Equip(Weapon W)
    {
        if(Equipped == W || SwappingWeapons)
            return;

        if(Equipped)
            if(Equipped.Reloading)
                Equipped.StopReload();
        
        StartCoroutine(SwapWeapon(W));
    }

    IEnumerator SwapWeapon(Weapon W)
    {
        SwappingWeapons = true;
        Equipped = null;
        An.SetInteger("AttackStance", 0);

        yield return new WaitForSeconds(0.2f);
        
        Equipped = W;

        if(W)
        {
            An.SetInteger("AttackStance", 1);
            Equipped.Holder = U;
        }
        SwappingWeapons = false;
    }

    public void Fire()
    {
        LastAlerted = Time.time;
        if(Equipped)
        {   
            if(MoveStance != 2)
                Equipped.PullTrigger();
        }
    }

    public void ChangeBehavior()
    {
        if(State == States.Dead)
            return;
        
        if(!U.Controller)
        {
            SearchForEnemy();

            CheckObjectiveStatus();

            SetState(GetState());
            
            if(State == States.Wandering)
                ChangeRandom();
        }
    }
    
    States GetState()
    {
        //DETERMINED
        
        if(Enemy)
            return States.Fighting;

        if(FarAway())
            return States.GoingToObjective;

        //RANDOM

        float choice = Random.value;

        if(choice > 0.75f)
            return States.Wandering;
        else if(choice > 0.4f)
            return States.LookingAround;
        else
            return States.Idle;
    }
    
    void SearchForEnemy()
    {
        if (Enemy)
            if (!LineOfSight(Enemy.gameObject, Enemy.Target.position))
                ForgetEnemy();

        if (!Enemy)
        {
            Enemy = FindNewUnit();

            if (Enemy)
                if (Enemy.Team == Game.TeamOne)
                    Logic.L.LastKnownTarget = Enemy;
        }
    }
    
    void CheckObjectiveStatus()
    {
        if (State == States.GoingToObjective)
        {
            if(ObjectiveFlag.Owner == (U.Team == Game.TeamOne ? 1 : -1))
            {
                UpdateObjective();
                NavMesh.CalculatePath(this.transform.position + new Vector3(0f, 0.25f, 0f), Objective, NavInfo, PathToGoal);
                PathIndex = 0;
            }
        }
    }

    public void Aim(bool A)
    {
        if(MoveStance == 2)
            A = false;

        if(!Equipped)
            A = false;

        if(Equipped && Equipped.Reloading)
            A = false;

        if(A)
        {
            LastAlerted = Time.time;
            Aiming = true;
            An.SetInteger("AttackStance", 3);   //Aiming
        }else
        {
            Aiming = false;
        }
    }

    public void LookAt(Vector3 Pos)
    {
        float PitchSpeed = 5f;
        float TurnSpeed = 5f;

        if(U.Controller)
        {
            PitchSpeed *= 5f;
            TurnSpeed *= 5f;
        }

        Vector3 TargetDirection = Vector3.zero;
        Quaternion Look = Quaternion.identity;

        //PITCH - Chest
        TargetDirection = Pos - (Chest.transform.position + (Chest.transform.up * 0.58f));
        Look = Quaternion.LookRotation(TargetDirection, Chest.transform.right);

        float Angle = Look.eulerAngles.x;
        if(Angle > 180f)
        {
            if(Angle < 360f - headUpperLimit)
            {
                pitch = -headUpperLimit;
                Angle = 360f - headUpperLimit;
            }
        }else
        {
            if(Angle > headLowerLimit)
            {
                pitch = headLowerLimit;
                Angle = headLowerLimit;
            }
        }

        Look = Quaternion.Euler(Angle, Chest.rotation.eulerAngles.y, Chest.rotation.eulerAngles.z);
        Chest.transform.rotation = Quaternion.Lerp(Chest.transform.rotation, Look, Time.deltaTime * PitchSpeed);

        //TURNING
        TargetDirection = Pos - this.transform.position;
        Look = Quaternion.LookRotation(TargetDirection, this.transform.up);
        Look = Quaternion.Euler(this.transform.rotation.eulerAngles.x, Look.eulerAngles.y, this.transform.rotation.eulerAngles.z);
        this.transform.rotation = Quaternion.Lerp(this.transform.rotation, Look, Time.deltaTime * TurnSpeed);

        yaw = this.transform.localRotation.eulerAngles.y;
        pitch = Chest.transform.localRotation.eulerAngles.x;
        if(pitch > 180f)
            pitch -= 360f;
    }

    bool LinedUp(Vector3 _pos)
    {
        if(!Equipped)
            return false;

        if(LinedUp(_pos, (Chest.position + (Chest.up * 0.58f)), Chest.forward, 10f))
            if(LinedUp(_pos, Chest.position + (Chest.up * 0.58f), Equipped.FirePosition.forward, 4f))
                return true;

        return false;
    }

    public void Hurt()
    {
        LastAlerted = Time.time;
        LastHurt = Time.time;

        UpdateModel();
    }

    public void UpdateModel()
    {
        Damage D = GetComponent<Damage>();
        HealthAlpha = (float) D.Health/(float) D.MaxHealth;
        if(HealthAlpha > 1f)
            HealthAlpha = 1f;

        for(int i = 0; i < SMR.materials.Length; i++)
            SMR.materials[i].SetFloat("Damage", 1f - HealthAlpha);
    }

    public void Die()
    {
        if(!Dead)
        {
            Dead = true;

            Primary.Drop(r.velocity);
            Secondary.Drop(r.velocity);

            if(U.Controller)
                GameObject.FindWithTag("Camera").GetComponent<Player>().Died();
                //GameObject.FindWithTag("Camera").GetComponent<Player>().FindNewBody(false, false);

            Damage D = GetComponent<Damage>();
            float amount = (float) D.Health/(float) D.MaxHealth;

            GameObject NewRagdoll = Instantiate(RagdollPrefab, this.transform.position, this.transform.rotation);
            NewRagdoll.GetComponent<Ragdoll>().Init(this, (r.velocity * 4f) + (HitDirection * 2f), r.angularVelocity, 1f - amount);

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
                if(U.Team == Game.TeamTwo)
                {
                    Game.Defense_Enemies--;
                    if(!Lost)
                        Game.Defense_Money += Game.Defense_InfantryKillBonus;
                }
                else
                {
                    Game.Defense_Allies--;
                    if(Game.Defense_Allies == 0)
                        Game.EndGame();
                }
            }else if(Game.GameMode == GameModes.Conquest)
            {
                List<Flag> Targets = Manager.M.GetFlagsOfTeam(U.Team, true);
                Manager.M.DelaySpawnInfantry(Targets[Random.Range(0, Targets.Count)], U.Team, true);

                if(U.Team == Game.TeamOne)
                    Game.Conquest_TeamOneTickets--;
                else
                    Game.Conquest_TeamTwoTickets--;

            }else if(Game.GameMode == GameModes.Hill)
            {
                if(!Game.FlipSpawns)
                {
                    Manager.M.DelaySpawnInfantry(U.Team == Game.TeamOne ? Logic.L.TeamOneSpawn : Logic.L.TeamTwoSpawn, U.Team, true);
                }else
                {
                    Manager.M.DelaySpawnInfantry(U.Team == Game.TeamOne ? Logic.L.TeamTwoSpawn : Logic.L.TeamOneSpawn, U.Team, true);
                }
                
                
                if(U.Team == Game.TeamOne)
                    Game.Conquest_TeamOneTickets--;
                else
                    Game.Conquest_TeamTwoTickets--;
            }
            
            SetState(States.Dead);
            Destroy(this.gameObject);
        }
    }

    Vector3 GetRandomOffset()
    {
        Vector3 Potential = Vector3.zero;
        RaycastHit hit;

        float DesiredSpace = 10f;

        while(true)
        {  
            Potential = new Vector3(Random.Range(-20f, 20f), 0f, Random.Range(-20f, 20f));
            Potential.x += randomOffset.x > 0 ? 5f : -5f;
            Potential.z += randomOffset.z > 0 ? 5f : -5f;
            Potential += this.transform.position;
            if(Physics.Raycast(this.transform.position + new Vector3(0f, 0.5f, 0f), (Potential - this.transform.position).normalized, out hit, DesiredSpace, Game.IgnoreSelectMask))
            {
                //Debug.DrawRay(Chest.transform.position, (Potential - this.transform.position).normalized * DesiredSpace, Color.red, 1f);
                DesiredSpace -= 4f;
                if(DesiredSpace < 1f)
                    DesiredSpace = 1f;
            }else
            {
                //Debug.DrawRay(this.transform.position + new Vector3(0f, 0.5f, 0f), (Potential - this.transform.position).normalized * DesiredSpace, Color.green, 1f);
                return Potential;
            }
        }
    

    }

    public void SetState(States S)
    {
        State = S;
        if(!U.Controller)
        {
            string letter = Manager.M.Factions[(int) U.Team].Prefix;

            string UnitName = "infantry";
            this.gameObject.name = letter + " - " + UnitName + " - " + State.ToString().ToLower();
        }else
        {
            this.gameObject.name = "Player";
        }
    }

    bool TryToShoot()
    {
        LastAlerted = Time.time;

        if(LineOfSight(Enemy.gameObject, Enemy.Target.position))
        {
            Rigidbody en = Enemy.transform.GetComponent<Rigidbody>();
            float strength = Mathf.Lerp(0f, 0.75f, Vector3.Distance(Chest.position, Enemy.Target.position)/U.DetectDistance);
            Vector3 ShootAt = Enemy.Target.position + (en.velocity * strength);

            //Debug.DrawLine(Enemy.Target.position, ShootAt, Color.red); show aim target
            LookAt(ShootAt);

            Aim(true);
            if(LinedUp(ShootAt) && MoveStance != 2)
                Fire();

            return true;
        }else
        {
            return false;
        }
        
       
    }

    void StayInPlace()
    {
        if(State == States.Fighting)
        {
            S_Fighting();
        }else
        {
            S_LookingAround();
        }
    }

    ////////////////////////////////////////////////ORDERS
    void O_Follow()
    {
        if(Player.Controlled.Type == Unit.Types.Infantry)
        {
            if(Vector3.Distance(this.transform.position, Player.Controlled.transform.position) > Game.Defense_UnitFollowDistance)
            {
                HeadTo(Player.Controlled.transform.position, true);
            }else
            {
                StayInPlace();
            }
        }
        else
        {
            Order = Orders.None;
            ChangeBehavior();
        }
    }

    void O_DefendPosition()
    {
        StayInPlace();
    }

    void O_None()
    {
        if(State == States.Fighting)
        {
            S_Fighting();
        }else
        {
            Aim(false);
            switch(State)
            {
                case States.Wandering :         S_Wandering();                               break;
                case States.LookingAround :     S_LookingAround();     break;
                case States.Idle :              S_Idling();                                 break;
                case States.GoingToObjective :  S_GoingToObjective();                           break;
            }
        }
    }

    ////////////////////////////////////////////////STATES
    void S_Fighting()
    {
        if(!Enemy)
        {
            ForgetEnemy();
            ChangeBehavior();
            return;
        }else if(!Enemy.Targetable)
        {
            ForgetEnemy();
            ChangeBehavior();
            return;
        }

        bool TakeCover = HealthAlpha < 0.25f || Vector3.Distance(this.transform.position, Enemy.transform.position) > Game.DetectDistance/5f;

        if(CoverLocation == Vector3.zero && TakeCover)
            FindCover();

        if(CoverLocation == Vector3.zero || !TakeCover)
        {
            Move(Vector3.zero, false, true);
            TryToShoot();
        }else
        {
            if(Game.DEBUG_ShowCoverPoint)
                Debug.DrawRay(CoverLocation, Vector3.up, Color.red, 2f);
            
            //Debug.DrawRay(CoverLocation, Vector3.up, Color.red, 2f);
            if(Vector3.Distance(this.transform.position, CoverLocation) < 1f)   //if within stop distance of cover then shoot
            {
                Move(Vector3.zero, false, false);
                if(!TryToShoot())
                {
                    Aim(false);
                    if(CoverTall)
                        LookAt(this.transform.position + HeadHeight + CoverDirection);
                }

            }else           //if farther from the cover then go to it
            {
                Aim(false);
                if(Vector3.Distance(this.transform.position, CoverLocation) > 3f)   //run if farther 
                    HeadTo(CoverLocation, true);
                else
                    HeadTo(CoverLocation, false);
            }
        }
    }

    public void CheckLocation(Vector3 Location)//getting shot at from position
    {   
        Collider[] near = Physics.OverlapSphere(Location, 5f * 2f, Game.UnitOnlyMask);
        foreach(Collider col in near)
        {
            try 
            {   
                Unit _uni = col.transform.root.gameObject.GetComponent<Unit>();

                if(_uni)
                {
                    if(U.Team != _uni.Team && _uni.Targetable && _uni.Type == Unit.Types.Infantry)
                    {
                        if(U.Targetable)
                        {
                            CoverLocation = Vector3.zero;
                            CoverDirection = Vector3.zero;
                            Enemy = _uni;
                            SetState(States.Fighting);
                        }
                    }
                }
            }
            catch{}
        }
    }

    void FindCover()
    {
        CoverLocation = Vector3.zero;
        CoverDirection = Vector3.zero;
        CoverTall = false;

        int rays = 8;
        RaycastHit hit;

        Vector3 HideFrom = Enemy.Target.position;

        for(int i = 0; i < rays; i++)
        {
            if(CoverLocation != Vector3.zero)
                break;

            if(Physics.Raycast(Chest.transform.position, Quaternion.AngleAxis((360f/rays) * i, Vector3.up) * Vector3.forward, out hit, 20f, Game.IgnoreSelectMask))
            {
                if(hit.transform.gameObject.CompareTag("Landable") || hit.transform.gameObject.CompareTag("Unmoving"))
                {
                    GameObject check = hit.transform.gameObject;

                    Vector3 LockedNormal = new Vector3(hit.normal.x, 0f, hit.normal.z).normalized;
                    Vector3 CoverAt = hit.point + (LockedNormal * 0.5f);
                    Vector3 enemyDir = (HideFrom - CoverAt).normalized;
                    float angle = Vector3.Angle(LockedNormal, enemyDir);
                    if(angle > 90f)
                    {
                        if(Physics.Raycast(CoverAt, -Vector3.up, out hit, 2f, Game.IgnoreSelectMask))
                        {
                            CoverLocation = hit.point;
                            CoverDirection = LockedNormal;
                            if(Game.DEBUG_ShowCoverPoint)
                                Debug.DrawRay(hit.point, Vector3.up * 2f, Color.green, 2f);
                            if(Physics.Raycast(U.Target.position, Quaternion.AngleAxis((360f/rays) * i, Vector3.up) * Vector3.forward, out hit, 20f, Game.IgnoreSelectMask))
                                CoverTall = check == hit.transform.gameObject;
                        }
                    }
                }
            }
        }
    }

    void S_Wandering()
    {
        HeadTo(randomOffset, false);
    }

    void S_GoingToObjective()
    {
        if(PathToGoal.status == NavMeshPathStatus.PathComplete)
        {
            Lost = false;
            if(PathIndex == PathToGoal.corners.Length - 1)
            {
                HeadTo(Objective, true);
            }else
            {
                HeadTo(PathToGoal.corners[PathIndex], true);
                if(Vector3.Distance(this.transform.position, PathToGoal.corners[PathIndex]) < 2f)  //reached path corner
                    PathIndex++;              //go to next one
            }
        }else
        {
            Lost = true;
            Die();
        }
    }

    void HeadTo(Vector3 Destination, bool Sprint)
    {
        RaycastHit hit;
        Quaternion Dir;
        Vector3 Origin = this.transform.position + new Vector3(0f, 1f, 0f) + (this.transform.forward * -.1f);

        float len = 0.75f;

        float LookOffset = 0f;
        float MoveOffset = 0f;

        Dir = Quaternion.AngleAxis(55f, this.transform.up);
        if (Physics.Raycast(Origin, (Dir * this.transform.forward), out hit, len, Game.IgnoreSelectMask))
        {
            //Debug.DrawRay(Origin, (Dir * this.transform.forward) * len, Color.red);
            LookOffset -= (len - hit.distance)/len;
        }//else
            //Debug.DrawRay(Origin, (Dir * this.transform.forward) * len, Color.green);

        Dir = Quaternion.AngleAxis(-55f, this.transform.up);
        if (Physics.Raycast(Origin, (Dir * this.transform.forward), out hit, len, Game.IgnoreSelectMask))
        {
            //Debug.DrawRay(Origin, (Dir * this.transform.forward) * len, Color.red);
            LookOffset += (len - hit.distance)/len;
        }//else
            //Debug.DrawRay(Origin, (Dir * this.transform.forward) * len, Color.green);
            
            
        Dir = Quaternion.AngleAxis(25f, this.transform.up);
        if (Physics.Raycast(Origin, (Dir * this.transform.forward), out hit, 1.25f, Game.IgnoreSelectMask))
        {
            //Debug.DrawRay(Origin, (Dir * this.transform.forward) * 1.25f, Color.red);
            MoveOffset += 1f;
        }//else
            //Debug.DrawRay(Origin, (Dir * this.transform.forward) * 1.25f, Color.green);
            
        Dir = Quaternion.AngleAxis(-25f, this.transform.up);
        if (Physics.Raycast(Origin, (Dir * this.transform.forward), out hit, 1.25f, Game.IgnoreSelectMask))
        {
            //Debug.DrawRay(Origin, (Dir * this.transform.forward) * 1.5f, Color.red);
            MoveOffset -= 1f;
        }//else
            //Debug.DrawRay(Origin, (Dir * this.transform.forward) * 1.5f, Color.green);  
        

        Vector3 Turned = Destination + HeadHeight - this.transform.position;

        LookAt(Quaternion.Euler(new Vector3(0f, LookOffset * 120f, 0f)) * Turned + this.transform.position);
        //LookAt(Destination + HeadHeight);
        Move(Quaternion.AngleAxis(MoveOffset * -30f, this.transform.up) * this.transform.forward, Sprint, false);
        //Move(this.transform.forward, Sprint, false);
    }

    Weapon GetRightGun(Unit.Types Type)
    {
        if(Primary.Info.UseCase.AntiInfantry && Type == Unit.Types.Infantry)
            return Primary;
        if(Primary.Info.UseCase.AntiArmor && Type == Unit.Types.Tank)
            return Primary;
        if(Primary.Info.UseCase.AntiAir && Type == Unit.Types.Plane)
            return Primary;

        if(Secondary.Info.UseCase.AntiInfantry && Type == Unit.Types.Infantry)
            return Secondary;
        if(Secondary.Info.UseCase.AntiArmor && Type == Unit.Types.Tank)
            return Secondary;
        if(Secondary.Info.UseCase.AntiAir && Type == Unit.Types.Plane)
            return Secondary;

        return null;
    }
    
    public void Move(Vector3 Direction, bool Sprint, bool Crouch)
    {
        if(Sprint && !Exhausted && HealthAlpha > 0.6f)
            MoveStance = 2;
        else
            MoveStance = 1;

        if(Direction == Vector3.zero)
            MoveStance = 0;
        
        if(!Crouch)
        {
            if(Direction != Vector3.zero)
            {
                float lerp = Mathf.Lerp(1f, 0f, r.velocity.magnitude/(WalkSpeed * CurrentSprint));
                r.AddForce((Direction * CurrentSprint * MovementForce) * lerp);

                if(Direction != -transform.forward)
                    An.SetFloat("Direction", Mathf.Lerp(0.5f, 1f, HealthAlpha));
                else
                    An.SetFloat("Direction", -Mathf.Lerp(0.5f, 1f, HealthAlpha));
            }
        }else
        {
            MoveStance = -1;
        }
    }

    void ForgetEnemy()
    {
        Enemy = null;
        CoverLocation = Vector3.zero;
        CoverDirection = Vector3.zero;
    }

    void S_LookingAround()
    {
        Move(Vector3.zero, false, false);
        
        Aim(false);
        LookAt(randomLook); 
    }

    void S_Idling()
    {
        Move(Vector3.zero, false, false);
    }

    bool FarAway()
    {
        if(Order == Orders.Follow)
            return false;

        if(U.Team == Game.TeamTwo || Game.GameMode != GameModes.Defense)
        {
            if(Vector3.Distance(this.transform.position, Objective) > Game.AttackerPushDistance)
                return true;
        }else
        {
            if(Vector3.Distance(this.transform.position, Objective) > Game.DefenderStayDistance)
                return true;
        }

        return false;
    }
}