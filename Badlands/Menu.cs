using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.Rendering;

public abstract class Menu : MonoBehaviour
{
    [System.Serializable]
    public struct Screen
    {
        public string Title;
	    public GameObject Reference;
        public bool PausesGame;
        public bool LockCursor;
        public bool ShowVisionEffect;
    }
    
    [NonReorderable]
    public List<Screen> Screens;
    public string CurrentScreen = "";
    string PreviousScreen = "";
    Volume V;
    
    VisionEffectComponent Vision;
    void Initialize()
    {
        V = GameObject.FindWithTag("Volume").GetComponent<Volume>();

        if(V.profile.TryGet<VisionEffectComponent>(out VisionEffectComponent tmpVision) )
            Vision = tmpVision;
    }
    
    public void SetScreen(string Index)
    {
        if(!V)
            Initialize();

        PreviousScreen = CurrentScreen;
        CurrentScreen = Index;

        for(int i = 0; i < Screens.Count; i++)
        {
            if(Screens[i].Title == Index)
            {
                Screens[i].Reference.SetActive(true);

                if(Screens[i].PausesGame)
                {
                    Time.timeScale = 0f;
                }else
                {
                    Time.timeScale = 1f;
                }

                Vision.intensity.Override(Screens[i].ShowVisionEffect ? 1f : 0f);

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
        SetScreen(PreviousScreen);
    }
}
