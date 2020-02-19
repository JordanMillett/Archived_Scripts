using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Jellyfier : MonoBehaviour
{
    public float bounceSpeed;
    public float fallForce;
    public float stiffness;

    public MeshFilter meshFilter;
    public MeshCollider meshCollider;
    private Mesh mesh;

    JellyVertex[] jellyVertices;
    Vector3[] currentMeshVertices;

    public void Start()
    {

        //meshFilter = GetComponent<MeshFilter>();
        //meshCollider = GetComponent<MeshCollider>();
        mesh = meshFilter.mesh;

        GetVertices();

    }

    private void GetVertices()
    {
        jellyVertices = new JellyVertex[mesh.vertices.Length];
        currentMeshVertices = new Vector3[mesh.vertices.Length];

        for(int i = 0; i < mesh.vertices.Length; i++)
        {

            jellyVertices[i] = new JellyVertex(i, mesh.vertices[i], mesh.vertices[i], Vector3.zero);
            currentMeshVertices[i] = mesh.vertices[i];

        }

    }

    private void Update()
    {

        UpdateVertices();

    }

    private void UpdateVertices()
    {

        for(int i = 0; i < jellyVertices.Length; i++)
        {

            jellyVertices[i].UpdateVelocity(bounceSpeed);
            jellyVertices[i].Settle(stiffness);

            jellyVertices[i].currentVertexPosition += jellyVertices[i].currentVelocity * Time.fixedDeltaTime;
            currentMeshVertices[i] = jellyVertices[i].currentVertexPosition;

        }

        mesh.vertices = currentMeshVertices;
        mesh.RecalculateBounds();
        mesh.RecalculateNormals();
        mesh.RecalculateTangents();

        meshCollider.sharedMesh = mesh;

    }

    public void OnCollisionEnter(Collision other)
    {

        ContactPoint[] collisionPoints = other.contacts;
        for(int i = 0; i < collisionPoints.Length; i++)
        {
            Vector3 inputPoint = collisionPoints[i].point + (collisionPoints[i].point * .1f);
            ApplyPressureToPoint(inputPoint, fallForce);

        }

    }

    public void ApplyPressureToPoint(Vector3 _point, float _pressure)
    {

        for(int i = 0; i < jellyVertices.Length; i++)
        {

            jellyVertices[i].ApplyPressureToVertex(transform, _point, _pressure);

        }

    }
}
