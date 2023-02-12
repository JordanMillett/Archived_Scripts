using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;
using UnityEngine.AI;

public class Tank : AI
{
    public enum States
    {
        Idle,
        LookingAround,
        Fighting,
        GoingToObjective,
        Dead
    };

    public States State;
    
    public Transform Turret;
    public Transform ToPitch;
    public Transform GunSlot;

    public float lookUpperLimit = 40f;
    public float lookLowerLimit = 40f;

    public bool PlayerUsingPrimary = true;
    public Weapon Primary;
    public Weapon Secondary;

    public bool HeavyTank = true;

    Vector3 randomLook = Vector3.zero;

    public GameObject ExplodeEffect;
    
    public Transform CenterOfMass;

    public float MaxSpeed = 8f;
    public float MovementForce = 750f;

    NavMeshPath PathToGoal;
    NavMeshQueryFilter NavInfo;
    
    public float yaw = 0f;
    public float pitch = 0f;

    public Vector3 HeadHeight;

    public Transform WheelBoneParent;
    public Transform WheelModelParent;
    public Transform WheelColliderParent;

    List<Transform> WheelBones = new List<Transform>();
    List<Transform> WheelModels = new List<Transform>();
    List<WheelCollider> Wheels = new List<WheelCollider>();
    
    public GameObject ShootExplosion;
    public float EffectSize = 4f;
    public float ExplosionSize = 2f;
    public int ExplosionDamage = 100;
    public float SoundVolume = 1f;
    
    int PathIndex = 0;
    //bool Lost = false;
    Vector3 LastPos = Vector3.zero;

    public AudioSourceController Engine;
    public AudioSourceController TurretRing;

    float EngineAlpha = 0f;
    float PitchHigh = 1f;
    float PitchLow = 0.5f;

    void Start()
    {
        Initialize();
        StartCoroutine(WaitForGameStart());
    }
    
    IEnumerator WaitForGameStart()
    {
        while(!Game.Started)
            yield return null;
            
        UpdateObjective();
        NavMesh.CalculatePath(this.transform.position + new Vector3(0f, 0.25f, 0f), Objective, NavInfo, PathToGoal);
        PathIndex = 0;
        
        InvokeRepeating("ChangeBehavior", 1f, Random.Range(3f, 5f));
    }

    public override void Initialize()
    {
        base.Initialize();
        
        r.centerOfMass = CenterOfMass.localPosition;
        PathToGoal = new NavMeshPath();

        U.Capabilities.AntiInfantry = Primary.Info.UseCase.AntiInfantry || Secondary.Info.UseCase.AntiInfantry ? true : false;
        U.Capabilities.AntiArmor = Primary.Info.UseCase.AntiArmor || Secondary.Info.UseCase.AntiArmor ? true : false;
        U.Capabilities.AntiAir = Primary.Info.UseCase.AntiAir || Secondary.Info.UseCase.AntiAir ? true : false;

        for(int i = 0; i < WheelBoneParent.childCount; i++)
            WheelBones.Add(WheelBoneParent.GetChild(i));

        for(int i = 0; i < WheelModelParent.childCount; i++)
            WheelModels.Add(WheelModelParent.GetChild(i));

        for(int i = 0; i < WheelColliderParent.childCount; i++)
            Wheels.Add(WheelColliderParent.GetChild(i).GetComponent<WheelCollider>());

        NavInfo = new NavMeshQueryFilter();
        NavInfo.areaMask = 1;
        NavInfo.agentTypeID = -1372625422;
        
        LastPos = this.transform.position;
        InvokeRepeating("CheckStuck", 8f, 8f);

        Primary.Holder = U;
        Secondary.Holder = U;

        if(State == States.Idle)
            SetState(States.Idle);

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

        Engine.SetVolume(0f);
        Engine.Play();
    }

    public void ShootExplode()
    {
        GameObject Dec = Instantiate(ShootExplosion, Primary.FirePosition.position, Quaternion.identity);
        Dec.transform.rotation = Quaternion.LookRotation(Primary.FirePosition.forward, Vector3.up);
        Dec.GetComponent<Explosion>().Explode(EffectSize, ExplosionSize, ExplosionDamage, SoundVolume, U.Controller);
    }

    void Update()
    {
        Engine.SetPitch(Mathf.Lerp(PitchLow, PitchHigh, EngineAlpha));
        if(!Dead)
            Engine.SetVolume(0.4f);
        else
            Engine.SetVolume(0f);
    }

    void FixedUpdate()
    {
        for(int i = 0; i < Wheels.Count; i++)
            UpdateWheelModel(Wheels[i], WheelModels[i], WheelBones[i]);

        if(State == States.Dead)
        {
            U.Targetable = false;
            Move(0);
            return;
        }

        if(!U.Controller)
        {
            if(State == States.Fighting)
            {
                S_Fighting();
            }else
            {
                switch(State)
                {
                    case States.LookingAround :     S_LookingAround();     break;
                    case States.Idle :              S_Idling();                                 break;
                    case States.GoingToObjective :  S_GoingToObjective();                           break;
                }
            }
        }
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

    void UpdateWheelModel(WheelCollider Wheel, Transform Model, Transform Bone)
    {
        Vector3 Pos = Bone.position;
        Quaternion Rot = Model.rotation;

        Wheel.GetWorldPose(out Pos, out Rot);

        Bone.position = Pos;
        Model.position = Pos;
        Model.rotation = Rot;
    }

    public void Die()
    {   
        if(!Dead)
        {
            Dead = true;

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
                    Game.Defense_Money += Game.Defense_TankKillBonus;
                }

                
                if(U.Team == Game.TeamTwo)
                {
                    Game.Defense_Enemies--;
                }
                else
                {
                    Game.Defense_Allies--;
                }
                
            }else
            {
                Manager.M.DelaySpawnTank(U.Team, HeavyTank);
                
                if(U.Team == Game.TeamOne)
                    Game.Conquest_TeamOneTickets--;
                else
                    Game.Conquest_TeamTwoTickets--;
            }

            SetState(States.Dead);
            
            if(U.Controller)
                GameObject.FindWithTag("Camera").GetComponent<Player>().Died();
                
            ExplodeEffect.SetActive(true);
            if(HeavyTank)
                ExplodeEffect.GetComponent<Explosion>().Explode(6f, 4f, 100, 0.4f, false);
            else
                ExplodeEffect.GetComponent<Explosion>().Explode(4f, 3f, 100, 0.4f, false);
                
            MeshRenderer[] Meshes = this.gameObject.GetComponentsInChildren<MeshRenderer>();
            foreach (MeshRenderer M in Meshes)
            {
                Material[] DestroyedMaterials = M.materials;
                for (int i = 0; i < DestroyedMaterials.Length; i++)
                    DestroyedMaterials[i] = Manager.M.DestroyedMaterial;
                M.materials = DestroyedMaterials;
            }
            
            SkinnedMeshRenderer[] SkinnedMeshes = this.gameObject.GetComponentsInChildren<SkinnedMeshRenderer>();
            foreach (SkinnedMeshRenderer M in SkinnedMeshes)
            {
                Material[] DestroyedMaterials = M.materials;
                for (int i = 0; i < DestroyedMaterials.Length; i++)
                    DestroyedMaterials[i] = Manager.M.DestroyedMaterial;
                M.materials = DestroyedMaterials;
            }
            
            U.Targetable = false;
            Move(0);

            Despawn D = this.gameObject.AddComponent(typeof(Despawn)) as Despawn;
            D.DespawnTime = 10f;
            //Destroy(this);
       
        }
    }

    void S_Idling()
    {
        Move(0);
    }

    public void FireCannon()
    {
        Primary.PullTrigger();
    }

    public void FireMG()
    {
        Secondary.PullTrigger();
    }

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
        
        Move(0);

        LookAt(Enemy.Target.position);
        if(LinedUp(Enemy.Target.position, GunSlot.transform.position, GunSlot.transform.forward, 5f))
        {
            if(Enemy.Type == Unit.Types.Infantry)
                FireMG();
            else
                FireCannon();
        }
    }

    void S_LookingAround()
    {
        Move(0);

        LookAt(randomLook + HeadHeight);
    }
    
    void S_GoingToObjective()
    {
        if(PathToGoal.status == NavMeshPathStatus.PathComplete)
        {
            //Lost = false;
            if(PathIndex == PathToGoal.corners.Length - 1)
            {
                HeadTo(Objective);
            }else
            {
                HeadTo(PathToGoal.corners[PathIndex]);
                if(Vector3.Distance(this.transform.position, PathToGoal.corners[PathIndex]) < 2f)  //reached path corner
                    PathIndex++;              //go to next one
            }
        }else
        {
            //Lost = true;
            Die();
        }
    }

    void HeadTo(Vector3 Destination)
    {
        Face(Destination);
        LookAt(Destination + HeadHeight);

        RaycastHit hit;
        if (!Physics.Raycast(CenterOfMass.transform.position, this.transform.forward, out hit, 5f, Game.IgnoreSelectMask))
        {
            Move(1);
        }else
        {
            Move(0);
        }
        
    }

    public void Move(int Direction)
    {
        if(Direction == 1)
            EngineAlpha = Mathf.Lerp(EngineAlpha, 1f, Time.fixedDeltaTime);
        else
            EngineAlpha = Mathf.Lerp(EngineAlpha, 0f, Time.fixedDeltaTime);

        if(Direction != 0)
        {
            float lerp = Mathf.Lerp(1f, 0f, r.velocity.magnitude/MaxSpeed);

            for(int i = 0; i < Wheels.Count; i++)
            {
                Wheels[i].brakeTorque = 0f;
                if(Wheels[i].isGrounded)
                    Wheels[i].motorTorque = (MovementForce * (float) Direction) * lerp;
                else
                    Wheels[i].motorTorque = 0f;
            }
        }else
        {
            for(int i = 0; i < Wheels.Count; i++)
            {
                Wheels[i].brakeTorque = 100f;
                Wheels[i].motorTorque = 0f;
            }
        }
    }

    public void LookAt(Vector3 Pos)
    {
        float PitchSpeed = HeavyTank ? 1f : 2f;
        float SpinSpeed = HeavyTank ? 1f : 2f;

        if(U.Controller)
        {
            PitchSpeed = 5f;
            SpinSpeed = 5f;
        }

        Vector3 TargetDirection = Vector3.zero;
        Quaternion Look = Quaternion.identity;


        //PITCH
        TargetDirection = Pos - ToPitch.transform.position;
        Look = Quaternion.LookRotation(TargetDirection, Turret.transform.forward);
        ToPitch.transform.rotation = Quaternion.Lerp(ToPitch.transform.rotation, Look, Time.fixedDeltaTime * PitchSpeed);
        //if(U.Controller)
        //Debug.Log(Quaternion.Angle(ToPitch.transform.rotation, Look));
        //float ang = Quaternion.Angle(ToPitch.transform.rotation, Look);
        //ang -= 1f;
        //ang = Mathf.Max(ang, 0f);

        //TurretRing.SetVolume(Mathf.Lerp(0f, 0.4f, ang/50f));

        float Angle = ToPitch.transform.localRotation.x;
        
        if(Angle < 0.60f)
        {
            Angle = 0.60f;
        }
        if(Angle > 0.75f)
        {
            Angle = 0.75f;
        }
        
        ToPitch.transform.localRotation = new Quaternion(Angle, 0f, 0f, ToPitch.transform.localRotation.w);

        //TURNING
        TargetDirection = Pos - Turret.transform.position;
        Look = Quaternion.LookRotation(TargetDirection, this.transform.up);
        Turret.transform.rotation = Quaternion.Lerp(Turret.transform.rotation, Look, Time.fixedDeltaTime * SpinSpeed);
        Turret.transform.localRotation = new Quaternion(0f, 0f, Turret.transform.localRotation.z, Turret.transform.localRotation.w);
        
        //if(U.Controller)
            //Debug.Log(Quaternion.Angle(Turret.transform.rotation, Look));
        //TurretRing.SetVolume(Mathf.Lerp(0f, 0.4f, Quaternion.Angle(Turret.transform.rotation, Look)));

        yaw = Turret.localRotation.eulerAngles.y;
        pitch = Vector3.SignedAngle(-Turret.transform.up, ToPitch.transform.forward, Turret.transform.right);
    }

    public void Face(Vector3 Pos)
    {
        float TurnSpeed = 0.5f;

        Vector3 TargetDirection = Vector3.zero;
        Quaternion Look = Quaternion.identity;

        //TURNING
        TargetDirection = Pos - this.transform.position;
        Look = Quaternion.LookRotation(TargetDirection, this.transform.up);
        Look = Quaternion.Euler(this.transform.rotation.eulerAngles.x, Look.eulerAngles.y, this.transform.rotation.eulerAngles.z);
        this.transform.rotation = Quaternion.Lerp(this.transform.rotation, Look, Time.fixedDeltaTime * TurnSpeed);
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
  
        if(choice > 0.4f)
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

    void ForgetEnemy()
    {
        //SetState(States.Idle);
        Enemy = null;
    }

    void ChangeRandom()
    {
        randomLook = new Vector3(Random.Range(-2f, 2f), Random.Range(-0.5f, 0.3f), Random.Range(-2f, 2f));
        randomLook.x += randomLook.x > 0 ? 5f : -5f;
        randomLook.z += randomLook.z > 0 ? 5f : -5f;
    }

    public void SetState(States S)
    {
        State = S;
        if(!U.Controller)
        {
            string letter = Manager.M.Factions[(int) U.Team].Prefix;

            string UnitName = "tank";
            this.gameObject.name = letter + " - " + UnitName + " - " + State.ToString().ToLower();
        }else
        {
            this.gameObject.name = "Player";
        }
    }
    
    bool FarAway()
    {
        if(U.Team == Game.TeamTwo || Game.GameMode != GameModes.Defense)
        {
            if(Vector3.Distance(this.transform.position, Objective) > (Game.AttackerPushDistance * 2f))
                return true;
        }else
        {
            if(Vector3.Distance(this.transform.position, Objective) > Game.DefenderStayDistance)
                return true;
        }

        return false;
    }
}
