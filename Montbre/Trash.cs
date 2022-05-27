/*

CallOutRoutine = CallOut(SI);
StartCoroutine(CallOutRoutine);

StopCoroutine(CallOutRoutine);
Mouth.Stop();
CurrentSpeaker = null;

IEnumerator CallOut(SpotInfo SI)
{
    //UNIT TYPE
    AudioClip Sound = V.Rifleman;
    switch (SI.UnitType)
    {
        case UnitTypes.Rifleman :          Sound = V.Rifleman;              break;
        case UnitTypes.AutomaticRifleman : Sound = V.AutomaticRifleman;     break;
        case UnitTypes.Sniper :            Sound = V.Sniper;                break;
        case UnitTypes.MachineGunner :     Sound = V.MachineGunner;         break;
    }

    Mouth.clip = Sound;
    Mouth.Play();
    yield return new WaitForSeconds(Sound.length);

    //DISTANCE
    if(SI.Distance > 50)
    {
        Sound = V.Hundreds[Mathf.FloorToInt(SI.Distance/100) - 1];
        Mouth.clip = Sound;
        Mouth.Play();
        yield return new WaitForSeconds(Sound.length);
    }

    if(SI.Distance % 100 == 50 || SI.Distance == 50)
    {
        Sound = V.Fifty;
        Mouth.clip = Sound;
        Mouth.Play();
        yield return new WaitForSeconds(Sound.length);
    }
    
    //Meters
    Sound = V.Meters;
    Mouth.clip = Sound;
    Mouth.Play();
    yield return new WaitForSeconds(Sound.length);

    //DIRECTION
    switch (SI.Direction)
    {
        case "North" :  Sound = V.North;        break;
        case "East" :   Sound = V.East;         break;
        case "South" :  Sound = V.South;        break;
        case "West" :   Sound = V.West;         break;
    }

    Mouth.clip = Sound;
    Mouth.Play();
    yield return new WaitForSeconds(Sound.length);
    yield return new WaitForSeconds(0.5f);
    CurrentSpeaker = null;
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="New Voice", menuName = "Voice")]
public class Voice : ScriptableObject
{
    //50. 100. 200. 300. 400. 500. Meters. North. East. South. West. Rifleman. Automatic Rifleman. Machine Gunner. Sniper. OUCH!

    public List<AudioClip> Hundreds;
    public AudioClip Fifty;
    public AudioClip Meters;

    public AudioClip Rifleman;
    public AudioClip AutomaticRifleman;
    public AudioClip Sniper;
    public AudioClip MachineGunner;

    public AudioClip North;
    public AudioClip East;
    public AudioClip South;
    public AudioClip West;

    public List<AudioClip> Death;
}

void Spot(Unit U)
{
    CurrentSpeaker = this;

    SpotInfo SI = new SpotInfo();

    U.Spotted = true;

    SI.UnitType = U.inf.Kit.UnitType;
    SI.Distance = Mathf.RoundToInt(Vector3.Distance(this.transform.position, U.transform.position));

    SI.Distance = Mathf.RoundToInt(SI.Distance / 50f) * 50;

    if(SI.Distance > 500)
        SI.Distance = 500;
    else if(SI.Distance < 50)
        SI.Distance = 50;

    string Direction = "ERROR";
    Vector3 Difference = U.transform.position - this.transform.position;
    Difference.y = 0f;
    
    Direction = Game.GetDirection(Difference.normalized, false);

    SI.Direction = Direction;

    GameObject.FindWithTag("Camera").GetComponent<Player>().NewMessage(SI);

    
}


if(Enemy.Spotted == false && Enemy.Spottable && CurrentSpeaker == null && U.Team != Game.TeamTwo)
    Spot(Enemy);

public void NewMessage(Infantry.SpotInfo SI)
{
    H.Defense_SpotText.text = SI.UnitType.ToString() + " " + SI.Distance.ToString() + "m " + SI.Direction;
}

public struct SpotInfo
{
    public UnitTypes UnitType;
    public int Distance;
    public string Direction;
}

public enum UnitTypes
{
    Rifleman,
    AutomaticRifleman,
    Sniper,
    MachineGunner
};

public static string GetDirection(Vector3 Difference, bool Letter)
{
    float angle = Vector3.SignedAngle(Difference, Vector3.forward, Vector3.up);

    string Direction = "ERROR";

    if(angle >= -45f && angle <= 45f)
    {
        Direction = Letter ? "N" : "North";
    }
    if(angle < -45f && angle >= -135f)
    {
        Direction = Letter ? "E" : "East";
    }
    if(angle > 45f && angle <= 135f)
    {
        Direction = Letter ? "W" : "West";
    }
    if(angle < -135f || angle > 135f)
    {
        Direction = Letter ? "S" : "South";
    }

    return Direction;
}


Collider[] near = Physics.OverlapSphere(_uni.Target.position, _uni.DetectDistance * 2f);
foreach(Collider col in near)
{
    try 
    {   
        Unit U = col.transform.root.gameObject.GetComponent<Unit>();

        if(U)
            if(U.Team != _uni.Team && U.Targetable && CanTarget(_uni.Type, U.Type))
                if(!_los || LineOfSight(_uni.Target.position, U.gameObject, U.Target.position))
                    if((Random.value > Vector3.Distance(_uni.Target.position, U.transform.position)/_uni.DetectDistance))
                        return U; 
    }
    catch{}
}


return null;

void S_ManningEmplacement()
{
    Move(Vector3.zero, false, false);

    this.transform.localRotation = Quaternion.Euler(0f, Turret.Spin.transform.eulerAngles.y, 0f);
    Chest.transform.localRotation = Quaternion.Euler(0f, 0f, 0f);

    An.SetFloat("Pitch", -Turret.pitch);
}

void S_HeadingToEmplacement()
{
    if(NearbyTurret.Occupied)
        NearbyTurret = null;

    if(!NearbyTurret)
    {
        UpdateState(States.Idle);
    }else
    {
        if(Vector3.Distance(this.transform.position, NearbyTurret.transform.position) > 1f)
        {
            HeadTo(NearbyTurret.transform.position, true);
        }else
        {
            UpdateState(States.ManningEmplacment);
            Use(NearbyTurret);
        }
    }
}

public void Use(Emplacement E)
{
    Equip(HandStates.Free);

    E.Occupied = true;
    Turret = E;
    E.Toggle(this);
}

 inf.Turret.yaw += y;
inf.Turret.pitch -= p;

if(inf.Turret.pitch >= inf.Turret.pitchLimit)
    inf.Turret.pitch = inf.Turret.pitchLimit;
    
if(inf.Turret.pitch <= -inf.Turret.pitchLimit)
    inf.Turret.pitch = -inf.Turret.pitchLimit;

inf.transform.localRotation = Quaternion.Euler(0f, inf.Turret.Spin.transform.eulerAngles.y, 0f);
inf.Chest.transform.localRotation = Quaternion.Euler(0f, 0f, 0f);

inf.Turret.Spin.transform.localRotation = Quaternion.Euler(0f, inf.Turret.yaw, 0f);
inf.Turret.GunSlot.transform.localRotation = Quaternion.Euler(inf.Turret.pitch, 0f, 0f);
inf.An.SetFloat("Pitch", -inf.Turret.pitch);

if(MoveStance == -2 && U.Controller)
    Turret.Equipped.PullTrigger();
    
void FindNewEmplacement()
{
    if(!NearbyTurret && Random.value < 0.2f)
    {
        Collider[] near = Physics.OverlapSphere(this.transform.position, 10f * 2f);
        foreach(Collider col in near)
        {
            if(Turret)
                break;

            try 
            {   
                Emplacement E = col.transform.gameObject.GetComponent<Emplacement>();

                if(E)
                    if(!E.Occupied)
                        NearbyTurret = E;
                                
            }
            catch{}
        }
    }
}

    
bool Instant = false;
if(Time.time > RelaxTime + LastAlerted)
    Instant = true;
bool Aler = HS != HandStates.Free;
StartCoroutine(SwitchWeapon(W, Instant, Aler));
//IEnumerator SwitchWeapon(Weapon W, bool Instant, bool Alert)


NavMesh.CalculatePath(this.transform.position, CoverLocation, NavInfo, PathToGoal);
if(PathToGoal.corners.Length > 1)   //if there is a path
{
    if(Vector3.Distance(this.transform.position, PathToGoal.corners[1]) < 1f)   //if within stop distance of cover then shoot
    {
        Move(Vector3.zero, false, false);
        if(!TryToShoot())
        {
            Aim(false);
            if(CoverTall)
                LookAt(this.transform.position + HeadHeight + CoverDirection);
        }

    }else           //if farther from the cover then go to it
    {
        Aim(false);
        if(Vector3.Distance(this.transform.position, PathToGoal.corners[1]) > 2f)   //run if farther 
            HeadTo(PathToGoal.corners[1], true);
        else
            HeadTo(PathToGoal.corners[1], false);
    }
}else
{
    CoverLocation = Vector3.zero;
    CoverDirection = Vector3.zero;
    CoverTall = false;
}

*/