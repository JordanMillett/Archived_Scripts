using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node
{
    public HUD H;
    public Vector2Int Location;
    public Vector2Int Index;
    public int GCost = -1; //Distance to start, traceback lowest GCost to find the path at the end
    public int HCost = -1; //Distance to end
    public bool Blocked = true;
    public bool Used = false;

    Node Start;
    Node End;

    List<Vector2Int> LookupOffset = new List<Vector2Int>()
    {
        new Vector2Int(-1, 1),  //0
        new Vector2Int(0, 1),
        new Vector2Int(1, 1),   //2
        new Vector2Int(-1, 0),
        new Vector2Int(1, 0),
        new Vector2Int(-1, -1), //5
        new Vector2Int(0, -1),
        new Vector2Int(1, -1)   //7
    };

    public void SetLocation(HUD H, Vector2Int Index)
    {
        this.Blocked = false;
        this.H = H;
        this.Index = Index;
        this.Location = new Vector2Int(((Index.x + 1) * 16) - 1600, (Index.y * 16) - 1600);
    }

    public void Initialize(Node Start, Node End)
    {
        this.Start = Start;
        this.End = End;
        this.GCost = -1;
        this.HCost = -1; 
        this.Used = false;
    }

    public void Mark()
    {
        if(this != Start)
            H.UnmarkedNodes.Remove(this);
        this.Used = true;
        //Debug.Log("G : " + this.GCost);
        //Debug.Log("H : " + this.HCost);

        Debug.Log("H : " + this.HCost + "G : " + this.GCost);

        if(this != End)
        {
            for(int i = 0; i < LookupOffset.Count; i++)
            {
                //Node Neighbor = H.GetNode(Location - LookupOffset[i]);
                //Debug.Log(H);
                //Debug.Log(Index);
                //Debug.Log(LookupOffset[i]);
                Node Neighbor = H.Nodes[Index.x + LookupOffset[i].x][Index.y + LookupOffset[i].y];
                //Debug.Log("Passed");

                if(!Neighbor.Blocked)
                {
                    if(!Neighbor.Used)
                    {
                        Neighbor.Recalculate(this, i);
                        if(!H.UnmarkedNodes.Contains(Neighbor))
                            H.UnmarkedNodes.Add(Neighbor);
                    }
                }
            }

            H.FindNextNode();   
        }else if(this == End)
        {
            //Debug.Log("Traceback Began");
            Traceback();
        }
    }
        
    public void Recalculate(Node N, int D)
    {
        if(N.GCost != -1)
            GCost = N.GCost + (D == 0 || D == 2 || D == 5 || D == 7 ? 14 : 10);
        else
            GCost = D == 0 || D == 2 || D == 5 || D == 7 ? 14 : 10;

        HCost = Mathf.RoundToInt
        (
            Mathf.Sqrt
            (
                Mathf.Pow((((float)End.Index.x - Index.x)),2) + 
                Mathf.Pow((((float)End.Index.y - Index.y)),2)
            )
        );

        //HCost = Mathf.RoundToInt(Mathf.Sqrt(HCost));

        //Debug.Log("Calced H : " + HCost);
    }

    public void Traceback()
    {
        H.NavigationPath.Add(new Vector3(Location.x, -9.95f, Location.y));

        if(GCost == -1)
        {
            H.PathFinished();
            return;
        }

        for(int i = 0; i < LookupOffset.Count; i++)
        {
            Node Neighbor = H.Nodes[Index.x + LookupOffset[i].x][Index.y + LookupOffset[i].y];

            if(!Neighbor.Blocked)
            {
                if(Neighbor.Used && Neighbor.GCost < this.GCost)
                {
                    Neighbor.Traceback();
                }
            }
        }
        //if used and GCost < this.gcost
    }
}
