using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class NPC : MonoBehaviour
{
    public float ConversationFOV;

    public Dialogue D;
    public Voice V;

    public bool ConversationOver = false;           //controls the use of idle talk

    float StartingFOV;

    float LookAlpha = 0f;
    float LookSpeed = 0.04f;

    bool WalkDelayRunning = false;

    /*float LookAlpha = 0f;
    float LookSpeed = 0.05f;
    Vector3 LookAngle;*/

    bool isTalking = false;

    Camera Cam;
    PlayerController PC;
    TextBox TB;
    public AudioSource AS;
    Transform LookPosition;
    Routine R;

    void Start()
    {
        Cam = GameObject.FindWithTag("Camera").GetComponent<Camera>();
        PC = GameObject.FindWithTag("Player").GetComponent<PlayerController>();
        TB = GameObject.FindWithTag("TextBox").GetComponent<TextBox>();
        LookPosition = transform.GetChild(0).transform;
        AS = GetComponent<AudioSource>();
        R = GetComponent<Routine>();
        StartingFOV = Cam.fieldOfView;
    }

    public void StartConversation()
    {
        if(!isTalking && !WalkDelayRunning)
        {   
            isTalking = true;
            PC.CanMove = false;
            PC.CanUseMouse = false;
            R.Frozen = true;
            TB.CurrentVoice = V;
            TB.CurrentSource = AS;
            //Cam.fieldOfView = ConversationFOV;

            StartCoroutine(Conversation());
            StartCoroutine(LookAtNPC());
        }
    }

    IEnumerator LookAtNPC()
    {
        LookAlpha = 0f;
        float StartX = PC.yaw;
        float StartY = PC.pitch;
        Vector3 TargetXDir = new Vector3(
            LookPosition.position.x - PC.transform.position.x,
            0f,
            LookPosition.position.z - PC.transform.position.z
            );
        Vector3 TargetYDir = new Vector3(
            LookPosition.position.x - PC.transform.GetChild(0).position.x,
            LookPosition.position.y - PC.transform.GetChild(0).position.y,
            LookPosition.position.z - PC.transform.GetChild(0).position.z
            );
        float DestX = Vector3.SignedAngle(PC.transform.forward, TargetXDir, Vector3.up) + StartX;
        float DestY = Vector3.SignedAngle(PC.transform.GetChild(0).transform.forward, TargetYDir, PC.transform.right) + StartY;

        while(LookAlpha < 1f)
        {
            yield return null;
            Cam.fieldOfView = Mathf.Lerp(StartingFOV, ConversationFOV, LookAlpha);
            //PC.yaw = Mathf.Lerp(StartX, DestX, LookAlpha);
            //PC.pitch = Mathf.Lerp(StartY, DestY, LookAlpha);
            LookAlpha += LookSpeed;
        }

        Cam.fieldOfView = ConversationFOV;
    }

    IEnumerator ZoomOut()
    {
        float alpha = 0f;

        while(alpha < 1f)
        {
            yield return null;
            Cam.fieldOfView = Mathf.Lerp(ConversationFOV, StartingFOV, alpha);
            alpha += LookSpeed;
        }

        Cam.fieldOfView = StartingFOV;
    }

    IEnumerator Conversation()
    {
        TB.SetName(D.Name);                                                 //Assign name of npc to text box
        bool Exit = false;                                                  //Controls the exit condition
        int LoopState = 0;
        int ConversationIndex = 0;                                          //the index of the current conversation, always enter with 0

        TB.DisplayText(D.Conversation[ConversationIndex].Reply[0]);

        while(!Exit)
        {
            yield return null;

            if(LoopState == 0)
            {
                if(Input.GetMouseButtonDown(0))                           //Click to advance to next step
                {
                    if(TB.isPrinting)
                    {
                        TB.FastForward(D.Conversation[ConversationIndex].Reply[0]);
                    }else
                    {
                        if(D.Conversation[ConversationIndex].Choices.Count == 0)
                        {
                            Exit = true;
                        }else
                        {
                            List<string> Actions = new List<string>();                               
                            foreach(Dialogue.Choice DC in D.Conversation[ConversationIndex].Choices)
                                Actions.Add(DC.Text);
                            TB.UpdateChoices(Actions);
                            LoopState = 1;
                        }
                    }
                }
            }else if(LoopState == 1)                                    //Click answer and get result
            {
                if(Input.mouseScrollDelta.y > 0f)
                {
                    TB.ShiftSelection(true, D.Conversation[ConversationIndex].Choices.Count);
                }else if(Input.mouseScrollDelta.y < 0f)
                {
                    TB.ShiftSelection(false, D.Conversation[ConversationIndex].Choices.Count);
                }

                if(Input.GetMouseButtonDown(0))
                {
                    if(D.Conversation[ConversationIndex].Choices.Count == 0)
                    {
                        Exit = true;
                    }else
                    {
                        ConversationIndex = D.Conversation[ConversationIndex].Choices[TB.SelectionIndex].ReturnValue;
                        if(ConversationIndex == -1)
                        {
                            Exit = true;
                        }else
                        {
                            TB.DisplayText(D.Conversation[ConversationIndex].Reply[0]);
                            LoopState = 0;
                        }
                    }
                }
            }

            

            /*
            if(Input.GetMouseButtonDown(0))                                 //click to advance through multiple strings if reply is greater than 0
            {   
                ConversationIndex++;
                if(ConversationIndex > D.Conversation[0].Reply.Count - 1)
                    Exit = true; //exit when responses are finished
                else
                    TB.DisplayText(D.Conversation[0].Reply[ConversationIndex]);
            }

            if(Input.GetKeyDown(KeyCode.Escape))
                Exit = true;
                */
        }

        EndConversation();

    }

    void EndConversation()
    {
        StartCoroutine(ZoomOut());
        PC.CanMove = true;
        PC.CanUseMouse = true;
        StartCoroutine(StartWalkingDelay());
        TB.Toggle();
        //Cam.fieldOfView = StartingFOV;
        isTalking = false;
    }

    IEnumerator StartWalkingDelay()
    {
        WalkDelayRunning = true;
        yield return new WaitForSeconds(0.5f);
        R.Frozen = false;
        WalkDelayRunning = false;
    }
    
}
