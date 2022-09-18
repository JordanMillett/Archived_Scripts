using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ship : MonoBehaviour
{
    public Transform FirePos;
    public GameObject FirePrefab;

    public Transform World;
    float Speed = 150f;
    float TurnSpeed = 50f;

    float CurrentSpeed = 0f;

    float SpeedAlpha = 0f;

    void Update()
    {
        Movement();

        if (Input.GetMouseButtonDown(0))
        {
            GameObject Fired = Instantiate(FirePrefab, FirePos.position, Quaternion.identity);
            Fired.transform.SetParent(GameObject.FindWithTag("World").transform);
            Fired.GetComponent<Projectile>().Speed += CurrentSpeed;
        }
    }
    
    void Movement()
    {
        
        if (Input.GetKey("a"))
        {
            World.transform.RotateAround(Vector3.zero, Vector3.up, TurnSpeed * Time.deltaTime);
        }

        if (Input.GetKey("d"))
        {
            World.transform.RotateAround(Vector3.zero, Vector3.up, -TurnSpeed * Time.deltaTime);
        }
        
        if (Input.GetKey("w"))
        {
            World.transform.RotateAround(Vector3.zero, Vector3.right, -TurnSpeed * Time.deltaTime);
        }

        if (Input.GetKey("s"))
        {
            World.transform.RotateAround(Vector3.zero, Vector3.right, TurnSpeed * Time.deltaTime);
        }
        
        if (Input.GetKey("e"))
        {
            World.transform.RotateAround(Vector3.zero, Vector3.forward, TurnSpeed * Time.deltaTime);
        }

        if (Input.GetKey("q"))
        {
            World.transform.RotateAround(Vector3.zero, Vector3.forward, -TurnSpeed * Time.deltaTime);
        }

        if(Input.mouseScrollDelta.y > 0f)
        {
            if(SpeedAlpha < 1f)
                SpeedAlpha += 0.2f;
        }

        if(Input.mouseScrollDelta.y < 0f)
        {
            if(SpeedAlpha > 0f)
                SpeedAlpha -= 0.2f;
        }

        CurrentSpeed = Mathf.Lerp(0f, Speed, SpeedAlpha);


        World.transform.Translate((-Vector3.forward * CurrentSpeed) * Time.deltaTime, Space.World);
        


        RenderSettings.skybox.SetVector("_Up", Vector3.up);
        RenderSettings.skybox.SetVector("_Right", Vector3.right);
        RenderSettings.skybox.SetVector("_Forward", Vector3.forward);

        RenderSettings.skybox.SetVector("_Rotation", new Vector3(-World.transform.eulerAngles.y, -World.transform.eulerAngles.x, -World.transform.eulerAngles.z));
    }
    
    void OnCollisionEnter(Collision col)
    {
        Debug.Log("DEAD");
    }
}
