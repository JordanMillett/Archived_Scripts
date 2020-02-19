using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueMenu : MonoBehaviour
{
    public float maxDistance = 2f;
    public float Radius = 40f;
    public float exitRadius = 70f;
    public float minRadius = 0.25f;
    public float DampEffect = 5f;
    public PlayerController PC; //damped mouse sensitivity in relation to distance from the starting mouse position
    public bool Enabled = false;
    

    GameObject UI; 

    float SensAlpha = 0f;
    float InitYaw = 0f;
    float InitPitch = 0f;
    float maxSpeed = 0f;

    void Start()
    {
        UI = transform.GetChild(0).transform.gameObject;
        UI.SetActive(false);
    }

    void Update()
    {   
        if(Enabled)
        {
            if(Input.GetKeyDown(KeyCode.Escape))
                Toggle();

            UpdateAlpha();         
            if(Enabled)   //the state of enabled could've changed.
                PC.MouseSens = Mathf.Lerp(PC.DefaultMouseSens/DampEffect , PC.DefaultMouseSens, SensAlpha);
        }
    }

    public void Toggle()
    {

        Enabled = !Enabled;
        UI.SetActive(Enabled);
        //PC.Frozen = Enabled;

        if(Enabled)
        {
            maxSpeed = PC.Maxspeed;
            PC.Maxspeed = 0f;
            InitYaw = PC.yaw;
            InitPitch = PC.pitch;

        }else
        {
            PC.Maxspeed = maxSpeed;
            PC.MouseSens = PC.DefaultMouseSens;
        }

    }

    void UpdateAlpha()
    {

        

        float yawDist = Mathf.Abs(InitYaw - PC.yaw);
        float pitchDist = Mathf.Abs(InitPitch - PC.pitch);

        float biggestDist = 0f;

        if(yawDist > pitchDist)
            biggestDist = yawDist;
        else
            biggestDist = pitchDist;

        if(biggestDist >= exitRadius && (Enabled))    //Ends the conversation when the player looks too far away
        {

            Toggle();

        }else
        {
            float percent = 0f;

            if(biggestDist < minRadius)
                percent = 0f;
            else
                percent = biggestDist/Radius;
           
            SensAlpha = Mathf.Clamp(percent, 0f, Radius); //clamps so the mouse sensitivity doesn't speed up when outside the max damping range
        }

    }
}
