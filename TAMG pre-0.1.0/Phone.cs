using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class Phone : MonoBehaviour
{
    float downPosition = -540f;
    float upPosition = -190f;
    float offset = 500f;
    public float lerpAlpha = 0.02f;

    public bool Out = false;

    public RectTransform Select;
    public List<Vector2> SelectorPositions;
    public List<App> Apps;
    int selectIndex = 0;

    GameManager GM;
    public TextMeshProUGUI Clock;

    public bool AppOpen = false;

    public bool PopupOpen = false;

    Animator An;

    void Start()
    {
        GM = GameObject.FindWithTag("Manager").GetComponent<GameManager>();
        An = GameObject.FindWithTag("Player").GetComponent<Player>().An;
    }

    void Update()
    {
        if(GameServer.GS.GameManagerInstance != null)
            UpdateClock();

        //An.SetBool("Phone", Out);

        if(PopupOpen)
        {

            

        }else
        {
            if(!AppOpen)
            {
                if(!Out)
                {
                    if(Input.mouseScrollDelta.y != 0f || Input.GetMouseButtonDown(1))
                    {
                        TogglePhone();
                    }
                }else
                {
                    if(Input.mouseScrollDelta.y > 0f)
                    {
                        MoveSelector(1);
                    }

                    if(Input.mouseScrollDelta.y < 0f)
                    {
                        MoveSelector(-1);
                    }

                    if(Input.GetMouseButtonDown(1))
                    {
                        TogglePhone();
                    }

                    if(Input.GetMouseButtonDown(0))
                    {
                        AppOpen = true;
                        Apps[selectIndex].Toggle();
                    }
                }
            }else
            {
                if(Input.GetMouseButtonDown(1))
                {
                    ExitApp();
                }
            }
        }
    }

    public void ExitApp()
    {
        AppOpen = false;
        Apps[selectIndex].Toggle();
    }

    void MoveSelector(int Direction)
    {
        int newIndex = selectIndex + Direction;
        if(newIndex >= 0 && newIndex < SelectorPositions.Count)
        {
            selectIndex = newIndex;
        }else if(newIndex < 0)
        {
            selectIndex = SelectorPositions.Count - 1;
        }else if(newIndex == SelectorPositions.Count)
        {
            selectIndex = 0;
        }

        Select.anchoredPosition = SelectorPositions[selectIndex];
    
    }

    void TogglePhone()
    {
        Out = !Out;

        if(Out)
        {
            this.transform.GetComponent<RectTransform>().anchoredPosition = new Vector2(offset, upPosition);
        }else
        {
            if(AppOpen)
            {
                ExitApp();
                //Apps[selectIndex].Toggle();
            }
            this.transform.GetComponent<RectTransform>().anchoredPosition = new Vector2(offset, downPosition);
        }
    }

    void UpdateClock()
    {
        string ClockText = "";
        string suffix = "";

        if(GameServer.GS.GameManagerInstance.CurrentTime.Hours > 12)
        {
            if(GameServer.GS.GameManagerInstance.CurrentTime.Hours - 12 < 10)
                ClockText += "0";
            ClockText += GameServer.GS.GameManagerInstance.CurrentTime.Hours - 12;
    
        }else
        {
            suffix = "am";
            if(GameServer.GS.GameManagerInstance.CurrentTime.Hours < 10)
                ClockText += "0";
            ClockText += GameServer.GS.GameManagerInstance.CurrentTime.Hours;
        }

        if(GameServer.GS.GameManagerInstance.CurrentTime.Hours >= 12)
            suffix = "pm";

        if(GameServer.GS.GameManagerInstance.CurrentTime.Minutes < 10)
            ClockText += ":0" + GameServer.GS.GameManagerInstance.CurrentTime.Minutes;
        else
            ClockText += ":" + GameServer.GS.GameManagerInstance.CurrentTime.Minutes;

        Clock.text = ClockText + suffix;
    }
}
