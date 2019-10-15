using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spin : MonoBehaviour
{
    public float X_Rotate_Speed;
    public float Y_Rotate_Speed;
    public float Z_Rotate_Speed;

    void Update()
    {
        //this.transform.eulerAngles += new Vector3(X_Rotate_Speed, Y_Rotate_Speed, Z_Rotate_Speed);
        this.transform.RotateAround(this.transform.position, transform.up, X_Rotate_Speed * Time.deltaTime);
        this.transform.RotateAround(this.transform.position, transform.right, Y_Rotate_Speed * Time.deltaTime);
        this.transform.RotateAround(this.transform.position, transform.forward, Z_Rotate_Speed * Time.deltaTime);

    }
}
