using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlackHole : MonoBehaviour
{
    //Use checksphere to pull all rigidbodies instead eventually
    public float PullForce;
    public float ForceThreshold;
    public Vector3 TeleportLocation;
    public float FOVWarpDistanceStart;
    public float FOVWarpAmount;
    
    Rigidbody Player;
    Camera PlayerCamera;

    float PlayerDistance = 0f;
    float AppliedForce = 0f;
    float DefaultFOV = 40f;

    Vector3 ForceVector = Vector3.zero;

    void Start()
    {

        Player = GameObject.FindWithTag("Player").GetComponent<Rigidbody>();
        PlayerCamera = GameObject.FindWithTag("Camera").GetComponent<Camera>();
        DefaultFOV = PlayerCamera.fieldOfView;

    }

    void Update()
    {

        PlayerDistance = Vector3.Distance(Player.transform.position, this.transform.position);

        AppliedForce = PullForce/PlayerDistance;

        if(AppliedForce > ForceThreshold)
        {
            //Debug.Log(AppliedForce);
            ForceVector = this.transform.position - Player.transform.position;
            Player.AddForce(ForceVector * AppliedForce);
        }

        if(PlayerDistance < FOVWarpDistanceStart)
        {

            PlayerCamera.fieldOfView = Mathf.Lerp(FOVWarpAmount, DefaultFOV, (PlayerDistance/FOVWarpDistanceStart));

        }

    }

    void OnCollisionEnter(Collision col)
    {
        if(col.transform.gameObject == Player.gameObject)
        {
            Player.transform.position = TeleportLocation;
            PlayerCamera.fieldOfView = DefaultFOV;
        }
    }
}
