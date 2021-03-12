using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class EpicConsole : MonoBehaviour
{
    public TextMeshProUGUI ConsoleText;
    public TMP_InputField ConsoleInput;
    public GameObject Screen;
    bool Active = false;

    //public TextMeshProUGUI CpuStat;
    //public TextMeshProUGUI GpuStat;
    public TextMeshProUGUI MemStat;
    public TextMeshProUGUI FPSStat;

    List<string> ConsoleContents = new List<string>();

    //List<string> PastCommands = new List<string>();

    string lastCommand = "";

    //int PastCommandIndex = -1;

    MenuManager MM;

    int FPS = 0;

    void Awake()
    {
        MM = GameObject.FindWithTag("Camera").GetComponent<MenuManager>();
        InvokeRepeating("UpdateStats", 0.5f, 0.5f);
    }

    void OnEnable()
    {
        Application.logMessageReceived += HandleLog;
    }

    void OnDisable()
    {
        Application.logMessageReceived -= HandleLog;
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.BackQuote))
        {
            Active = !Active;
            Screen.SetActive(Active);

            if(Active)
            {
                //PastCommandIndex = -1;
                ConsoleInput.text = "";
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;

                ConsoleInput.Select();
                ConsoleInput.ActivateInputField();

                try
                {
                    MenuManager.MMInstance.ConsoleOpen = true;
                }
                catch{}
            }else
            {
                try
                {
                    MenuManager.MMInstance.ConsoleOpen = false;
                }
                catch{}
                FixCursorState();
            }
        }
        
        if(Active)
        {
            FPS = (int)(1f / Time.unscaledDeltaTime);

            if(Input.GetKeyDown(KeyCode.UpArrow) && lastCommand != "")
            {
                ConsoleInput.text = lastCommand;
            }

            /*
            if(Input.GetKeyDown(KeyCode.UpArrow))
            {
                if(PastCommandIndex < PastCommands.Count - 1)
                    PastCommandIndex++;

                if(PastCommands.Count != 0 && PastCommandIndex != -1)
                {
                    ConsoleInput.text = PastCommands[(PastCommands.Count - 1) - PastCommandIndex];
                }else
                {
                    ConsoleInput.text = "";
                }
            }

            if(Input.GetKeyDown(KeyCode.DownArrow))
            {
                if(PastCommandIndex > -2)
                    PastCommandIndex--;

                if(PastCommands.Count != 0 && PastCommandIndex != -1)
                {
                    ConsoleInput.text = PastCommands[(PastCommands.Count - 1) - PastCommandIndex];
                }else
                {
                    ConsoleInput.text = "";
                }
            }*/

            if(Input.GetKeyDown(KeyCode.Return))
            {
                ParseConsoleInput();
            }
        }

        /*
        if(Active)
        {
            FrameTimingManager.CaptureFrameTimings();
            CpuStat.text = "CPU: " + FrameTimingManager.GetCpuTimerFrequency().ToString() + "ms";
            GpuStat.text = "GPU: " + FrameTimingManager.GetGpuTimerFrequency().ToString() + "ms";
        }
        */
        
    }

    void UpdateStats()
    {
        if(Active)
        {
            MemStat.text = "Memory Usage\n" + Mathf.Round((System.GC.GetTotalMemory(true) * 0.00001f)).ToString() + "mb";
            FPSStat.text = "FPS: " + FPS.ToString();
        }
    }

    void FixCursorState()
    {
        int Index = 0;
        for(int i = 0; i < MM.MenuScreens.Count; i++)
        {
            if(MM.CurrentScreen == MM.MenuScreens[i].Title)
            {
                Index = i;
            }
        }

        if(MM.MenuScreens[Index].LockCursor)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }else
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }

    void ParseConsoleInput()
    {
        string inputString = ConsoleInput.text;

        if(inputString.Length > 0)
        {   
            lastCommand = inputString;
            /*
            if(PastCommands.Count > 25)
            {
                PastCommands.RemoveAt(0);
            }
            PastCommands.Add(inputString);
            */

            ConsoleInput.text = "";
            string[] input = inputString.Split(' ');

            switch (input[0].ToLower())
            {
                case "spawn":
                    Debug.Log(inputString);
                    if(input.Length > 1)
                    {
                        if(input.Length > 2)
                        {
                            GameServer.GS.SpawnObject(input[1], CameraRayPos() + new Vector3(0f, int.Parse(input[2]), 0f), Quaternion.identity, false, true);
                        }else
                        {
                            GameServer.GS.SpawnObject(input[1], CameraRayPos(), Quaternion.identity, false, true);
                        }
                    }else
                    {
                        Debug.LogError("Enter object name to be spawned");
                    }
                    break;
                case "help":
                    Debug.Log("Commands : ");
                    Debug.Log("clear, help, spawn, spawnlist, teleport");
                    break;
                case "clear":
                    ConsoleContents = new List<string>();
                    ConsoleText.text = "";
                    break;
                case "spawnlist":
                    PrintAllSpawnables();
                    break;
                case "teleport":
                    Debug.Log(inputString);
                    if(input.Length > 1)
                    {
                        if(input.Length > 3)
                        {
                            GameObject.FindWithTag("Player").transform.position = new Vector3(int.Parse(input[1]), int.Parse(input[2]), int.Parse(input[3]));
                        }else
                        {
                            Debug.LogError("enter three coordinates to teleport");
                        }
                    }else
                    {
                        Debug.LogError("Enter location to teleport to");
                    }
                    break;
                default: 
                    Debug.LogError(inputString);
                    Debug.LogError("Command not recognized");
                    break;
            }
        }

        ConsoleInput.Select();
        ConsoleInput.ActivateInputField();
    }

    void PrintAllSpawnables()
    {
        string printstatement = "";
        for(int i = 0; i < GameServer.GS.Spawnables.Count; i++)
        {
            if(i == (GameServer.GS.Spawnables.Count - 1))
                printstatement += GameServer.GS.Spawnables[i].Name;
            else
                printstatement += GameServer.GS.Spawnables[i].Name + ", ";
        }
        
        Debug.Log(printstatement);
    }

    Vector3 CameraRayPos()
    {
        GameObject Cam = GameObject.FindWithTag("Camera").gameObject;

        RaycastHit hit;

		if(Physics.Raycast(Cam.transform.position, Cam.transform.forward, out hit, 100f))
		{
			return hit.point;
        }

        return Cam.transform.position + (Cam.transform.forward * 100f);
    }

    void HandleLog(string logString, string stackTrace, LogType type)
    {
        if(ConsoleContents.Count > 20)
        {
            ConsoleContents.RemoveAt(0);
        }

        ConsoleContents.Add(logString);

        ConsoleText.text = "";

        foreach(string s in ConsoleContents)
        {
            ConsoleText.text += s + "\n";
        }
    }
}
