using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AI_Turret : MonoBehaviour
{
    public Weapon CurrentGun;
    public float TurnSpeed = 4f;
    public Transform P;

    void Start()
    {
        P = GameObject.FindWithTag("Player").GetComponent<Player>().AimTarget;
    }

    void FixedUpdate()
    {
        if(LineOfSight())
        {
            Vector3 Direction = P.position - this.transform.position;
            this.transform.rotation = Quaternion.Lerp(this.transform.rotation, Quaternion.LookRotation(Direction, Vector3.up), Time.fixedDeltaTime * TurnSpeed);

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