using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Steamworks;
using System.Text;
using TMPro;
using UnityEngine.InputSystem;

public class E_Chat : MonoBehaviour
{
    public TMP_InputField Chat;
    public TextMeshProUGUI ChatBox;

    string[] Lines = new string[8];

    InputAction Action_OpenChat;
    void RegisterInput()
    {
        Action_OpenChat = Game.ActionMap.FindAction("Open Chat");
        
        Action_OpenChat.performed += Result_OpenChat;

        Action_OpenChat.Enable();
    }
    
    void Result_OpenChat(InputAction.CallbackContext action)
    {
        if(Chat.isFocused)
            return;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        Chat.ActivateInputField();
    }

    void Start()
    {
        RegisterInput();

        Chat.onSubmit.AddListener(SubmitChat);
        Chat.onDeselect.AddListener(ExitChat);
    }
    
    void SubmitChat(string message)
    {
        if(Chat.text != "")
        {
            Say(SteamUser.GetSteamID(), Chat.text);
            Chat.text = "";
            Chat.DeactivateInputField();
        }
    }
    
    void ExitChat(string message)
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void Say(CSteamID user, string message)
    {
        //byte[] bytes = Encoding.ASCII.GetBytes(SteamFriends.GetFriendPersonaName(user).ToString() + ": " + message);
        byte[] bytes = Encoding.ASCII.GetBytes(Server.instance.ServerRoster[Server.instance.SteamRoster[user]].Username.ToString() + ": " + message);

        SteamMatchmaking.SendLobbyChatMsg((CSteamID) OnlineManager.singleton.GetComponent<SteamLobby>().current_lobbyID, bytes, bytes.Length);
    }
    
    public void Clear()
    {
        for (int i = 0; i < Lines.Length; i++)
            Lines[i] = "";

        ChatBox.text = "";
    }
    
    public void Hear(string message)
    {
        for (int i = 0; i < Lines.Length - 1; i++)
            Lines[i] = Lines[i + 1];
        Lines[Lines.Length - 1] = message;

        ChatBox.text = string.Join("\n", Lines);
    }
}
