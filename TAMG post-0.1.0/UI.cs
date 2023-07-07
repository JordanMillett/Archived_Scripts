using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using TMPro;
using Mirror.FizzySteam;

public class UI : MonoBehaviour
{
    [System.Serializable]
    public struct Menu
    {
        public string Title;
	    public GameObject Screen;
        public bool BlurBackground;
        public bool LockCursor;
    }

    [NonReorderable]
    public List<Menu> MenuScreens;
    public string CurrentScreen = "";
    string PreviousScreen = "";

    public bool ConsoleOpen = false;
    VolumeController VC;

    public static UI Instance;

    AudioSource AS;
    public AudioClip Click;

    Camera Cam;
    
    public TextMeshProUGUI SteamInfo;
    public TextMeshProUGUI DiscordInfo;

    void Awake()
    {
        if(Instance && Instance != this)
        {
            Destroy(this.gameObject);
        }else
        {
            Instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
    }

    void Start()
    {
        Cam = GetComponent<Camera>();
        AS = GetComponent<AudioSource>();
        VC = GetComponent<VolumeController>();
        SetScreen("Main");
        
        Invoke("CheckPrograms", 1f);
    }
    
    void CheckPrograms()
    {
        if (Online.Instance.GetComponent<FizzyFacepunch>().SteamUserID != 0)
        {
            SteamInfo.text = "Running";
        }
        else
        {
            SteamInfo.text = "Not Running";
            Debug.LogError("ERROR SteamConnectionFailed");
        }

        if(Online.Instance.discordRunning)
            DiscordInfo.text = "Running";
        else
            DiscordInfo.text = "Not Running";
    }

    void Update()
    {
        if (CurrentScreen == "Loading")
            return;

        if (ConsoleOpen)
        {
            if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.BackQuote))
                SetScreen(CurrentScreen);
        }
        else
        {
            if (Input.GetKeyDown(KeyCode.BackQuote))
            {
                SetScreen("Console");
            }
            else
            {
                if (CurrentScreen == "HUD")
                {
                    if (Input.GetKeyDown(KeyCode.Escape))
                    {
                        SetScreen("Pause");
                    }
                    else if (Input.GetKeyDown(KeyCode.Tab))
                    {
                        SetScreen("Scoreboard");
                    }
                }
                else if (CurrentScreen == "Pause")
                {
                    if (Input.GetKeyDown(KeyCode.Escape))
                        SetScreen("HUD");
                }
                else if (CurrentScreen == "Option")
                {
                    if (Input.GetKeyDown(KeyCode.Escape))
                        GoToLastMenu();
                }
                else if (CurrentScreen == "PlayerCustomization")
                {
                    if (Input.GetKeyDown(KeyCode.Escape))
                        GoToLastMenu();
                }
                else if (CurrentScreen == "Scoreboard")
                {
                    if (Input.GetKeyDown(KeyCode.Escape))
                        GoToLastMenu();
                    else if (Input.GetKeyDown(KeyCode.Tab))
                        GoToLastMenu();
                }else if (CurrentScreen == "Start")
                {
                    if (Input.GetKeyDown(KeyCode.Escape))
                        SetScreen("Main");
                }
            }
        }
    }

    public GameObject GetScreen(string Index)
    {
        for (int i = 0; i < MenuScreens.Count; i++)
        {
            if (MenuScreens[i].Title == Index)
                return MenuScreens[i].Screen;
        }

        return null;
    }

    public void SetScreen(string Index)
    {
        if (Index == "Console")
        {
            ConsoleOpen = true;
            for (int i = 0; i < MenuScreens.Count; i++)
            {
                if (MenuScreens[i].Title == Index)
                {
                    MenuScreens[i].Screen.SetActive(true);
                }
            }
        }
        else
        {
            if(!ConsoleOpen)
                PreviousScreen = CurrentScreen;
            ConsoleOpen = false;
            CurrentScreen = Index;

            for (int i = 0; i < MenuScreens.Count; i++)
            {
                if (MenuScreens[i].Title == Index)
                {
                    MenuScreens[i].Screen.SetActive(true);

                    if (VC)
                        VC.BackgroundBlur(MenuScreens[i].BlurBackground);
                    if (MenuScreens[i].LockCursor)
                    {
                        Cursor.lockState = CursorLockMode.Locked;
                        Cursor.visible = false;
                    }
                    else
                    {
                        Cursor.lockState = CursorLockMode.None;
                        Cursor.visible = true;
                    }
                }
                else
                {
                    MenuScreens[i].Screen.SetActive(false);
                }
            }
        }
    }

    public void CloseGame()
    {
        Debug.Log("Quit");
		Application.Quit();
    }

    public void GoToLastMenu()
    {
        SetScreen(PreviousScreen);
    }

    public void PlaySound()
    {
        AS.volume = (0.4f * (Settings._sfxVolume/100f)) * (Settings._masterVolume/100f);
        AS.clip = Click;
        AS.Play();
    }
    
    public bool Busy()
    {
        if(CurrentScreen == "HUD" && !ConsoleOpen)
            return false;
        else
            return true;
    }
}
