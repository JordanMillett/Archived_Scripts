using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestShipControl : MonoBehaviour
{
    public Transform World;

    public float speedspintest;
    public float speed;

    void Start()
    {
        
    }

    void FixedUpdate()
    {

        //MoveForward();
        Rotate();

    }

    void Rotate()
    {

        
        //World.transform.rotation *= Quaternion.Lerp(transform.rotation, Quaternion.Euler(transform.eulerAngles + new Vector3(0f, speedspintest, 0f))  ,Time.fixedDeltaTime);
        //World.transform.RotateAround(this.transform.position, Vector3.up, 20f * Time.deltaTime);
        //World.transform.RotateAround(this.transform.position, Vector3.up, speedspintest * Time.deltaTime);
    }

    public void MoveForward()
    {

        //r.velocity += transform.forward * 10f;
        World.transform.position = (World.transform.position + (-transform.forward * speed) * Time.fixedDeltaTime);

    }

    public void Turn(bool right)
    {

        if(right)
        {
            World.transform.RotateAround(this.transform.position, Vector3.up, -20f * Time.deltaTime);

        }
        else
        {
            World.transform.RotateAround(this.transform.position, Vector3.up, 20f * Time.deltaTime);

        }
        
            //World.transform.rotation *= Quaternion.Lerp(transform.rotation, Quaternion.Euler(transform.eulerAngles + new Vector3(0f, speedspintest, 0f))  , 30f * Time.fixedDeltaTime);
        
            //World.transform.rotation *= Quaternion.Lerp(transform.rotation, Quaternion.Euler(transform.eulerAngles + new Vector3(0f, -speedspintest, 0f))  , 30f * Time.fixedDeltaTime);


        //r.AddTorque(transform.up * speedspintest);
           // r.AddTorque(-transform.up * speedspintest);

    }
}
