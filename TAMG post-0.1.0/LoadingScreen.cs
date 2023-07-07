using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using Mirror;

public enum LoadState
{
    Idle,
    StartingHost,
    Joining,
    Client,
    Host,
    Leaving
};

public class LoadingScreen : MonoBehaviour
{   
    public LoadState CurrentState = LoadState.Idle;

    void Start()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnEnable()
    {
        if (SceneManager.GetActiveScene().buildIndex == 0)     //loading in
        {
            SceneManager.LoadScene(1, LoadSceneMode.Single);
        }

        if (SceneManager.GetActiveScene().buildIndex == 1)    //loading out
        {
            CurrentState = LoadState.Leaving;
            SceneManager.LoadScene(0, LoadSceneMode.Single);
        }
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if(SceneManager.GetActiveScene().buildIndex == 1)
        {
            if (CurrentState == LoadState.StartingHost)
            {
                Online.Instance.StartHost();
                CurrentState = LoadState.Host;
            }else if(CurrentState == LoadState.Joining)
            {
                CurrentState = LoadState.Client;
            }
        }

        if(SceneManager.GetActiveScene().buildIndex == 0)
        {
            StartCoroutine(LoadOut());
        }
    }
    
    IEnumerator LoadOut()
    {
        if(NetworkClient.isConnected)
            Online.Instance.StopClient();
        while(NetworkClient.isConnected)
            yield return null;
        
        if(NetworkServer.active)
            Online.Instance.StopServer();
        while(NetworkServer.active)
            yield return null;

        Client.Instance.Reset();
        UI.Instance.SetScreen("Main");
        Online.Instance.StatusMainMenu();
        CurrentState = LoadState.Idle;
    }
}
