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
        public Vector3 PerpDir;
    }

    public bool InvertDirection = false;

    public List<Node> Nodes;
    

    public Node GetFirstNode(Transform Info)
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

    public Node GetNextNode(int Index)
    {
        if(InvertDirection)
        {
            return Nodes[GetBack(Index)];
        }else
        {
            return Nodes[GetForward(Index)];
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

    void Start()
    {
        if(Nodes.Count == 0)
            GenerateNodes();
    }

    void GenerateNodes()
    {
        Mesh mesh;
        Vector3[] vertices;
        mesh = GetComponent<MeshFilter>().mesh;
        vertices = mesh.vertices;

        for (var i = 0; i < vertices.Length; i += 4)
        {
            Node NewNode = new Node();
            NewNode.Index = i/4;
            NewNode.Pos = new Vector3(0f, 0f, 0f);
            NewNode.PerpDir = new Vector3(0f, 0f, 0f);

            Vector3 PosOne = vertices[i] + vertices[i + 2];
            Vector3 PosTwo = vertices[i + 1] + vertices[i + 3];
            PosOne = new Vector3(PosOne.x/2f, PosOne.y/2f, PosOne.z/2f);
            PosTwo = new Vector3(PosTwo.x/2f, PosTwo.y/2f, PosTwo.z/2f);
            PosOne = Quaternion.Euler(-90, 0, 0) * PosOne;
            PosTwo = Quaternion.Euler(-90, 0, 0) * PosTwo;

            NewNode.PerpDir = (PosOne - PosTwo).normalized;
            NewNode.PerpDir = Vector3.Cross(NewNode.PerpDir, Vector3.up);
            
            if(NewNode.PerpDir == new Vector3(0f, 0f, 0f))
            {
                PosOne = vertices[i] + vertices[i + 3];
                PosTwo = vertices[i + 1] + vertices[i + 2];
                PosOne = new Vector3(PosOne.x/2f, PosOne.y/2f, PosOne.z/2f);
                PosTwo = new Vector3(PosTwo.x/2f, PosTwo.y/2f, PosTwo.z/2f);
                PosOne = Quaternion.Euler(-90, 0, 0) * PosOne;
                PosTwo = Quaternion.Euler(-90, 0, 0) * PosTwo;

                NewNode.PerpDir = (PosOne - PosTwo).normalized;
                NewNode.PerpDir = Vector3.Cross(NewNode.PerpDir, Vector3.up);
            }
            
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
        foreach(Node N in Nodes)
        {
            Gizmos.color = new Color(0f, 1f, 0f, 0.5f);
            Gizmos.DrawSphere(N.Pos, 0.5f);
        }
    }
}
