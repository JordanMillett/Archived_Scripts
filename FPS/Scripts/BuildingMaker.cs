using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingMaker : MonoBehaviour
{
    /*
    public int xSize;
    public int ySize;

    public GameObject Floor;
    public GameObject Wall;
    public GameObject Doorway;
    public GameObject Window;

    public GameObject Entities;
    public GameObject Enemy;

    float GridSize = 4f;*/

    public List<GameObject> Rooms;

    public int NumberOfRooms;

    public List<Room> Connectors;

    int MostRecentBool = 0;
    Room MostRecentTransform;

    void Start()
    {
        MakeStart();

        MakeRooms();

        //StartCoroutine(MakeRooms());


        //spawn room
        //connect room

        //MakeFloors();
        //MakeWalls();
    }

    void MakeStart()
    {

        GameObject Room = Instantiate(Rooms[0], this.transform.position, Quaternion.identity);
        Room.transform.SetParent(this.transform);
        Connectors.Add(Room.GetComponent<Room>());
        Room.GetComponent<Room>().Used[0] = true;

    }

    void MakeRooms()
    {

        for(int i = 0; i < NumberOfRooms; i++)
        {
            //yield return null;
            //Debug.Log(FindOpenConnector());        
            int RanIndex = Random.Range(0, Rooms.Count);
            //int RanDoor = Random.Range(0, Rooms[RanIndex].GetComponent<Room>().Connectors.Count);
            int RanDoor = 0;
            //ran connector

            Vector3 ConnectOffset = -Rooms[RanIndex].GetComponent<Room>().Connectors[RanDoor].localPosition;
            //Debug.Log(ConnectOffset);
            //Vector3 ConnectOffset = Vector3.zero;
            //Quaternion.Euler(0f, Rooms[RanIndex].GetComponent<Room>().Rotations[0],0f)
            //GameObject Room = Instantiate(Rooms[RanIndex], this.transform.position + FindOpenConnector() + ConnectOffset, Quaternion.identity);
            GameObject Room = Instantiate(Rooms[RanIndex], this.transform.position, Quaternion.identity);
            Room.name = i.ToString();
            Room.transform.SetParent(this.transform);
            
            //Room.transform.localEulerAngles = Rooms[RanIndex].GetComponent<Room>().Connectors[RanDoor].localEulerAngles;
            //Room.transform.localPosition = FindOpenConnector();
            //Room.transform.RotateAround(ConnectOffset, Vector3.up, Rooms[RanIndex].GetComponent<Room>().Connectors[RanDoor].localEulerAngles.y);

            Transform OpenConnector = FindOpenConnector();

            //if(OpenConnector == null)
            //if(false)
            //{

                //Destroy(Room);
                

            //}else
            //{
                
            

                Room.transform.localPosition = ConnectOffset + OpenConnector.position;
                //Room.transform.RotateAround(ConnectOffset, Vector3.up, -90f);

                //Debug.Log(Rooms[RanIndex].GetComponent<Room>().Connectors[1].eulerAngles.y);
                //Debug.Log(Rooms[RanIndex].GetComponent<Room>().Connectors[1].localEulerAngles.y);

                Room.transform.RotateAround(OpenConnector.position, Vector3.up, OpenConnector.eulerAngles.y);
                
                //if collids, delete an incriment i--

                GameObject E = Room.transform.GetChild(1).gameObject;
                E.SetActive(false);

                //if(Room.GetComponent<Room>().CollidersHit())
                if(false)
                {
                    //i--;
                    MostRecentTransform.Used[MostRecentBool] = false;
                    Destroy(Room);
                    Debug.Log("Destroyed");
                    
                    //i--;

                }else
                {
                    Connectors.Add(Room.GetComponent<Room>());
                    Room.GetComponent<Room>().Used[RanDoor] = true;
                    E.SetActive(true);
                }

                

                //offset spawned room by its open connector
                //Raycast out to make windows
                //make rotations work right
            //}
        }

    }

    Transform FindOpenConnector()
    {

        foreach (Room R in Connectors)
        {
            int Index = HasOpenConnector(R);

            if(Index != 0)
            {
                MostRecentBool = Index;
                MostRecentTransform = R;
                return R.Connectors[Index];
            }
                //return R.Connectors[Index].localPosition + R.transform.localPosition;
        }

        Debug.Log("ohno");
        return null;

    }

    int HasOpenConnector(Room R)
    {

        int Index = 0;

        //for(int i = 0; i < R.Used.Count; i++)
        //{
        bool Found = false;
        int leave = 0;

        do{
            int i = Random.Range(0, R.Used.Count);

            leave++;

            if(R.Used[i] == false)
            {
                Index = i;
                Found = true;
            }

            if(leave > 10)
                Found = true;


        }while(!Found);
        //}

        if(Index != 0)
        {   
            R.Used[Index] = true;
        }

        

        return Index;

    }

    /*

    void MakeFloors()
    {

        for(int x = 0; x < xSize; x++)
        {
            for(int y = 0; y < ySize; y++)
            {   
                Instantiate(Floor, this.transform.position + new Vector3(x * GridSize, 0f, y * GridSize), Quaternion.identity);
            }

        }

    }

    void MakeWalls()
    {
        for(int x = 0; x < xSize; x++)
        {
            for(int y = 0; y < ySize; y++)
            {   
                int a = Random.Range(0,2);
                if(a == 0)
                {
                    float yRot = Random.Range(0,4);
                    Instantiate(Wall, this.transform.position + new Vector3(x * GridSize, 0f, y * GridSize), Quaternion.Euler(-90f, yRot * 90f, 0f));
                    yRot = Random.Range(0,4);
                    Instantiate(Doorway, this.transform.position + new Vector3(x * GridSize, 0f, y * GridSize), Quaternion.Euler(-90f, yRot * 90f, 0f));
                    yRot = Random.Range(0,4);
                    Instantiate(Window, this.transform.position + new Vector3(x * GridSize, 0f, y * GridSize), Quaternion.Euler(-90f, yRot * 90f, 0f));
                }

                int b = Random.Range(0,100);
                if(b == 0)
                {

                    GameObject E = Instantiate(Enemy, this.transform.position + new Vector3(x * GridSize, 0f, y * GridSize), Quaternion.identity);
                    E.transform.SetParent(Entities.transform);
                }
            }

        }

    }
    */

}
