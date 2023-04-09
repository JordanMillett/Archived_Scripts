using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    public enum States
    {
        Idle,
        Chasing
    };
    
    public enum Types
    {
        Regular,
        Rusher,
        Sniper
    }

    public Faction F;

    public string Name = "";

    public int Level = 1;

    public States State;
    
    Rigidbody _rigidbody;

    float MovementForce = 80f;
    public float WalkSpeed = 6f;

    public Types Type;

    NavMeshQueryFilter NavInfo;
    NavMeshPath PathToGoal;
    int PathIndex = 0;
    
    [HideInInspector]
    public Gun Primary;

    public List<Weapon> PossibleWeapons;

    public Transform ShootDirection;
    public Animator Model;
    public Transform Hands;

    public Transform PopupTrack;

    Popup_Enemy PopupReference;

    bool PlayerSpotted = false;

    public int Style = 1;

    void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();

        PathToGoal = new NavMeshPath();
        NavInfo = new NavMeshQueryFilter();
        NavInfo.areaMask = 1;
        NavInfo.agentTypeID = 0;

        NavMesh.CalculatePath(this.transform.position, Player.P.LastPosition, NavInfo, PathToGoal);
        PathIndex = 0;

        EquipWeapon(PossibleWeapons[Random.Range(0, PossibleWeapons.Count)]);

        Life L = this.transform.GetComponent<Life>();
        L.MaxHealth = F.GetHealth(Level);
        L.Health = F.GetHealth(Level);
        L.MaxShields = F.GetShields(Level);
        L.Shields = F.GetShields(Level);

        PopupReference = UIManager.UI.M_Game.CreateEnemyPopup();
        PopupReference.Target = PopupTrack;
        PopupReference.E = this;
        PopupReference.L = GetComponent<Life>();

        Model.SetFloat("Style", Style);

        SetState(States.Idle);
        InvokeRepeating("ChangeBehavior", 2f, 2f);
    }

    void FixedUpdate()
    {
        switch(State)
        {
            case States.Idle :         S_Idle();     break;
            case States.Chasing :         S_Chasing();     break;
        }
    }
    
    public void EquipWeapon(Weapon W)
    {
        GameObject NewWeapon = GameObject.Instantiate(W.Prefab, Vector3.zero, Quaternion.identity);
        NewWeapon.transform.SetParent(Hands.transform);
        NewWeapon.transform.localPosition = Vector3.zero;
        NewWeapon.transform.localEulerAngles = Vector3.zero;
        
        Primary = NewWeapon.GetComponent<Gun>();
        NewWeapon.GetComponent<Gun>().ShootDirection = ShootDirection;
    }
    
    void Update()
    {
        float SpotDistance = 20f;
        
        float FireDistance = 15f;
        if(Type == Types.Rusher)
            FireDistance = 12f;
        if(Type == Types.Sniper)
            FireDistance = 18f;
        
        if (Vector3.Distance(this.transform.position, Player.P.transform.position) < SpotDistance)
        {
            Vector3 dir = Player.P.transform.position - this.transform.position;
            RaycastHit hit;
            if (Physics.Raycast(this.transform.position + new Vector3(0f, 2.1f, 0f), dir.normalized, out hit, SpotDistance))
            {
                if (hit.transform.root.gameObject == Player.P.gameObject)
                {
                    if(!PlayerSpotted)
                        Invoke("React", 0.5f);//reaction time to initial player spotting

                    if(Vector3.Distance(this.transform.position, Player.P.transform.position) < FireDistance)
                        if(PlayerSpotted)
                            Primary.PullTrigger();
                    
                    if(PopupReference)
                        PopupReference.gameObject.SetActive(true);
                }
            }
        }else
        {
            if(PopupReference)
                PopupReference.gameObject.SetActive(false);
        }

        //Hands.LookAt(Player.P.transform.position + new Vector3(0f, 1.25f, 0f));
        
        //Make sure gun shoots in right direction
        Vector3 TargetDirection = Player.P.ShootDirection.position - ShootDirection.position;
        Quaternion Look = Quaternion.LookRotation(TargetDirection, Vector3.up);
        ShootDirection.rotation = Look;
        ShootDirection.localRotation = new Quaternion(0f, ShootDirection.localRotation.y, 0f, ShootDirection.localRotation.w);
        
        float Angle = Vector3.SignedAngle(Model.transform.forward, ShootDirection.forward, Vector3.up);
        Model.SetFloat("Degrees", Angle);
    }
    
    void React()
    {
        PlayerSpotted = true;
    }
    
    public void OnHurt()
    {
        if(PopupReference)
            PopupReference.transform.SetAsLastSibling();
    }
    
    public void Die()
    {
        if(PopupReference)
            Destroy(PopupReference.gameObject);
        Destroy(this.gameObject);
    }
    
    public void ChangeBehavior()
    {
        if (State == States.Chasing)
        {
            NavMesh.CalculatePath(this.transform.position, Player.P.LastPosition, NavInfo, PathToGoal);
            PathIndex = 0;
        }
        
        SetState(GetState());
    }
    
    public void SetState(States S)
    {
        State = S;
        this.gameObject.name = Level + " Enemy" + " - " + State.ToString().ToLower();
    }
    
    States GetState()
    {
        return States.Chasing;
    }
    
    void S_Chasing()
    {
        if(PathToGoal.status == NavMeshPathStatus.PathComplete)
        {
            if(PathIndex == PathToGoal.corners.Length - 1)
            {
                float CloseTo = 8f;
                if(Type == Types.Rusher)
                    CloseTo = 5f;
                if(Type == Types.Sniper)
                    CloseTo = 11f;

                if (Vector3.Distance(this.transform.position, Player.P.transform.position) > CloseTo)
                {
                    HeadTo(Player.P.transform.position);
                }
                else
                {
                    Vector3 BackupDirection = (this.transform.position - Player.P.transform.position).normalized;
                    HeadTo(this.transform.position + BackupDirection);
                }
            }else
            {
                HeadTo(PathToGoal.corners[PathIndex]);
                if(Vector3.Distance(this.transform.position, PathToGoal.corners[PathIndex]) < 2f)  //reached path corner
                    PathIndex++;              //go to next one
            }
        }else
        {
            S_Idle();
        }
    }
    
    void HeadTo(Vector3 Destination)
    {
        Vector3 TargetDirection = Destination - this.transform.position;
        Quaternion Look = Quaternion.LookRotation(TargetDirection, Vector3.up);
        this.transform.rotation = Quaternion.Lerp(this.transform.rotation, Look, Time.fixedDeltaTime * 3f);//speed
        this.transform.localRotation = new Quaternion(0f, this.transform.localRotation.y, 0f, this.transform.localRotation.w);
            
        float lerp = Mathf.Lerp(1f, 0f, _rigidbody.velocity.magnitude / WalkSpeed);
        _rigidbody.AddForce((this.transform.forward * MovementForce) * lerp, ForceMode.Acceleration);
        
        Model.SetFloat("Speed", 1f - lerp);
    }
    
    void S_Idle()
    {
        
    }
}
