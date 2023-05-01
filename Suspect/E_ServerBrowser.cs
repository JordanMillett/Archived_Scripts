using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Steamworks;
using UnityEngine.UI;
using Mirror;

public class E_ServerBrowser : MonoBehaviour
{
    public List<GameObject> Lobbies = new List<GameObject>();
    public GameObject LobbyPrefab;
    public Transform LobbyParent;

    void Awake()
    {
        Refresh();
    }
    
    public void Refresh()
    {
        OnlineManager.singleton.GetComponent<SteamLobby>().GetListOfLobbies();
    }

    public void DisplayLobbies(List<CSteamID> lobbyIDS, LobbyDataUpdate_t result)
    {
        ClearLobbies();

        int Yloc = 0;
        for(int i = 0; i < lobbyIDS.Count; i++)
        {
            GameObject ListItem = Instantiate(LobbyPrefab, Vector3.zero, Quaternion.identity);
            ListItem.transform.SetParent(LobbyParent.transform);
            ListItem.GetComponent<RectTransform>().anchoredPosition = new Vector3(0f, Yloc, 0f);
            ListItem.GetComponent<RectTransform>().transform.localScale = Vector3.one;
            
            ListItem.GetComponent<E_Lobby>().Name.text = SteamMatchmaking.GetLobbyData((CSteamID)lobbyIDS[i].m_SteamID, "name");
            ListItem.GetComponent<E_Lobby>().PlayerCount.text = SteamMatchmaking.GetNumLobbyMembers((CSteamID)lobbyIDS[i].m_SteamID) + " / 16";
            AddListener(ListItem.GetComponent<E_Lobby>().JoinButton, lobbyIDS[i]);

            Yloc -= 125;

            Lobbies.Add(ListItem);
        }
    }
    
    void AddListener(Button B, CSteamID Value)
    {
        B.onClick.AddListener(delegate {Join(Value);});
    }
    
    public void Join(CSteamID ID)
    {
        if(!NetworkClient.active)
            OnlineManager.singleton.GetComponent<SteamLobby>().JoinLobby(ID);
    }
    
    void ClearLobbies()
    {
        Debug.Log("Clearing Lobbies");
        foreach (GameObject Lobby in Lobbies)
        {
            GameObject Target = Lobby;
            Destroy(Target);
            Target = null;
        }
        Lobbies.Clear();
    }
}
