using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Glass : MonoBehaviour
{
    public Material Broken;

    //int NumPoints = 2;
    bool Shattered = false;

    public void Break()
    {
        if(!Shattered)
        {
            Shattered = true;

            //Split();

            this.transform.GetComponent<MeshRenderer>().sharedMaterial = Broken;
            Rigidbody r = this.gameObject.AddComponent(typeof(Rigidbody)) as Rigidbody;

        }
    }

    void Split()
    {
        /*
        Vector3 Limits = new Vector3(GetComponent<BoxCollider>().bounds.size.x, GetComponent<BoxCollider>().bounds.size.y, 0f);
        Limits /= 2f;

        Vector3 DirPointOne = new Vector3(Random.Range(-Limits.x, Limits.x), Random.Range(-Limits.y, Limits.y), 0f);
        Vector3 DirPointTwo = new Vector3(Random.Range(-Limits.x, Limits.x), Random.Range(-Limits.y, Limits.y), 0f);

        Debug.DrawLine(this.transform.position + DirPointOne, this.transform.position + DirPointTwo, Color.red, 2f);

        Debug.Log(DirPointOne);
        Debug.Log(DirPointOne.x/Limits.x + " : " + DirPointOne.y/Limits.y);

        Vector3 EndPointOne = DirPointOne;
        if(Mathf.Abs(EndPointOne.x) > Mathf.Abs(EndPointOne.y))
            EndPointOne = new Vector3(Limits.x * (EndPointOne.x > 0f ? 1f : -1f), DirPointOne.y, 0f);
        else
            EndPointOne = new Vector3(DirPointOne.x, Limits.y * (EndPointOne.y > 0f ? 1f : -1f), 0f);

        Vector3 EndPointTwo = DirPointTwo;
        if(Mathf.Abs(EndPointTwo.x) > Mathf.Abs(EndPointTwo.y))
            EndPointTwo = new Vector3(Limits.x * (EndPointTwo.x > 0f ? 1f : -1f), DirPointTwo.y, 0f);
        else
            EndPointTwo = new Vector3(DirPointTwo.x, Limits.y * (EndPointTwo.y > 0f ? 1f : -1f), 0f);

        Debug.DrawLine(this.transform.position + EndPointOne, this.transform.position + EndPointTwo, Color.green, 2f);
        */
        /*
        Rigidbody r = this.gameObject.AddComponent(typeof(Rigidbody)) as Rigidbody;
        
        Mesh Pmesh = gameObject.GetComponent<MeshFilter>().mesh;
        Vector3[] Pvertices = Pmesh.vertices;

        for(int i = 0; i < 2; i++)
        {
            GameObject half = new GameObject();
            Mesh mesh = new Mesh();
            MeshFilter meshFil = half.gameObject.AddComponent(typeof(MeshFilter)) as MeshFilter;
            MeshRenderer meshRen = half.gameObject.AddComponent(typeof(MeshRenderer)) as MeshRenderer;
            meshRen.sharedMaterial = Broken;
            meshFil.mesh = mesh;
            Vector3[] normals = new Vector3[6];

            Vector3[] vertices = new Vector3[6];
        }

        this.gameObject.SetActive(false);*/
    }
}
