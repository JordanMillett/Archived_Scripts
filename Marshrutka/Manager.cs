using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class Manager : MonoBehaviour
{
    //DATATYPES

    //PUBLIC COMPONENTS
    public static Manager MInstance;
    public ManagerUI MUI;
    public GameObject Event;
    public TextBox TB;
    public Console C;

    //PUBLIC VARS
    public int TimeIndex = 0;
    public GameSettings.Month ThisMonth;
    public int ThisDay;
    public int ThisYear;
    public bool Paused = false;
    public bool GameStarted = false;
    [HideInInspector]
    public string TowNumber = "4990551336";

    //PUBLIC LISTS
    public List<Conversation> Conversations;  
    public List<Person> People;  
    public List<Texture2D> MaleFaces;
    public List<Texture2D> FemaleFaces;    
    public List<GameObject> Cars;    

    //COMPONENTS
    PlayerController Bus;
    MenuManager MM;
    AudioSourceController ASC_Ambient;
    
    Map M;

    //VARS
    WaitForSeconds TickSpeed = new WaitForSeconds(1f);
    IEnumerator NightCoroutine;

    //LISTS
    List<BusStop> AllStops = new List<BusStop>();

    void Awake()
    { 
        if(MInstance == null)
        {
            MInstance = this;
            DontDestroyOnLoad(this.gameObject);
        }else
        {
            Destroy(this.gameObject);
        }
    }

    void Start()
    {
        ASC_Ambient = GetComponent<AudioSourceController>();
        MM = GameObject.FindWithTag("MenuManager").GetComponent<MenuManager>();
        Event.SetActive(true);

        PlayAmbient();

        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = 60;

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    public void PickSave(int Index)
    {
        GameSettings.SC.SaveIndex = Index;
        if(!GameSettings.SC.GetNew())
            GameSettings.SC.SetNight(GameSettings.SC.GetNight() - 1);

        if(GameSettings.SC.GetNew())
        {
            MM.SetScreen("Intro");
        }else
        {
            LaunchGame();
        }
    }

    public void LaunchGame()
    {
        if(!GameStarted)
        {
            MM.SetScreen("Loading");
            StartCoroutine(LaunchGameFreezeDelay());
        }
    }

    IEnumerator LaunchGameFreezeDelay()
    {
        yield return null;
        SceneManager.LoadScene("Game");
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if(scene.buildIndex != 1)
        {
            StartGame();
        }else
        {
            MM.SetScreen("Main");
            if(GameSettings.SC.path != "")
            {
                GameSettings.SC.LoadSaves();
            }
        }
    }

    void StartGame()
    {
        M = GameObject.FindWithTag("Map").GetComponent<Map>();
        Bus = GameObject.FindWithTag("Player").GetComponent<PlayerController>();
        Bus.SetFrozen(true);

        AllStops.Clear();
        foreach (Transform child in M.BusStops)
        {
            AllStops.Add(child.GetComponent<BusStop>());
        }

        ThisMonth = (GameSettings.Month)Random.Range(0, GameSettings.Month.GetNames(typeof(GameSettings.Month)).Length);
        ThisDay = Random.Range(1, GameSettings.MonthDays[(int) ThisMonth] + 1);
        UpdateCalendar();

        MUI.Initialize();

        //PlayAmbient();

        StartCoroutine(M.Load());
    }

    void PlayAmbient()
    {
        ASC_Ambient.SetVolume(1f);
        ASC_Ambient.Play();
    }

    public void StartNewNight()     //CALLED TO PROMPT START AND BEGIN LOOP
    {
        if(TimeIndex != 0)
            StopCoroutine(NightCoroutine);

        NightCoroutine = NightLoop();
        StartCoroutine(NightCoroutine);
    }

    void Update()
    {
        ASC_Ambient.Refresh();

        if(Input.GetKeyDown(KeyCode.Escape))
        {
            if(MM.CurrentScreen == "HUD" || MM.CurrentScreen == "Pause" && !C.Active)
            {
                if(Paused)
                    MM.SetScreen("HUD");
                else
                    MM.SetScreen("Pause");
            }

            if(MM.CurrentScreen == "Credits")
            {
                MM.GoToLastMenu();
            }
        }

        if(Input.GetKeyDown(KeyCode.BackQuote))
        {
            if(Settings._devConsoleEnabled && MM.CurrentScreen == "HUD")
            {
                C.Toggle();
            }
        }
    }

    IEnumerator NightLoop()     //THE LOOP THAT COUNTS THE TIME AND THEN ENDS
    {
        foreach (BusStop BS in AllStops)
        {
            BS.Closed = (Random.value < GameSettings.ClosedChance);
            BS.GenerateNPCs();
        }

        Debug.Log("Night Started");

        Bus.ClearPassengers();
        Bus.Fuel = Bus.MaxFuel;
        Bus.GetComponent<Rigidbody>().velocity = Vector3.zero;
        Bus.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
        Bus.transform.position = M.Spawn.position;
        Bus.transform.rotation = M.Spawn.rotation;
        Bus.Unconscious = false;

        MUI.StartMoney = GameSettings.SC.GetMoney();
        MUI.TopSpeed = 0f;
        MUI.Fines = 0;

        Bus.SetFrozen(false);

        TimeIndex = 0;

        if(GameObject.FindWithTag("Camera").transform.childCount > 0)
        {
            GameObject.FindWithTag("Camera").transform.GetChild(0).GetComponent<DashObject>().RemoveFromHands();
        }

        Rigidbody r = Bus.GetComponent<Rigidbody>();
        while(TimeIndex < GameSettings.NightDuration)
        {
            if(!Bus.B.isWarning)
            {
                Bus.B.WarningText.fontSize = 0.02f;

                if((GameSettings.NightDuration - TimeIndex) > 0)
                    Bus.B.WarningText.text = (GameSettings.NightDuration - TimeIndex).ToString();
                else
                    Bus.B.WarningText.text = "DONE";
            }

            if(!Bus.BusStopped)
                TimeIndex++;
            yield return TickSpeed;

            if(r.velocity.magnitude > MUI.TopSpeed)
                MUI.TopSpeed = r.velocity.magnitude;
        }

        while(Bus.BusStopped)
        {
            Debug.Log("NIGHT POSTPONED FOR BUS");
            yield return null;
        }

        
        Bus.B.PlayAlarm();
        
        yield return new WaitForSeconds(3f);

        MUI.ShowResultsMenu();
    }

    public Document.Information GenerateInformation(Passenger P)
    {
        Document.Information Generated = new Document.Information();

        Generated.Male = P.Male;

        if(Generated.Male)
        {
            Generated.FirstName = GameSettings.MaleFirstNames[Random.Range(0, GameSettings.MaleFirstNames.Count)];
            Generated.LastName = GameSettings.MaleLastNames[Random.Range(0, GameSettings.MaleLastNames.Count)];
        }else
        {
            Generated.FirstName = GameSettings.FemaleFirstNames[Random.Range(0, GameSettings.FemaleFirstNames.Count)];
            Generated.LastName = GameSettings.FemaleLastNames[Random.Range(0, GameSettings.FemaleLastNames.Count)];
        }

        Generated.Weight = P.Weight;
        Generated.HeightFoot = P.HeightFoot;
        Generated.HeightInch = P.HeightInch;

        int DayOffset = Random.Range(GameSettings.ExpirationOffsetRange.x, GameSettings.ExpirationOffsetRange.y);
        Generated.ExpirationDay = ThisDay;
        Generated.ExpirationMonth = ThisMonth;
        Generated.ExpirationYear = ThisYear;

        if(ThisDay + DayOffset > GameSettings.MonthDays[(int)ThisMonth])
        {
            DayOffset -= GameSettings.MonthDays[(int)ThisMonth] - ThisDay;
            Generated.ExpirationDay = DayOffset;

            if((int)ThisMonth == 11)
            {
                Generated.ExpirationMonth = (GameSettings.Month) 0;
                Generated.ExpirationYear++;
            }else
            {
                Generated.ExpirationMonth = (GameSettings.Month) ((int)Generated.ExpirationMonth + 1);
            }
        }else
        {
            Generated.ExpirationDay = ThisDay + DayOffset;
        }

        Generated.Citizenship = GameSettings.Republics[Random.Range(0, GameSettings.Republics.Count)];
        Generated.Photo = P.Face;

        if(Random.value < GameSettings.PercentFake) //if fake
        {
            Generated.Fake = (Document.FakedPart)Random.Range(1, Document.FakedPart.GetNames(typeof(Document.FakedPart)).Length);
        }else
        {
            Generated.Fake = Document.FakedPart.None;
        }

        if(Generated.Fake == Document.FakedPart.FirstName || Generated.Fake == Document.FakedPart.LastName || Generated.Fake == Document.FakedPart.Expiration)      //REMOVE ONCE BOOK IS ADDED
            Generated.Fake = Document.FakedPart.None;

        if(Generated.Fake == Document.FakedPart.None)
            return Generated;

        //Generated.Fake = Document.FakedPart.Expiration; //debug expiration

        switch(Generated.Fake)
        {
            case Document.FakedPart.Gender : 
                Generated.Male = !Generated.Male;
            break;
            case Document.FakedPart.FirstName: 
                Generated.FirstName = Generated.Male ? GameSettings.FemaleFirstNames[Random.Range(0, GameSettings.FemaleFirstNames.Count)] : GameSettings.MaleFirstNames[Random.Range(0, GameSettings.MaleFirstNames.Count)];
            break;
            case Document.FakedPart.LastName : 
                Generated.LastName = Generated.Male ? GameSettings.FemaleLastNames[Random.Range(0, GameSettings.FemaleLastNames.Count)] : GameSettings.MaleLastNames[Random.Range(0, GameSettings.MaleLastNames.Count)];
            break;
            case Document.FakedPart.Weight : 
                Generated.Weight = Random.value > 0.5f ? Random.Range(1, (GameSettings.WeightRange.x/4)) : Random.Range(GameSettings.WeightRange.y*4, GameSettings.WeightRange.y*6);
            break;
            case Document.FakedPart.Height : 
                if(Random.value > 0.5f)
                {
                    Generated.HeightFoot = Generated.HeightFoot * 3;
                }else
                {
                    Generated.HeightInch = Random.Range(13, 25);
                }
            break;
            case Document.FakedPart.Expiration : 
                DayOffset = Random.Range(GameSettings.ExpirationOffsetRange.x, GameSettings.ExpirationOffsetRange.y);
                Generated.ExpirationDay = ThisDay;
                Generated.ExpirationMonth = ThisMonth;
                Generated.ExpirationYear = ThisYear;

                if(ThisDay - DayOffset < 1)
                {
                    DayOffset = GameSettings.MonthDays[(int)ThisMonth] + (ThisDay - DayOffset);
                    Generated.ExpirationDay = DayOffset;

                    if((int)ThisMonth == 0)
                    {
                        Generated.ExpirationMonth = (GameSettings.Month) 11;
                        Generated.ExpirationYear--;
                    }else
                    {
                        Generated.ExpirationMonth = (GameSettings.Month) ((int)Generated.ExpirationMonth - 1);
                    }
                }else
                {
                    Generated.ExpirationDay = ThisDay - DayOffset;
                }
            break;
            case Document.FakedPart.Citizenship : 
                Generated.Citizenship = GameSettings.FakeRepublics[Random.Range(0, GameSettings.FakeRepublics.Count)];
            break;
            case Document.FakedPart.Photo : 
                while(Generated.Photo == P.Face)
                {
                    if(Generated.Male)
                        Generated.Photo = MaleFaces[Random.Range(0, MaleFaces.Count)];
                    else
                        Generated.Photo = FemaleFaces[Random.Range(0, FemaleFaces.Count)];
                }
            break;
        }

        return Generated; 
    }

    public void UpdateCalendar()
    {
        if(ThisDay == GameSettings.MonthDays[(int) ThisMonth])    //if it is the end of the month
        {
            ThisDay = 1;
            if((int)ThisMonth == 11)
            {
                ThisMonth = (GameSettings.Month) 0;
                ThisYear++;
            }else
            {
                ThisMonth = (GameSettings.Month) ((int)ThisMonth + 1);
            }
        }else
        {
            ThisDay++;
        }

        MUI.NewNightDate.text = ThisMonth.ToString() + ". " + ThisDay + GameSettings.GetPlaceSuffix(ThisDay);
    }

    public void LeaveGame()
    {
        if(GameStarted)
        {
            MM.SetScreen("Loading");
            GameStarted = false;
            //SceneManager.MoveGameObjectToScene(this.gameObject, SceneManager.GetActiveScene());
            if(TimeIndex != 0)
                StopCoroutine(NightCoroutine);
            
            GameSettings.SC.SaveIndex = -1;

            if(TB.isActive)
                TB.Toggle();

            //ASC_Ambient.Stop();
            
            StartCoroutine(LeaveGameFreezeDelay());
        }
    }

    IEnumerator LeaveGameFreezeDelay()
    {
        yield return null;
        SceneManager.LoadScene(1);
    }

    /*
    public void TogglePaused()
    {
        Paused = !Paused;
        Time.timeScale = Paused ? 0f : 1f;
        AudioListener.volume = Paused ? 0f : 1f;
    }*/

    public Transform GetListenerTransform()
    {
        if(Bus)
        {
            return Bus.ToPitch;
        }else
        {
            return this.transform;
        }
    }

    public void QuitGame()
	{
		Debug.Log("Quit");
		Application.Quit();
	}
}
