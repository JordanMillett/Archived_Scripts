using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    public float Size = 10f;
    public int AmountOfTiles = 5;
    public GameObject Tile;
    public Material Mat;
    public int TileVertices = 5;

    float TileSize = 5;
    public float YOffset = -250;

    public GameObject Tree;
    public int NumberPerTile = 5;

    void Start()
    {
        TileSize = Size/AmountOfTiles;

        for(int x = 0; x < AmountOfTiles; x++)
            for(int y = 0; y < AmountOfTiles; y++)
                MakeTile(x, y);

        this.transform.position += new Vector3(-Size/2f, YOffset, -Size/2f);
    }

    void MakeTile(int xLoc, int yLoc)
    {

        string Name = xLoc + "_" + yLoc;

        GameObject Empty = new GameObject(Name);
		Empty.transform.SetParent(this.transform);

        Empty.transform.position = new Vector3(xLoc * TileSize, 0f, yLoc * TileSize);
        Empty.transform.localScale = new Vector3(TileSize, 1f, TileSize);
        LODGroup LOD = Empty.AddComponent(typeof(LODGroup)) as LODGroup;

        LOD[] meshLODS = new LOD[5];

        meshLODS[0] = MakeLOD(1, Empty, .8f);

        //LOD[] quickLODS = new LOD[1];
        //quickLODS[0] = meshLODS[0];
        //LOD.SetLODs(quickLODS);
        //LOD.RecalculateBounds();


        meshLODS[1] = MakeLOD(2, Empty, 0.5f);

        meshLODS[2] = MakeLOD(4, Empty, 0.25f);

        meshLODS[3] = MakeLOD(8, Empty, 0.1f);

        meshLODS[4] = MakeLOD(16, Empty, 0f);

        LOD.SetLODs(meshLODS);
        
        LOD.fadeMode = LODFadeMode.CrossFade;

        //LOD.RecalculateBounds();

        
        for(int i = 0; i < NumberPerTile; i++)
        {

            GameObject Obj = Instantiate(Tree, Vector3.zero, Quaternion.identity);
            Obj.transform.SetParent(Empty.transform);
            Obj.transform.localPosition = Vector3.zero;
            Obj.transform.position += new Vector3(Random.Range(0f, TileSize), 0f, Random.Range(0f, TileSize));
            //Obj.transform.position += new Vector3(0f, GetNoiseSample(Obj.transform.localPosition.x + xLoc * TileSize, Obj.transform.localPosition.z + yLoc * TileSize), 0f);
            //Obj.transform.position += new Vector3(-Size/2f, YOffset, -Size/2f);
            Obj.transform.position += new Vector3(0f, GetNoiseSample((Obj.transform.localPosition.x/TileVertices * TileSize) + Empty.transform.position.x, (Obj.transform.localPosition.z/TileVertices * TileSize) + Empty.transform.position.z)/2f, 0f);
        }

        
        

    }

    LOD MakeLOD(int Level, GameObject Empty, float distance)
    {

        GameObject LOD0 = Instantiate(Tile, Vector3.zero, Quaternion.identity);
        LOD0.transform.SetParent(Empty.transform);
        LOD0.transform.localPosition = Vector3.zero;

        //if(Level != 1)
            //LOD0.transform.localScale += new Vector3(0.1f, 0.1f, 0.1f);

        Vector3[] vertices;
	    int[] triangles;

        vertices = new Vector3[(TileVertices/Level + 1) * (TileVertices/Level + 1)];  //3 squares has 4 verts

        

        for(int i = 0, z = 0; z <= TileVertices/Level; z++)
        {   
			for (int x = 0; x <= TileVertices/Level; x++) 
            {  


                //float y = GetNoiseSample(x/TileVertices, z/TileVertices);
                //float y = GetNoiseSample((x/TileVertices * TileSize), (z/TileVertices * TileSize));
                float y = GetNoiseSample(((float)x/TileVertices*Level * TileSize) + Empty.transform.position.x, ((float)z/TileVertices*Level * TileSize) + Empty.transform.position.z);
                //float y = GetNoiseSample(x + (xLoc * TileSize)/TileVertices, z + (yLoc * TileSize)/TileVertices);
				//float y = GetNoiseSample((x/TileVertices * TileSize) + (xLoc * TileSize), (z/TileVertices * TileSize) + (yLoc * TileSize));
                //float y = GetNoiseSample(((x + xLoc)/TileVertices * TileSize), ((z + yLoc)/TileVertices * TileSize));
                //float y = Random.Range(0f, 1f);
                vertices [i] = new Vector3 (x, y/Level, z)/TileVertices*Level * TileSize;
				i++;
			}
		}

		triangles = new int[TileVertices/Level * TileVertices/Level * 6];

		int vert = 0;         
		int tris = 0;
		for (int z = 0; z < TileVertices/Level; z++)
		{
			for(int x = 0; x < TileVertices/Level; x++)
			{
			
				triangles[tris + 0] = vert + 0;
				triangles[tris + 1] = vert + TileVertices/Level + 1;
				triangles[tris + 2] = vert + 1;
				triangles[tris + 3] = vert + 1;
				triangles[tris + 4] = vert + TileVertices/Level + 1;
				triangles[tris + 5] = vert + TileVertices/Level + 2;

				vert++;
				tris += 6;
			}
			vert++;
		}

        Vector2[] uvs = new Vector2[vertices.Length];
		for(int i = 0, z = 0; z <= TileVertices/Level; z++)
		{
			for (int x = 0; x <= TileVertices/Level; x++){

				uvs[i] = new Vector2((float)x / TileVertices, (float)z / TileVertices*Level);
				i++;

			}
		}

        Mesh mesh = new Mesh();
        LOD0.GetComponent<MeshFilter>().mesh = mesh;
        LOD0.GetComponent<MeshRenderer>().material = Mat;

        mesh.Clear();
		mesh.vertices = vertices;
		mesh.triangles = triangles;
        mesh.uv = uvs;
		mesh.RecalculateNormals();
		mesh.RecalculateBounds();

        if(Level == 4)
        {
            GameObject Col = new GameObject("Collider");
		    Col.transform.SetParent(Empty.transform);
            Col.transform.localPosition = Vector3.zero;
            Col.transform.localScale = LOD0.transform.localScale;
            MeshCollider MC = Col.AddComponent(typeof(MeshCollider)) as MeshCollider;
            //mesh.RecalculateBounds();
            MC.sharedMesh = mesh;

        }

        return new LOD(distance, new MeshRenderer[]{LOD0.GetComponent<MeshRenderer>()});

    }

    float GetNoiseSample(float x, float z)
	{

        float frequency = 0.05f;
        float amplitude = 2f;

        float noise = (Mathf.PerlinNoise(x * frequency * .05f, z * frequency * .05f) - 0.5f) * amplitude * 20f;

        noise += (Mathf.PerlinNoise(x * frequency, z * frequency) - 0.5f) * amplitude;

        noise += (Mathf.PerlinNoise(x * frequency * 10f, z * frequency * 10f) - 0.5f) * amplitude * .15f;

        noise += (Mathf.PerlinNoise(x * frequency * 30f, z * frequency * 30f) - 0.5f) * amplitude * .15f;

		return noise;
	}
}
