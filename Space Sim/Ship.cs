using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Ship : MonoBehaviour
{
    Transform World_Space;
    Transform Docking_Point;
    GameObject Player;
    PlayerController PlayerCont;
    int currentLightMode = 1;
    bool AlertMode = false;
    float ShieldRechargeDelay = 5f;
    Coroutine Charge;
    bool Docking = false;
    Transform Docking_Target;
    float lerp = 1f;
    bool ShipDestroyed = false;

    public string Name = "Blank";
    public float Max_Hull = 100;
    public float Hull = 100;
    public float Max_Shields = 100;
    public float Shields = 100;
    public float Max_Fuel = 100f;
    public float Fuel = 100f;
    public float Max_Power = 100;
    public float Power = 100;
	public float Max_Oxygen = 100;
	public float Oxygen = 100;
    //fuel efficiancy, power efficiancy, turnspeed, misc, idle power usage


    public int Max_Speed = 100;
    public float Speed = 0;
    public float TurnSpeed = 20f;
    public float Acceleration = 1f;


    public bool canDock = false;


    public Ship_Notifications Note;
    public GameObject[] Cargo;
    Ship_Gun[] Weapons;
    Light[] Lights;
    public PilotChair Chair;

    public float CurrentTurnSpeed = 0f;
    public Vector3 TurnVector = Vector3.zero;

    void Start()
    {
        World_Space = GameObject.FindWithTag("Space").transform;
        Docking_Point = this.transform.GetChild(2).transform;
        Docking_Target = this.transform.GetChild(3).transform;
        Docking_Target.transform.SetParent(World_Space);
        Lights = transform.GetChild(0).gameObject.GetComponentsInChildren<Light>();
        Weapons = transform.GetChild(1).gameObject.GetComponentsInChildren<Ship_Gun>();
        InvokeRepeating("IdlePowerUse",5f,5f);
        Charge = StartCoroutine(Recharge());
        Player = GameObject.FindWithTag("Player");
        PlayerCont = Player.GetComponent<PlayerController>();
    }

    void Update()
    {

        MessageSystem();

        
    }

    void FixedUpdate()
    {
        //Accelerate();
        if(Chair.Enabled)
            Pilot_Movement();

        if(Docking)
            Dock();
            
        Move();
        Rotate();

        /* 
        if(Speed > (Max_Speed * .95f))
        {

            StartCoroutine(PlayerCont.ScreenShake(0.03f,.1f));

        }
        */
    }

    void Move()
    {
        if(Speed > 0)
            World_Space.transform.position = (World_Space.transform.position + (-transform.forward * Speed) * Time.fixedDeltaTime);

    }

    public void Accelerate()
    {
        if(Speed < Max_Speed && Fuel > 0)
        {
            Speed += Acceleration;
            Fuel -= Acceleration;
        }
    }

    public void Decelerate()
    {
        if(Speed > 0 && Fuel > 0)
        {
            Speed -= Acceleration;
            Fuel -= Acceleration;
        }
    }

    public void Turn(float Amount)
    {

        if(Fuel > 0)
        {
            World_Space.transform.RotateAround(this.transform.position, Vector3.up, -TurnSpeed * Amount * Time.fixedDeltaTime);
            Fuel--;

        }

        /* 
        if(Right)
        {
            if(Fuel > 0)
            {
                World_Space.transform.RotateAround(this.transform.position, Vector3.up, -TurnSpeed * Time.fixedDeltaTime);
                Fuel--;
            }

        }
        else
        {
            if(Fuel > 0)
            {
                World_Space.transform.RotateAround(this.transform.position, Vector3.up, TurnSpeed * Time.fixedDeltaTime);
                Fuel--;
            }
        }
        */
    }

    public void Pitch(float Amount)
    {

        if(Fuel > 0)
        {
            World_Space.transform.RotateAround(this.transform.position, Vector3.right, TurnSpeed * Amount * Time.fixedDeltaTime);
            Fuel--;

        }

        /* 
        if(Up)
        {
            if(Fuel > 0)
            {
                World_Space.transform.RotateAround(this.transform.position, Vector3.right, -TurnSpeed * Time.fixedDeltaTime);
                Fuel--;
            }

        }
        else
        {
            if(Fuel > 0)
            {
                World_Space.transform.RotateAround(this.transform.position, Vector3.right, TurnSpeed * Time.fixedDeltaTime);
                Fuel--;
            }
        }
        */

    }
    
    public void Roll(bool Right)
    {

        if(Right)
        {
            if(Fuel > 0)
            {
                World_Space.transform.RotateAround(this.transform.position, Vector3.forward, -TurnSpeed * Time.fixedDeltaTime);
                Fuel--;
            }

        }
        else
        {
            if(Fuel > 0)
            {
                World_Space.transform.RotateAround(this.transform.position, Vector3.forward, TurnSpeed * Time.fixedDeltaTime);
                Fuel--;
            }
        }

    }

    void Rotate()
    {
        if(CurrentTurnSpeed != 0f)
            World_Space.transform.RotateAround(this.transform.position, TurnVector, CurrentTurnSpeed * Time.fixedDeltaTime);

    }

    void ShipControls()
    {

        if(Fuel > 0)
        {

            float yaw = Input.GetAxis ("Mouse X");
		    float pitch = Input.GetAxis ("Mouse Y");
            
            //if(yaw == 0 && pitch == 0)
                //CurrentTurnSpeed--;
            //else if(CurrentTurnSpeed < TurnSpeed)
                //CurrentTurnSpeed++;

            CurrentTurnSpeed = TurnSpeed;

            TurnVector = new Vector3(pitch,-yaw,0f);


            //World_Space.transform.RotateAround(this.transform.position, Vector3.right, TurnSpeed * pitch * Time.fixedDeltaTime);
            Fuel--;

        }

    }

    void Pilot_Movement()
    {

        //float yaw = Input.GetAxis ("Mouse X");
		//float pitch = Input.GetAxis ("Mouse Y");

        //float yaw = Input.GetAxis ("Horizontal");
		//float pitch = Input.GetAxis ("Vertical");

        //float deadzone = 1f;

        ShipControls();

        //if(Mathf.Abs(pitch) > deadzone)
            //Pitch(pitch);

        //if(Mathf.Abs(yaw) > deadzone)
            //Turn(yaw);

        /* 
        if(pitch < 0f)
            Pitch(true);

        if(pitch > 0f)
            Pitch(false);
        */
        /* 

        if(yaw > 0f)
            Turn(true);

        if(yaw < 0f)
            Turn(false);

        */

        //set dead zones instead of 0f
        //if(Input.GetKey("space"))
        //if(Input.mouseScrollDelta.y > 0)
        if(Input.GetKey("w"))
            Accelerate();
        

        
        //if(Input.GetKey(KeyCode.LeftShift))
        //if(Input.mouseScrollDelta.y < 0)
        if(Input.GetKey("s"))
            Decelerate();

        
        //if(Input.GetAxis ("Mouse X") > 1f)
        //if(Input.GetKey("q"))
        if(Input.GetKey("a"))
            Roll(true);

        
        //if(Input.GetAxis ("Mouse X") < 1f)
        //if(Input.GetKey("e"))
        if(Input.GetKey("d"))
            Roll(false);

        if(Input.GetMouseButton(0))
            FireAllWeapons();

    }

	public void Jump(int P)
	{
		if(Power >= P)
		{
			Power -= P;
			World_Space.transform.position = (World_Space.transform.position + (-transform.forward * 20000f));
		}

	}

    void MessageSystem()
    {
        if(Shields < Max_Shields/2)         //SHIELDS
        {
            Note.Alert("Shields Low!",6);
        }

        if(Power < Max_Power/5)             //POWER
        {
            Note.Alert("Power Critical!",6);
            if(AlertMode == false)
                ToggleLights(2);
            AlertMode = true;
            
        }else if(Power < Max_Power/2)
        {
            Note.Alert("Power Low",5);
        }

        if(Hull < Max_Hull/2)               //HULL
        {
            Note.Alert("HULL CRITICAL!",10);
            if(AlertMode == false)
                ToggleLights(2);
            AlertMode = true;
        }

        if(Hull > Max_Hull/2 && Power > Max_Power/5) //Stop Alert Mode
        {
            if(AlertMode == true)
            {
                AlertMode = false;
                ToggleLights(1);
            }
            
        }
        
        
    }

    void FireAllWeapons()
    {

        foreach(Ship_Gun SW in Weapons)
        {

            SW.Fire();

        }

    }

    public void Damage(float Amount)
    {
        StopCoroutine(Charge);

        StartCoroutine(PlayerCont.ScreenShake(Amount * 0.001f,.4f));

        if(Shields == 0)
        {
            Hull -= Amount;
        }else
        {

            if(Shields >= Amount)
                Shields -= Amount;
            else if(Shields < Amount)
                Shields = 0;
        }

        if(Shields < Max_Shields)
            Charge = StartCoroutine(Recharge());

        if(Hull <= 0)
            StartCoroutine(Detonate());

    }

    public void Crash(float Amount, bool StopsShip, float SpeedThreshold)
    {
        if(Speed > SpeedThreshold)
        {
            Note.Alert("CRASHED!",9);

            Damage(Speed * Amount);

            if(StopsShip)
                Speed = 0;
        }   

    }

    IEnumerator Recharge()
    {

        yield return new WaitForSeconds(ShieldRechargeDelay);
        float amount = Max_Shields - Shields;
        for(int i = 0; i < amount;i++)
        {
            yield return null;
            Shields++;
        }

    }

    void IdlePowerUse()
    {
        if(Power > 0)
        {
            if(currentLightMode == 1)
                if(Power >= Lights.Length)
                    Power -= Lights.Length;
                else
                    Power = 0;
            else if(currentLightMode == 2)
                Power--;
        }

    }

    public void ToggleLights(int state)
    {

        if(AlertMode)
            state = 2;

        if(state != currentLightMode)
        {

            currentLightMode = state;
            foreach (Light l in Lights)
            {
                switch(state)
                {

                    case 0 : l.color = Color.black; break;
                    case 1 : l.color = Color.white; break;
                    case 2 : l.color = Color.red; break;

                }

            }
        }
        

    }

    IEnumerator Detonate()
    {

        if(ShipDestroyed == false)
        {
            ShipDestroyed = true;

            Note.Alert("10",999);
            StartCoroutine(PlayerCont.ScreenShake(0.05f,10f));

            for(int i = 9; i > -1; i--)
            {
                yield return new WaitForSeconds(1f);
                Note.Alert(i.ToString(),999);
                if(i == 0)
                {
                    foreach(Transform Child in transform)
                        Destroy(Child.gameObject);

                }
            }

        }
        

    }


	//DOCKING STUFF

    public void MoveDockingTarget(Transform T)
    {
        Docking_Target.position = T.position;
        Docking_Target.rotation = Quaternion.Euler(T.localEulerAngles);
        //Docking_Target.rotation = T.localRotation;

        //Debug.Log(T.localEulerAngles.y);
    }

    public void StartDock()
    {   
        if(canDock)
        {
            lerp = 1f;
            Docking = true;
            StartCoroutine(FinishedDocking());
        }
    }

    void Dock()
    {
        lerp -= Time.fixedDeltaTime/5f;
        World_Space.transform.position = Vector3.Lerp(World_Space.transform.position - Docking_Target.position + Docking_Point.position, World_Space.transform.position ,lerp);
        World_Space.transform.rotation = Quaternion.Lerp(Quaternion.Inverse(Docking_Target.rotation), World_Space.transform.rotation, lerp);
    }

    IEnumerator FinishedDocking()
    {
        while (lerp > 0)
        {
            yield return null;
        }
        Docking = false;
    }

}
