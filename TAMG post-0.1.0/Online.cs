using System.Collections;
using Steamworks;
using Steamworks.Data;
using UnityEngine;
using Mirror;
using System.Threading.Tasks;
using UnityEngine.SceneManagement;
using System.Linq;

public class Online : NetworkManager
{
    public static Online Instance;
    
    public static Lobby currentLobby;
    public Discord.Discord discord;

	public bool discordRunning = false;
    
    public override void Awake()
    {
        base.Awake();

        if(Instance && Instance != this)
        {
            Destroy(this.gameObject);
        }else
        {
            Instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
    }
	
	void Update() 
    {
		if(discordRunning)
			discord.RunCallbacks();
            
        if(!NetworkClient.isConnected && SceneManager.GetActiveScene().buildIndex == 1)
            UI.Instance.SetScreen("Loading");
    }

	public override void OnApplicationQuit()
    {
        base.OnApplicationQuit();

        if(discordRunning)
        	discord.Dispose();
    }
    
    public override void Start()
    {
        SteamFriends.OnGameLobbyJoinRequested += OnGameLobbyJoinRequested;
        
        for (int i = 0; i < System.Diagnostics.Process.GetProcesses().Length; i++)
        {
            try
            {
                if (System.Diagnostics.Process.GetProcesses()[i].ToString() == "System.Diagnostics.Process (Discord)")
                {
                    discordRunning = true;
                    break;
                }
            }catch{}
        }

		if(discordRunning)
		{
			discord = new Discord.Discord(790445532467298324, (System.UInt64)Discord.CreateFlags.Default);
			StatusMainMenu();
		}
    }
    
    public void StatusMainMenu()
	{
		if(discordRunning)
		{
			var activityManager = discord.GetActivityManager();
			var activity = new Discord.Activity
			{
				State = "",
				Details = "Staring at the Main Menu",				
				Assets =
				{
					LargeImage = "icon",
					LargeText = "The Amazing Mail Game"
				},
			};

			activityManager.UpdateActivity(activity, (res) => {});
		}
	}

	public void StatusInGame(int Day)
	{
		if(discordRunning)
		{
			var activityManager = discord.GetActivityManager();
			var activity = new Discord.Activity
			{
				State = "Day : " + Day.ToString(),
				Details = "Moving Boxes",				
				Assets =
				{
					LargeImage = "icon",
					LargeText = "The Amazing Mail Game"
				},
			};

			activityManager.UpdateActivity(activity, (res) => {});
		}
	}
    
    public async void OnGameLobbyJoinRequested(Lobby lobby, SteamId id)
    {
        if(SceneManager.GetActiveScene().buildIndex != 0)
            UI.Instance.SetScreen("Loading");
            
        while(SceneManager.GetActiveScene().buildIndex != 0)
            await Task.Delay(100);
        
        networkAddress = id.ToString();
        StartClient();

        float startTime = Time.time;
        float waitFor = GetComponent<Mirror.FizzySteam.FizzyFacepunch>().Timeout;

        while(Time.time < startTime + waitFor)
            if(NetworkClient.isConnected)
                break;
            else
                await Task.Delay(100);

        if(NetworkClient.isConnected)
        {
            UI.Instance.SetScreen("Loading");
        }else
        {
            Debug.LogError("ERROR FailedToJoinInvite");
        }
        
        
    }
    
    public async void CreateLobbyAsync()
    {
        bool result = await CreateLobby();
    }
    
    public static async Task<bool> CreateLobby()
    {
        try
        {
            var createLobbyOutput = await SteamMatchmaking.CreateLobbyAsync();
            if (!createLobbyOutput.HasValue)
            {
                Debug.Log("Lobby created but not correctly instantiated.");
                return false;
            }
            currentLobby = createLobbyOutput.Value;

            currentLobby.SetPublic();
            //currentLobby.SetPrivate();
            currentLobby.SetJoinable(true);

            return true;
        }
        catch(System.Exception exception)
        {
            Debug.Log("Failed to create multiplayer lobby : " + exception);
            return false;
        }
    }
    
    public void LeaveLobby()
    {
        currentLobby.Leave();
    }
    
    public override void OnStartServer()    //Prepare the server to recieve new messages
    {
        NetworkServer.RegisterHandler<ConnectMessage>(OnConnectToServer);

        CreateLobbyAsync();
    }
    
    public override void OnStopServer() 
    {
        LeaveLobby();
    }

    public override void OnClientConnect()  //Sent by the client when they connect to the server
    {
        StartCoroutine(PrepareClient());
    }

    public override void OnStopClient()//called when client is stopped
    {
        if(UI.Instance.CurrentScreen != "Loading")
            UI.Instance.SetScreen("Loading");
        LeaveLobby();
    }

    public override void OnServerDisconnect(NetworkConnectionToClient NC)   //Called on the server when someone leaves the server
    {
        //remove from scoreboard here
        NetworkServer.DestroyPlayerForConnection(NC);   //Destroy all traces of that person on the server
    }

    void OnConnectToServer(NetworkConnectionToClient NC, ConnectMessage message)
    {   
        if(NC.connectionId == 0)
            Client.Instance.SpawnManager(NC);

        int i = Client.Instance.ServerInstance.SpawnLocationIndex;

        GameObject NewPlayer = Instantiate(playerPrefab,
        Client.Instance.SpawnPoints[i].position,
        Client.Instance.SpawnPoints[i].rotation);
        NewPlayer.GetComponent<MirrorPlayer>().playerName = message.name;
        
        Client.Instance.SpawnObject("Veh_Golf", 
        Client.Instance.GetNearestVehicleSpawn(Client.Instance.SpawnPoints[i].position).position, 
        Client.Instance.GetNearestVehicleSpawn(Client.Instance.SpawnPoints[i].position).rotation
        , 1, false);

        Client.Instance.ServerInstance.SpawnLocationIndex++; //make command

        NetworkServer.AddPlayerForConnection(NC, NewPlayer);
    }
    
    IEnumerator PrepareClient()
    {
        while (SceneManager.GetActiveScene().buildIndex != 1)
            yield return null;

        StartCoroutine(Client.Instance.Init());
        while (!Client.Instance.LoadingComplete)
        {
            yield return null;
        }
        
        if (!NetworkClient.ready)
            NetworkClient.Ready();
        ConnectMessage message = new ConnectMessage 
        {
            name=SteamClient.Name.ToString(), 
            id=SteamClient.SteamId.ToString()
        };
        NetworkClient.Send(message);
    }

    public struct ConnectMessage : NetworkMessage
    {
        public string name;
        public string id;
    }
    
    new public void StopClient()
    {
        if(!NetworkClient.isConnected)
            return;
            
        OnStopClient();

        NetworkClient.Disconnect();
        NetworkClient.Shutdown();
    }
    
    new public void StopServer()
    {
        if (!NetworkServer.active)
            return;

        OnStopServer();

        NetworkServer.Shutdown();
    }
    
    void RegisterServerMessages()
    {
        NetworkServer.OnConnectedEvent = OnServerConnectInternal;
        NetworkServer.OnDisconnectedEvent = OnServerDisconnect;
        NetworkServer.OnErrorEvent = OnServerError;
        NetworkServer.RegisterHandler<AddPlayerMessage>(OnServerAddPlayerInternal);
        NetworkServer.ReplaceHandler<ReadyMessage>(OnServerReadyMessageInternal);
    }
    
    void OnServerConnectInternal(NetworkConnectionToClient conn)
    {
        OnServerAuthenticated(conn);
    }
    
    void OnServerAuthenticated(NetworkConnectionToClient conn)
    {
        conn.isAuthenticated = true;

        // proceed with the login handshake by calling OnServerConnect
        if (networkSceneName != "" && networkSceneName != offlineScene)
        {
            SceneMessage msg = new SceneMessage() { sceneName = networkSceneName };
            conn.Send(msg);
        }

        OnServerConnect(conn);
    }
        
    void OnServerReadyMessageInternal(NetworkConnectionToClient conn, ReadyMessage msg)
    {
        OnServerReady(conn);
    }

    void OnServerAddPlayerInternal(NetworkConnectionToClient conn, AddPlayerMessage msg)
    {
        if (conn.identity != null)
        {
            Debug.LogError("There is already a player for this connection.");
            return;
        }

        OnServerAddPlayer(conn);
    }
    
    void RegisterClientMessages()
    {
        NetworkClient.OnConnectedEvent = OnClientConnectInternal;
        NetworkClient.OnDisconnectedEvent = OnClientDisconnectInternal;
        NetworkClient.OnErrorEvent = OnClientError;
        NetworkClient.RegisterHandler<NotReadyMessage>(OnClientNotReadyMessageInternal);

        if (playerPrefab != null)
            NetworkClient.RegisterPrefab(playerPrefab);

        foreach (GameObject prefab in spawnPrefabs.Where(t => t != null))
            NetworkClient.RegisterPrefab(prefab);
    }
    
    void OnClientConnectInternal()
    {
        OnClientAuthenticated();
    }

    // called after successful authentication
    void OnClientAuthenticated()
    {
        NetworkClient.connection.isAuthenticated = true;
        OnClientConnect();
    }

    void OnClientDisconnectInternal()
    {
        OnClientDisconnect();
    }

    void OnClientNotReadyMessageInternal(NotReadyMessage msg)
    {
        NetworkClient.ready = false;
        OnClientNotReady();
    }
    
    new public void StartHost()
    {
        if (NetworkServer.active || NetworkClient.active)
        {
            Debug.LogWarning("Server or Client already started.");
            return;
        }

        Application.runInBackground = true;
        NetworkServer.Listen(maxConnections);
        OnStartServer();
        RegisterServerMessages();
     
        NetworkClient.ConnectHost();
        NetworkServer.SpawnObjects();

        networkAddress = "localhost";
        NetworkServer.ActivateHostScene();
        RegisterClientMessages();
        NetworkClient.ConnectLocalServer();
        OnStartClient();
    }
    
    
    new public void StartServer()
    {
        if (NetworkServer.active)
        {
            Debug.LogWarning("Server already started");
            return;
        }

        Application.runInBackground = true;

        NetworkServer.Listen(maxConnections);

        OnStartServer();

        RegisterServerMessages();
        
        NetworkServer.SpawnObjects();
    }
    
    new public void StartClient()
    {
        if (NetworkClient.active)
        {
            Debug.LogWarning("Client already started.");
            return;
        }

        Application.runInBackground = true;

        RegisterClientMessages();

        NetworkClient.Connect(networkAddress);

        OnStartClient();
    }


}
