using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;
using UnityEngine.AI;

public class Tank : MonoBehaviour
{
    public enum States
    {
        Idle,
        LookingAround,
        Fighting,
        GoingToObjective,
        Blownup
    };

    public Unit U;

    public States State;

    Rigidbody r;

    public Transform Turret;
    public Transform ToPitch;
    public Transform GunSlot;

    public float lookUpperLimit = 40f;
    public float lookLowerLimit = 40f;

    public bool PlayerUsingPrimary = true;
    public Weapon Primary;
    public Weapon Secondary;

    public Unit Enemy;

    Vector3 randomLook = Vector3.zero;

    public GameObject ExplodeEffect;

    public List<MeshRenderer> MRS;
    public List<SkinnedMeshRenderer> SMRS;
    public AudioSource AS;

    Vector3 LastObjective = Vector3.zero;

    public Transform CameraParent;
    public Transform Target;

    float MaxSpeed = 8f;
    float MovementForce = 750f;

    NavMeshPath PathToGoal;
    NavMeshQueryFilter NavInfo;

    float LastGotObjective = -100f;
    float DecideDelay = 20f;

    int ForgetCounter = 0;
    
    public float yaw = 0f;
    public float pitch = 0f;

    Vector3 HeadHeight = new Vector3(0f, 1.66f, 0f);

    public Transform WheelBoneParent;
    public Transform WheelModelParent;
    public Transform WheelColliderParent;

    List<Transform> WheelBones = new List<Transform>();
    List<Transform> WheelModels = new List<Transform>();
    List<WheelCollider> Wheels = new List<WheelCollider>();

    public bool Exploded = false;
    public GameObject FireEffect;

    void Start()
    {
        r = GetComponent<Rigidbody>();
        PathToGoal = new NavMeshPath();
        U = GetComponent<Unit>();

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

        DecideDelay = Random.Range(15f, 30f);
        LastObjective = this.transform.position;

        Primary.Holder = U;
        Secondary.Holder = U;

        foreach (MeshRenderer MR in MRS)
            MR.materials[0].SetColor("Team", U.Team == Game.TeamOne ? Game.FriendlyColor : Game.EnemyColor);
        foreach (SkinnedMeshRenderer SMR in SMRS)
            SMR.materials[0].SetColor("Team", U.Team == Game.TeamOne ? Game.FriendlyColor : Game.EnemyColor);

        if(State == States.Idle)
            UpdateState(States.Idle);

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

        InvokeRepeating("ChangeBehavior", 1f, Random.Range(3f, 5f));
    }

    void FixedUpdate()
    {   
        UpdateWheelModels();

        if(State == States.Blownup)
        {
            U.Targetable = false;
            Move(0);
            return;
        }

        if(!U.Controller)
        {

            if(Enemy != null)
            {
                if(AI.LineOfSight(GunSlot.transform.position, Enemy.gameObject, Enemy.Target.position))
                {
                    switch(State)
                    {
                        case States.Fighting :      S_Fighting();    break;
                    }
                }else
                {
                    switch(State)
                    {
                        case States.LookingAround :      S_LookingAround();    break;
                    }
                }
                
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

    void UpdateWheelModels()
    {   
        for(int i = 0; i < Wheels.Count; i++)
            UpdateWheelModel(Wheels[i], WheelModels[i], WheelBones[i]);

        float LeftTrack = 0f;
        float RightTrack = 0f;

        for(int i = 0; i < Wheels.Count/2; i++)
            LeftTrack += Wheels[i].rpm > 30f ? 1f : Wheels[i].rpm < -30f ? -1f : 0f;
        

        for(int i = Wheels.Count/2; i < Wheels.Count; i++)
            RightTrack += Wheels[i].rpm > 30f ? 1f : Wheels[i].rpm < -30f ? -1f : 0f;

        LeftTrack = LeftTrack >= Wheels.Count/2 ? 1.25f : LeftTrack <= -Wheels.Count/2 ? -1.25f: 0f;
        RightTrack = RightTrack >= Wheels.Count/2 ? 1.25f : RightTrack <= -Wheels.Count/2 ? -1.25f: 0f;

        SMRS[0].materials[0].SetFloat("Speed", LeftTrack);
        SMRS[1].materials[0].SetFloat("Speed", RightTrack);
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
                Manager.M.CreateTank(U.Team);
                
                if(U.Team == Game.TeamOne)
                    Game.Conquest_TeamOneTickets--;
                else
                    Game.Conquest_TeamTwoTickets--;
            }

            UpdateState(States.Blownup);
            StartCoroutine(Detonate());
        }
    }

    IEnumerator Detonate()
    {
        if(U.Controller)
            GameObject.FindWithTag("Camera").GetComponent<Player>().FindNewBody(false, false);

        yield return new WaitForSeconds(0.1f);
        ExplodeEffect.SetActive(true);
        ExplodeEffect.GetComponent<Explosion>().Explode(6f, 4f, 100, 1, false);


        AS.enabled = false;

        Despawn D = this.gameObject.AddComponent(typeof(Despawn)) as Despawn;
        D.DespawnTime = 10f;
        Destroy(this);
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
        Move(0);

        LookAt(Enemy.Target.position);
        if(AI.LinedUp(Enemy.Target.position, GunSlot.transform.position, GunSlot.transform.forward, 5f))
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
        if(PathToGoal.corners.Length > 1)
        {
            HeadTo(PathToGoal.corners[1]);

            if(Vector3.Distance(this.transform.position, PathToGoal.corners[1]) < 3f)
                NavMesh.CalculatePath(this.transform.position, GetObjective(), NavInfo, PathToGoal);
        }
    }

    void HeadTo(Vector3 Destination)
    {
        Face(Destination);
        LookAt(Destination + HeadHeight);

        Move(1);
    }

    public void Move(int Direction)
    {
        if(Direction != 0)
        {
            float lerp = Mathf.Lerp(1f, 0f, r.velocity.magnitude/MaxSpeed);

            for(int i = 0; i < Wheels.Count; i++)
            {
                Wheels[i].brakeTorque = 0f;
                Wheels[i].motorTorque = (MovementForce * (float) Direction) * lerp;
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
        float PitchSpeed = 1f;
        float SpinSpeed = 1f;

        if(U.Controller)
        {
            PitchSpeed *= 5f;
            SpinSpeed *= 5f;
        }

        Vector3 TargetDirection = Vector3.zero;
        Quaternion Look = Quaternion.identity;

        //PITCH
        TargetDirection = Pos - ToPitch.transform.position;
        Look = Quaternion.LookRotation(TargetDirection, Turret.transform.forward);
        ToPitch.transform.rotation = Quaternion.Lerp(ToPitch.transform.rotation, Look, Time.fixedDeltaTime * PitchSpeed);

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
        
        yaw = Turret.localRotation.eulerAngles.y;
        pitch = Vector3.SignedAngle(-Turret.transform.up, ToPitch.transform.forward, Turret.transform.right);
        //ToPitch.localRotation.eulerAngles.x;
        //if(pitch > 180f)
        //pitch -= 360f;
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

    void ChangeBehavior()
    {
        if(State == States.Blownup)
            return;

        if(!U.Controller)
        {
            FindNewEnemy();

            if(State != States.Fighting)
                UpdateState(GetState());

            ChangeRandom();

            if(State == States.GoingToObjective)
            {
                NavMesh.CalculatePath(this.transform.position, GetObjective(), NavInfo, PathToGoal);
            }
        }
    }

    void ForgetEnemy()
    {
        UpdateState(States.Idle);
        Enemy = null;
        ForgetCounter = 0;
    }

    void FindNewEnemy()
    {
        if(Enemy)
            if(!AI.LineOfSight( GunSlot.transform.position, Enemy.gameObject, Enemy.Target.position))
                ForgetCounter++;
        
        if(ForgetCounter > 3)
        {
            ForgetEnemy();
        }
        
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

    void ChangeRandom()
    {
        randomLook = new Vector3(Random.Range(-2f, 2f), Random.Range(-0.5f, 0.3f), Random.Range(-2f, 2f));
        randomLook.x += randomLook.x > 0 ? 5f : -5f;
        randomLook.z += randomLook.z > 0 ? 5f : -5f;
    }

    States GetState()
    {
        float choice = Random.value;
        States chosen = States.Idle;

        if(!Enemy)
        {
            if(choice > 0.4f)
                chosen = States.LookingAround;
            else
                chosen = States.Idle;

        }else
        {
            if(choice > 0.3f)
                chosen = States.Fighting;
        }

        if(FarAway())
            if(!Enemy)
                chosen = States.GoingToObjective;

        return chosen;
    }

    public void UpdateState(States S)
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
