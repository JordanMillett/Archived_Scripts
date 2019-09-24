using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FreeCam : MonoBehaviour 
{
    public GameObject PlayerCamera;
    public GameObject FreeCamera;

    public PlayerController Mov;
    public FreeCamMovement camMov;

    bool Enabled = false;
    bool MovEnabled = false;
    bool slowToggled = false;

	void Update () 
    {
		if(Input.GetKeyDown("c"))
        {
            ToggleCamera();
        }

        if(Enabled == true)
            if(Input.GetKeyDown("v"))
                ToggleMovement();
        
        if(Enabled == true)
            if(Input.GetKeyDown("b"))
                ToggleSlow();
	}

    void ToggleCamera()
    {

        Enabled = !Enabled;

        PlayerCamera.SetActive(!Enabled);
        FreeCamera.SetActive(Enabled);

        if(Enabled == false)
        {
            if(slowToggled)
                ToggleSlow();

            if(MovEnabled)
                ToggleMovement();
        }

    }

    void ToggleMovement()
    {

        MovEnabled = !MovEnabled;

        Mov.invopen = MovEnabled;

        Rigidbody r = Mov.transform.gameObject.GetComponent<Rigidbody>();
    
        if(MovEnabled)  
            r.isKinematic = true;
        else
            r.isKinematic = false;

        camMov.enabled = MovEnabled;

    }
    
    void ToggleSlow()
    {
    
        slowToggled = !slowToggled;

        if(slowToggled)
            Time.timeScale = 0.5f;

        if(!slowToggled)
            Time.timeScale = 1f;

    }
}
