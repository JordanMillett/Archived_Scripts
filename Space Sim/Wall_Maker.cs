using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wall_Maker : MonoBehaviour
{/* 
    public GameObject Wall;
    public GameObject Light;
    public GameObject Doorway;
    public Station_Maker Station;

    public GameObject[] Miscs;

    int[] W = new int[4];
    float Unit;

    public void MakeWalls()
    {
        Unit = this.transform.localScale.x;

        foreach(int i in W)
            W[i] = 0;

        Collider[] ForwardFloor = Physics.OverlapSphere(this.transform.position + new Vector3(0f,0f,Unit),1f);
        if(ForwardFloor.Length == 0)
        {
            Create(this.transform.position + new Vector3(0f,0f,Unit/2f),0f,Wall);
            W[0] = 1;
        }
      
        Collider[] RightFloor = Physics.OverlapSphere(this.transform.position + new Vector3(Unit,0f,0f),1f);
        if(RightFloor.Length == 0)
        {
            Create(this.transform.position + new Vector3(Unit/2f,0f,0f),90f,Wall);
            W[1] = 1;
        }
        
        Collider[] BackFloor = Physics.OverlapSphere(this.transform.position + new Vector3(0f,0f,-Unit),1f);
        if(BackFloor.Length == 0)
        {
            Create(this.transform.position + new Vector3(0f,0f,-Unit/2f),180f,Wall);
            W[2] = 1;
        }
        
        Collider[] LeftFloor = Physics.OverlapSphere(this.transform.position + new Vector3(-Unit,0f,0f),1f);
        if(LeftFloor.Length == 0)
        {
            Create(this.transform.position + new Vector3(-Unit/2f,0f,0f),270f,Wall);
            W[3] = 1;
        }

        MakeLights();

    }

    void Create(Vector3 Pos,float Yrot, GameObject gam)
    {
        Collider[] OtherWalls = Physics.OverlapSphere(Pos + new Vector3(0f,3f,0f),1f);
        if(OtherWalls.Length == 0)
        {
            int Length = OtherWalls.Length;

            for (int x = 0; x < Length; x++)
            {
                Station.Walls.Remove(OtherWalls[x].transform.gameObject);
                Destroy(OtherWalls[x].transform.gameObject);
            }
        }

        GameObject NewWall = Instantiate(gam, Pos + new Vector3(0f,2f,0f), Quaternion.Euler(0, Yrot, 0));
        NewWall.transform.SetParent(this.transform);
        Station.Walls.Add(NewWall);

    }

    void MakeLights()
    {

        GameObject l = Instantiate(Light, this.transform.position + new Vector3(0f,4f,0f), Quaternion.identity);
        l.name = "Light";
        l.transform.SetParent(this.transform);

    }

    public void MakeDoorway()
    {
        int total = 0;
        int doorwaynum = 0;


        for(int i = 0; i < W.Length;i++)
            if(W[i] == 1)
                total++;
            else
                doorwaynum = i;



        if(total == 3)
        {
                

            switch(doorwaynum)
            {

                case 0 : Create(this.transform.position + new Vector3(0f,0f,Unit/2f),0f,Doorway); break;
                case 1 : Create(this.transform.position + new Vector3(Unit/2f,0f,0f),90f,Doorway); break;
                case 2 : Create(this.transform.position + new Vector3(0f,0f,-Unit/2f),0f,Doorway); break;
                case 3 : Create(this.transform.position + new Vector3(-Unit/2f,0f,0f),90f,Doorway); break;



            }

        }

        if(total <= 2)
        {

            float ranX = Random.Range(-Unit/2f,Unit/2f);
            float ranZ = Random.Range(-Unit/2f,Unit/2f);

            int Index = Random.Range(0,Miscs.Length);

            GameObject O = Instantiate(Miscs[Index], this.transform.position + new Vector3(ranX,0f,ranZ), Quaternion.identity);
			O.transform.localScale = Vector3.one;
			O.transform.SetParent(Station.transform.gameObject.transform);
			O.transform.localScale = Vector3.one;


        }

    }
    */
}
