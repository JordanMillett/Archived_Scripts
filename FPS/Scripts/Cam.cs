using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cam : MonoBehaviour
{

    //scroll wheel fov

    public bool Toggled = false;
    Camera cam;
    public float yaw = 0.0f;
    public float pitch = 0.0f;
	public float MouseSens = 1f;
    public float ZoomSpeed = 1f;
    public float MaxFov = 80f;  
    public float MinFov = 30f;
    public float pitchMin = -15f;
    public float pitchMax = 15f;
    public float yawMin = -15f;
    public float yawMax = 15f;
    float FovOffset = 0;

    void Start()
    {
        FovOffset = MaxFov;
        cam = this.gameObject.transform.GetChild(0).gameObject.GetComponent<Camera>();
        cam.transform.gameObject.SetActive(false);
    }

    void Update()
    {
        if(Toggled)
        {
            cam.transform.gameObject.SetActive(true);
            ZoomControls();
            LookControls();
        }else
        {
            cam.transform.gameObject.SetActive(false);
        }
            
    }

    void ZoomControls()
    {

        if(Input.mouseScrollDelta.y < 0)
        {
            if(FovOffset < MaxFov)
                FovOffset += ZoomSpeed;
            

        }else if(Input.mouseScrollDelta.y > 0)
        {
            if(FovOffset > MinFov)
                FovOffset -= ZoomSpeed;

        }

        cam.fieldOfView = FovOffset;

    }

    void LookControls()
    {

        yaw += MouseSens * Input.GetAxis ("Mouse X");
        pitch -= MouseSens * Input.GetAxis ("Mouse Y");
        
		if(pitch >= pitchMax)
			pitch = pitchMax;
		
		if(pitch <= pitchMin)
			pitch = pitchMin;

        if(yaw >= yawMax)
			yaw = yawMax;
		
		if(yaw <= yawMin)
			yaw = yawMin;

        float y = cam.transform.localEulerAngles.y;
        float z = cam.transform.localEulerAngles.z;

        cam.transform.localEulerAngles = new Vector3(90f + pitch, yaw - 90f, 0f);

    }
}
