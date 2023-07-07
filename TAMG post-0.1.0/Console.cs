using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class Console : MonoBehaviour
{
    public class ConsoleMessage
    {
        public string Message;
        public bool Error;
    }

    public enum Command
    {
        error,
        clear,
        help,
        rule,
        rulelist,
        spawn,
        spawnlist,
        teleport
    }

    public enum GameRule
    {
        packagedamagethreshold,
        damagemultiplier,
        distancevaluemultiplier
    }

    Dictionary<string, Vector3> TeleportPositions = new Dictionary<string, Vector3>
    {
        {"beach", new Vector3(0f, 1f, 605f)}
    };

    public TextMeshProUGUI ConsoleText;
    public TMP_InputField ConsoleInput;
    public GameObject Screen;

    public TextMeshProUGUI MemStat;
    public TextMeshProUGUI FPSStat;

    List<ConsoleMessage> ConsoleContents = new List<ConsoleMessage>();

    string lastCommand = "";

    int FPS = 0;

    void Awake()
    {
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
    
    public void Activate()
    {
        ConsoleInput.text = "";
        ConsoleInput.Select();
        ConsoleInput.ActivateInputField();
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.RightControl))
        {
            ParseConsoleInput(lastCommand);
        }
        
        FPS = (int)(1f / Time.unscaledDeltaTime);

        if(Input.GetKeyDown(KeyCode.UpArrow) && lastCommand != "")
        {
            ConsoleInput.text = lastCommand;
        }

        if(Input.GetKeyDown(KeyCode.Return))
        {
            ParseConsoleInput(ConsoleInput.text);
        }
    }

    Vector3 CameraRayPos()
    {
        GameObject Cam = GameObject.FindWithTag("Camera").gameObject;

        RaycastHit hit;

		if(Physics.Raycast(Cam.transform.position, Cam.transform.forward, out hit, 1000f))
		{
			return hit.point;
        }

        return Cam.transform.position + (Cam.transform.forward * 1000f);
    }

    void HandleLog(string logString, string stackTrace, LogType type)
    {
        if(ConsoleContents.Count > 20)
        {
            ConsoleContents.RemoveAt(0);
        }

        ConsoleMessage newMessage = new ConsoleMessage();
        newMessage.Message = logString;
        newMessage.Error = type != LogType.Log;

        ConsoleContents.Add(newMessage);

        ConsoleText.text = "";

        for(int i = 0; i < ConsoleContents.Count; i++)
        {
            if(ConsoleContents[i].Error)
                ConsoleText.text += "<color=\"red\">" + ConsoleContents[i].Message + "\n";
            else
                ConsoleText.text += "<color=\"white\">" + ConsoleContents[i].Message + "\n";
        }
    }

    void ParseConsoleInput(string inputString)
    {
        if(inputString.Length > 0)
        {   
            lastCommand = inputString;

            ConsoleInput.text = "";
            string[] input = inputString.Split(' ');

            Command CalledCommand = Command.error;
            try
            {
                CalledCommand = (Command) System.Enum.Parse (typeof (Command), input[0].ToLower());
            }catch
            {
                Debug.LogError("Command \"" + inputString + "\" not recognized");
            }

            if(CalledCommand != Command.error)
            {
                try
                {
                    switch (CalledCommand)
                    {
                        case Command.clear:
                            Command_clear(input);
                        break;
                        case Command.help:
                            Command_help(input);
                        break;
                        case Command.rule:
                            Command_rule(input);
                        break;
                        case Command.rulelist:
                            Command_rulelist(input);
                        break;
                        case Command.spawn:
                            Command_spawn(input);
                        break;
                        case Command.spawnlist:
                            Command_spawnlist(input);
                        break;
                        case Command.teleport:
                            Command_teleport(input);
                        break;
                    }
                }
                catch
                {
                    Debug.LogError("An error has occured");
                }
            }
        }
        
        ConsoleInput.Select();
        ConsoleInput.ActivateInputField();
    }

    void UpdateStats()
    {
        MemStat.text = "Memory Usage\n" + Mathf.Round((System.GC.GetTotalMemory(true) * 0.00001f)).ToString() + "mb";
        FPSStat.text = "FPS: " + FPS.ToString();
    }

    void Command_clear(string[] arguments)
    {
        ConsoleContents = new List<ConsoleMessage>();
        ConsoleText.text = "";
    }

    void Command_help(string[] arguments)
    {
        Debug.Log("Commands : ");
        string all = "";
        for(int i = 1; i < Enum.GetValues(typeof(Command)).Length; i++)
        {
            all += ((Command)i).ToString() + ", ";
        }   
        Debug.Log(all.Substring(0, all.Length - 2));
    }

    void Command_rule(string[] arguments)
    {
        if(arguments.Length == 1)   //rule
        {
            Debug.LogError("Enter a rule to be modified");
        }else
        {
            if(arguments.Length == 2)   //rule damage
            {
                Debug.LogError("Enter a value to set the rule");
            }else
            {
                Debug.LogError("FEATURE NOT IMPLEMENTED");
            }
        }
    }

    void Command_rulelist(string[] arguments)
    {    
        Debug.Log("Commands : ");
        string all = "";
        for(int i = 0; i < Enum.GetValues(typeof(GameRule)).Length; i++)
        {
            all += ((GameRule)i).ToString() + ", ";
        }   
        Debug.Log(all.Substring(0, all.Length - 2));
    }

    void Command_spawn(string[] arguments)       //spawn
    {
        if(arguments.Length > 1)                 //spawn thing
        {
            if(arguments[1] == "item")           //spawn item
            {
                if(arguments.Length > 2)         //spawn item thing
                {
                    if(arguments.Length > 3)     //spawn item thing 4
                    {
                        Client.Instance.SpawnItem(arguments[2], CameraRayPos(), Quaternion.identity, int.Parse(arguments[3]), true);
                    }else                        //spawn item thing
                    {
                        Client.Instance.SpawnItem(arguments[2], CameraRayPos(), Quaternion.identity, 1, true);
                    }
                }else
                {
                    Debug.LogError("Enter item name to be spawned");
                }
            }else                                 //spawn object
            {
                if (arguments.Length > 2)         //spawn object 4
                {
                    Client.Instance.SpawnObject(arguments[1], CameraRayPos(), Quaternion.identity, int.Parse(arguments[2]), true);
                }
                else                            //spawn object
                {
                    Client.Instance.SpawnObject(arguments[1], CameraRayPos(), Quaternion.identity, 1, true);
                }
            }
        }else
        {
            Debug.LogError("Enter object name to be spawned");
        }
    }

    void Command_spawnlist(string[] arguments)
    {    
        string all = "";
        for(int i = 0; i < Client.Instance.Spawnables.Count; i++)
        {
            if(!Client.Instance.Spawnables[i].HideInConsole)
            {
                if(i == (Client.Instance.Spawnables.Count - 1))
                    all += Client.Instance.Spawnables[i].name;
                else
                    all += Client.Instance.Spawnables[i].name + ", ";
            }
        }
        Debug.Log(all.Substring(0, all.Length - 2));
    }

    void Command_teleport(string[] arguments)
    {
        if(arguments.Length == 1)
        {
            Debug.LogError("Enter X Y Z location to teleport to or a location name");
        }else
        {
            if(arguments.Length == 2)
            {
                if(TeleportPositions.ContainsKey(arguments[1].ToLower()))
                {
                    GameObject.FindWithTag("Player").transform.position = TeleportPositions[arguments[1].ToLower()];
                    Debug.Log("Player Teleported");
                }
                else
                {
                    Debug.LogError("Area name not recognized");
                }
            }else
            {
                if(arguments.Length > 3)
                {
                    GameObject.FindWithTag("Player").transform.position = new Vector3(int.Parse(arguments[1]), int.Parse(arguments[2]), int.Parse(arguments[3]));
                    Debug.Log("Player Teleported");
                }else
                {
                    Debug.LogError("Enter three coordinates to teleport to");
                }
            }
        }
    }
}