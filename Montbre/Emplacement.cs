using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Emplacement : MonoBehaviour
{
    public WeaponInfo PrimaryInfo;

    public Transform Spin;
    public Transform GunSlot;
    public Transform Sync;

    [HideInInspector]
    public Weapon Equipped;

    public Infantry User;

    public float pitchLimit = 15f;
    public float yawLimit = 30f;

    public float pitch = 0f;
    public float yaw = 0f;

    Collider C;

    bool InUse = false;

    Infantry Enemy;
    Plane EnemyPlane;

    public bool Occupied = false;

    void Start()
    {
        C = GetComponent<Collider>();

        if(PrimaryInfo != null)
        {
            Equipped = PrimaryInfo.CreateWeapon(GunSlot, false);
        }

        InvokeRepeating("ChangeBehavior", 1f, Random.Range(1f, 2f));
    }

    void Update()
    {
        if(InUse)
        {
            if(User != null)
                User.transform.position = Sync.transform.position;
            else
                Toggle(null);

            if(!User.U.Controller)
            {
                Target(PrimaryInfo.Flak);
            }
        }
    }

    void ChangeBehavior()
    {
        if(User != null)
        {
            if(!User.U.Controller)
            {
                FindNewEnemy();
                if(EnemyPlane == null)
                    EnemyPlane = AI.FindNewPlane(GunSlot.position, User.U.Team, Equipped.Info.FlakDistanceMax * 1.5f);
            }
        }
    }

    void FindNewEnemy()
    {
        if(Enemy != null)
            if(!AI.LineOfSight(GunSlot.position, Enemy.gameObject, Enemy.Chest.position))
                Enemy = null;
        
        if(Enemy == null)
        {
            Enemy = AI.FindNewInfantry(GunSlot.position, User.U.Team, Game.DetectDistance, true);
        }
    }

    void Target(bool Flak)
    {
        if(!Flak)
        {
            if(Enemy != null)
            {
                LookAt(Enemy.Chest.position);
                if(AI.LinedUp(Enemy.Chest.position, GunSlot.position, GunSlot.forward, 5f) && LineOfSight(Enemy))
                    Equipped.PullTrigger();
            }
        }else
        {
            if(EnemyPlane != null)
                if(EnemyPlane.State == Plane.States.Falling)
                    EnemyPlane = null;

            if(EnemyPlane != null)
            {
                
                Vector3 Target = EnemyPlane.transform.position + (EnemyPlane.transform.forward * (EnemyPlane.CurrentSpeed/2f));
                LookAt(Target);
                if(AI.LinedUp(Target, GunSlot.position, GunSlot.forward, 15f) && Vector3.Distance(this.transform.position, EnemyPlane.transform.position) < Equipped.Info.FlakDistanceMax * 1.25f)
                    Equipped.PullTrigger();
            }
        }
    }

    void LookAt(Vector3 Pos)
    {
        float PitchSpeed = 5f;
        float TurnSpeed = 5f;

        Vector3 TargetDirection = Vector3.zero;
        Quaternion Look = Quaternion.identity;

        //PITCH
        TargetDirection = Pos - Equipped.transform.position;
        Look = Quaternion.LookRotation(TargetDirection, this.transform.right);
        Look = Quaternion.Euler(Look.eulerAngles.x, GunSlot.rotation.eulerAngles.y, GunSlot.rotation.eulerAngles.z);
        GunSlot.transform.rotation = Quaternion.Lerp(GunSlot.transform.rotation, Look, Time.fixedDeltaTime * PitchSpeed);

        //TURNING
        TargetDirection = Pos - Spin.transform.position;
        Look = Quaternion.LookRotation(TargetDirection, this.transform.up);
        Look = Quaternion.Euler(Spin.transform.rotation.eulerAngles.x, Look.eulerAngles.y, Spin.transform.rotation.eulerAngles.z);
        Spin.transform.rotation = Quaternion.Lerp(Spin.transform.rotation, Look, Time.fixedDeltaTime * TurnSpeed);

        yaw = this.transform.localRotation.eulerAngles.y;
        pitch = GunSlot.transform.localRotation.eulerAngles.x;
    }
    
    void FindNewEnemyPlane()
    {
        if(EnemyPlane)
            if(Vector3.Distance(this.transform.position, EnemyPlane.transform.position) > 500)
                EnemyPlane = null;

        if(EnemyPlane == null)
        {
            Collider[] Nearby = Physics.OverlapSphere(this.transform.position, 500f);
            foreach(Collider Col in Nearby)
            {
                if(EnemyPlane != null)
                    break;

                try 
                {   
                    Plane I = Col.transform.root.gameObject.GetComponent<Plane>();

                    if(I != null)
                        if(I.State != Plane.States.Falling)
                            EnemyPlane = I;//if(I.Type == Types.Fighter)
                }
                catch{}
            }
        }
    }

    bool LineOfSight(Infantry E)
    {
        bool See = false;

        Vector3 Dir = E.Chest.transform.position - Equipped.transform.position;

        float CheckDistance = Game.DetectDistance;
        //if(E.State == Infantry.States.DroppingIn)
           // CheckDistance /= 3f;

        RaycastHit hit;             
        if(Physics.Raycast(Equipped.transform.position, Dir.normalized, out hit, CheckDistance, Game.IgnoreSelectMask))
            if(hit.transform.root.gameObject == E.transform.root.gameObject)
                See = true;        

        return See;
    }

    public void Toggle(Infantry U)
    {
        if(!InUse)
        {
            C.enabled = false;
            User = U;
            InUse = true;
        }else
        {
            //if(U != null)
                //U.Turret = null;

            C.enabled = true;
            InUse = false;
            Occupied = false;
        }
    }
}
