using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomAsteroid : MonoBehaviour
{
    MeshFilter meshFil;
    MeshRenderer meshRen;
    Mesh mesh;
    Vector3[] vertices;
    int[] triangles;
    Vector3[] normals;

    public Material Mat;

    [Range(1f,25f)]
    public float maxSize;
    public float NoiseStrength;
    public float ranDirStrength;

    void Start()
    {
        
        Init();
        makeVerts();
        makeTris();
        applyMesh();
        finalizeObject();
      
    }

    void Init()
    {

        mesh = new Mesh ();
        meshFil = this.gameObject.AddComponent(typeof(MeshFilter)) as MeshFilter;
        meshRen = this.gameObject.AddComponent(typeof(MeshRenderer)) as MeshRenderer;
        meshFil.mesh = mesh;
        normals = new Vector3[12 * 5];

    }

    void makeVerts()
    {

        vertices = new Vector3[12 * 5]; //12 for number of verts, 3 for each vert existing 3 times

        float t = (1f + Mathf.Sqrt(5f)) / 2f;

        Vert(0,-1f,t,0f);
        Vert(5,1f,t,0f);
        Vert(10,-1f,-t,0f);
        Vert(15,1f,-t,0f);

        Vert(20,0f,-1f,t);
        Vert(25,0f,1f,t);
        Vert(30,0f,-1f,-t);
        Vert(35,0f,1f,-t);

        Vert(40,t,0f,-1f);
        Vert(45,t,0f,1f);
        Vert(50,-t,0f,-1f);
        Vert(55,-t,0f,1f);

        /*
        vertices[0] = new Vector3(-1f,t,0f);
        vertices[1] = new Vector3(1f,t,0f);
        vertices[2] = new Vector3(-1f,-t,0f);
        vertices[3] = new Vector3(1f,-t,0f);

        vertices[4] = new Vector3(0f,-1f,t);
        vertices[5] = new Vector3(0f,1f,t);
        vertices[6] = new Vector3(0f,-1f,-t);
        vertices[7] = new Vector3(0f,1f,-t);

        vertices[8] = new Vector3(t,0f,-1f);
        vertices[9] = new Vector3(t,0f,1f);
        vertices[10] = new Vector3(-t,0f,-1f);
        vertices[11] = new Vector3(-t,0f,1f);
        */

    }

    void Vert(int Index,float a,float b,float c)
    {
        float offset = Random.Range(1f,1f + NoiseStrength);

        for(int i = 0; i < 4 + 1;i++)
        {
            vertices[Index + i] = new Vector3(a * offset,b * offset,c * offset);
        }
        

    }

    void makeTris()
    {
        triangles = new int[20 * 3]; //20 for number of tris, 3 for each point on tris

        Tri(0,  0 + 0,  55 + 0, 25 + 0);
        Tri(3,  0 + 1,  25 + 1, 5 + 0);
        Tri(6,  0 + 2,  5 + 1,  35 + 0);
        Tri(9,  0 + 3,  35 + 1, 50 + 0);
        Tri(12, 0 + 4,  50 + 1, 55 + 1);

        Tri(15, 5 + 2,  25 + 2, 45 + 0);
        Tri(18, 25 + 3, 55 + 2, 20 + 0);
        Tri(21, 55 + 3, 50 + 2, 10 + 0);
        Tri(24, 50 + 3, 35 + 2, 30 + 0);
        Tri(27, 35 + 3, 5 + 3,  40 + 0);
        
        Tri(30, 15 + 0, 45 + 1, 20 + 1);
        Tri(33, 15 + 1, 20 + 2, 10 + 1);
        Tri(36, 15 + 2, 10 + 2, 30 + 1);
        Tri(39, 15 + 3, 30 + 2, 40 + 1);
        Tri(42, 15 + 4, 40 + 2, 45 + 2);

        Tri(45, 20 + 3, 45 + 3, 25 + 4);
        Tri(48, 10 + 3, 20 + 4, 55 + 4);
        Tri(51, 30 + 3, 10 + 4, 50 + 4);
        Tri(54, 40 + 3, 30 + 4, 35 + 4);
        Tri(57, 45 + 4, 40 + 4, 5 + 4);

        /* 
        Tri(0,0,11,5);
        Tri(3,0,5,1);
        Tri(6,0,1,7);
        Tri(9,0,7,10);
        Tri(12,0,10,11);

        Tri(15,1,5,9);
        Tri(18,5,11,4);
        Tri(21,11,10,2);
        Tri(24,10,7,6);
        Tri(27,7,1,8);
        
        Tri(30,3,9,4);
        Tri(33,3,4,2);
        Tri(36,3,2,6);
        Tri(39,3,6,8);
        Tri(42,3,8,9);

        Tri(45,4,9,5);
        Tri(48,2,4,11);
        Tri(51,6,2,10);
        Tri(54,8,6,7);
        Tri(57,9,8,1);
        */

    }

    void Tri(int Index,int a,int b,int c)
    {

        triangles[Index] = a;
        triangles[Index + 1] = b;
        triangles[Index + 2] = c;

    }

    void CalculateNormals() //may be obsolete
    {
        for(int i = 0; i < triangles.Length; i += 3) 
        {
            int tri0 = triangles[i];
            int tri1 = triangles[i + 1];
            int tri2 = triangles[i + 2];
            Vector3 vert0 = vertices[tri0];
            Vector3 vert1 = vertices[tri1];
            Vector3 vert2 = vertices[tri2];
            Vector3 normal = new Vector3()
            {
                x = vert0.y * vert1.z - vert0.y * vert2.z - vert1.y * vert0.z + vert1.y * vert2.z + vert2.y * vert0.z - vert2.y * vert1.z,
                y = -vert0.x * vert1.z + vert0.x * vert2.z + vert1.x * vert0.z - vert1.x * vert2.z - vert2.x * vert0.z + vert2.x * vert1.z,
                z = vert0.x * vert1.y - vert0.x * vert2.y - vert1.x * vert0.y + vert1.x * vert2.y + vert2.x * vert0.y - vert2.x * vert1.y
            };
            normals[tri0] += normal;
            normals[tri1] += normal;
            normals[tri2] += normal;
      }
 
      for (int i = 0; i < normals.Length; i++) {
        Vector3 norm = normals[i];
        float invlength = 1.0f / (float)System.Math.Sqrt(norm.x * norm.x + norm.y * norm.y + norm.z * norm.z);
        normals[i].x = norm.x * invlength;
        normals[i].y = norm.y * invlength;
        normals[i].z = norm.z * invlength;
      }
    }

    void applyMesh()
    {
        MeshCollider Collider = this.gameObject.AddComponent(typeof(MeshCollider)) as MeshCollider;
		Mesh Mesh_Collider = new Mesh();
		Mesh_Collider.vertices  = vertices;
		Mesh_Collider.triangles = triangles;
		Mesh_Collider.RecalculateBounds();
		Collider.sharedMesh = Mesh_Collider;
        Collider.convex = true;

        meshRen.material = Mat;
        mesh.Clear ();
		mesh.vertices = vertices;
		mesh.triangles = triangles;
        mesh.normals = normals;
		mesh.RecalculateNormals();
		mesh.RecalculateBounds();

    }

    void finalizeObject()
    {
        float scaleFactor = Random.Range(1f,maxSize);
        this.transform.localScale = new Vector3(scaleFactor,scaleFactor,scaleFactor);

        Hit H = this.gameObject.AddComponent(typeof(Hit)) as Hit;
        H.DamageAmount = 0.4f;
        H.StopsShip = false;
        H.SpeedThreshold = 5f;

        Rigidbody rb = this.gameObject.AddComponent(typeof(Rigidbody)) as Rigidbody;
        rb.mass = 100f * scaleFactor;
        rb.useGravity = false;
        Vector3 ranDir = new Vector3(Random.Range(-1f,1f),Random.Range(-1f,1f),Random.Range(-1f,1f));
        Vector3 ranRot = new Vector3(Random.Range(-180f,180f),Random.Range(-180f,180f),Random.Range(-180f,180f));
        rb.drag = 0f;
        rb.angularDrag = 0f;
        rb.AddForce(ranDir * ranDirStrength * 1000f);
        rb.AddTorque(ranRot * ranDirStrength);

        Damageable D = this.gameObject.AddComponent(typeof(Damageable)) as Damageable;
        D.Health = 100 * scaleFactor;
        D.DamageColor = Color.red;
        D.R[0] = meshRen;
    }

}
