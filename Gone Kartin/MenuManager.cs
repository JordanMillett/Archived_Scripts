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
        public UnityEvent Action;
    }

    public List<Menu> MenuScreens;
    public string CurrentScreen = "";
    string PreviousScreen = "";

    void Start()
    {
        SetScreen("Main");
    }

    public void SetScreen(string ToLoad)
    {
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
            if(MenuScreens[i].Title == ToLoad)      //if match found to enable
            {
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

                MenuScreens[i].Action.Invoke();
            }
            else
            {
                MenuScreens[i].Screen.SetActive(false); //disable wrong menus
            }
        }
    }

    public void GoToLastMenu()
    {
        SetScreen(PreviousScreen);
    }
}