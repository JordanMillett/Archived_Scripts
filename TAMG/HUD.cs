using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class HUD : MonoBehaviour
{
    public MenuManager MM;
    public TextMeshProUGUI Clock;
    public TextMeshProUGUI Money;

    public GameObject BuyPopupObject;
    public TextMeshProUGUI BuyName;
    public TextMeshProUGUI BuyPrice;

    Transform Cam;

    void Start()
    {
        Cam = GameObject.FindWithTag("Camera").transform;
    }

    void Update()
    {
        if(GameServer.GS.GameManagerInstance != null)
        {
            UpdateClock();
            Money.text = "$" + PlayerInfo.Balance.ToString();

            if(MenuManager.MMInstance.CurrentScreen == "HUD" && !MenuManager.MMInstance.ConsoleOpen)
            {
                //CameraControls();
                if(Input.GetKeyDown(KeyCode.Tab))
                {
                    MenuManager.MMInstance.SetScreen("Map");
                }

                if(Input.GetKeyDown(KeyCode.Escape))
                    MenuManager.MMInstance.SetScreen("Pause");

                if(Input.GetKeyDown("o"))
                    MenuManager.MMInstance.SetScreen("Freecam");

                BuyPopup();

            }else if(!MenuManager.MMInstance.ConsoleOpen)
            {
                if(Input.GetKeyDown(KeyCode.Escape) && MenuManager.MMInstance.CurrentScreen == "Pause")
                    MenuManager.MMInstance.SetScreen("HUD");

                if(Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.Tab))
                {
                    if(MenuManager.MMInstance.CurrentScreen == "Map")
                    {
                        MenuManager.MMInstance.SetScreen("HUD");
                    }
                }

                if((Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown("o")) && MenuManager.MMInstance.CurrentScreen == "Freecam")
                    MenuManager.MMInstance.SetScreen("HUD");
            }
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

    void BuyPopup()
    {
        RaycastHit hit;

        if(Physics.Raycast(Cam.transform.position, Cam.transform.forward, out hit, 5f))
        {
            try 
            {
                            
                PurchaseAble P = hit.collider.transform.root.gameObject.GetComponent<PurchaseAble>();

                if(P != null)
                {
                    BuyPopupObject.SetActive(true);
                    BuyName.text = P.Name;
                    BuyPrice.text = "$" + P.Cost;
                }else
                {
                    BuyPopupObject.SetActive(false);
                    BuyName.text = "ERROR";
                    BuyPrice.text = "ERROR";
                }
                    
            }
            catch{}
        }else
        {
            BuyPopupObject.SetActive(false);
            BuyName.text = "ERROR";
            BuyPrice.text = "ERROR";
        }
    }
}
