using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    [System.Serializable]
    public struct Menu
    {
        public string Title;
	    public GameObject Screen;
        public bool PausesGame;
        public bool LockCursor;
        public int SceneIndex;
    }

    public List<Menu> MenuScreens;
    public string CurrentScreen = "";
    string PreviousScreen = "";

    public static MenuManager MM;

    AudioSource AS;
    public AudioClip Click;

    void Awake()
    { 
        MM = this;
    }

    void Start()
    {
        AS = GetComponent<AudioSource>();
        SetScreen("Main");
    }

    public void Reset()
    {
        Game.ResetGame();
        SceneManager.LoadScene(1, LoadSceneMode.Single);
    }

    public void SetScreen(string Index)
    {
        PreviousScreen = CurrentScreen;
        CurrentScreen = Index;

        for(int i = 0; i < MenuScreens.Count; i++)
        {
            if(MenuScreens[i].Title == Index)
            {
                if(MenuScreens[i].SceneIndex != -1 && SceneManager.GetActiveScene().buildIndex != MenuScreens[i].SceneIndex)
                {
                    SceneManager.LoadScene(MenuScreens[i].SceneIndex, LoadSceneMode.Single);
                    return;
                }
                
                MenuScreens[i].Screen.SetActive(true);

                if(MenuScreens[i].PausesGame)
                {
                    Time.timeScale = 0f;
                }else
                {
                    Time.timeScale = 1f;
                }

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
