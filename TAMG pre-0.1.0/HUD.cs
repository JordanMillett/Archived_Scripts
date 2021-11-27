using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class HUD : MonoBehaviour
{
    public MenuManager MM;
    public TextMeshProUGUI Clock;
    public TextMeshProUGUI Money;

    public GameObject BuyPopupObject;
    public TextMeshProUGUI BuyName;
    public TextMeshProUGUI BuyPrice;

    public GameObject RadioPopup;
    public TextMeshProUGUI RadioName;
    public RawImage RadioImage;

    Transform Cam;
    public Vehicle VC;
    public bool InCar = false;

    public List<RadioStation> Stations;
    bool RadioSwapping = false;
    IEnumerator RadioCoroutine;
    public int CurrentStation = 0;

    public Texture2D MapTexture;
    public LineRenderer NavigationLine;
    public List<List<Node>> Nodes = new List<List<Node>>();
    public List<Node> UnmarkedNodes = new List<Node>();
    public List<Vector3> NavigationPath = new List<Vector3>();

    bool Navigating = false;
    bool RouteFound = false;
    Vector3 CurrentEnd;
    
    //int RouteCalculations = 0;
    //int ErrorAmount = 250;

    List<int> StationSongIndices = new List<int>();
    List<float> StationSongTimes = new List<float>();

    void Start()
    {
        Cam = GameObject.FindWithTag("Camera").transform;

        for(int i = 0; i < Stations.Count; i++)
        {
            StationSongIndices.Add(0);
            StationSongTimes.Add(0f);
        }

        for(int i = 0; i < Stations.Count; i++)
        {
            StartCoroutine(StationManager(i));
        }

        for(int x = 0; x < 200; x++)
        {
            List<Node> NodeList = new List<Node>();
            for(int y = 0; y < 200; y++)
            {
                NodeList.Add(new Node());
            }
            Nodes.Add(NodeList);
        }

        //pixels to nodes
        Color[] Pixels = MapTexture.GetPixels(0, 0, 200, 200);

        for(int i = 0; i < Pixels.Length; i++)
        {
            if(Pixels[i] == Color.white)
            {
                //Nodes.Add(new Node(this, new Vector2Int(Mathf.RoundToInt(((i % 200) * 16f) - 1600f), Mathf.FloorToInt(((i / 200) * 16f) - 1600f))));
                //Nodes[i % 200][i / 200].SetLocation(new Vector2Int(Mathf.RoundToInt(((i % 200) * 16f) - 1600f), Mathf.FloorToInt(((i / 200) * 16f) - 1600f)));
                //Debug.Log(new Vector2Int(i % 200, i / 200));
                Nodes[i % 200][i / 200].SetLocation(this, new Vector2Int(i % 200, i / 200));
            }
        }

        DontDestroyOnLoad(NavigationLine.gameObject);
        InvokeRepeating("RecalculateRoute", 0.5f, 0.5f);
    }

    public void Navigate(Vector3 End)
    {
        CurrentEnd = End;

        Node StartNode = GetNearestNode(Cam.position);
        Node EndNode = GetNearestNode(End);

        for(int x = 0; x < 200; x++)
        {
            for(int y = 0; y < 200; y++)
            {
                Nodes[x][y].Initialize(StartNode, EndNode);
            }
        }

        Navigating = true;
        RecalculateRoute();
    }

    public void ClearNavigation()       //clear everything
    {
        NavigationLine.positionCount = 0;
        //RouteCalculations = 0;
        Navigating = false;
        RouteFound = false;
    }

    void RefreshNavigation()            //called every update
    {
        if(RouteFound)
        {
            NavigationLine.positionCount = NavigationPath.Count;
            NavigationLine.SetPositions(NavigationPath.ToArray());
        }
    }

    void RecalculateRoute()             //called every half second
    {
        if(Navigating)
        {
            //RouteCalculations = 0;
            NavigationPath.Clear(); //clear old path
            UnmarkedNodes.Clear();  //clear old options

            Node StartNode = GetNearestNode(Cam.position);
            Node EndNode = GetNearestNode(CurrentEnd);

            for(int x = 0; x < 200; x++)
            {
                for(int y = 0; y < 200; y++)
                {
                    if(!Nodes[x][y].Blocked)
                        Nodes[x][y].Initialize(StartNode, EndNode);
                }
            }

            GetNearestNode(Cam.position).Mark();

            //GetNode(GetNearestVector(Cam.position)).Mark();
            //Nodes[Mathf.RoundToInt(Cam.position.x/32)][i / 200].Mark();


            //UnmarkedNodes.Add(GetNode(GetNearestVector(Cam.position)));
            //FindNextNode();     
        }
    }

    public void FindNextNode()
    {   
        /*
        RouteCalculations++;
        if(RouteCalculations >= ErrorAmount)
        {
            ClearNavigation();
            return;
        }*/

        //Debug.Log(UnmarkedNodes.Count + " should not be zero");

        int LowestNumber = 100000;
        for(int i = 0; i < UnmarkedNodes.Count; i++)
        {
            if(UnmarkedNodes[i].GCost + UnmarkedNodes[i].HCost <= LowestNumber) //find the lowest F value of all possibilities
            {
                LowestNumber = UnmarkedNodes[i].GCost + UnmarkedNodes[i].HCost;
            }
        }

        List<Node> LowestNodes = new List<Node>();
        int LowestHCost = 100000;
        for(int i = 0; i < UnmarkedNodes.Count; i++)
        {
            if(UnmarkedNodes[i].GCost + UnmarkedNodes[i].HCost == LowestNumber) //find all that share the same F value
            {
                LowestNodes.Add(UnmarkedNodes[i]);

                if(UnmarkedNodes[i].HCost < LowestHCost)                        //find the lowest H cost of all possibilities
                {
                    LowestHCost = UnmarkedNodes[i].HCost;
                }
            }
        }

        List<Node> CloseNodes = new List<Node>();

        for(int i = 0; i < LowestNodes.Count; i++)
        {
            if(LowestNodes[i].HCost <= LowestHCost) //find the nodes closest to the end point
            {
                CloseNodes.Add(LowestNodes[i]);
            }
        }

        //Debug.Log("Lowest F : " + LowestNumber + " Lowest H : " + LowestHCost);

        CloseNodes[Random.Range(0, CloseNodes.Count)].Mark();       //Mark the node with the Lowest F value and the lowest H value at random
    }

    public void PathFinished()
    {
        Debug.Log("FOUND");
        //H.NavigationPath.Add(new Vector3(Location.x, -9.95f, Location.x));
        //NavigationPath.Add(Vector3.zero); //gets replaced by camera position before updated
        //NavigationPath[0] = new Vector3(Cam.position.x, -9.95f, Cam.position.z);


        //NavigationPath.Add(new Vector3(Cam.position.x, -9.95f, Cam.position.z));
        //NavigationPath.Add(new Vector3(CurrentEnd.x, -9.95f, CurrentEnd.z));
        RouteFound = true;
    }
    
    public Node GetNode(Vector2Int Location)
    {
        /*
        for(int i = 0; i < Nodes.Count; i++)
        {
            if(Location == Nodes[i].Location)
            {
                return Nodes[i];
            }
        }*/

        for(int x = 0; x < 200; x++)
        {
            for(int y = 0; y < 200; y++)
            {
                if(Location == Nodes[x][y].Location)
                {
                    return Nodes[x][y];
                }
            }
        }

        return null;
    }

    public Node GetNearestNode(Vector3 VecThree)
    {
        Vector2 VecTwo = new Vector2(VecThree.x, VecThree.z);
        Vector2Int NearestIndex = new Vector2Int(0, 0);
        float NearestDistance = 50000f;

        float dist = 0f;

        for(int x = 0; x < 200; x++)
        {
            for(int y = 0; y < 200; y++)
            {
                if(!Nodes[x][y].Blocked)
                {
                    //Debug.Log(VecThree);
                    dist = Vector2.Distance(Nodes[x][y].Location, VecTwo);

                    if(dist < NearestDistance)
                    {
                        NearestIndex = new Vector2Int(x, y);
                        NearestDistance = dist; 
                    }
                }
            }
        }
 
        return Nodes[NearestIndex.x][NearestIndex.y];
    }
    
    public Vector2Int GetNearestVector(Vector3 VecThree)
    {
        Vector2 VecTwo = new Vector2(VecThree.x, VecThree.z);
        Vector2Int NearestIndex = new Vector2Int(0, 0);
        float NearestDistance = 50000f;

        float dist = 0f;

        for(int x = 0; x < 200; x++)
        {
            for(int y = 0; y < 200; y++)
            {
                if(!Nodes[x][y].Blocked)
                {
                    //Debug.Log(VecThree);
                    dist = Vector2.Distance(Nodes[x][y].Location, VecTwo);

                    if(dist < NearestDistance)
                    {
                        NearestIndex = new Vector2Int(x, y);
                        NearestDistance = dist; 
                    }
                }
            }
        }
        /*
        for(int i = 0; i < Nodes.Count; i++)
        {
            dist = Vector2.Distance(Nodes[i].Location, VecTwo);

            if(dist < NearestDistance)
            {
                NearestIndex = i;
                NearestDistance = dist; 
            }
        }
        */
        //Debug.Log(NearestIndex);
        //Debug.Log(Nodes[NearestIndex.x][NearestIndex.y].Location);
        return Nodes[NearestIndex.x][NearestIndex.y].Location;
    }

    IEnumerator StationManager(int Index)
    {
        
        while(Index != 0)
        {
            /*
            if(StationSongIndices[Index] + 1 == Stations[Index].Songs.Count)    //if the current song playing is the same as max limit of songs then go back to 0
            {
                StationSongIndices[Index] = 0;
            }else
            {
                StationSongIndices[Index]++;
            }*/
            StationSongIndices[Index] = Random.Range(0, Stations[Index].Songs.Count);
            StationSongTimes[Index] = Time.time;
            yield return new WaitForSeconds(Stations[Index].Songs[StationSongIndices[Index]].length);
        }

        yield break;
    }

    void Update()
    {
        if(GameServer.GS.GameManagerInstance != null)
        {
            RefreshNavigation();
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

                if(VC != null)
                {
                    if(!VC.AS_Radio.isPlaying && CurrentStation != 0)
                        PlaySong();
                }

                if(InCar)
                {
                    if(Input.mouseScrollDelta.y > 0f)
                    {
                        ChangeStation(true);
                    }

                    if(Input.mouseScrollDelta.y < 0f)
                    {
                        ChangeStation(false);
                    }
                }

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

        if(Physics.Raycast(Cam.transform.position, Cam.transform.forward, out hit, 2f))
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

    public void SetStation(int Index)
    {
        CurrentStation = Index;

        if(RadioSwapping)
            StopCoroutine(RadioCoroutine);

        RadioCoroutine = ChangeChannel(Index);
        StartCoroutine(RadioCoroutine);

        PlaySong();
    }

    void ChangeStation(bool Forward)
    {
        int Index = CurrentStation;

        if(Forward && (Index + 1) == Stations.Count)
        {
            Index = 0;
        }else if(!Forward && (Index - 1) == -1)
        {
            Index = Stations.Count - 1;
        }else
        {
            Index += Forward ? 1 : -1;
        }

        SetStation(Index);
    }

    IEnumerator ChangeChannel(int Index)
    {
        RadioSwapping = true;
        RadioImage.color = Color.white;
        RadioName.color = Color.white;
        RadioImage.texture = Stations[Index].Image;
        RadioName.text = Stations[Index].Name;
        RadioPopup.SetActive(true);
        
        yield return new WaitForSeconds(1f);
        Color FadeColor = new Color(1f, 1f, 1f, 1f);
        int FadeTime = 30;
        for(int i = 0; i < FadeTime;i++)
        {
            FadeColor.a = (float)(FadeTime - i)/(float)FadeTime;
            RadioImage.color = FadeColor;
            RadioName.color = FadeColor;
            yield return null;
        }

        FadeColor.a = 0f;
        RadioImage.color = FadeColor;
        RadioName.color = FadeColor;

        RadioPopup.SetActive(false);
        RadioSwapping = false;
    }

    void PlaySong()
    {
        if(Stations[CurrentStation].Songs.Count > 0)
        {
            //Debug.Log("Station : " + Stations[CurrentStation].Name + " " + Stations[CurrentStation].Songs[StationSongIndices[CurrentStation]].name + " " + (Time.time - StationSongTimes[CurrentStation]).ToString());
            if(Time.time - StationSongTimes[CurrentStation] < Stations[CurrentStation].Songs[StationSongIndices[CurrentStation]].length)
            {
                VC.AS_Radio.volume = (.25f * (Settings._musicVolume/100f)) * (Settings._masterVolume/100f);
                VC.AS_Radio.clip = Stations[CurrentStation].Songs[StationSongIndices[CurrentStation]];
                //Debug.Log(Time.time - StationSongTimes[CurrentStation] + " " + Stations[CurrentStation].Songs[StationSongIndices[CurrentStation]].length);
                VC.AS_Radio.time = Time.time - StationSongTimes[CurrentStation];
                VC.AS_Radio.Play();
                //VC.AS_Radio.clip = Stations[CurrentStation].Songs[Random.Range(0, Stations[CurrentStation].Songs.Count)];
                //VC.AS_Radio.Play();
            }
        }else
        {
            VC.AS_Radio.Stop();
        }
    }
}
