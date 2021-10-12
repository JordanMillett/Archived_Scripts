using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class MenuManager : MonoBehaviour
{
    [System.Serializable]
    public struct Menu
    {
        public string Title;
	    public GameObject Screen;
        public bool LockCursor;
        public bool PauseGame;
        public UnityEvent Action;
    }

    Manager M;

    public List<Menu> MenuScreens;
    public string CurrentScreen = "";
    //public int CurrentScreenIndex = 0;
    string PreviousScreen = "";

    void Start()
    {
        M = GameObject.FindWithTag("Manager").GetComponent<Manager>();
        SetScreen(CurrentScreen);
    }

    public void SetScreen(string ToLoad)
    {

        //Debug.Log(ToLoad);

        PreviousScreen = CurrentScreen;

        for(int i = 0; i < MenuScreens.Count; i++)  //loop every screen
        {
            if(MenuScreens[i].Title == PreviousScreen)      //if match found to activate
            {
                MenuScreens[i].Action.Invoke();
            }
        }

        CurrentScreen = ToLoad;

        for(int i = 0; i < MenuScreens.Count; i++)  //loop every screen
        {

            if(MenuScreens[i].Title == CurrentScreen)      //if match found to enable
            {
                //CurrentScreenIndex = i; //to remove?

                MenuScreens[i].Screen.SetActive(true);  //enable it
    
                if(MenuScreens[i].LockCursor)
                {
                    Cursor.lockState = CursorLockMode.Locked;
                    Cursor.visible = false;
                }else
                {
                    Cursor.lockState = CursorLockMode.None;
                    Cursor.visible = true;
                }

                M.Paused = MenuScreens[i].PauseGame;
                Time.timeScale = M.Paused ? 0f : 1f;

                MenuScreens[i].Action.Invoke();
            }
            else
            {
                if(MenuScreens[i].Title == "HUD" && M.C.Active)
                    M.C.Toggle();

                MenuScreens[i].Screen.SetActive(false); //disable wrong menus
            }
        }
    }

    public void GoToLastMenu()
    {
        SetScreen(PreviousScreen);
    }
}