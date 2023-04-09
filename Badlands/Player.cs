using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Player : MonoBehaviour
{
    Rigidbody _rigidbody;

    float MovementForce = 80f;
    float MaxSpeed = 10f;

    public bool Freezing = false;

    public bool SpawnedIn = false;

    public int Stamina = 100;
    public int StaminaDrainSpeed = 5;
    public int StaminaRechargeSpeed = 10;
    public int HealthRechargeSpeed = 10;
    public int StaminaRechargeDelay = 2;
    bool StaminaRecharging = false;
    IEnumerator StaminaRechargeCoroutine;
    bool StaminaHitZero = false;

    public Transform ShootDirection;

    public Animator Model;
    public Transform Hand;
    public Camera _camera;
    public Gun Primary;
    public Gun Secondary;
    Plane Floor;

    public static Player P;

    Life L;

    public Vector3 LastPosition;
    public Vector3 AimAt;

    void Awake()
    { 
        P = this;
    }

    void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
        L = GetComponent<Life>();
        Floor = new Plane(Vector3.up, new Vector3(0f, 1.25f, 0f));
        
        Application.targetFrameRate = 60;
        QualitySettings.vSyncCount = 1;
    }
    
    public void SpawnIn(Vector3 Position)
    {
        this.transform.position = Position;
        _rigidbody.isKinematic = false;
        Invoke("Ready", 0.5f);
    }
    
    public void Die()
    {
        Game.EndRun(false);
    }
    
    public bool PickupItem(Item I)
    {
        switch (I)
        {
            case Medicine:
            {
                if(Game.RunData.HealingMatter == Game.SaveData.MaxHealingMatter)
                    return false;
                
                Medicine Casted = I as Medicine;
                Game.RunData.HealingMatter += Casted.Health;
                if(Game.RunData.HealingMatter > Game.SaveData.MaxHealingMatter)
                    Game.RunData.HealingMatter = Game.SaveData.MaxHealingMatter;
                    
                return true;
            }
            case Valuable:
            {
                Valuable Casted = I as Valuable;
                
                if(Game.RunData.HoldingCredits == Game.SaveData.MaxHoldingCredits)
                    return false;
                
                Game.RunData.HoldingCredits += Casted.Value;
                Game.RunData.HoldingCredits = Mathf.Min(Game.RunData.HoldingCredits, Game.SaveData.MaxHoldingCredits);

                return true;
            }
            case Special :
            {
                Special Casted = I as Special;
                
                if(Casted.Type == Special.Types.FuelRod)
                {
                    Game.RunData.FuelRods++;
                    return true;
                }

                return false;
            }
            case Weapon:
            {
                bool PrimaryActive = true;
                if(!Primary)
                {
                    PrimaryActive = true;
                }else if(!Secondary)
                {
                    PrimaryActive = false;
                }else
                {
                    if(Primary.MR.enabled)
                        PrimaryActive = true;
                    else if(Secondary.MR.enabled)
                        PrimaryActive = false;
                }

                if(PrimaryActive && Primary)
                    Destroy(Primary.gameObject);
                if(!PrimaryActive && Secondary)
                    Destroy(Secondary.gameObject);
                    
                Weapon Casted = I as Weapon;

                GameObject NewWeapon = GameObject.Instantiate(Casted.Prefab, Vector3.zero, Quaternion.identity);
                NewWeapon.transform.SetParent(Hand.transform);
                NewWeapon.transform.localPosition = Vector3.zero;
                NewWeapon.transform.localEulerAngles = Vector3.zero;
                NewWeapon.GetComponent<Gun>().PlayerControlled = true;
                NewWeapon.GetComponent<Gun>().ShootDirection = ShootDirection;

                if(PrimaryActive) 
                    Primary = NewWeapon.GetComponent<Gun>();
                else
                    Secondary = NewWeapon.GetComponent<Gun>();
                    
                return true;
            }
        }

        return false;
    }
    
    void Update()
    {
        if (Game.RunData == null)
        {
            Model.gameObject.SetActive(false);
            return;
        }
        
        Model.gameObject.SetActive(true);
        

        if(Game.SaveData != null)
        {
            L.MaxHealth = Game.RunData.ActiveRaider.MaxHealth;
            L.MaxShields = Game.RunData.ActiveRaider.MaxShields;
        }
            
        NavMeshHit hit;
        if(NavMesh.SamplePosition(this.transform.position, out hit, 1f, 1))
            LastPosition = hit.position;
        
        Ray Clicked = _camera.ScreenPointToRay(Input.mousePosition);
        float Distance = 10f;
        if(Floor.Raycast(Clicked, out Distance))
        {
            AimAt = Clicked.GetPoint(Distance);
            //Make sure gun shoots in right direction
            Vector3 TargetDirection = Clicked.GetPoint(Distance) - ShootDirection.position;
            Quaternion Look = Quaternion.LookRotation(TargetDirection, Vector3.up);
            ShootDirection.rotation = Look;
            ShootDirection.localRotation = new Quaternion(0f, ShootDirection.localRotation.y, 0f, ShootDirection.localRotation.w);
            
            float Angle = Vector3.SignedAngle(Model.transform.forward, ShootDirection.forward, Vector3.up);
            Model.SetFloat("Degrees", Angle);
        }
        
        //Model.SetFloat("Speed", )
            
            
        if (Input.GetKey("q"))
        {
            if(Game.RunData.HealingMatter > 0)
            {
                int HealthNeeded = L.MaxHealth - L.Health;
                if (HealthNeeded != 0)
                {
                    if (HealthNeeded >= 10)
                    {
                        if (Game.RunData.HealingMatter >= 10)
                        {
                            L.Health += 10;
                            Game.RunData.HealingMatter -= 10;
                        }
                        else
                        {
                            L.Health += Game.RunData.HealingMatter;
                            Game.RunData.HealingMatter = 0;
                        }
                    }else
                    {
                        if(Game.RunData.HealingMatter > HealthNeeded)
                        {
                            L.Health = L.MaxHealth;
                            Game.RunData.HealingMatter -= HealthNeeded;
                        }else
                        {
                            L.Health += Game.RunData.HealingMatter;
                            Game.RunData.HealingMatter = 0;
                        }
                    }
                }
            }
        }
        
        /*
        if (Input.GetKeyDown(KeyCode.R))
        {
            Game.PurchasePlayerUpgrade(PlayerUpgradeType.IncreaseSpeed);
        }*/

        if(Primary)
        {
            if (Input.GetMouseButton(0))
            {
                Primary.MR.enabled = true;
                if(Secondary)
                    Secondary.MR.enabled = false;
            }

            if (Primary.Info.Firemode == FireModes.Semi)
            {
                if (Input.GetMouseButtonDown(0))
                    Primary.TryFire();
            }
            else
            {
                if(Input.GetMouseButton(0))
                    Primary.PullTrigger();
            }
        }
        
        if(Secondary)
        {
            if (Input.GetMouseButton(1))
            {
                if(Primary)
                    Primary.MR.enabled = false;
                Secondary.MR.enabled = true;
            }

            if (Secondary.Info.Firemode == FireModes.Semi)
            {
                if (Input.GetMouseButtonDown(1))
                    Secondary.TryFire();
            }
            else
            {
                if(Input.GetMouseButton(1))
                    Secondary.PullTrigger();
            }

        }
    }
    
    void Freeze()
    {
        if(Freezing)
            L.Hurt(50, DamageBonus.None);
    }
    
    void Ready()
    {
        InvokeRepeating("Freeze", 1f, 1f);
        SpawnedIn = true;
    }

    void FixedUpdate()
    {
        if(Game.RunData == null)
            return;
            
        Model.gameObject.SetActive(true);
        
        MovementControls();
    }
    
    void MovementControls()
    {
        Vector3 MoveDirection = Vector3.zero;
        float SprintMultiplier = 1f;
        if (Input.GetKey("w") || Input.GetKey("a") || Input.GetKey("s") || Input.GetKey("d"))
        {
            if (Input.GetKey("w"))
                MoveDirection += this.transform.forward;

            if (Input.GetKey("a"))
                MoveDirection += -this.transform.right;

            if (Input.GetKey("s"))
                MoveDirection += -this.transform.forward;

            if (Input.GetKey("d"))
                MoveDirection += this.transform.right;

            /*
            if (Physics.Raycast(this.transform.position + new Vector3(0f, 0.5f, 0f), MoveDirection, 1f))
            {
                Debug.DrawRay(this.transform.position + new Vector3(0f, 0.5f, 0f), MoveDirection * 1f, Color.red);
                return;
            }else
            {
                Debug.DrawRay(this.transform.position + new Vector3(0f, 0.5f, 0f), MoveDirection * 1f, Color.green);
            }*/

            if(Input.GetKey(KeyCode.LeftShift) && !StaminaHitZero)
            {
                SprintMultiplier = 1.5f;
                Stamina -= StaminaDrainSpeed;
                if(Stamina <= 0)
                {
                    Stamina = 0;
                    StaminaHitZero = true;
                }

                if(StaminaRecharging)
                    StopCoroutine(StaminaRechargeCoroutine);
                StaminaRechargeCoroutine = RechargeStamina();
                StartCoroutine(StaminaRechargeCoroutine);
            }

            if (MoveDirection != Vector3.zero)
            {
                Quaternion Look = Quaternion.LookRotation(MoveDirection, Vector3.up);
                Model.transform.rotation = Quaternion.Lerp(Model.transform.rotation, Look, Time.fixedDeltaTime * 4f);//speed
                Model.transform.localRotation = new Quaternion(0f, Model.transform.localRotation.y, 0f, Model.transform.localRotation.w);
            }
        }
        
        float lerp = Mathf.Lerp(1f, 0f, _rigidbody.velocity.magnitude / (MaxSpeed * SprintMultiplier));
        
        _rigidbody.AddForce((MoveDirection * MovementForce * SprintMultiplier) * lerp, ForceMode.Acceleration);
        
        Model.SetFloat("Speed", _rigidbody.velocity.magnitude / MaxSpeed);
        
    }
    
    IEnumerator RechargeStamina()
    {
        StaminaRecharging = true;
        yield return new WaitForSeconds(StaminaRechargeDelay);

        while(Stamina < Game.RunData.ActiveRaider.MaxStamina)
        {
            Stamina += StaminaRechargeSpeed;
            yield return null;
        }

        StaminaHitZero = false;
        StaminaRecharging = false;
    }
}
