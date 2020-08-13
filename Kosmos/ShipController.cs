using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipController : MonoBehaviour
{
    public bool PlayerControlled = false;
    public float Speed = 2f;
    public float TurnSpeed = 2f;

    public bool Busy = false;

    bool isTraveling = false;

    Plane GroundPlane;
    Camera Cam;
    Vector3 Target = Vector3.zero;
    GameObject Model;
    IEnumerator TravelCoroutine;

    void Start()
    {
        if(PlayerControlled)
        {
            Cam = transform.GetChild(1).GetChild(0).GetComponent<Camera>();
        }

        Model = transform.GetChild(0).transform.gameObject;
        GroundPlane = new Plane(Vector3.up, 0f);
    }

    void Update()
    {
        if(PlayerControlled)
        {
            PlayerMovement();
        }
        
        ShipRotation();
    }

    public void GoTo(Vector3 Position)
    {
        Target = Position - this.transform.position;

        if(isTraveling)
        {
            StopCoroutine(TravelCoroutine);
            isTraveling = false;
        }
            
        TravelCoroutine = Travel(Position);
        StartCoroutine(TravelCoroutine);
    }

    IEnumerator Travel(Vector3 Position)
    {
        isTraveling = true;

        float Distance = Vector3.Distance(this.transform.position, Position);

        while(Distance > 0.1f)  //distance threshold
        {
            this.transform.position = Vector3.MoveTowards(this.transform.position, Position, Time.fixedDeltaTime * Speed);
            Distance = Vector3.Distance(this.transform.position, Position);
            yield return UniversalConstants.FrameTime;
        }

        isTraveling = false;
    }

    void ShipRotation()
    {
        if(Target != Vector3.zero)
        {
            Quaternion newRotation = Quaternion.LookRotation(Target, Vector3.up);
            Model.transform.rotation = Quaternion.Slerp(Model.transform.rotation, newRotation, Time.fixedDeltaTime * TurnSpeed);
        }
    }

    void PlayerMovement()
    {
        if(Input.GetMouseButton(0) && !Busy)
        {
            float hitdist = 0f;
            Ray ray = Cam.ScreenPointToRay(Input.mousePosition);
            
            if(GroundPlane.Raycast(ray, out hitdist))
            {
                GoTo(ray.GetPoint(hitdist));
            }
        }
    }
}
