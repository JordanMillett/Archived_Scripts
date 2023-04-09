using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StationGenerator : MonoBehaviour
{
    public List<Station> Stations;
    public List<Texture2D> ZoneMaps;
    public Tileset Tileset_1x1;
    public Tileset Tileset_2x1;
    public Tileset Tileset_2x2;
    
    public GameObject WallPrefab;
    public GameObject DoorwayPrefab;

    public GameObject SpawnRoomPrefab;
    public GameObject EscapeRoomPrefab;

    StationDirector _station;

    public List<GameObject> FunctionalRoomPrefabs;

    public int StationIndex = 0;

    int ZoneMapIndex = 0;

    List<Vector3> InsideWalls = new List<Vector3>();

    public const int MAP_SIZE = 16;
    public const int TILE_SIZE = 20;

    class Walls 
    {
        public List<WallData> W = new List<WallData>();
        
        public int Contains(Vector3 Position)
        {
            for (int i = 0; i < W.Count; i++)
            {
                if(W[i].Position == Position)
                    return i;
            }

            return -1;
        }
    }
    
    class WallData
    {
        public Vector3 Position;
        public List<Room> Owners = new List<Room>();
    }
    
    class Node
    {
        public Vector2Int Index = Vector2Int.zero;
        public bool Free = false;     //is able to generate random room onto that space
        public bool Walkable = false; //is an actual room that you can navigate
        public List<Node> Neighbors = new List<Node>();
        public Node ParentNode;
        public int GCost = 0;//dist from start
        public int HCost = 0;//dist from end

        public int PathNumber = 0;
        public int PathIndex = 0;
        public Zone Type = Zone.Crew;

        public int FCost()
        {
            return GCost + HCost;
        }
        
        public void Reset()
        {
            ParentNode = null;
            GCost = 0;
        }
    }

    public void Begin(Texture2D StationMap)
    {
        _station = GetComponent<StationDirector>();

        StartCoroutine(GenerateStation(StationMap));
    }
    
    IEnumerator GenerateStation(Texture2D StationMap)
    {
        //StationMap = Stations[Random.Range(0, Stations.Count)].Map;
        //StationMap = Stations[StationIndex].Map;
        UIManager.UI.M_Game.E_Minimap.SetStation(StationMap);

        Node[,] Nodes = new Node[MAP_SIZE, MAP_SIZE];
        Node SpawnRoom = null;
        Node EscapeRoom = null;
        List<Node> FunctionalRooms = new List<Node>();

        ZoneMapIndex = Random.Range(0, ZoneMaps.Count);

        Color[] Pixels = StationMap.GetPixels(0);  //standard 16x16 - 256 pixels
        Color[] ZonePixels = ZoneMaps[ZoneMapIndex].GetPixels(0); //64x64 - 4096 pixels
        //int Ratio = Mathf.RoundToInt((float)ZoneMap.width / (float)StationMap.width); //width only square maps   64/16 = 4
        //Debug.Log(ZonePixels.Length);
        int TotalFreeSpaces = 0;
        for (int i = 0; i < Pixels.Length; i++)
        {
            //Debug.Log(ZonePixels[i * Ratio]);
            Nodes[i % MAP_SIZE, Mathf.FloorToInt(i / MAP_SIZE)] = new Node();
            Nodes[i % MAP_SIZE, Mathf.FloorToInt(i / MAP_SIZE)].Index = new Vector2Int(i % MAP_SIZE, Mathf.FloorToInt(i / MAP_SIZE));
            Nodes[i % MAP_SIZE, Mathf.FloorToInt(i/MAP_SIZE)].Free = Pixels[i] == Color.white;
            if(Pixels[i] != Color.black)
                TotalFreeSpaces++;
            Nodes[i % MAP_SIZE, Mathf.FloorToInt(i/MAP_SIZE)].Type = ZonePixels[GetPixelAtIndex(i)] == Color.white ? Zone.Crew : ZonePixels[GetPixelAtIndex(i)] == Color.red ? Zone.Cargo : Zone.Medical;
            
            if (Pixels[i] == Color.red)
            {
                SpawnRoom = Nodes[i % MAP_SIZE, Mathf.FloorToInt(i / MAP_SIZE)];
            }else if(Pixels[i] == Color.blue)
            {
                FunctionalRooms.Add(Nodes[i % MAP_SIZE, Mathf.FloorToInt(i / MAP_SIZE)]);
            }else if(Pixels[i] == Color.green)
            {
                EscapeRoom = Nodes[i % MAP_SIZE, Mathf.FloorToInt(i / MAP_SIZE)];
            }
        }
        
        //Pathfind doorways
        Node StartingNode = null;
        Node EndingNode = null;

        //Set Walkable
        for (int i = 0; i < Pixels.Length; i++)
        {
            Nodes[i % MAP_SIZE, Mathf.FloorToInt(i / MAP_SIZE)].Walkable = Pixels[i] != Color.black;
            
            if(Pixels[i] == Color.red)
                StartingNode = Nodes[i % MAP_SIZE, Mathf.FloorToInt(i / MAP_SIZE)];
            if(Pixels[i] == Color.green)
                EndingNode = Nodes[i % MAP_SIZE, Mathf.FloorToInt(i / MAP_SIZE)];
        }

        List<Node> RemainingRooms = new List<Node>();

        //Set neighbors && calculating distances
        for (int y = 0; y < Nodes.GetLength(1); y++)
        {
            for (int x = 0; x < Nodes.GetLength(0); x++)
            {
                if (Nodes[x, y].Walkable)
                {
                    RemainingRooms.Add(Nodes[x, y]);

                    if(x > 0)
                        if(Nodes[x - 1, y].Walkable)
                            Nodes[x, y].Neighbors.Add(Nodes[x - 1, y]); 
                    if(x < Nodes.GetLength(0) - 1)
                        if(Nodes[x + 1, y].Walkable)
                            Nodes[x, y].Neighbors.Add(Nodes[x + 1, y]);
                    if(y > 0)
                        if(Nodes[x, y - 1].Walkable)
                            Nodes[x, y].Neighbors.Add(Nodes[x, y - 1]);
                    if(y < Nodes.GetLength(1) - 1)
                        if(Nodes[x, y + 1].Walkable)
                            Nodes[x, y].Neighbors.Add(Nodes[x, y + 1]);

                    Nodes[x, y].HCost = Mathf.Abs(x - EndingNode.Index.x) + Mathf.Abs(y - EndingNode.Index.y);
                }
            }
        }

        List<Vector3> LinkLocations = new List<Vector3>();

        int TotalLoops = 0;
        while (true)
        {
            yield return null;
            UIManager.UI.M_Loading.UpdateStatus((Mathf.Min(1f, (float)TotalLoops/75f) * 25f), "Calculating Paths - " + (TotalLoops + 1));
            List<Node> Open = new List<Node>();
            List<Node> Closed = new List<Node>();
            Node Trace;
            Open.Add(StartingNode);
            while (true)
            {
                int Lowest = 0;
                for (int i = 0; i < Open.Count; i++)
                    if (Open[i].FCost() < Open[Lowest].FCost())
                        Lowest = i;

                Node Current = Open[Lowest];
                Open.Remove(Current);
                Closed.Add(Current);

                if (Current == EndingNode || !RemainingRooms.Contains(Current))
                {
                    Trace = Current;
                    break;
                }

                foreach (Node Neighbor in Current.Neighbors)
                {
                    if (!Neighbor.Walkable || Closed.Contains(Neighbor))
                    {

                    }
                    else
                    {
                        if (Neighbor.FCost() < Current.FCost() || !Open.Contains(Neighbor))
                        {
                            Neighbor.GCost = Current.GCost + 1;
                            Neighbor.ParentNode = Current;
                            if (!Open.Contains(Neighbor))
                                Open.Add(Neighbor);
                        }
                    }
                }
            }

            //traceback parents from target node until you reach starting node
            int TracebackCount = 0;
            while (true)
            {
                if (RemainingRooms.Contains(Trace))
                {
                    Trace.PathNumber = TotalLoops;
                    Trace.PathIndex = TracebackCount;
                    TracebackCount++;
                    RemainingRooms.Remove(Trace);
                }

                if (Trace.ParentNode != null)
                {
                    LinkLocations.Add
                    (
                        (
                        new Vector3((Trace.Index.x * TILE_SIZE) - (MAP_SIZE / 2) * TILE_SIZE, 0f, (Trace.Index.y * TILE_SIZE) - (StationMap.height / 2) * TILE_SIZE) +
                        new Vector3((Trace.ParentNode.Index.x * TILE_SIZE) - (MAP_SIZE / 2) * TILE_SIZE, 0f, (Trace.ParentNode.Index.y * TILE_SIZE) - (StationMap.height / 2) * TILE_SIZE)
                        ) / 2f
                    );

                    Trace = Trace.ParentNode;
                }
                else
                {
                    break;
                }
            }

            for (int y = 0; y < Nodes.GetLength(1); y++)
            {
                for (int x = 0; x < Nodes.GetLength(0); x++)
                {
                    Nodes[x, y].Reset();
                }
            }

            if (RemainingRooms.Count == 0)
                break;
            else
            {
                StartingNode = RemainingRooms[Random.Range(0, RemainingRooms.Count)];
                TotalLoops++;
            }
        }

        //Generate rooms
        Walls AllWalls = new Walls();

        int SpacesUsed = 0;
        int RoomsGenerated = 0;

        for (int y = 0; y < Nodes.GetLength(1); y++)
        {
            for (int x = 0; x < Nodes.GetLength(0); x++)
            {
                if (Nodes[x, y].Free)
                {
                    if(x < Nodes.GetLength(0) - 1)
                    {
                        if(Nodes[x + 1, y].Free)
                        {
                            if(y < Nodes.GetLength(1) - 1)
                            {
                                if(Nodes[x + 1, y + 1].Free)
                                {
                                    if(Nodes[x, y + 1].Free)
                                    {
                                        if (Random.value > 0.75f)
                                        {
                                            Vector3 RoomPosition = new Vector3(((x + x + 1) * (TILE_SIZE/2)) - ((MAP_SIZE / 2) * TILE_SIZE), 0f, ((y + y + 1) * (TILE_SIZE/2)) - ((StationMap.height / 2) * TILE_SIZE));
                                            List<Vector3> DoorPositions = new List<Vector3>();
                                            List<int> WallIndices = new List<int>();              //list of walls the room needs to own
                                            foreach (Vector3 Spot in Tileset_2x2.WallPositions)
                                            {
                                                int WallIndex = AllWalls.Contains(RoomPosition + Spot);
                                                if (WallIndex == -1)
                                                {
                                                    WallData NewWallData = new WallData();
                                                    NewWallData.Position = RoomPosition + Spot;
                                                    AllWalls.W.Add(NewWallData);
                                                    WallIndices.Add(AllWalls.W.Count - 1);
                                                }else
                                                {
                                                    WallIndices.Add(WallIndex);
                                                }

                                                if(LinkLocations.Contains(RoomPosition + Spot))
                                                    DoorPositions.Add(Spot);
                                            }
                                            
                                            int[] ZoneCounts = new int[3] { 0, 0, 0 };
                                            ZoneCounts[(int)Nodes[x, y].Type]++;
                                            ZoneCounts[(int)Nodes[x + 1, y].Type]++;
                                            ZoneCounts[(int)Nodes[x, y + 1].Type]++;
                                            ZoneCounts[(int)Nodes[x + 1, y + 1].Type]++;

                                            Tileset.RoomData NewData = Tileset_2x2.MatchingRoom(GetMajority(ZoneCounts), DoorPositions);
                                            GameObject Room = Instantiate(NewData.Room, RoomPosition, Quaternion.Euler(0f, NewData.Spin, 0f));
                                            Room.transform.parent = this.transform;
                                            Nodes[x, y].Free = false;
                                            Nodes[x + 1, y].Free = false;
                                            Nodes[x, y + 1].Free = false;
                                            Nodes[x + 1, y + 1].Free = false;

                                            List<Node> RoomNodes = new List<Node>();
                                            RoomNodes.Add(Nodes[x, y]);
                                            RoomNodes.Add(Nodes[x + 1, y]);
                                            RoomNodes.Add(Nodes[x, y + 1]);
                                            RoomNodes.Add(Nodes[x + 1, y + 1]);
                                            SetLowestPath(Room.GetComponent<Room>(), RoomNodes);
                                            
                                            foreach(int W in WallIndices)
                                                AllWalls.W[W].Owners.Add(Room.GetComponent<Room>());

                                            Room.GetComponent<Room>().Pixels.Add(new Vector2Int(x, y));
                                            Room.GetComponent<Room>().Pixels.Add(new Vector2Int(x + 1, y));
                                            Room.GetComponent<Room>().Pixels.Add(new Vector2Int(x, y + 1));
                                            Room.GetComponent<Room>().Pixels.Add(new Vector2Int(x + 1, y + 1));
                                            _station.Rooms.Add(Room.GetComponent<Room>());
                                            yield return null;
                                            RoomsGenerated++;
                                            SpacesUsed += 4;
                                            UIManager.UI.M_Loading.UpdateStatus(25f + ((float)SpacesUsed/TotalFreeSpaces * 40f), "Generating Rooms - " + (RoomsGenerated + 1));
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
        
        for (int y = 0; y < Nodes.GetLength(1); y++)
        {
            for (int x = 0; x < Nodes.GetLength(0); x++)
            {
                if (Nodes[x, y].Free)
                {
                    if(x < Nodes.GetLength(0) - 1)
                    {
                        if(Nodes[x + 1, y].Free)
                        {
                            if (Random.value > 0.5f)
                            {
                                Vector3 RoomPosition = new Vector3(((x + x + 1) * (TILE_SIZE/2)) - ((MAP_SIZE / 2) * TILE_SIZE), 0f, (y * TILE_SIZE) - ((StationMap.height / 2) * TILE_SIZE));
                                List<Vector3> DoorPositions = new List<Vector3>();
                                List<int> WallIndices = new List<int>();              //list of walls the room needs to own
                                foreach (Vector3 Spot in Tileset_2x1.WallPositions)
                                {
                                    int WallIndex = AllWalls.Contains(RoomPosition + Spot);
                                    if (WallIndex == -1)
                                    {
                                        WallData NewWallData = new WallData();
                                        NewWallData.Position = RoomPosition + Spot;
                                        AllWalls.W.Add(NewWallData);
                                        WallIndices.Add(AllWalls.W.Count - 1);
                                    }else
                                    {
                                        WallIndices.Add(WallIndex);
                                    }
            
                                    if(LinkLocations.Contains(RoomPosition + Spot))
                                        DoorPositions.Add(Spot);
                                }
                                
                                int[] ZoneCounts = new int[3] { 0, 0, 0 };
                                ZoneCounts[(int)Nodes[x, y].Type]++;
                                ZoneCounts[(int)Nodes[x + 1, y].Type]++;
                                
                                Tileset.RoomData NewData = Tileset_2x1.MatchingRoom(GetMajority(ZoneCounts), DoorPositions);
                                GameObject Room = Instantiate(NewData.Room, RoomPosition, Quaternion.Euler(0f, NewData.Spin, 0f));
                                Room.transform.parent = this.transform;
                                Nodes[x, y].Free = false;
                                Nodes[x + 1, y].Free = false;
                                
                                List<Node> RoomNodes = new List<Node>();
                                RoomNodes.Add(Nodes[x, y]);
                                RoomNodes.Add(Nodes[x + 1, y]);
                                SetLowestPath(Room.GetComponent<Room>(), RoomNodes);
                                
                                foreach(int W in WallIndices)
                                    AllWalls.W[W].Owners.Add(Room.GetComponent<Room>());
                                
                                Room.GetComponent<Room>().Pixels.Add(new Vector2Int(x, y));
                                Room.GetComponent<Room>().Pixels.Add(new Vector2Int(x + 1, y));
                                _station.Rooms.Add(Room.GetComponent<Room>());
                                yield return null;
                                RoomsGenerated++;
                                SpacesUsed += 2;
                                UIManager.UI.M_Loading.UpdateStatus(25f + ((float)SpacesUsed/TotalFreeSpaces * 40f), "Generating Rooms - " + (RoomsGenerated + 1));
                            }
                        }
                    }
                }
            }
        }
        
        for (int y = 0; y < Nodes.GetLength(1); y++)
        {
            for (int x = 0; x < Nodes.GetLength(0); x++)
            {
                if (Nodes[x, y].Free)
                {
                    if(y < Nodes.GetLength(1) - 1)
                    {
                        if(Nodes[x, y + 1].Free)
                        {
                            if (Random.value > 0.5f)
                            {
                                Vector3 RoomPosition = new Vector3((x * TILE_SIZE) - ((MAP_SIZE / 2) * TILE_SIZE), 0f, ((y + y + 1) * (TILE_SIZE/2)) - ((StationMap.height / 2) * TILE_SIZE));
                                List<Vector3> DoorPositions = new List<Vector3>();
                                List<int> WallIndices = new List<int>();              //list of walls the room needs to own
                                foreach (Vector3 Spot in Tileset_2x1.WallPositions)
                                {
                                    Vector3 Temp = Quaternion.Euler(0f, 90f, 0f) * Spot;
                                    Temp = new Vector3(Mathf.Round(Temp.x), Mathf.Round(Temp.y), Mathf.Round(Temp.z));
                                    int WallIndex = AllWalls.Contains(RoomPosition + Temp);
                                    if (WallIndex == -1)
                                    {
                                        WallData NewWallData = new WallData();
                                        NewWallData.Position = RoomPosition + Temp;
                                        AllWalls.W.Add(NewWallData);
                                        WallIndices.Add(AllWalls.W.Count - 1);
                                    }else
                                    {
                                        WallIndices.Add(WallIndex);
                                    }

                                    if(LinkLocations.Contains(RoomPosition + Temp))
                                        DoorPositions.Add(Spot);
                                }
                                
                                int[] ZoneCounts = new int[3] { 0, 0, 0 };
                                ZoneCounts[(int)Nodes[x, y].Type]++;
                                ZoneCounts[(int)Nodes[x, y + 1].Type]++;
                                
                                Tileset.RoomData NewData = Tileset_2x1.MatchingRoom(GetMajority(ZoneCounts), DoorPositions);
                                GameObject Room = Instantiate(NewData.Room, RoomPosition, Quaternion.Euler(0f, NewData.Spin, 0f));
                                Room.transform.eulerAngles = new Vector3(0f, 90f, 0f);
                                Room.transform.parent = this.transform;
                                Nodes[x, y].Free = false;
                                Nodes[x, y + 1].Free = false;
                                
                                List<Node> RoomNodes = new List<Node>();
                                RoomNodes.Add(Nodes[x, y]);
                                RoomNodes.Add(Nodes[x, y + 1]);
                                SetLowestPath(Room.GetComponent<Room>(), RoomNodes);
                                
                                foreach(int W in WallIndices)
                                    AllWalls.W[W].Owners.Add(Room.GetComponent<Room>());

                                Room.GetComponent<Room>().Pixels.Add(new Vector2Int(x, y));
                                Room.GetComponent<Room>().Pixels.Add(new Vector2Int(x, y + 1));
                                _station.Rooms.Add(Room.GetComponent<Room>());
                                yield return null;
                                RoomsGenerated++;
                                SpacesUsed += 2;
                                UIManager.UI.M_Loading.UpdateStatus(25f + ((float)SpacesUsed/TotalFreeSpaces * 40f), "Generating Rooms - " + (RoomsGenerated + 1));
                            }
                        }
                    }
                }
            }
        }

        List<int> RandomOrder = new List<int>() { 0, 1, 2, 3 };
        for (int y = 0; y < Nodes.GetLength(1); y++)
        {
            for (int x = 0; x < Nodes.GetLength(0); x++)
            {
                if (Nodes[x, y].Free || Nodes[x, y] == SpawnRoom || Nodes[x, y] == EscapeRoom || FunctionalRooms.Contains(Nodes[x, y]))
                {
                    Vector3 RoomPosition = new Vector3((x * TILE_SIZE) - ((MAP_SIZE / 2) * TILE_SIZE), 0f, (y * TILE_SIZE) - ((StationMap.height / 2) * TILE_SIZE));
                    List<Vector3> DoorPositions = new List<Vector3>();
                    List<int> WallIndices = new List<int>();              //list of walls the room needs to own
                    foreach (Vector3 Spot in Tileset_1x1.WallPositions)
                    {
                        int WallIndex = AllWalls.Contains(RoomPosition + Spot);
                        if (WallIndex == -1)
                        {
                            WallData NewWallData = new WallData();
                            NewWallData.Position = RoomPosition + Spot;
                            AllWalls.W.Add(NewWallData);
                            WallIndices.Add(AllWalls.W.Count - 1);
                        }else
                        {
                            WallIndices.Add(WallIndex);
                        }
                       
                        if(LinkLocations.Contains(RoomPosition + Spot))
                            DoorPositions.Add(Spot);
                    }

                    GameObject Room;
                    if(Nodes[x, y] == SpawnRoom)
                    {
                        Room = Instantiate(SpawnRoomPrefab, RoomPosition, Quaternion.identity);
                    }else if (Nodes[x, y] == EscapeRoom)
                    {
                        Room = Instantiate(EscapeRoomPrefab, RoomPosition, Quaternion.identity);
                    }else if (FunctionalRooms.Contains(Nodes[x, y]))
                    {
                        int Chosen = RandomOrder[Random.Range(0, RandomOrder.Count)];
                        Room = Instantiate(FunctionalRoomPrefabs[Chosen], RoomPosition, Quaternion.identity);
                        RandomOrder.Remove(Chosen);
                    }
                    else
                    {
                        Tileset.RoomData NewData = Tileset_1x1.MatchingRoom(Nodes[x, y].Type, DoorPositions);
                        Room = Instantiate(NewData.Room, RoomPosition, Quaternion.Euler(0f, NewData.Spin, 0f));
                    }

                    Room.transform.parent = this.transform;  
                    Nodes[x, y].Free = false;

                    Room.GetComponent<Room>().PathNumber = Nodes[x, y].PathNumber;
                    Room.GetComponent<Room>().PathIndex = Nodes[x, y].PathIndex;
                    
                    foreach(int W in WallIndices)
                        AllWalls.W[W].Owners.Add(Room.GetComponent<Room>());
                    
                    Room.GetComponent<Room>().Pixels.Add(new Vector2Int(x, y));
                    _station.Rooms.Add(Room.GetComponent<Room>());
                    yield return null;
                    RoomsGenerated++;
                    SpacesUsed++;
                    UIManager.UI.M_Loading.UpdateStatus(25f + ((float)SpacesUsed/TotalFreeSpaces * 40f), "Generating Rooms - " + (RoomsGenerated + 1));
                }
            }
        }
        
        for (int i = 0; i < AllWalls.W.Count; i++)    //assign new physical walls the walldata they need
        {
            if (LinkLocations.Contains(AllWalls.W[i].Position))
                PlaceWall(DoorwayPrefab, AllWalls.W[i]);
            else
                PlaceWall(WallPrefab, AllWalls.W[i]);
                
            yield return null;
            UIManager.UI.M_Loading.UpdateStatus(65f + ((float)i/AllWalls.W.Count * 35f), "Generating Walls - " + (i + 1));
        }
        
        yield return null;
        UIManager.UI.M_Loading.UpdateStatus(100f, "Done");

        //Stations[StationIndex].LoadModifier();
        
        _station.Entry(new Vector3((SpawnRoom.Index.x * TILE_SIZE) - ((MAP_SIZE / 2) * TILE_SIZE), 0f, (SpawnRoom.Index.y * TILE_SIZE) - ((StationMap.height / 2) * TILE_SIZE)));
        
        Destroy(this);
    }
    
    void PlaceWall(GameObject Prefab, WallData Data)
    {
        bool Turned = false;
        if(Data.Position.z == 0)
            Turned = true;
        if(Data.Position.z % TILE_SIZE == 0)
            Turned = true;

        GameObject Wall = Instantiate(Prefab, Data.Position, Quaternion.Euler(-90f, 0f, Turned ? -90f : 0f));
        Wall.transform.parent = this.transform;
        foreach (Room Owner in Data.Owners)
        {
            Wall.GetComponent<Wall>().Owners.Add(Owner);
            Owner.Walls.Add(Wall.GetComponent<Wall>());
        }
    }
    
    void SetLowestPath(Room _room, List<Node> _nodes)
    {
        int NodeIndex = 0;
        for (int i = 0; i < _nodes.Count; i++)
        {
            if(_nodes[i].PathNumber < _nodes[NodeIndex].PathNumber)
                NodeIndex = i;
        }

        _room.PathNumber = _nodes[NodeIndex].PathNumber;
        _room.PathIndex = _nodes[NodeIndex].PathIndex;
    }
    
    Zone GetMajority(int[] Values)
    {
        if(Values[0] > Values[1] && Values[0] > Values[2])
            return Zone.Crew;
        if(Values[1] > Values[0] && Values[1] > Values[2])
            return Zone.Cargo;
        if(Values[2] > Values[0] && Values[2] > Values[1])
            return Zone.Medical;
            
        if(Values[0] == Values[1])
            return Random.value > 0.5f ? Zone.Crew : Zone.Cargo;
        if(Values[0] == Values[2])
            return Random.value > 0.5f ? Zone.Crew : Zone.Medical;
        if(Values[1] == Values[2])
            return Random.value > 0.5f ? Zone.Cargo : Zone.Medical;

        return Zone.Crew;
    }
    
    int GetPixelAtIndex(int Index) //8
    {
        int ActualRowLength = ZoneMaps[ZoneMapIndex].width;  //8

        int CountBy = ActualRowLength / MAP_SIZE; //8/4 = 2

        int SampleIndex = 0;

        for (int i = 0; i < ActualRowLength * ActualRowLength; i++)
        {
            if(Mathf.FloorToInt((float)i/ActualRowLength) % CountBy == 0)//if counting row
            {
                if (i % CountBy == 0)//if counting pixel
                {
                    SampleIndex++;
                    if(SampleIndex == Index) //if counted up to right number
                        return i; //return which pixel it is
                }
            }
        }

        return 0;
    }
}
