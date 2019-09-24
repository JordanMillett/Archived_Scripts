using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Station_Maker : MonoBehaviour
{
    Ship S;
    GameObject Docking_Point;

    public string Name;

    int Xsize; //has to be less than ten cause fuck C#
    int Ysize;

    const int Xmax = 10;
    const int Ymax = 10;

    const int Xmin = 4;
    const int Ymin = 4;

    GameObject[,] Floors = new GameObject[Xmax,Ymax];
    GameObject[,,] Walls = new GameObject[Xmax,Ymax,4];
    
    GameObject Tile;
    GameObject Wall;
    GameObject Doorway;
    GameObject WalkWay;

    float Unit;
    Vector2 WalkerStartingPoint;
    
    void Start()
    {
        Init();

        MakeFloors();
        MakeOuterWalls();
        MakeAirlock();
        MakeInnerWalls();
        MakeDoors();
        //MakeDoors();
    }

    void Update()
    {
        
        float Distance = Vector3.Distance(Vector3.zero, this.transform.position);

        if(Distance < 1000f)
        {
            S.MoveDockingTarget(Docking_Point.transform);
            S.canDock = true;

        }else
        {
            S.canDock = false;
        }
        

    }

    void Init()
    {
        Xsize = Random.Range(Xmin,Xmax);
        Ysize = Random.Range(Ymin,Ymax);

        S = GameObject.FindWithTag("Ship").GetComponent<Ship>();
        Docking_Point = this.transform.GetChild(0).gameObject;
        Tile = Resources.Load<GameObject>("Station/Tile");
        Wall = Resources.Load<GameObject>("Station/Wall");
        Doorway = Resources.Load<GameObject>("Station/Doorway");
        WalkWay = Resources.Load<GameObject>("Station/Walkway");
        Unit = Tile.transform.localScale.x;

    }

    void MakeFloors()
    {
        for(int x = 0; x < Xsize;x++)
            for(int y = 0; y < Ysize;y++)
            {
                Vector3 Pos = new Vector3(this.transform.position.x + (x * Unit),0f,this.transform.position.y + (y * Unit));
                Floors[x,y] = Instantiate(Tile, Pos, Quaternion.identity);
                Floors[x,y].name = Tile.name + " " + x + " " + y;
                Floors[x,y].transform.SetParent(this.transform);

            }
    }

    void MakeOuterWalls()
    {
        for(int x = 0; x < Xsize;x++)
            for(int y = 0; y < Ysize;y++)
            {
                if(x == 0)
                    MakeSingleWall(x,y,0);

                if(y == 0)
                    MakeSingleWall(x,y,1);
                   
                if(x == Xsize - 1)
                    MakeSingleWall(x,y,2);
                
                if(y == Ysize - 1)
                    MakeSingleWall(x,y,3);
            } 
    }

    void MakeSingleWall(int xLoc, int yLoc, int Index)
    {
        if(Walls[xLoc,yLoc,Index] == null && !HasOpposite(xLoc,yLoc,Index))
        {
            Vector3 Pos = Vector3.zero;
            float Rot = 0f;

            if(Index == 0){
                Pos = new Vector3(-Unit/2f,2f,0f);
                Rot = 90f;
            }else if(Index == 1){
                Pos = new Vector3(0f,2f,-Unit/2f);
                Rot = 0f;
            }else if(Index == 2){
                Pos = new Vector3(Unit/2f,2f,0f);
                Rot = -90f;
            }else if(Index == 3){
                Pos = new Vector3(0f,2f,Unit/2f);
                Rot = 180f;
            }

            Pos += Floors[xLoc,yLoc].transform.position;
            Walls[xLoc,yLoc,Index] = Instantiate(Wall, Pos, Quaternion.Euler(0f,Rot,0f));
            Walls[xLoc,yLoc,Index].name = Wall.name + " " + Index;
            Walls[xLoc,yLoc,Index].transform.SetParent(Floors[xLoc,yLoc].transform);
        }
    }
    
    bool HasOpposite(int xLoc,int yLoc,int Index)
    {
        Vector2 Direction = Vector2.zero;
        int Opp_Index = 0;

        if(Index == 0)
        {
            Direction = new Vector2(-1f,0f);
            Opp_Index = 2;
        }else
        if(Index == 1)
        {
            Direction = new Vector2(0f,-1f);
            Opp_Index = 3;
        }else
        if(Index == 2)
        {
            Direction = new Vector2(1f,0f);
            Opp_Index = 0;
        }else
        if(Index == 3)
        {
            Direction = new Vector2(0f,1f);
            Opp_Index = 1;
        }

        xLoc += (int)Direction.x;
        yLoc += (int)Direction.y;

        try{
        if(Walls[xLoc,yLoc,Opp_Index] != null)
            return true;
        }catch{}
        return false;


    }

    void MakeInnerWalls()
    {
        int Steps = Xsize * Ysize * 20;

        Vector2 Walker_Location = WalkerStartingPoint;

        for(int i = 0; i < Steps; i++)
        {
            Walker_Location += GetRanDir(Walker_Location);
            MakeWalls(Walker_Location.x,Walker_Location.y);
        }

    }

    void MakeWalls(float F_x, float F_y)
    {
        
        int xPos = (int)Mathf.Round(F_x);
        int yPos = (int)Mathf.Round(F_y);

        for(int i = 0; i < 4;i++)
        {
            int del = Random.Range(0,100);
            if(del == 1)
                MakeSingleWall(xPos,yPos,i);

        }

    }

    Vector2 GetRanDir(Vector2 V)
    {

        int xPos = (int)Mathf.Round(V.x);
        int yPos = (int)Mathf.Round(V.y);

        if(xPos >= Xsize - 1)
            return new Vector2(-1f,0f);
        
        if(xPos <= 0)
            return new Vector2(1f,0f);
        
        if(yPos >= Ysize - 1)
            return new Vector2(0f,-1f);
        
        if(yPos <= 0) 
            return new Vector2(0f,1f);

        int Num = Random.Range(0,4);
            

        switch(Num)
        {
            case 0 : return new Vector2(0f,1f);
            case 1 : return new Vector2(1f,0f);
            case 2 : return new Vector2(0f,-1f);
            case 3 : return new Vector2(-1f,0f);
            default : return new Vector2(0f,0f);

        }
        

    }

    void MakeDoors()
    {
        /* 
        int Door = Random.Range(0,Walls.Count);
        GameObject toDelete = Walls[Door];

		GameObject[] WW = new GameObject[4];

		Vector3 offset = new Vector3(toDelete.transform.position.x,0f,toDelete.transform.position.z);

		for(int x = 0; x < WW.Length; x++)
		{

			WW[x] = Instantiate(WalkWay, offset, toDelete.transform.rotation);
			WW[x].transform.SetParent(this.transform);
			offset += new Vector3(Unit,0f,0f);

		}
			


		Docking_Point.position = new Vector3(WW[WW.Length-1].transform.position.x,0f,WW[WW.Length-1].transform.position.z);


       	//Docking_Point.position = new Vector3(toDelete.transform.position.x,0f,toDelete.transform.position.z);
        
        Docking_Point.localEulerAngles = toDelete.transform.localEulerAngles;

        Docking_Point.localEulerAngles += new Vector3(0f,90f,0f);

        //Docking_Point.localRotation = toDelete.transform.localRotation;

        //float yRot = 0f;

        //if(Docking_Point.localRotation.y == 90f)
           // yRot = -90f;

        //if(Docking_Point.localRotation.y == -90f)
            //yRot = 90f;

        //Docking_Point.localRotation = Quaternion.Euler(Docking_Point.localRotation.x, yRot, Docking_Point.localRotation.z);
        Walls.Remove(toDelete);
        Destroy(toDelete);

        DoneMaking = true;
         */

    }

    void MakeAirlock() //make distance a variable
    {

        int Selector = Random.Range(1,Ysize - 1);
        Transform AirLockPos = Walls[0,Selector,0].transform;
        Destroy(Walls[0,Selector,0]);

        GameObject AirLock = Instantiate(Doorway, AirLockPos.position, Quaternion.Euler(0f,90f,0f));
        AirLock.name = "Airlock";
        AirLock.transform.SetParent(this.transform);

        Docking_Point.transform.position = new Vector3(AirLockPos.position.x,0f,AirLockPos.position.z);
        Docking_Point.transform.rotation = AirLockPos.rotation;
        Docking_Point.transform.Translate(Docking_Point.transform.right * (2f * Unit));
        Docking_Point.transform.localEulerAngles -= new Vector3(0f,90f,0f);

        GameObject WW1 = Instantiate(WalkWay, new Vector3(AirLockPos.position.x,0f,AirLockPos.position.z), Quaternion.Euler(0f,90f,0f));
        WW1.name = "Walkway 0";
        WW1.transform.SetParent(this.transform);
        WW1.transform.Translate(WW1.transform.right * (.5f * Unit));

        GameObject WW2 = Instantiate(WalkWay, WW1.transform.position, Quaternion.Euler(0f,90f,0f));
        WW2.name = "Walkway 1";
        WW2.transform.SetParent(this.transform);
        WW2.transform.Translate(WW2.transform.right * (1f * Unit));

        WalkerStartingPoint = new Vector2(0,Selector);

    }

}
