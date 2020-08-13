using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleShipController : MonoBehaviour
{
    // SHIP SPECIFIC
    public Ship ShipInfo;
    public bool PlayerControlled = false;
    public Vector3 AimTarget = Vector3.zero;
    public Vector3 MoveDirection = Vector3.zero;
    public Vector3 GunDirection = Vector3.zero;
    public bool ForwardGuns = true;
    bool CooledDown = true;
    Rigidbody r;
    ShipStats SS;

    // PLAYER SPECIFIC
    Transform CamEmpty;
    public float yaw = 0f;
    public float pitch = 0f;
    float MouseSens = 1f;
    float camLimits = 90f;
    bool FreeLookEnabled = false;
    float FreeLookYaw = 0f;
    float FreeLookPitch = 0f;

    // AI SPECIFIC
    ShipStats Target;
    bool GameFinished = false;
    public BattleMaker BM;

    // TO MOVE
    public GameObject TempProjectile;
    Transform GunPos;

    void Start()
    {
        SS = GetComponent<ShipStats>();
        r = GetComponent<Rigidbody>();
        GunPos = this.transform.GetChild(1).transform;
        if(PlayerControlled)
            CamEmpty = this.transform.GetChild(3).transform;
        else
            InvokeRepeating("FindTarget", Random.Range(5f, 10f), Random.Range(5f, 10f));

    }

    void FixedUpdate()
    {
        if(PlayerControlled)
        {
            if(CamEmpty == null)
                CamEmpty = this.transform.GetChild(3).transform;

            if(!SS.Dead)
                PlayerControls();
        }else
        {
            

            if(Target == null)
            {
                if(!GameFinished)
                    FindTarget();
            }else
            {
                float TargetDist = Vector3.Distance(this.transform.position, Target.transform.position);
                //if(TargetDist > SearchRadius)
                    //FindTarget();

                if(ForwardGuns)
                {
                    GunDirection = GunPos.forward;
                }else
                {
                    float GunDist = Vector3.Distance(GunPos.position, Target.transform.position);
                    GunDirection = (Target.transform.position - GunPos.position)/GunDist;
                }

                FaceTarget();

                float SlowDist = ShipInfo.StopDistance + Target.GetComponent<BattleShipController>().ShipInfo.StopDistance;

                if(SS.Shields > 0f)
                {
                    if(TargetDist > SlowDist)
                    {
                        MoveDirection = this.transform.forward;
                    }else
                    {
                        if((TargetDist/SlowDist) > 0.5f)
                        {
                            MoveDirection = this.transform.forward * (TargetDist/SlowDist);
                        }else
                        {
                            MoveDirection = -this.transform.forward;
                        }
                        
                        MoveDirection += this.transform.right * Random.Range(-1f, 1f);
                    }
                }
                else
                {
                    MoveDirection = -this.transform.forward;
                }

                MoveDirection += new Vector3(0f, Random.Range(-1f, 1f), 0f);

                Accelerate();

                if(TargetDist < (ShipInfo.WeaponsRange + Target.GetComponent<BattleShipController>().ShipInfo.WeaponsRange))
                {
                    float ran = Random.Range(0f, 20f);
                    if(ran < 1f)
                        Fire();
                }
            }

        }
    }

    void Update()
    {
        if(PlayerControlled && !SS.Dead)
        {   
            if(Input.GetMouseButton(0))
                Fire();
        }
    }

    void FaceTarget()
    {
        AimTarget = (Target.transform.position) - this.transform.position;
        AimTargetTurn();
    }


    void FindTarget()
    {   
        /*
        Collider[] hitColliders = Physics.OverlapSphere(this.transform.position, SearchRadius);
        List<ShipStats> PotentialTargets = new List<ShipStats>();
        for(int i = 0; i < hitColliders.Length; i++)
        {
            if(hitColliders[i].gameObject.GetComponent<ShipStats>() != null)
            {
                if(hitColliders[i].gameObject.GetComponent<ShipStats>().Team != SS.Team)
                {
                    PotentialTargets.Add(hitColliders[i].gameObject.GetComponent<ShipStats>());
                    //Target = hitColliders[i].gameObject.GetComponent<ShipStats>();
                    //i = hitColliders.Length; //break from loop
                }
            }
        }*/
        List<ShipStats> PotentialTargets;

        if(SS.Team == 0)
            PotentialTargets = BM.EnemyShips;
        else
            PotentialTargets = BM.FriendlyShips;

        if(PotentialTargets.Count == 0)
        {
            Target = null;
            GameFinished = true;
        }else
        {
            Target = PotentialTargets[Random.Range(0, PotentialTargets.Count)];
        }
    }

    public void Fire()//Index for weapon yadadada
    {
        if(SS.Energy >= ShipInfo.LaserCost && CooledDown)
        {
            CooledDown = false;
            SS.Drain(ShipInfo.LaserCost);
            GameObject Projectile = Instantiate(TempProjectile, GunPos.position, Quaternion.identity);
            Projectile.GetComponent<Projectile>().Damage = ShipInfo.LaserDamage;
            Projectile.GetComponent<Projectile>().Team = SS.Team;
            Projectile.transform.SetParent(this.transform.parent);
            //Projectile.GetComponent<Rigidbody>().AddForce(GunPos.forward * FireForce);
            Projectile.GetComponent<Rigidbody>().velocity = GunDirection * ShipInfo.LaserVelocity;
            Invoke("GunCool", ShipInfo.LaserCoolDownTime);
        }
    }

    void GunCool()
    {
        CooledDown = true;
    }

    void PlayerControls()
    {
        MoveDirection = Vector3.zero;

        if (Input.GetKey("w"))
            MoveDirection += transform.forward;

        if (Input.GetKey("a")) 
            MoveDirection += -transform.right;

        if (Input.GetKey("s")) 
            MoveDirection += -transform.forward;

        if (Input.GetKey("d")) 
            MoveDirection += transform.right;

        if (Input.GetKey("e")) 
            MoveDirection += Vector3.up;

        if (Input.GetKey("q")) 
            MoveDirection += -Vector3.up;

        
        Accelerate();

        yaw += MouseSens * Input.GetAxis("Mouse X");
        pitch -= MouseSens * Input.GetAxis("Mouse Y");
    
        if(pitch >= camLimits)
            pitch = camLimits;
            
        if(pitch <= -camLimits)
            pitch = -camLimits;

        CamEmpty.transform.eulerAngles = new Vector3(pitch, yaw, 0f);

        if(Input.GetKey(KeyCode.LeftAlt))
        {
            if(FreeLookEnabled == false)
            {
                FreeLookPitch = pitch;
                FreeLookYaw = yaw;
            }

            FreeLookEnabled = true;
        }else
        {
            if(FreeLookEnabled == true)
            {
                pitch = FreeLookPitch;
                yaw = FreeLookYaw;
            }


            FreeLookEnabled = false;
            AimTarget = CamEmpty.transform.forward;
        }

        if(ForwardGuns)
        {
            GunDirection = GunPos.forward;
        }else
        {
            GunDirection = AimTarget;
        }
        
        AimTargetTurn();

    }

    void Accelerate()
    {
        if(r.velocity.magnitude < 5f)      //global max speed
        {
            r.velocity += MoveDirection * ShipInfo.Speed;
        }
    }

    void AimTargetTurn()
    {
        if(AimTarget != Vector3.zero)
        {
            Quaternion newRotation = Quaternion.LookRotation(AimTarget, Vector3.up);
            this.transform.rotation = Quaternion.Slerp(this.transform.rotation, newRotation, Time.fixedDeltaTime * ShipInfo.TurnSpeed);
        }
    }
}
