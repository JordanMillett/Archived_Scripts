using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Infantry : MonoBehaviour
{   
    public enum States
    {
        Idle,
        Wandering,
        LookingAround,
        Fighting,
        GoingToObjective
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

    public Unit U;

    public Animator An;

    public Unit Enemy;

    public string Name;

    public Transform Chest;
    public Transform Target;
    public Transform Eyes;
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
    public bool Dead = false;
    public bool Aiming = false;
    public int MoveStance = 0;
    public GameObject ParachutePrefab;
    public bool DroppingIn = false;
    
    public Weapon Primary;
    public Weapon Secondary;
    Vector3 HeadHeight = new Vector3(0f, 1.75f, 0f);
    Vector3 CoverLocation = Vector3.zero;
    Vector3 CoverDirection = Vector3.zero;
    Vector3 LastObjective = Vector3.zero;
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
    Rigidbody r;

    float LastAlerted = -100f;
    float AlertTime = 10f;
    float RelaxTime = 30f;
    float LastHurt = -100f;
    float HealTime = 10f;
    float LastGotObjective = -100f;
    float DecideDelay = 20f;
    float Stamina = 100f;
    float WalkSpeed = 4f;
    float MovementForce = 200f;
    float SprintMultiplier = 1.5f;
    float CurrentSprint = 1f;
    bool Exhausted = false;
    bool CoverTall = false;
    bool SwappingWeapons = false;
    int LostCounter = 0;

    void Start()
    {
        Initialize();

        if(!DroppingIn)
        {
            InvokeRepeating("ChangeBehavior", 1f, Random.Range(2f, 3f));
        }
        else
        {
            PrepareToDrop();
        }
    }
    
    void Initialize()
    {
        r = GetComponent<Rigidbody>();
        PathToGoal = new NavMeshPath();
        U = GetComponent<Unit>();

        Name = Manager.M.GetName(U.Team);

        NavInfo = new NavMeshQueryFilter();
        NavInfo.areaMask = 1;
        NavInfo.agentTypeID = 0;

        DecideDelay = Random.Range(15f, 30f);
        LastObjective = this.transform.position;

        ChangeRandom();

        InvokeRepeating("CheckStamina", 0.1f, 0.1f);
        InvokeRepeating("CheckFall", 0.5f, 0.5f);
        InvokeRepeating("CheckHeal", 1f, 1f);

        for(int i = 0; i < SMR.materials.Length; i++)
        {
            SMR.materials[i].SetColor("Team", U.Team == Game.TeamOne ? Game.FriendlyColor : Game.EnemyColor);
            SMR.materials[i].SetTexture("Shirt", Manager.M.Factions[(int) U.Team].Shirt);
            SMR.materials[i].SetColor("BeltColor", Manager.M.Factions[(int) U.Team].BeltColor);
            SMR.materials[i].SetColor("ButtonColor", Manager.M.Factions[(int) U.Team].ButtonColor);
        }

        if(Kit)
        {
            Primary = Kit.CreatePrimary(PrimarySlot, U.Team);
            Secondary = Kit.CreateSecondary(SecondSlot, U.Team);

            U.Capabilities.AntiInfantry = Primary.Info.UseCase.AntiInfantry || Secondary.Info.UseCase.AntiInfantry ? true : false;
            U.Capabilities.AntiArmor = Primary.Info.UseCase.AntiArmor || Secondary.Info.UseCase.AntiArmor ? true : false;
            U.Capabilities.AntiAir = Primary.Info.UseCase.AntiAir || Secondary.Info.UseCase.AntiAir ? true : false;


            GameObject Hat = GameObject.Instantiate(Manager.M.Factions[(int) U.Team].Hat, HatLocation.transform.position, Quaternion.identity);
            Hat.transform.GetChild(0).GetComponent<MeshRenderer>().material.SetColor("Team", U.Team == Game.TeamOne ? Game.FriendlyColor : Game.EnemyColor);
            
            Hat.transform.SetParent(HatLocation);
            Hat.transform.localPosition = Vector3.zero;
            Hat.transform.localEulerAngles = Vector3.zero;
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
            UpdateState(States.Idle);
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

            UpdateState(States.Idle);

            InvokeRepeating("ChangeBehavior", 1f, Random.Range(1f, 2f));
        }

        if(Col.impulse.magnitude > 65f)
        {
            HitDirection = Col.relativeVelocity.normalized * 10f;
            Die();
        }
    }

    void FixedUpdate()
    {
        //AI SYSTEM
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
                LostCounter = 5;
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

    void ChangeBehavior()
    {
        if(!U.Controller)
        {
            FindNewEnemy();

            if(State != States.Fighting)
                UpdateState(GetState());

            if(State == States.Wandering)
                ChangeRandom();

            if(State == States.GoingToObjective)
            {
                NavMesh.CalculatePath(this.transform.position, GetObjective(), NavInfo, PathToGoal);
                LostCounter = PathToGoal.status == NavMeshPathStatus.PathComplete ? 0 : LostCounter + 1;
                if(LostCounter == 5)
                    Die();
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

        if(AI.LinedUp(_pos, (Chest.position + (Chest.up * 0.58f)), Chest.forward, 10f))
            if(AI.LinedUp(_pos, Chest.position + (Chest.up * 0.58f), Equipped.FirePosition.forward, 4f))
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
                GameObject.FindWithTag("Camera").GetComponent<Player>().FindNewBody(false, false);

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
                    if(LostCounter < 5)
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
                Manager.M.SpawnAtPoint(Targets[Random.Range(0, Targets.Count)], U.Team);

                if(U.Team == Game.TeamOne)
                    Game.Conquest_TeamOneTickets--;
                else
                    Game.Conquest_TeamTwoTickets--;

            }else if(Game.GameMode == GameModes.Hill)
            {
                Manager.M.SpawnAtPoint(U.Team == Game.TeamOne ? Logic.L.TeamOneSpawn : Logic.L.TeamTwoSpawn, U.Team);
                
                if(U.Team == Game.TeamOne)
                    Game.Conquest_TeamOneTickets--;
                else
                    Game.Conquest_TeamTwoTickets--;
            }

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

    public void UpdateState(States S)
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

    States GetState()
    {
        float choice = Random.value;
        States chosen = States.Idle;

        if(choice > 0.75f)
            chosen = States.Wandering;
        else if(choice > 0.4f)
            chosen = States.LookingAround;
        else
            chosen = States.Idle;
        
        if(FarAway())
            if(!Enemy)
                chosen = States.GoingToObjective;
                

        return chosen;
    }

    bool TryToShoot()
    {
        LastAlerted = Time.time;

        if(AI.LineOfSight(U.Target.position, Enemy.gameObject, Enemy.Target.position))
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
            
            Debug.DrawRay(CoverLocation, Vector3.up, Color.red, 2f);
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
                            UpdateState(States.Fighting);
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
                if(hit.transform.gameObject.isStatic)
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
        if(PathToGoal.corners.Length > 1)
        {
            HeadTo(PathToGoal.corners[1], true);

            if(Vector3.Distance(this.transform.position + HeadHeight, PathToGoal.corners[1]) < 3f)
                NavMesh.CalculatePath(this.transform.position, GetObjective(), NavInfo, PathToGoal);
        }
    }

    Vector3 GetObjective()
    {
        Vector3 Objective = Vector3.zero;

        if(Game.GameMode == GameModes.Defense)
        {
            Objective = Logic.L.Defense.transform.position;
            if(U.Team != Game.TeamOne && Logic.L.LastKnownTarget)
                Objective = Logic.L.LastKnownTarget.transform.position;
        }else if(Game.GameMode == GameModes.Conquest)
        {
            if(Time.time > LastGotObjective + DecideDelay && Vector3.Distance(this.transform.position, LastObjective) < Game.AttackerPushDistance)
            {
                Objective = Manager.M.GetConquestObjective(transform.position, U.Team);
                LastObjective = Objective;
                LastGotObjective = Time.time;
            }else
            {
                Objective = LastObjective;
            }
        }else if(Game.GameMode == GameModes.Hill)
        {
            Objective = Logic.L.Hill.transform.position;
        }

        return Objective;
    }

    void HeadTo(Vector3 Destination, bool Sprint)
    {
        LookAt(Destination + HeadHeight);
        Move(GetAvoidVector(), Sprint, false);
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

    Vector3 GetAvoidVector()
    {
        RaycastHit hit;
        Quaternion Dir;
        Vector3 Origin = this.transform.position + new Vector3(0f, 1f, 0f) + (this.transform.forward * -.2f);

        bool Left = false;
        bool Right = false;
        bool Forward = false;

        Dir = Quaternion.AngleAxis(30f, this.transform.up);
        if(Physics.Raycast(Origin, (Dir * this.transform.forward), out hit, 2f, Game.UnitOnlyMask))
            Right = true;
        Dir = Quaternion.AngleAxis(15f, this.transform.up);
        if(Physics.Raycast(Origin, (Dir * this.transform.forward), out hit, 2f, Game.UnitOnlyMask))
            Right = true;
        
        Dir = Quaternion.AngleAxis(-30f, this.transform.up);
        if(Physics.Raycast(Origin, (Dir * this.transform.forward), out hit, 2f, Game.UnitOnlyMask))
            Left = true;
        Dir = Quaternion.AngleAxis(-15f, this.transform.up);
        if(Physics.Raycast(Origin, (Dir * this.transform.forward), out hit, 2f, Game.UnitOnlyMask))
            Left = true;
        

        if(Physics.Raycast(Origin, this.transform.forward, out hit, 2f, Game.UnitOnlyMask))
        {
            Forward = true;
        }

        if(Left && Right && Forward)
        {
            return Vector3.zero;
        }
    
        if(Right)
        {
            Dir = Quaternion.AngleAxis(-40f, this.transform.up);
            return (Dir * this.transform.forward);
        }
        if(Left)
        {
            Dir = Quaternion.AngleAxis(40f, this.transform.up);
            return (Dir * this.transform.forward);
        }
        if(Forward)
        {
            Dir = Quaternion.AngleAxis(40f, this.transform.up);
            return (Dir * this.transform.forward);
        }

        return transform.forward;
    }
    
    /// <summary>
    /// Moves the Unit at a given speed. Can also tell the Unit to crouch still.
    /// </summary>
    /// <param name="Direction">The direction to move the Unit</param>
    /// <param name="Sprint"></param>
    /// <param name="Crouch"></param>
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
        UpdateState(States.Idle);
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

    void FindNewEnemy()
    {
        if(Enemy)
            if(!AI.LineOfSight(Chest.position, Enemy.gameObject, Enemy.Target.position))
                ForgetEnemy();
        
        if(!Enemy)
        {
            Enemy = AI.FindNewUnit(U, U.Capabilities, true);

            if(Enemy)
            {
                UpdateState(States.Fighting);

                if(Enemy.Team == Game.TeamOne)
                    Logic.L.LastKnownTarget = Enemy;
            }
        }
    }

    bool FarAway()
    {
        if(Order == Orders.Follow)
            return false;

        if(U.Team == Game.TeamTwo || Game.GameMode != GameModes.Defense)
        {
            if(Vector3.Distance(this.transform.position, GetObjective()) > Game.AttackerPushDistance)
                return true;
        }else
        {
            if(Vector3.Distance(this.transform.position, GetObjective()) > Game.DefenderStayDistance)
                return true;
        }

        return false;
    }
}