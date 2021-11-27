using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Mirror;

public class MenuManager : MonoBehaviour
{
    [System.Serializable]
    public struct Menu
    {
        public string Title;
	    public GameObject Screen;
        //public int PausesGame;
        public bool BlurBackground;
        public bool LockCursor;
        public int SceneIndex;
        public float FOV;
    }

    public CustomizationMenu CM;

    public List<Menu> MenuScreens;
    public string CurrentScreen = "";
    string PreviousScreen = "";
    
    VolumeController VC;

    public static MenuManager MMInstance;

    //Mirror.FizzySteam.FizzyFacepunch FFP;
    OnlineManager OM;

    AudioSource AS;
    public AudioClip Click;

    public bool clientConnected = false;

    Camera Cam;

    public float fovOffset;

    public bool ConsoleOpen = false;

    void Awake()
    { 
        if(MMInstance == null)
        {
            MMInstance = this;
            DontDestroyOnLoad(this.gameObject);
        }else
        {
            Destroy(this.gameObject);
        }
    }

    void Start()
    {
        Cam = GetComponent<Camera>();
        AS = GetComponent<AudioSource>();
        //FFP = GameObject.FindWithTag("Information").GetComponent<Mirror.FizzySteam.FizzyFacepunch>();
        OM = GameObject.FindWithTag("Information").GetComponent<OnlineManager>();
        SceneManager.sceneLoaded += OnSceneLoaded;
        VC = GameObject.FindGameObjectWithTag("Volume").GetComponent<VolumeController>();
        SetScreen("Main");
    }

    void Update()
    {
        Cam.fieldOfView = 90f + fovOffset;

        if(clientConnected)
        {
            if(!NetworkClient.isConnected)
            {
                clientConnected = false;
                SetScreen("Main");
            }
        }
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        //Debug.Log("Scene Changed : " + scene.name);
        VC = GameObject.FindGameObjectWithTag("Volume").GetComponent<VolumeController>();
        if(CurrentScreen == "Main")
        {
            this.transform.position = new Vector3(0f, 2f, 0f);
            this.transform.eulerAngles = new Vector3(0f, 90f, 0f);
        }else
        {
            //Debug.Log("Scene load changed, ready to call init");
            //GameServer.GS.Init();
            StartCoroutine(GameServer.GS.Init());
            //CM.Init();
        }
            //FFP.ServerStop();
        SetScreen(CurrentScreen);
    }

    public void SetScreen(string Index)
    {
        PreviousScreen = CurrentScreen;
        CurrentScreen = Index;

        

        //Debug.Log("Current Screen : " + MenuScreens[CurrentScreen].Screen.name);

        for(int i = 0; i < MenuScreens.Count; i++)
        {
            if(MenuScreens[i].Title == Index)
            {
                if(MenuScreens[i].SceneIndex != -1 && SceneManager.GetActiveScene().buildIndex != MenuScreens[i].SceneIndex)
                {
                    if(MenuScreens[i].SceneIndex == 0)
                    {
                        if (NetworkServer.active && NetworkClient.isConnected)
                        {
                            OM.StopHost();
                        }else if (NetworkClient.isConnected)
                        {
                            OM.StopClient();
                        }  
                    }

                    SceneManager.LoadScene(MenuScreens[i].SceneIndex, LoadSceneMode.Single);
                    return;
                }

                //Debug.Log(Index);
                
                MenuScreens[i].Screen.SetActive(true);
                /*
                if(MenuScreens[i].PausesGame != -1)
                {
                    GameInfo.Paused = MenuScreens[i].PausesGame == 1;
                }*/

                //GetComponent<Camera>().fieldOfView = MenuScreens[i].FOV;

                if(GameInfo.Paused)
                {
                    Time.timeScale = 0f;
                }else
                {
                    Time.timeScale = 1f;
                }

                VC.BackgroundBlur(MenuScreens[i].BlurBackground);
                if(MenuScreens[i].LockCursor)
                {
                    Cursor.lockState = CursorLockMode.Locked;
                    Cursor.visible = false;
                }else
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
}
