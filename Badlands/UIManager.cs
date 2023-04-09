using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager : Menu
{
    public static UIManager UI;

    public Menu_Loading M_Loading;
    public Menu_Game M_Game;
    
    void Awake()
    { 
        UI = this;
    }

    void OnEnable()
    {
        SetScreen("Main");
    }
    
    public void Reset()
    {
        SceneManager.LoadScene(0, LoadSceneMode.Single);
    }

    public void CloseGame()
    {
        Debug.Log("Quit");
		Application.Quit();
    }
}