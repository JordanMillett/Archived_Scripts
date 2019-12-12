using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FacePlayer : MonoBehaviour
{

    Transform Camera;
    float Yaw;

    Vector3 Yaw_Dir;

    void Start()
    {
        Camera = GameObject.FindWithTag("MainCamera").transform;    
    }

    void Update()
    {
        Yaw_Dir = transform.position - Camera.position;
        Yaw_Dir = new Vector3(Yaw_Dir.x, 0f, Yaw_Dir.z);

        Yaw = Vector3.SignedAngle(Yaw_Dir, Vector3.forward, Vector3.up) + 180f;


        this.transform.localEulerAngles = new Vector3(90f,0f,Yaw);
    }
}
