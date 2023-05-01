using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Menu : MonoBehaviour
{
    [System.Serializable]
    public struct Screen
    {
        public string Title;
	    public GameObject Reference;
        public bool LockCursor;
    }
    
    [NonReorderable]
    public List<Screen> Screens;
    public string CurrentScreenIndex = "";
    public Screen CurrentScreen;
    
    private string previousScreenIndex = "";

    public void SetScreen(string Index)
    {
        previousScreenIndex = CurrentScreenIndex;
        CurrentScreenIndex = Index;

        for(int i = 0; i < Screens.Count; i++)
        {
            if(Screens[i].Title == Index)
            {
                CurrentScreen = Screens[i];

                Screens[i].Reference.SetActive(true);
                
                if(Screens[i].LockCursor)
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
                Screens[i].Reference.SetActive(false);
            }
        }
    }
    
    public void GoToLastMenu()
    {
        SetScreen(previousScreenIndex);
    }
}