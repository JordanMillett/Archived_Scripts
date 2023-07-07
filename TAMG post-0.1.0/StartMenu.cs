using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Mirror;

public class StartMenu : MonoBehaviour
{
    public TMP_InputField HostText;
    public TMP_InputField JoinText;

    public TextMeshProUGUI Error;

    Mirror.FizzySteam.FizzyFacepunch FFP;

    IEnumerator TimeoutWait;

    LoadingScreen LS;

    void Awake()
    {
        LS = UI.Instance.GetScreen("Loading").GetComponent<LoadingScreen>();
        FFP = Online.Instance.GetComponent<Mirror.FizzySteam.FizzyFacepunch>();
    }
    
    void Start()
    {
        SetHostID();
    }

    void OnEnable()
    {
        Error.text = "";
        Error.color = Color.black;
    }

    void OnDisable()
    {
        if(LS.CurrentState == LoadState.Joining)
        {
            LS.CurrentState = LoadState.Idle;
            StopCoroutine(TimeoutWait);
        }
    }

    public void Host()
    {
        if(LS.CurrentState != LoadState.Joining)
        {
            LS.CurrentState = LoadState.StartingHost;
            UI.Instance.SetScreen("Loading");
        }
    }

    public void Join()
    {
        if(LS.CurrentState == LoadState.Joining)
            StopCoroutine(TimeoutWait);

        if(JoinText.text.Length == 17)
        {
            Error.text = "Loading...";
            Error.color = Color.white;

            LS.CurrentState = LoadState.Joining;
            
            Online.Instance.networkAddress = JoinText.text;
            Online.Instance.StartClient();

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
        if(LS.CurrentState == LoadState.Joining) //waiting for load, if client suddenly exists then connect
        {
            if(NetworkClient.isConnected)
            {
                UI.Instance.SetScreen("Loading");    //used to set loading screen
            }
        }
    }

    IEnumerator TimeoutTimer()
    {
        yield return new WaitForSeconds(FFP.Timeout);
        Error.text = "Failed";
        Error.color = Color.red;
        LS.CurrentState = LoadState.Idle;
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
}
