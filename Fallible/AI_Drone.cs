using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AI_Drone : MonoBehaviour
{
    public Weapon CurrentGun;
    public Transform P;
    public Transform ToPitch;
    public Transform ToYaw;

    public float MaxSpeed;
    public float MovementForce;
    
    public float PitchSpeed = 4f;
    public float YawSpeed = 4f;
    public float TurnSpeed = 4f;

    Rigidbody r;

    void Start()
    {
        r = GetComponent<Rigidbody>();
        P = GameObject.FindWithTag("Player").GetComponent<Player>().AimTarget;   
    }

    void FixedUpdate()
    {   
        if(LineOfSight())
        {
            //YAW
            Vector3 DirFromYaw = P.position - ToYaw.transform.position;
            Quaternion YawLook = Quaternion.LookRotation(DirFromYaw, this.transform.up);
            YawLook = Quaternion.Euler(ToYaw.rotation.eulerAngles.x, YawLook.eulerAngles.y, ToYaw.rotation.eulerAngles.z);
            ToYaw.transform.rotation = Quaternion.Lerp(ToYaw.transform.rotation, YawLook, Time.fixedDeltaTime * YawSpeed);

            //PITCH
            Vector3 DirFromPitch = P.position - ToPitch.transform.position;
            Quaternion PitchLook = Quaternion.LookRotation(DirFromPitch, this.transform.right);
            PitchLook = Quaternion.Euler(PitchLook.eulerAngles.x, ToPitch.rotation.eulerAngles.y, ToPitch.rotation.eulerAngles.z);
            ToPitch.transform.rotation = Quaternion.Lerp(ToPitch.transform.rotation, PitchLook, Time.fixedDeltaTime * PitchSpeed);

            //TURNING
            Vector3 DirFromPos = P.position - this.transform.position;
            Quaternion PosLook = Quaternion.LookRotation(DirFromPos, this.transform.up);
            PosLook = Quaternion.Euler(this.transform.rotation.eulerAngles.x, PosLook.eulerAngles.y, this.transform.rotation.eulerAngles.z);
            this.transform.rotation = Quaternion.Lerp(this.transform.rotation, PosLook, Time.fixedDeltaTime * TurnSpeed);

            //MOVEMENT
            Vector3 MoveDirection = this.transform.forward;
            if(Vector3.Distance(this.transform.position, P.position) > 5f)
                r.AddForce((MoveDirection * MovementForce) * (1f - (r.velocity.magnitude/MaxSpeed)));

            CurrentGun.Fire();
        }
    }

    bool LineOfSight()
    {
        Vector3 Dir = P.position - CurrentGun.FirePosition.position;
        
        RaycastHit hit;

        if(Physics.Raycast(CurrentGun.FirePosition.position, Dir, out hit, Vector3.Distance(CurrentGun.FirePosition.position, P.position) * 1.25f))
        {
            Player PlayerRef = hit.collider.transform.root.gameObject.GetComponent<Player>();

            if(PlayerRef != null)
                return true;
            
        }

        return false;
    }
}