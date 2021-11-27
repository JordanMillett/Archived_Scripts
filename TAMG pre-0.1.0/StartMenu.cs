using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using Mirror;

public class StartMenu : MonoBehaviour
{

    public TMP_InputField HostText;
    public TMP_InputField JoinText;

    public TextMeshProUGUI Error;

    Mirror.FizzySteam.FizzyFacepunch FFP;

    MenuManager MM;

    OnlineManager OM;

    int WaitingForLoad = 0; //False, Host, Join

    IEnumerator TimeoutWait;

    void Awake()
    {
        FFP = GameObject.FindWithTag("Information").GetComponent<Mirror.FizzySteam.FizzyFacepunch>();
        OM = FFP.gameObject.GetComponent<OnlineManager>();
        MM = GameObject.FindWithTag("Camera").GetComponent<MenuManager>();
    }
    
    void Start()
    {
        SceneManager.sceneLoaded += OnSceneLoaded; 
        SetHostID();
    }

    void OnEnable()
    {
        Error.text = "";
        Error.color = Color.black;
    }

    void OnDisable()
    {
        if(WaitingForLoad == 2)
        {
            WaitingForLoad = 0;
            StopCoroutine(TimeoutWait);
        }
    }

    void SetHostID()
    {
        if(FFP.SteamUserID != 0)
        {
            HostText.text = FFP.SteamUserID.ToString();
        }else
        {
            Invoke("SetHostID", 0.5f);
        }
    }

    public void Host()
    {
        //Debug.Log("Host Button Pressed");

        //OM.StartServer();
        if(WaitingForLoad != 2)
        {
            WaitingForLoad = 1;
            MM.SetScreen("Loading"); //used to be loading
        }
    }

    public void Join()
    {
        if(WaitingForLoad == 2)
            StopCoroutine(TimeoutWait);

        if(JoinText.text.Length == 17)
        {
            Error.text = "Loading...";
            Error.color = Color.white;

            WaitingForLoad = 2;
            
            OM.networkAddress = JoinText.text;
            OM.StartClient();

            TimeoutWait = TimeoutTimer();
            StartCoroutine(TimeoutWait);
        }else
        {
            Error.text = "Wrong Length";
            Error.color = Color.red;
        }
    }

    void Update()
    {
        //Debug.Log(NetworkClient.isConnected);

        if(Input.GetKeyDown(KeyCode.Escape))
        {
            MM.SetScreen("Main");
        }

        if(WaitingForLoad == 2) //waiting for load, if client suddenly exists then connect
        {
            if(NetworkClient.isConnected)
            {
                MM.clientConnected = true;
                MM.SetScreen("Loading");    //used to set loading screen
            }
        }
    }

    IEnumerator TimeoutTimer()
    {
        yield return new WaitForSeconds(FFP.Timeout);
        Error.text = "Failed";
        Error.color = Color.red;
        WaitingForLoad = 0;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if(WaitingForLoad == 2)     //JOIN
        {
            WaitingForLoad = 0;
        }else if(WaitingForLoad == 1)    //HOST
        {
            WaitingForLoad = 0;
            OM.StartHost();
        }
    }
}
