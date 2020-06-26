using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Turret : MonoBehaviour
{
 
    public float DelayBetweenShots = 0.25f;
    public float Range = 100f;
    public float RotationSpeed = 2f;
    public float Accuracy = 85f;
    public GameObject Projectile;

    Transform EndPosition;
    Transform Player;

    Vector3 Target;
    Quaternion newRotation;

    void Start()
    {
        Player = GameObject.FindWithTag("Player").transform.GetChild(0).transform;
        EndPosition = transform.GetChild(0).transform;

        InvokeRepeating("Fire", DelayBetweenShots, DelayBetweenShots);

    }

    void Update()
    {

        Target = Player.transform.position - this.transform.position;
        newRotation = Quaternion.LookRotation(Target);
        this.transform.rotation = Quaternion.Slerp(this.transform.rotation, newRotation, Time.fixedDeltaTime * RotationSpeed);

    }

    void Fire()
    {

        if(CanSee())
        {

            float Inaccuracy = 100f - Accuracy;
            Vector3 OffsetVector = Random.insideUnitSphere.normalized * Random.Range(0f, 1f);
            OffsetVector *= Inaccuracy;
            OffsetVector = OffsetVector / 10f;

            GameObject P = Instantiate(Projectile, EndPosition.position, Quaternion.identity);
            P.transform.eulerAngles = EndPosition.eulerAngles + OffsetVector;

        }

    }

    bool CanSee()
    {

        Target = Player.transform.position - this.transform.position;

        RaycastHit hit;
        if(Physics.Raycast(this.transform.position, Target, out hit, Range))
        {

            if(hit.transform.GetComponent<PlayerController>() != null)
                return true;
            else
                return false;

        }

        return false;

    }
}
