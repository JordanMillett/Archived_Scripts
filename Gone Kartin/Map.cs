using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Map : MonoBehaviour
{
    /*
    [System.Serializable]
    public struct LapTime
    {
        public int Minutes;
        public int Seconds;
        public int Milliseconds;
    }*/

    Manager M;

    public MapSettings RaceData;

    public GameObject KartPrefab;
    public GameObject CameraPrefab;

    public bool DeveloperMode = false;
    public int PlayerCount = 1;
    public int AICount = 1;
    public KartBody DefaultBody;
    public KartWheel DefaultWheel;

    public List<KartController> AllKarts;
    public List<Checkpoint> Checkpoints;
    //public List<LapTime> LapTimes;

    public bool RaceStarted = false;
    public float SecondsPassed = 0f;
    public int CheckpointCount = 0;

    public Road R;

    bool CameraLoaded = false;

    int PlayersFinished = 0;

    public int CountDown = 3;

    void Start()
    {
        M = GameObject.FindWithTag("Manager").GetComponent<Manager>();

        if(DeveloperMode)
        {
            RaceData = new MapSettings();
            RaceData.PlayerCount = PlayerCount;
            RaceData.AICount = AICount;

            StartCoroutine(StartGame());
        }
    }

    void Update()
    {
        if(RaceStarted)
            SecondsPassed += Time.deltaTime;
    }

    public void Initialize(MapSettings Data)
    {
        RaceData = Data;

        StartCoroutine(StartGame());
    }

    IEnumerator StartGame()
    {
        if(RaceData.Items == false)
        {
            this.transform.GetChild(2).transform.gameObject.SetActive(false);
        }

        while(RaceData.PlayerCount == 0)
        {
            yield return null;
        }

        SpawnVehicles();

        foreach(KartController KC in AllKarts)
        {
            KC.Frozen = true;
        }

        InitializeCheckpoints();
        StartCoroutine(SpawnCameras());
        
        while(!CameraLoaded)
        {
            yield return null;
        }

        M.MM.SetScreen("Game");
        M.GameStarted = true;

        CountDown = 3;
        yield return new WaitForSeconds(1f);
        CountDown = 2;
        yield return new WaitForSeconds(1f);
        CountDown = 1;
        yield return new WaitForSeconds(1f);
        CountDown = 0;
        yield return new WaitForSeconds(0.25f);
        CountDown = -1;

        foreach(KartController KC in AllKarts)
        {
            KC.Frozen = false;
        }

        RaceStarted = true;
    }

    void SpawnVehicles()
    {
        Transform Spawns = transform.GetChild(0);
        int SpawnsFree = Spawns.childCount;

        if(RaceData.PlayerCount + RaceData.AICount > SpawnsFree)
        {
            Debug.LogError("Not Enough Spawns on Map");
            return;
        }

        /*
        for(int i = 0; i < RaceData.PlayerCount; i++)
        {
            GameObject NewKart = Instantiate(KartPrefab, Spawns.GetChild(Index).position, Spawns.GetChild(Index).rotation);
            NewKart.GetComponent<KartController>().InControl = (KartController.Driver) Index + 1;
            KartConfig KF = new KartConfig();
            KF.KB = RaceData.PlayerKarts[i].KB;
            KF.KW = RaceData.PlayerKarts[i].KW;
            NewKart.GetComponent<KartController>().Load(KF);
            AllKarts.Add(NewKart.GetComponent<KartController>());
            Index++;
        }*/

        //RaceData.AICount = SpawnsFree - RaceData.PlayerCount;

        for(int i = 0; i < RaceData.AICount + RaceData.PlayerCount; i++)
        {
            GameObject NewKart = Instantiate(KartPrefab, Spawns.GetChild(i).position, Spawns.GetChild(i).rotation);
            NewKart.GetComponent<KartController>().InControl = (KartController.Driver) 0;
            AllKarts.Add(NewKart.GetComponent<KartController>());
            AllKarts[i].Place = i;
        }

        int SlotsFound = 0;
        while(SlotsFound != RaceData.PlayerCount)
        {
            int Guess = Random.Range(0, AllKarts.Count);
            if(AllKarts[Guess].InControl == (KartController.Driver) 0)
            {
                AllKarts[Guess].InControl = (KartController.Driver) SlotsFound + 1;
                SlotsFound++;
            }
        }

        int AIIndex = 0;
        for(int i = 0; i < AllKarts.Count; i++)
        {
            KartConfig KF = new KartConfig();
            if(AllKarts[i].InControl == (KartController.Driver) 0)
            {
                KF.KB = RaceData.AIKarts[AIIndex].KB;
                KF.KW = RaceData.AIKarts[AIIndex].KW;
                AIIndex++;
            }else
            {
                KF.KB = RaceData.PlayerKarts[(int) AllKarts[i].InControl - 1].KB;
                KF.KW = RaceData.PlayerKarts[(int) AllKarts[i].InControl - 1].KW;
            }

            AllKarts[i].name = AllKarts[i].InControl.ToString();
            AllKarts[i].Load(KF);
        }
    }

    IEnumerator SpawnCameras()
    {
        for(int i = 0; i < RaceData.PlayerCount; i++)
        {
            GameObject NewCamera = Instantiate(CameraPrefab, Vector3.zero, Quaternion.identity);
            NewCamera.GetComponent<PlayerCamera>().Target = (KartController.Driver) i + 1;

            while(!NewCamera.GetComponent<PlayerCamera>().Synced)
            {
                yield return null;
            }
        }

        yield return null;

        CameraLoaded = true;
    }

    void InitializeCheckpoints()
    {
        Transform CheckpointObject = transform.GetChild(1);
        CheckpointCount = CheckpointObject.childCount;
        
        for(int i = 0; i < CheckpointCount; i++)
        {
            Checkpoints.Add(CheckpointObject.GetChild(i).GetComponent<Checkpoint>());
            
            Checkpoints[i].Index = i;
            Checkpoints[i].M = this;
        }
    }

    IEnumerator Scuffed(KartController KC)
    {
        yield return new WaitForSeconds(3f);
        KC.LastLapTime = 0f;
    }

    public void CheckpointHit(int Index, KartController KC)
    {
        if(Index == 0)
        {
            KC.CheckpointIndex = 0;
            KC.LastLapTime = SecondsPassed - KC.LapOffset;
            StartCoroutine(Scuffed(KC));
            KC.LapOffset = SecondsPassed;
            KC.LapCount++;

            KC.Score++;
            KC.Place = GetPlace(KC);

            if(KC.LapCount == RaceData.Laps)
            {
                Manager.KartInfo KI = new Manager.KartInfo();
                KI.Place = M.ScoreboardInfo.Count + 1;
                KI.Controller = KC.InControl;
                KI.Time = KC.LapOffset;

                M.ScoreboardInfo.Add(KI);
                if(KC.InControl != KartController.Driver.AI)
                {
                    KC.CameraEmpty.GetComponent<PlayerCamera>().Spectate();
                    PlayersFinished++;

                    if(PlayersFinished == RaceData.PlayerCount)
                    {
                        for(int i = 0; i < 64; i++)
                        {
                            if(i < M.ScoreboardInfo.Count)
                                M.ScoreboardUIS[i].SetValues(M.ScoreboardInfo[i]);
                            else if(i < RaceData.PlayerCount + RaceData.AICount)
                                M.ScoreboardUIS[i].SetNotFinished(i + 1);
                            else
                                M.ScoreboardUIS[i].SetHidden();
                        }

                        M.MM.SetScreen("Results");
                    }
                }else
                {
                    if(PlayersFinished == RaceData.PlayerCount) //if game finished
                    {
                        M.ScoreboardUIS[M.ScoreboardInfo.Count - 1].SetValues(M.ScoreboardInfo[M.ScoreboardInfo.Count - 1]);
                    }
                }

                Destroy(KC.gameObject);
            }
        }else
        {
            KC.CheckpointIndex++;
            KC.Score++;
            KC.Place = GetPlace(KC);
        }
    }

    int GetPlace(KartController KC)
    {
        int Place = 0;

        for(int i = 0; i < AllKarts.Count; i++)
        {
            if(AllKarts[i].Score >= KC.Score)
            {
                Place++;
            }
        }

        return Place;
    }
}
