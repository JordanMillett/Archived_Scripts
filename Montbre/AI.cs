using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AI : MonoBehaviour
{
    //Dont need to see
    [HideInInspector]
    public Unit U;
    [HideInInspector]
    public Rigidbody r;
    [HideInInspector]
    public bool Dead = false;
    [HideInInspector]
    public Vector3 Objective;
    
    //Need to set up
    public Transform LineOfSightEyes;
    public Transform Target;
    public Transform CameraParent;
    public bool NeedsLineOfSight;

    //Useful to see
    public Flag ObjectiveFlag;
    public Unit Enemy;

    public virtual void Initialize()
    {
        U = GetComponent<Unit>();
        r = GetComponent<Rigidbody>();
    }
    
    public bool LinedUp(Vector3 _pos, Vector3 _org, Vector3 _for, float _fov)
    {
        Vector3 targetDirection = Vector3.zero;

        targetDirection = _pos - _org;
        
        float angle = Vector3.Angle(targetDirection.normalized, _for);

        if(angle < _fov && Game.DEBUG_ShowAimDirection)
            Debug.DrawRay(_org, _for * 200f, Color.green);

        return angle < _fov;
    }
    
    public void UpdateObjective()
    {
        if(Game.GameMode == GameModes.Defense)
        {
            ObjectiveFlag = Logic.L.Defense;

            if (U.Team != Game.TeamOne && Logic.L.LastKnownTarget)    //if attacking and have known enemy location
                Objective = Logic.L.LastKnownTarget.transform.position;
            else
                Objective = ObjectiveFlag.transform.position;               //flag location

        }else if(Game.GameMode == GameModes.Conquest)
        {
            ObjectiveFlag = Manager.M.GetConquestFlag(this.transform.position, U.Team);
            Objective = ObjectiveFlag.transform.position; 
        }else if(Game.GameMode == GameModes.Hill)
        {
            ObjectiveFlag = Logic.L.Hill;
            Objective = ObjectiveFlag.transform.position;
        }
    }

    public Unit FindNewUnit()
    {
        if(NeedsLineOfSight)
        {
            foreach(Unit Found in U.Team == Game.TeamOne ? Manager.M.TeamTwo : Manager.M.TeamOne)
            {
                if(Found)
                    if(Found.Team != U.Team && Found.Targetable && CanTarget(U.Capabilities, Found.Type))
                        if(LineOfSight(Found.gameObject, Found.Target.position))
                            if((Random.value > Vector3.Distance(U.Target.position, Found.transform.position)/U.DetectDistance))
                                return Found; 
            }
        }else
        {
            List<Unit> Targets = new List<Unit>();
            foreach(Unit Found in U.Team == Game.TeamOne ? Manager.M.TeamTwo : Manager.M.TeamOne)
            {
                if(Found)
                    if(Found.Team != U.Team && Found.Targetable && CanTarget(U.Capabilities, Found.Type))
                        Targets.Add(Found);
            }
            if(Targets.Count > 0)
                return Targets[Random.Range(0, Targets.Count)];
            else
                return null;
        }
        
        return null;
    }

    public bool CanTarget(WeaponInfo.Capabilities WC, Unit.Types Target)
    {
        if(WC.AntiInfantry && Target == Unit.Types.Infantry)
        {
            return true;
        }else if(WC.AntiArmor && Target == Unit.Types.Tank)
        {
            return true;
        }else if(WC.AntiAir && Target == Unit.Types.Plane)
        {
            return true;
        }
        

        return false;
    }

    public bool LineOfSight(GameObject _tar, Vector3 _aim)
    {
        bool sight = false;
        Vector3 dir = _aim - LineOfSightEyes.position;
        
        RaycastHit hit;             
        if(Physics.Raycast(LineOfSightEyes.position, dir.normalized, out hit, Game.DetectDistance, Game.IgnoreSelectMask))
            if(hit.transform.root.gameObject == _tar.transform.root.gameObject)
                sight = true;

        return sight;
    }
}
