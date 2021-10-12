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
        cheat,
        cheatslist,
        enablecheats,
        clear,
        help, 
        spawn, 
        spawnlist, 
        teleport
    }

    public TextMeshProUGUI ConsoleText;
    public TMP_InputField ConsoleInput;
    public GameObject Screen;
    public bool Active = false;

    List<ConsoleMessage> ConsoleContents = new List<ConsoleMessage>();

    string lastCommand = "";

    MenuManager MM;
    Manager M;

    public bool CheatsEnabled = false;

    void Start()
    {
        M = GameObject.FindWithTag("Manager").GetComponent<Manager>();
        MM = GameObject.FindWithTag("MenuManager").GetComponent<MenuManager>();
    }

    void OnEnable()
    {
        Application.logMessageReceived += HandleLog;
        CheatsEnabled = GameObject.FindWithTag("Manager").GetComponent<SaveController>().GetCheating();
    }

    void OnDisable()
    {
        Application.logMessageReceived -= HandleLog;
    }

    public void Toggle()
    {
        Active = !Active;
        Screen.SetActive(Active);

        if(Active)
            ConsoleInput.text = "";
        
    }

    void Update()
    {
        if(Active)
        {
            ConsoleInput.Select();
            ConsoleInput.ActivateInputField();

            if(Input.GetKeyDown(KeyCode.UpArrow) && lastCommand != "")
            {
                ConsoleInput.text = lastCommand;
            }

            if(Input.GetKeyDown(KeyCode.Return))
            {
                ParseConsoleInput();
            }
        }
        
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
        if(ConsoleContents.Count > 18)
        {
            ConsoleContents.RemoveAt(0);
        }

        ConsoleMessage newMessage = new ConsoleMessage();
        //newMessage.Message = logString.Length >  ? logString.Substring(0, 40) : logString;
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

    int GetSpawnIndex(string N)
    {
        for(int i = 0; i < M.Cars.Count; i++)
        {
            if(N.ToLower() == M.Cars[i].name.ToLower())
                return i;
        }

        return -1;
    }

    void ParseConsoleInput()
    {
        string inputString = ConsoleInput.text;

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
                        case Command.cheat:      
                            Command_cheat(input);
                        break;
                        case Command.cheatslist:
                            Command_cheatslist(input);
                        break;
                        case Command.enablecheats:
                            Command_enablecheats(input);
                        break;
                        case Command.clear:
                            Command_clear(input);
                        break;
                        case Command.help:
                            Command_help(input);
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

    void Command_cheat(string[] arguments)
    {
        if(CheatsEnabled)
        {
            if(arguments.Length > 1)
            {
                try
                {
                    PhoneCheats.Cheat ToCall = (PhoneCheats.Cheat) System.Enum.Parse (typeof (PhoneCheats.Cheat), arguments[1].ToLower());
                    GameObject.FindWithTag("Player").GetComponent<PlayerController>().P.gameObject.GetComponent<PhoneCheats>().ActivateCheat(ToCall);
                    Debug.LogError("Cheat \"" + ToCall.ToString() + "\" activated!");
                }
                catch
                { 
                    Debug.LogError("Cheat \"" + arguments[1] + "\" not recognized");
                }
            }else
            {
                Debug.LogError("Enter a cheat to be activated");
            }
        }else
        {
            Debug.LogError("Cheats must be enabled to use this command");
        }
    }

    void Command_cheatslist(string[] arguments)
    {
        Debug.Log("Cheats : ");
        string all = "";
        for(int i = 1; i < Enum.GetValues(typeof(PhoneCheats.Cheat)).Length; i++)
        {
            all += ((PhoneCheats.Cheat)i).ToString() + ", ";
        }   
        Debug.Log(all.Substring(0, all.Length - 2));
    }

    void Command_enablecheats(string[] arguments)
    {
        if(!CheatsEnabled)
        {
            GameObject.FindWithTag("Manager").GetComponent<SaveController>().SetCheating(true);
            CheatsEnabled = true;
            Debug.Log("Cheats Enabled");   
        }
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

    void Command_spawn(string[] arguments)
    {
        if(CheatsEnabled)
        {
            if(arguments.Length > 1)
            {
                if(GetSpawnIndex(arguments[1]) > -1)
                {
                    Transform PlayerPos = GameObject.FindWithTag("Player").transform;
                    Instantiate(M.Cars[GetSpawnIndex(arguments[1])], PlayerPos.position + (PlayerPos.forward * 10f), Quaternion.identity);
                }else
                {
                    Debug.LogError("Object name \"" + arguments[1] + "\" not recognized");
                }
            }else
            {
                Debug.LogError("Enter object name to be spawned");
            }
        }else
        {
            Debug.LogError("Cheats must be enabled to use this command");
        }
    }

    void Command_spawnlist(string[] arguments)
    {
        Debug.Log("Spawnables : ");
        string all = "";
        for(int i = 0; i < M.Cars.Count; i++)
        {
            all += M.Cars[i].name + ", ";
        }   
        Debug.Log(all.Substring(0, all.Length - 2));
    }

    void Command_teleport(string[] arguments)
    {
        if(CheatsEnabled)
        {
            if(arguments.Length > 1)
            {
                if(arguments.Length > 3)
                {
                    GameObject.FindWithTag("Player").transform.position = new Vector3(int.Parse(arguments[1]), int.Parse(arguments[2]), int.Parse(arguments[3]));
                    Debug.Log("Player Teleported");
                }else
                {
                    Debug.LogError("Enter three coordinates to teleport to");
                }
            }else
            {
                Debug.LogError("Enter X Y Z location to teleport to");
            }
        }else
        {
            Debug.LogError("Cheats must be enabled to use this command");
        }
    }
    
}