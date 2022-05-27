using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class AI
{
    public static bool LinedUp(Vector3 _pos, Vector3 _org, Vector3 _for, float _fov)
    {
        Vector3 targetDirection = Vector3.zero;

        targetDirection = _pos - _org;
        
        float angle = Vector3.Angle(targetDirection.normalized, _for);

        if(angle < _fov && Game.DEBUG_ShowAimDirection)
            Debug.DrawRay(_org, _for * 200f, Color.green);

        return angle < _fov;
    }

    public static Infantry FindNewInfantry(Vector3 _pos, Faction _fac, float _dis, bool _los)
    {
        Collider[] near = Physics.OverlapSphere(_pos, _dis * 2f);
        foreach(Collider col in near)
        {
            try 
            {   
                Infantry I = col.transform.gameObject.GetComponent<Infantry>();

                if(I != null)
                    if(I.U.Team != _fac)
                        if(I.U.Targetable)
                            if(!_los || LineOfSight(_pos, I.gameObject, I.Chest.position))
                                if(Random.value > Vector3.Distance(_pos, I.transform.position)/_dis)
                                    return I;
                    
                            
            }
            catch{}
        }
        

        return null;
    }

    public static Unit FindNewUnit(Unit _uni, WeaponInfo.Capabilities WC, bool _los)
    {
        foreach(Unit U in _uni.Team == Game.TeamOne ? Manager.M.TeamTwo : Manager.M.TeamOne)
        {
            if(U)
                if(U.Team != _uni.Team && U.Targetable && CanTarget(WC, U.Type))
                    if(!_los || LineOfSight(_uni.Target.position, U.gameObject, U.Target.position))
                        if((Random.value > Vector3.Distance(_uni.Target.position, U.transform.position)/_uni.DetectDistance))
                            return U; 
        }

        return null;
    }

    static bool CanTarget(WeaponInfo.Capabilities WC, Unit.Types Target)
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

    public static Plane FindNewPlane(Vector3 _pos, Faction _fac, float _dis)
    {
        Collider[] near = Physics.OverlapSphere(_pos, _dis * 2f);
        foreach(Collider col in near)
        {
            try 
            {   
                Plane P = col.transform.root.gameObject.GetComponent<Plane>();

                if(P != null)
                    if(P.U.Team != _fac)
                        if(P.State != Plane.States.Falling)
                            return P;
            }
            catch{}
        }

        return null;
    }

    public static bool LineOfSight(Vector3 _pos, GameObject _tar, Vector3 _aim)
    {
        bool sight = false;
        Vector3 dir = _aim - _pos;
        
        RaycastHit hit;             
        if(Physics.Raycast(_pos, dir.normalized, out hit, Game.DetectDistance, Game.IgnoreSelectMask))
            if(hit.transform.root.gameObject == _tar.transform.root.gameObject)
                sight = true;

        /*
        if(sight)
        {
            Debug.DrawRay(_pos, dir.normalized * 25f, Color.green);
        }else
        {
            Debug.DrawRay(_pos, dir.normalized * 25f, Color.red);
        }*/

        return sight;
    }
}
