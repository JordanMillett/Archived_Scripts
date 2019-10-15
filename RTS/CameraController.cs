using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{

    //position values smaller than 0 or greater than the screen dimensions (Screen.width,Screen.height) indicate that the mouse cursor is outside of the game window.
    //this.transform.position += new Vector3(-MoveSpeed,0f,0f);

    public float MoveSpeed = 1f;
    public float Threshold = 20f;
    public float ScrollMult = 10f;

    //float ZoomMult = 1f;

    Vector2 MousePos = new Vector2(0f,0f);

    void Start()
    {
        Cursor.lockState = CursorLockMode.Confined;
    }

    void FixedUpdate()
    {
        MousePos = Input.mousePosition;

        if(MousePos.x <= 0 + Threshold || Input.GetAxis("Horizontal") < 0)
            this.transform.Translate(-Vector3.right * MoveSpeed * Time.fixedDeltaTime);

        if(MousePos.x >= Screen.width - Threshold || Input.GetAxis("Horizontal") > 0)
            this.transform.Translate(Vector3.right * MoveSpeed * Time.fixedDeltaTime);

        if(MousePos.y >= Screen.height - Threshold || Input.GetAxis("Vertical") > 0)
            this.transform.Translate(new Vector3(0, 0, 1) * MoveSpeed * Time.fixedDeltaTime, Space.World);            

        if(MousePos.y <= 0 + Threshold || Input.GetAxis("Vertical") < 0)
            this.transform.Translate(new Vector3(0, 0, -1) * MoveSpeed * Time.fixedDeltaTime, Space.World);    

        if(Input.mouseScrollDelta.y > 0f)
            this.transform.Translate(Vector3.forward * MoveSpeed * ScrollMult * Time.fixedDeltaTime);  

        if(Input.mouseScrollDelta.y < 0f)
            this.transform.Translate(-Vector3.forward * MoveSpeed * ScrollMult * Time.fixedDeltaTime); 

        //move by vector3 at end, add to vector3 instead of multiple translates


    }
}
