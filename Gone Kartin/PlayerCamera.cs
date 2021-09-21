using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerCamera : MonoBehaviour
{
    public KartController.Driver Target;
    public KartController Synced;

    public TextMeshProUGUI MPH;
    public TextMeshProUGUI Place;
    public TextMeshProUGUI Time;
    public TextMeshProUGUI Laps;
    public TextMeshProUGUI Player;

    public TextMeshProUGUI Finished;

    public TextMeshProUGUI CountDown;

    public TextMeshProUGUI LapTime;

    Map M;

    void Start()
    {
        M = GameObject.FindWithTag("Map").GetComponent<Map>();
        Invoke("Initialize", 1f);

        Place.gameObject.SetActive(false);/////////REMOVEEEEEEEEEEEEEEEEEEEEE
    }

    void Update()
    {
        if(Synced)
        {
            MPH.text = Mathf.Round(Synced.GetComponent<Rigidbody>().velocity.magnitude).ToString() + " MPH";

            Place.text = Synced.Place.ToString() + TextFormatter.GetPlaceSuffix(Synced.Place);

            Laps.text = (Synced.LapCount + 1).ToString() + "/" + M.RaceData.Laps;

            switch(Synced.InControl)
            {
                case KartController.Driver.AI : Player.text = "AI"; break;
                case KartController.Driver.Player1 : Player.text = "P1"; break;
                case KartController.Driver.Player2 : Player.text = "P2"; break;
                case KartController.Driver.Player3 : Player.text = "P3"; break;
                case KartController.Driver.Player4 : Player.text = "P4"; break;
            }

            Time.text = TextFormatter.GetTimeFormatted(M.SecondsPassed-Synced.LapOffset);

            CountDown.text = TextFormatter.GetCountDownFormatted(M.CountDown);

            if(Synced.LastLapTime != 0)
            {
                LapTime.text = TextFormatter.GetTimeFormatted(Synced.LastLapTime);
            }else
            {
                LapTime.text = "";
            }
        }
    }

    public void Spectate()
    {
        MPH.gameObject.SetActive(false);
        Place.gameObject.SetActive(false);
        Time.gameObject.SetActive(false);
        Laps.gameObject.SetActive(false);
        Player.gameObject.SetActive(false);
        LapTime.gameObject.SetActive(false);

        //Enable finished
        Finished.gameObject.SetActive(true);
    }

    void Initialize()
    {
        GameObject[] AllKartsObjects = GameObject.FindGameObjectsWithTag("Kart");
        List<KartController> AllKarts = new List<KartController>();

        for(int i = 0; i < AllKartsObjects.Length; i++)
            AllKarts.Add(AllKartsObjects[i].GetComponent<KartController>());

        foreach (KartController K in AllKarts)
        {
            if(K.InControl == Target)
            {
                K.CameraEmpty = this.transform;
                Synced = K;
            }
        }

        Map M = GameObject.FindWithTag("Map").GetComponent<Map>();

        switch(M.RaceData.PlayerCount)
        {
            case 1 :
                switch((int) Target - 1)
                {
                    case 0 : transform.GetChild(0).GetComponent<Camera>().rect = new Rect(0.0f, 0.0f, 1.0f, 1.0f); break;
                }
            break;
            case 2 :
                switch((int) Target - 1)
                {
                    case 0 : transform.GetChild(0).GetComponent<Camera>().rect = new Rect(0.0f, 0.5f, 1.0f, 0.5f); break;
                    case 1 : transform.GetChild(0).GetComponent<Camera>().rect = new Rect(0.0f, 0.0f, 1.0f, 0.5f); break;
                }
            break;
            case 3 :
                switch((int) Target - 1)
                {
                    case 0 : transform.GetChild(0).GetComponent<Camera>().rect = new Rect(0.0f, 0.5f, 1.0f, 0.5f); break;
                    case 1 : transform.GetChild(0).GetComponent<Camera>().rect = new Rect(0.0f, 0.0f, 0.5f, 0.5f); break;
                    case 2 : transform.GetChild(0).GetComponent<Camera>().rect = new Rect(0.5f, 0.0f, 0.5f, 0.5f); break;
                }
            break;
            case 4 :
                switch((int) Target - 1)
                {
                    case 0 : transform.GetChild(0).GetComponent<Camera>().rect = new Rect(0.0f, 0.5f, 0.5f, 0.5f); break;
                    case 1 : transform.GetChild(0).GetComponent<Camera>().rect = new Rect(0.5f, 0.5f, 0.5f, 0.5f); break;
                    case 2 : transform.GetChild(0).GetComponent<Camera>().rect = new Rect(0.0f, 0.0f, 0.5f, 0.5f); break;
                    case 3 : transform.GetChild(0).GetComponent<Camera>().rect = new Rect(0.5f, 0.0f, 0.5f, 0.5f); break;
                }
            break;
        }
    }
}
