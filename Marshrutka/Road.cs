using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Road : MonoBehaviour
{
    [System.Serializable]
    public struct Node
    {
        public int Index;
	    public Vector3 Pos;
        public float CorrectionAngle;
        public int ExitChance;
        public bool LeaveFromPink;
        public Road ExitRoad;
        [HideInInspector]
        public Vector3 ForwardDir;
        [HideInInspector]
        public Vector3 UpDir;
        [HideInInspector]
        public Vector3 PerpDir;
    }
    
    public int HIGHTLIGHTINDEX = 0;

    public bool FourLanes = false;
    public float LaneSpacing = 1.8f;
    public float CenterLaneSpacing = 1f;
    public float Speedlimit = 10f;

    public List<Node> Nodes;

    public Node GetFirstNode(Transform Info, bool Opposite)
    {
        Node Nearest = Nodes[0];
        float NearestDist = Vector3.Distance(Info.position, Nearest.Pos);

        for(int i = 0; i < Nodes.Count; i++)
        {
            if(Vector3.Distance(Info.position, Nodes[i].Pos) < NearestDist)
            {
                Nearest = Nodes[i];
                NearestDist = Vector3.Distance(Info.position, Nodes[i].Pos);
            }
        }

        return Nearest;
    }

    public Node GetNextNode(int Index, bool pinkLane)
    {
        if(pinkLane)   //Pink lane always counts up 
        {
            return Nodes[GetForward(Index)];
        }else
        {
            return Nodes[GetBack(Index)];
        }
    }

    public bool isInPinkLane(int Index, Transform Info)
    {
        Vector3 Direction = Vector3.zero;
        if(Index + 1 != Nodes.Count)    //if there is a node ahead
        {
            Direction = Nodes[Index + 1].Pos - Info.transform.position; //get direction to forward node
        }else
        {
            Direction = Info.transform.position - Nodes[Index - 1].Pos; //get inverse direction to backward node
        }
         
        float Angle = Vector3.SignedAngle(Direction, Info.transform.forward, this.transform.up);

        if(Mathf.Abs(Angle) > 90f)   //if direction is not towards the normal path get backwards node
        {
            return false;
        }else
        {
            return true;
        }
    }

    int GetForward(int Index)
    {
        if(Index + 1 != Nodes.Count)
            return Index + 1;  
        else
            return 0;
    }

    int GetBack(int Index)
    {
        if(Index - 1 != -1)
            return Index - 1;
        else
            return Nodes.Count - 1;
    }

    public void Generate()
    {
        //Nodes.Clear();

        if(Nodes.Count == 0)
            GenerateNodes();
    }

    void GenerateNodes()
    {
        Mesh mesh;
        Vector3[] vertices;
        mesh = GetComponent<MeshFilter>().sharedMesh;
        vertices = mesh.vertices;

        for (var i = 0; i < vertices.Length; i += 4)
        {
            //INITIALIZE NODE
            Node NewNode = new Node();
            NewNode.Index = i/4;
            NewNode.Pos = new Vector3(0f, 0f, 0f);
            NewNode.PerpDir = new Vector3(0f, 0f, 0f);
            NewNode.CorrectionAngle = 0f;

            //SET NODE DIRECTIONS
            Vector3 PosOne = (vertices[i + 1] + vertices[i + 2])/2f;    //front edge
            Vector3 PosTwo = (vertices[i] + vertices[i + 3])/2f;        //back edge
            PosOne = Quaternion.Euler(-90, 0, 0) * PosOne;
            PosTwo = Quaternion.Euler(-90, 0, 0) * PosTwo;

            //GET UP DIRECTION
            Vector3 PosThree = (vertices[i] + vertices[i + 1])/2f;      //bottom edge
            Vector3 PosFour = (vertices[i + 2] + vertices[i + 3])/2f;   //top edge
            PosThree = Quaternion.Euler(-90, 0, 0) * PosThree;
            PosFour = Quaternion.Euler(-90, 0, 0) * PosFour;
            
            NewNode.ForwardDir = (PosOne - PosTwo).normalized;
            NewNode.UpDir = (PosThree - PosFour).normalized;
            NewNode.PerpDir = Vector3.Cross(NewNode.ForwardDir, NewNode.UpDir).normalized;

            //SET NODE POSITION
            NewNode.Pos += vertices[i];
            NewNode.Pos += vertices[i + 1];
            NewNode.Pos += vertices[i + 2];
            NewNode.Pos += vertices[i + 3];
            NewNode.Pos = new Vector3(NewNode.Pos.x/4f, NewNode.Pos.y/4f, NewNode.Pos.z/4f);
            NewNode.Pos = Quaternion.Euler(-90, 0, 0) * NewNode.Pos;

            Nodes.Add(NewNode);
        }
    }

    void OnDrawGizmosSelected()
    {
        if(HIGHTLIGHTINDEX < 0)
            HIGHTLIGHTINDEX = 0;
        if(HIGHTLIGHTINDEX > Nodes.Count - 1)
            HIGHTLIGHTINDEX = Nodes.Count - 1;

        int Index = 0;

        foreach(Node N in Nodes)
        {
            Gizmos.color = Color.white;
            if(!FourLanes)
            {
                Gizmos.color = Color.magenta;
                if(HIGHTLIGHTINDEX != Index)
                    Gizmos.color = new Color(Gizmos.color.r, Gizmos.color.g, Gizmos.color.b, 0.25f);
                Gizmos.DrawSphere(N.Pos + ((Quaternion.Euler(0, N.CorrectionAngle, 0) * N.PerpDir) * LaneSpacing), 1);
                Gizmos.color = Color.green;
                if(HIGHTLIGHTINDEX != Index)
                    Gizmos.color = new Color(Gizmos.color.r, Gizmos.color.g, Gizmos.color.b, 0.25f);
                Gizmos.DrawSphere(N.Pos + ((Quaternion.Euler(0, N.CorrectionAngle, 0) * N.PerpDir) * -LaneSpacing), 1);
            }else
            {
                
                Gizmos.color = Color.magenta;
                if(HIGHTLIGHTINDEX != Index)
                    Gizmos.color = new Color(Gizmos.color.r, Gizmos.color.g, Gizmos.color.b, 0.25f);
                Gizmos.DrawSphere(N.Pos + ((Quaternion.Euler(0, N.CorrectionAngle, 0) * N.PerpDir) * LaneSpacing) + ((Quaternion.Euler(0, N.CorrectionAngle, 0) * N.PerpDir) * CenterLaneSpacing), 1);
                Gizmos.DrawSphere(N.Pos + ((Quaternion.Euler(0, N.CorrectionAngle, 0) * N.PerpDir) * LaneSpacing) + ((Quaternion.Euler(0, N.CorrectionAngle, 0) * N.PerpDir) * -CenterLaneSpacing), 1);
                
                Gizmos.color = Color.green;
                if(HIGHTLIGHTINDEX != Index)
                    Gizmos.color = new Color(Gizmos.color.r, Gizmos.color.g, Gizmos.color.b, 0.25f);
                Gizmos.DrawSphere(N.Pos + ((Quaternion.Euler(0, N.CorrectionAngle, 0) * N.PerpDir) * -LaneSpacing) + ((Quaternion.Euler(0, N.CorrectionAngle, 0) * N.PerpDir) * CenterLaneSpacing), 1);
                Gizmos.DrawSphere(N.Pos + ((Quaternion.Euler(0, N.CorrectionAngle, 0) * N.PerpDir) * -LaneSpacing) + ((Quaternion.Euler(0, N.CorrectionAngle, 0) * N.PerpDir) * -CenterLaneSpacing), 1);
                

                //Gizmos.color = Color.yellow;
                //Gizmos.DrawSphere(N.Pos, 0.5f);
            }

            try
            {
                if(N.ExitRoad != null)
                {
                    Gizmos.color = Color.red;
                    Gizmos.DrawSphere(N.Pos, 1);
                }
            }catch
            {
                Gizmos.color = Color.red;
                Gizmos.DrawSphere(N.Pos, 1);
            }

            
            
            //Gizmos.color = Color.red;
            //Gizmos.DrawLine(N.Pos, N.Pos + (N.PerpDir * 3f));
            Index++;
        }
    }
}
