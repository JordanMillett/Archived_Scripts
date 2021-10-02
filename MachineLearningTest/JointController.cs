using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JointController : MonoBehaviour
{
    //DATATYPES

    //PUBLIC COMPONENTS
    [HideInInspector]
    public Vector3 TorqueVector;

    public Vector3 TargetAngle;
    public Vector3Int EnabledAxis;

    [HideInInspector]
    public Rigidbody r;
    [HideInInspector]
    public CharacterJoint CJ;

    //PUBLIC VARS
    public float MotorForce = 10f;

    //PUBLIC LISTS

    //COMPONENTS

    //VARS

    //LISTS

    void Start()
    {
        r = GetComponent<Rigidbody>();
        CJ = GetComponent<CharacterJoint>();
    }

    void FixedUpdate()
    {
        //this.transform.localEulerAngles = TargetAngle;

        //Vector3 Difference = this.transform.localEulerAngles - TargetAngle;

        //r.AddRelativeTorque(Difference * MotorForce);

        //if(transform.localEulerAngles.x > CJ.highTwistLimit.limit)
            //Debug.Log(transform.localEulerAngles.x + " > " + CJ.highTwistLimit.limit);
        /*
        if(TargetAngle.x > CJ)
            TargetAngle.x = 1f;
        else if(TargetAngle.x < -1f)
            TargetAngle.x = -1f;

        if(TargetAngle.y > 1f)
            TargetAngle.y = 1f;
        else if(TargetAngle.y < -1f)
            TargetAngle.y = -1f;

        if(TargetAngle.z > 1f)
            TargetAngle.z = 1f;
        else if(TargetAngle.z < -1f)
            TargetAngle.z = -1f;
        */
        

        if(TorqueVector.x > 1f)
            TorqueVector.x = 1f;
        else if(TorqueVector.x < -1f)
            TorqueVector.x = -1f;

        if(TorqueVector.y > 1f)
            TorqueVector.y = 1f;
        else if(TorqueVector.y < -1f)
            TorqueVector.y = -1f;

        if(TorqueVector.z > 1f)
            TorqueVector.z = 1f;
        else if(TorqueVector.z < -1f)
            TorqueVector.z = -1f;
        
        //r.angularVelocity = TorqueVector * MotorForce;
        r.AddRelativeTorque(TorqueVector * MotorForce);
    }
}