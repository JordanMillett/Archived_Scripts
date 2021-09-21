using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;

public class Manager : MonoBehaviour
{
    [System.Serializable]
    public struct KartInfo
    {
        public int Place;
        public KartController.Driver Controller;
        public float Time;
    }

    public List<KartInfo> ScoreboardInfo;
    public List<ScoreUI> ScoreboardUIS;
    public Transform ScoresObject;

    public MenuManager MM;
    public GameObject Event;
    public static Manager MInstance;
    MapSettings RaceData;

    public List<KartBody> KartBodies;
    public List<KartWheel> KartWheels;

    public List<Texture2D> Palettes;
    public Material Mat;

    public List<Track> AllTracks;

    public List<Item> AllItems;

    List<int> BodyIndex = new List<int>(4){0,0,0,0};  
    List<int> WheelIndex = new List<int>(4){0,0,0,0};    
    public List<TextMeshProUGUI> BodyTexts;
    public List<TextMeshProUGUI> WheelTexts;

    int MapIndex = 0;
    public TextMeshProUGUI MapName;
    public TextMeshProUGUI MapDescription;
    public RawImage MapIcon;

    public List<KartPreviewer> Previews = new List<KartPreviewer>();
    public List<GameObject> PreviewsUI = new List<GameObject>();
    bool KartsToggled = true;
    
    bool Paused = false;

    public bool GameStarted = false;

    public Slider VehicleCountSlider;
    bool ItemsEnabled = true;

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
    
    public void ToggleItems()
    {
        ItemsEnabled = !ItemsEnabled;
    }

    void Start()
    {
        Event.SetActive(true);

        for(int i = 0; i < 64; i++)
        {
            ScoreboardUIS.Add(ScoresObject.transform.GetChild(i).GetComponent<ScoreUI>());
        }

        RaceData = new MapSettings();
        RaceData.PlayerCount = 1;
        
        SceneManager.sceneLoaded += OnSceneLoaded;

        Application.targetFrameRate = 60;

        for(int i = 0; i < 4; i++)
        {
            BodyIndex[i] = Previews[i].BodyIndex;
            WheelIndex[i] = Previews[i].WheelIndex;

            BodyTexts[i].text = KartBodies[BodyIndex[i]].Name;
            WheelTexts[i].text = KartWheels[WheelIndex[i]].Name;
        }

        MapName.text = AllTracks[MapIndex].TrackName;
        MapDescription.text = AllTracks[MapIndex].TrackDescription;
        MapIcon.texture = AllTracks[MapIndex].Icon;

        ToggleKarts();
    }

    public void ToggleKarts()
    {
        KartsToggled = !KartsToggled;

        if(KartsToggled)
        {
            for(int i = 0; i < Previews.Count; i++)
            {
                if(i < RaceData.PlayerCount)
                {
                    Previews[i].transform.gameObject.SetActive(true);
                    PreviewsUI[i].SetActive(true);
                }else
                {
                    Previews[i].transform.gameObject.SetActive(false);
                    PreviewsUI[i].SetActive(false);
                }
            }
        }else
        {
            for(int i = 0; i < Previews.Count; i++)
            {
                Previews[i].transform.gameObject.SetActive(false);
            }
        }
    }

    public void TogglePaused()
    {
        Paused = !Paused;
        Time.timeScale = Paused ? 0f : 1f;
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Alpha1))
        { Mat.SetTexture("Palette", Palettes[0]); }

        if(Input.GetKeyDown(KeyCode.Alpha2))
        { Mat.SetTexture("Palette", Palettes[1]); }

        if(Input.GetKeyDown(KeyCode.Alpha3))
        { Mat.SetTexture("Palette", Palettes[2]); }

        if(Input.GetKeyDown(KeyCode.Alpha4))
        { Mat.SetTexture("Palette", Palettes[3]); }

        if(Input.GetKeyDown(KeyCode.Alpha5))
        { Mat.SetTexture("Palette", Palettes[4]); }

        if(Input.GetKeyDown(KeyCode.Escape))
        {
            if(GameStarted)
            {
                if(Paused)
                    MM.SetScreen("Game");
                else
                    MM.SetScreen("Pause");
            }
        }
    }

    public void SetPlayerCount(int Count)
    {
        RaceData.PlayerCount = Count;
    }

    public void LaunchGame()
    {
        RaceData.Laps = AllTracks[MapIndex].LapCount;

        MM.SetScreen("Loading");
        SceneManager.LoadScene(AllTracks[MapIndex].BuildName);
    }

    public void LeaveGame()
    {
        MM.SetScreen("Loading");
        GameStarted = false;
        SceneManager.LoadScene(0);
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if(scene.buildIndex != 0)
        {
            RaceData.PlayerKarts = new List<KartConfig>();
            for(int i = 0; i < RaceData.PlayerCount; i++)
            {
                KartConfig KC = new KartConfig();
                KC.KB = KartBodies[BodyIndex[i]];
                KC.KW = KartWheels[WheelIndex[i]];
                
                //KC.KB = transform.GetChild(1).GetChild(i).GetComponent<KartPreviewer>().CurrentKart.KB;
                //KC.KW = transform.GetChild(1).GetChild(i).GetComponent<KartPreviewer>().CurrentKart.KW;
                //KC.KB = KartBodies[Random.Range(0, KartBodies.Count)];
                //KC.KW = KartWheels[Random.Range(0, KartWheels.Count)];
                RaceData.PlayerKarts.Add(KC);
            }

            RaceData.Items = ItemsEnabled;

            int VehicleCount = (int) VehicleCountSlider.value;

            if(VehicleCount < RaceData.PlayerCount)
            {
                VehicleCount = RaceData.PlayerCount;
            }
            

            RaceData.AICount = VehicleCount - RaceData.PlayerCount;
            //RaceData.AICount = AllTracks[MapIndex].SpawnCount - RaceData.PlayerCount;

            RaceData.AIKarts = new List<KartConfig>();
            for(int i = 0; i < RaceData.AICount; i++)
            {
                KartConfig KC = new KartConfig();
                KC.KB = KartBodies[Random.Range(0, KartBodies.Count)];
                KC.KW = KartWheels[Random.Range(0, KartWheels.Count)];
                RaceData.AIKarts.Add(KC);
            }

            GameObject.FindWithTag("Map").GetComponent<Map>().Initialize(RaceData);
        }else
        {
            ScoreboardInfo = new List<KartInfo>();
            MM.SetScreen("Main");
        }
    }

    public void NextBody(int Index)
    {
        if(BodyIndex[Index] < KartBodies.Count - 1)
            BodyIndex[Index]++;
        else
            BodyIndex[Index] = 0;

        BodyTexts[Index].text = KartBodies[BodyIndex[Index]].Name;
        Previews[Index].Refresh(BodyIndex[Index], WheelIndex[Index]);
    }

    public void BackBody(int Index)
    {
        if(BodyIndex[Index] != 0)
            BodyIndex[Index]--;
        else
            BodyIndex[Index] = KartBodies.Count - 1;

        BodyTexts[Index].text = KartBodies[BodyIndex[Index]].Name;
        Previews[Index].Refresh(BodyIndex[Index], WheelIndex[Index]);
    }

    public void NextWheel(int Index)
    {
        if(WheelIndex[Index] < KartWheels.Count - 1)
            WheelIndex[Index]++;
        else
            WheelIndex[Index] = 0;

        WheelTexts[Index].text = KartWheels[WheelIndex[Index]].Name;
        Previews[Index].Refresh(BodyIndex[Index], WheelIndex[Index]);
    }

    public void BackWheel(int Index)
    {
        if(WheelIndex[Index] != 0)
            WheelIndex[Index]--;
        else
            WheelIndex[Index] = KartWheels.Count - 1;


        WheelTexts[Index].text = KartWheels[WheelIndex[Index]].Name;
        Previews[Index].Refresh(BodyIndex[Index], WheelIndex[Index]);
    }

    public void NextMap()
    {
        if(MapIndex < AllTracks.Count - 1)
            MapIndex++;
        else
            MapIndex = 0;

        MapName.text = AllTracks[MapIndex].TrackName;
        MapDescription.text = AllTracks[MapIndex].TrackDescription;
        MapIcon.texture = AllTracks[MapIndex].Icon;
    }

    public void LastMap()
    {
        if(MapIndex > 0)
            MapIndex--;
        else
            MapIndex = AllTracks.Count - 1;

        MapName.text = AllTracks[MapIndex].TrackName;
        MapDescription.text = AllTracks[MapIndex].TrackDescription;
        MapIcon.texture = AllTracks[MapIndex].Icon;
    }

    public void QuitGame()
	{
		Debug.Log("Quit");
		Application.Quit();
	}
}
