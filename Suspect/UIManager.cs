using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager : Menu
{
    public Camera Cam;
    public Camera View;

    public Menu_Main M_Main;
    public Menu_Game M_Game;
    
    public GameObject Hands;
    
    public static UIManager instance { get; private set; }
    
    void Awake()
    {
        if(!instance)
            instance = this;
    }
    
    void Start()
    {
        Leave();
    }
    
    public void Leave()
    {
        SceneManager.LoadScene(1, LoadSceneMode.Single);
        SetScreen("Main");
    }

    public void CloseGame()
    {
        Debug.Log("Quit");
		Application.Quit();
    }
}