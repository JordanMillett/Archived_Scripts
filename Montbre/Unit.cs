using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour
{
    public enum Types
    {
        Infantry,
        Plane,
        Tank,
        Emplacement
    }

    [HideInInspector]
    public Infantry inf;
    [HideInInspector]
    public Plane pla;
    [HideInInspector]
    public Tank tan;

    public Faction Team;
    public Stances Stance;
    public Types Type;
    public WeaponInfo.Capabilities Capabilities;

    public Player Controller;

    [HideInInspector]
    public Transform Target;
    [HideInInspector]
    public Transform CameraParent;
    [HideInInspector]
    public float DetectDistance;

    public bool Targetable = true;

    //enum for type and also pass look and move controls into it
    //transition using cool digital effect to move between them, freeze the game and start the transition
    //then unpause

    //enum targettype  vehicle, plane, infantry

    // Start is called before the first frame update

    //aim position, use in general instead of getting each thing
    //Can shoot bool function to ping to stop from shooting falling planes and 
    void Start()
    {
        switch(Type)
        {
            case Types.Infantry : inf_Init(); break;
            case Types.Plane : pla_Init(); break;
            case Types.Tank : tan_Init(); break;
        }
    }

    void Update()
    {
        if(Controller)
        {
            LookControls();
            OtherControls();
        }
    }

    void FixedUpdate()
    {
        if(Controller)
            MoveControls();
    }

    public void SetGunImages()
    {
        if(Type == Types.Infantry)
        {
            Player.H.Primary.texture = inf.Primary.SelectionImage;
            Player.H.Secondary.texture = inf.Secondary.SelectionImage;
        }
        if(Type == Types.Tank)
        {
            Player.H.Primary.texture = tan.Primary.SelectionImage;
            Player.H.Secondary.texture = tan.Secondary.SelectionImage;
        }
        if(Type == Types.Plane)
        {
            Player.H.Primary.texture = pla.ATs[0].SelectionImage;
            Player.H.Secondary.texture = pla.MGs[0].SelectionImage;
        }
    }

    void inf_Init()
    {
        inf = GetComponent<Infantry>();
        Target = inf.Target;
        CameraParent = inf.Eyes;
        DetectDistance = Game.DetectDistance;
    }

    void inf_Look()
    {
        if(!Targetable)
            return;
       
        float y = (Game.mouseModifier/100f) * (Settings._mouseSensitivity/100f) * Input.GetAxis("Mouse X");
        float p = (Game.mouseModifier/100f) * (Settings._mouseSensitivity/100f) * Input.GetAxis("Mouse Y") * (Settings._invertedLook ? -1f : 1f);

   
        float max = 40f;
        float clamp = Mathf.Lerp(max, 4f, Mathf.Abs(inf.pitch)/inf.headUpperLimit);
        inf.yaw += Mathf.Clamp(y, -clamp, clamp);
        inf.pitch -= Mathf.Clamp(p, -max, max);

        Vector3 LookAngle = 
        (Quaternion.AngleAxis(inf.yaw, Vector3.up) * Vector3.forward * 10f) +
        (Quaternion.AngleAxis(inf.pitch, inf.Chest.transform.right) * inf.Chest.transform.forward * 10f);

        inf.LookAt(
            inf.Chest.transform.position + (inf.Chest.transform.up * 0.58f) + LookAngle
        );
     
        
    }

    void inf_Other()
    {
        if(!Targetable)
            return;

        //Zoom
        float FOV = inf.MoveStance == 2 ? Settings._FOV + Game.SprintingFOVChange : Settings._FOV;
        float Goal = FOV;
        Game.mouseModifier = 100f;

        if(Input.GetMouseButton(1))
        {
            inf.Aim(true);

            if(inf.Aiming)
            {
                Goal = inf.Aiming ? inf.Equipped.Info.FOV : FOV;
                Game.mouseModifier = Mathf.Lerp(Game.mouseModifier, Game.mouseModifier * (inf.Equipped.Info.FOV/100f), Controller.CurrentFOV/inf.Equipped.Info.FOV);
            }else
            {
                Goal = 40f;
                Game.mouseModifier = Mathf.Lerp(Game.mouseModifier, Game.mouseModifier * 0.4f, Controller.CurrentFOV/40f);
            }
        }else
        {
            inf.Aim(false);
        }
        Controller.ChangeFOV(Goal, 1f);
        
        //RELOADING
        if(Input.GetKeyDown("r") && inf.Equipped)
            inf.Equipped.StartReload();

        //WEAPON CHANGING
        if(Input.GetKeyDown("1"))
        {
            inf.Equip(inf.Primary);
        }
        if(Input.GetKeyDown("2"))
        {
            inf.Equip(inf.Secondary);
        }
        if(Input.GetKeyDown("3"))
        {
            inf.Equip(null);
        }

        if(Input.mouseScrollDelta.y > 0f)
        {
            if(inf.Equipped == inf.Primary)
                inf.Equip(null);   
            if(inf.Equipped == inf.Secondary)
                inf.Equip(inf.Primary); 
            if(inf.Equipped == null)
                inf.Equip(inf.Secondary); 
        }

        if(Input.mouseScrollDelta.y < 0f)
        {
            if(inf.Equipped == inf.Primary)
                inf.Equip(inf.Secondary);   
            if(inf.Equipped == inf.Secondary)
                inf.Equip(null); 
            if(inf.Equipped == null)
                inf.Equip(inf.Primary); 
        }

        

    }

    void inf_Move()
    {
        Vector3 MoveDirection = Vector3.zero;
        if(Input.GetKey("w") || Input.GetKey("a") || Input.GetKey("s") || Input.GetKey("d"))
        {
            if (Input.GetKey("w"))
                MoveDirection += inf.transform.forward;

            if (Input.GetKey("a")) 
                MoveDirection += -inf.transform.right * 0.75f;

            if (Input.GetKey("s")) 
                MoveDirection += -inf.transform.forward;

            if (Input.GetKey("d")) 
                MoveDirection += inf.transform.right * 0.75f;
        }

        inf.Move(MoveDirection, Input.GetKey(KeyCode.LeftShift), Input.GetKey(KeyCode.LeftControl));
    }

    void tan_Init()
    {
        tan = GetComponent<Tank>();
        Target = tan.Target;
        CameraParent = tan.CameraParent;
        DetectDistance = Game.DetectDistance;
    }

    void tan_Look()
    {
        Game.mouseModifier = 100f;

        float y = (Game.mouseModifier/100f) * (Settings._mouseSensitivity/100f) * Input.GetAxis("Mouse X");
        float p = (Game.mouseModifier/100f) * (Settings._mouseSensitivity/100f) * Input.GetAxis("Mouse Y") * (Settings._invertedLook ? -1f : 1f);

        if(y != 0f || p != 0f)
        {
            tan.yaw += y;
            tan.pitch -= p;
            
            Vector3 LookAngle = 
            (Quaternion.AngleAxis(tan.yaw, tan.transform.up) * -tan.Turret.transform.up * 10f) +
            (Quaternion.AngleAxis(tan.pitch, tan.Turret.transform.right) * tan.ToPitch.transform.forward * 10f);

            tan.LookAt(
                tan.Turret.transform.position + LookAngle
            );

            //Debug.DrawRay(tan.Turret.transform.position, tan.transform.up * 10f, Color.yellow, 0.5f);
            //Debug.DrawRay(tan.Turret.transform.position, -tan.Turret.transform.up * 10f, Color.blue, 0.5f);

            //Debug.DrawRay(tan.Turret.transform.position, tan.Turret.transform.right * 10f, Color.red, 0.5f);
            //Debug.DrawRay(tan.Turret.transform.position, tan.ToPitch.transform.forward * 10f, Color.green, 0.5f);

        }
    }

    void tan_Other()
    {
        //Zoom
        float Goal = Game.BaseFOV;
        if(Input.GetMouseButton(1))
        {
            Goal = 40f;
        }
        Controller.ChangeFOV(Goal, 1f);

        //WEAPON CHANGING
        if(Input.GetKeyDown("1"))
        {
            tan.PlayerUsingPrimary = true;
        }
        if(Input.GetKeyDown("2"))
        {
            tan.PlayerUsingPrimary = false;
        }

        if(Input.mouseScrollDelta.y > 0f || Input.mouseScrollDelta.y < 0f)
        {
            tan.PlayerUsingPrimary = !tan.PlayerUsingPrimary;
        }
    }

    void tan_Move()
    {
        int MoveDirection = 0;
        if(Input.GetKey("w") || Input.GetKey("s"))
        {
            if(Input.GetKey("w"))
                MoveDirection += 1;

            if(Input.GetKey("s")) 
                MoveDirection += -1;
        }

        tan.Move(MoveDirection);

        Vector3 TurnDirection = Vector3.zero;
        if(Input.GetKey("a") || Input.GetKey("d"))
        {
            if(Input.GetKey("a"))
                TurnDirection = (tan.transform.forward.normalized - tan.transform.right.normalized).normalized;

            if(Input.GetKey("d")) 
                TurnDirection = (tan.transform.forward.normalized + tan.transform.right.normalized).normalized;


            tan.Face(
                tan.Turret.transform.position + TurnDirection
            );
        }
    }

    void pla_Init()
    {
        pla = GetComponent<Plane>();
        Target = pla.transform;
        CameraParent = pla.CameraParent;
        DetectDistance = 1000f;
    }

    void pla_Look()
    {
        if(pla.Exploded)
            return;
            
        Vector3 PlaneRoll = pla.transform.up;
        if(Input.GetKey("a") || Input.GetKey("d"))
        {
            if(Input.GetKey("a"))
                PlaneRoll += Quaternion.AngleAxis(90f, pla.transform.forward) * pla.transform.up * 10f;

            if(Input.GetKey("d")) 
                PlaneRoll += Quaternion.AngleAxis(-90f, pla.transform.forward) * pla.transform.up * 10f;
        }

        PlaneInfo.Gear Gee = pla.PI.Normal;
        if(Input.GetKey("w") || Input.GetKey("s"))
        {
            if(Input.GetKey("w"))
                Gee = pla.PI.Normal;

            if(Input.GetKey("s")) 
                Gee = pla.PI.Strafe;
        }
    
        Vector3 LookAngle = 
            (pla.transform.forward * 10f) + 
            (pla.transform.right * Input.GetAxis("Mouse X") * 10f) +
            (pla.transform.up * Input.GetAxis("Mouse Y") * 10f);

        pla.FlyTo(pla.transform.position + LookAngle, PlaneRoll, Gee);
    }

    void pla_Other()
    {
        if(pla.Exploded)
            return;

        //Zoom
        float Goal = Settings._planeFOV;
        if(Input.GetMouseButton(1))
        {
            Goal = 40f;
        }
        Controller.ChangeFOV(Goal, 1f);

        //WEAPON CHANGING
        if(Input.GetKeyDown("1"))
        {
            pla.PlayerUsingPrimary = true;
        }
        if(Input.GetKeyDown("2"))
        {
            pla.PlayerUsingPrimary = false;
        }

        if(Input.mouseScrollDelta.y > 0f || Input.mouseScrollDelta.y < 0f)
        {
            pla.PlayerUsingPrimary = !pla.PlayerUsingPrimary;
        }
    }

    void pla_Move()
    {
        
    }

    void LookControls()
    {
        if(MenuManager.MM.CurrentScreen == "HUD" && !Game.MouseInputLocked)
        {
            switch(Type)
            {
                case Types.Infantry : inf_Look(); break;
                case Types.Plane : pla_Look(); break;
                case Types.Tank : tan_Look();break;
            }
        }
    }

    void OtherControls()
    {
        if(MenuManager.MM.CurrentScreen == "HUD")
        {
            switch(Type)
            {
                case Types.Infantry : inf_Other(); break;
                case Types.Plane : pla_Other(); break;
                case Types.Tank : tan_Other();break;
            }
        }
    }

    void MoveControls()
    {
        if(MenuManager.MM.CurrentScreen == "HUD")
        {
            switch(Type)
            {
                case Types.Infantry : inf_Move(); break;
                case Types.Plane : pla_Move(); break;
                case Types.Tank : tan_Move(); break;
            }
        }
    }

    public void Fire()
    {
        if(MenuManager.MM.CurrentScreen == "HUD" && !Game.MouseInputLocked)
        {
            switch(Type)
            {
                case Types.Infantry : inf.Fire(); break;
                case Types.Plane : if(pla.PlayerUsingPrimary) pla.FireAT(); else pla.FireMG(); break;
                case Types.Tank : if(tan.PlayerUsingPrimary) tan.FireCannon(); else tan.FireMG(); break;
            }
        }
    }

    public void Kill()
    {
        if(MenuManager.MM.CurrentScreen == "HUD" && !Game.MouseInputLocked)
        {
            switch(Type)
            {
                case Types.Infantry : inf.Die(); break;
                case Types.Plane : pla.Explode(); break;
                case Types.Tank : tan.Explode(); break;
            }
        }
    }
}
