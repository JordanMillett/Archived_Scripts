using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Look : MonoBehaviour
{


	public float MouseSens = 1f;
    public float camLimits = 80f;


    public float yaw = 0f;
    public float pitch = 0f;

    void Start()
    {

    }

    void FixedUpdate()
    {
        CameraControls(); //Looking
    }

    void CameraControls()  
    {

		yaw += MouseSens * Input.GetAxis ("Mouse X");
        //transform.localEulerAngles = new Vector3(0f, yaw, 0f);

        pitch -= MouseSens * Input.GetAxis ("Mouse Y");
        
		if(pitch >= camLimits)
			pitch = camLimits;
        
		
		if(pitch <= -camLimits)
			pitch = -camLimits;

        float y = this.transform.parent.transform.localEulerAngles.y;
        float z = this.transform.parent.transform.localEulerAngles.z;

        this.transform.parent.localEulerAngles = new Vector3(pitch, yaw, z);

    }

}
