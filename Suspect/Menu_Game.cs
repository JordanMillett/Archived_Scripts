using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Mirror;
using Steamworks;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class Menu_Game : Menu
{
    public TextMeshProUGUI Scoreboard;
    public E_Chat Chat;

    InputAction Action_Exit;
    InputAction Action_Scoreboard;

    public List<E_Bar> Needs;

    public Slider Master;
    public Slider SFX;
    public Slider Music;
    public Slider Voice;
    
    public Slider MicGain;

    public TextMeshProUGUI Timer;
    public TextMeshProUGUI YourName;
    public TextMeshProUGUI TargetInfo;
    public GameObject Seen;

    void RegisterInput()
    {
        Action_Exit = Game.ActionMap.FindAction("Exit");
        Action_Scoreboard = Game.ActionMap.FindAction("Scoreboard");

        Action_Exit.performed += Result_Exit;
        Action_Scoreboard.performed += Result_Scoreboard;
        
        Action_Exit.Enable();
        Action_Scoreboard.Enable();
    }
    
    void OnEnable()
    {
        SetScreen("HUD");

        Master.value = Game.SettingsData._masterVolume;
        SFX.value = Game.SettingsData._sfxVolume;
        Music.value = Game.SettingsData._musicVolume;
        Voice.value = Game.SettingsData._voiceVolume;
        MicGain.value = Game.SettingsData._micGain;
    }
    
    void OnDisable()
    {
        Chat.Clear();

        Game.SaveSettings();
    }
    
    void Start()
    {
        RegisterInput();
    }

    void Result_Exit(InputAction.CallbackContext action)
    {
        switch(CurrentScreenIndex)
        {
            case "HUD" : SetScreen("Pause"); break;
            case "Pause" : SetScreen("HUD"); break;
            case "Scoreboard" : SetScreen("HUD"); break;
        }
    }
    
    void Result_Scoreboard(InputAction.CallbackContext action)
    {
        switch(CurrentScreenIndex)
        {
            case "HUD" : SetScreen("Scoreboard"); break;
            case "Scoreboard" : SetScreen("HUD"); break;
        }
    }

    void Update()
    {
        Game.SettingsData._masterVolume = Mathf.RoundToInt(Master.value);
        Game.SettingsData._sfxVolume = Mathf.RoundToInt(SFX.value);
        Game.SettingsData._musicVolume = Mathf.RoundToInt(Music.value);
        Game.SettingsData._voiceVolume = Mathf.RoundToInt(Voice.value);
        Game.SettingsData._micGain = MicGain.value;
        
        Scoreboard.text = "";
        
        
        //SteamFriends.GetFriendPersonaName(Server.instance.Roster[key].ID)
        foreach (uint key in Server.instance.ServerRoster.Keys)
        {
            Scoreboard.text += Server.instance.ServerRoster[key].Username + " - $" + Server.instance.ServerRoster[key].BankMoney.ToString() + "\n";
        }
        
        if(Player.localPlayer)
        {
            int i = 0;
            foreach(Game.Needs key in Player.localPlayer.Needs.Keys)
            {
                Needs[i].Current = Player.localPlayer.Needs[key].Current;
                Needs[i].Max = Player.localPlayer.Needs[key].Max;

                Needs[i].gameObject.SetActive(Needs[i].Current / Needs[i].Max > 0.0f); //0.6


                i++;
            }

            YourName.text = Server.instance.ServerRoster[Player.localPlayer.netId].CharacterName;

            TargetInfo.text = "";

            Seen.SetActive(Player.localPlayer.Watchers.Count > 0);

            if(Server.instance.ServerRoster[Player.localPlayer.netId].Hunting)
            {
                TargetInfo.text += "Your Target\n\n";

                TargetInfo.text += Server.instance.ServerRoster[Server.instance.ServerRoster[Player.localPlayer.netId].Target_Id].CharacterName + "\n";
                TargetInfo.text += "Distance\t" + Vector3.Distance(Player.localPlayer.transform.position, NetworkClient.spawned[Server.instance.ServerRoster[Player.localPlayer.netId].Target_Id].transform.position).ToString("F0");

                Timer.text = "You have " + (Server.instance.NextRoundStart - NetworkTime.time).ToString("F0") + " seconds to hunt your target";
            }else
            {
                Timer.text = "Next round starts in " + (Server.instance.NextRoundStart - NetworkTime.time).ToString("F0") + " seconds";
            }
        }
    }
    
    public void Leave()
    {
        OnlineManager.singleton.GetComponent<SteamLobby>().Leave();
    }
}