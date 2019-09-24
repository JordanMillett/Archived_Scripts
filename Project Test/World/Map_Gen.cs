using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
public class Map_Gen : MonoBehaviour
{

	MeshRenderer mRen;
	Mesh mesh;
	MenuLoader Loader;

	[HideInInspector]
	public Vector3[] vertices;
	int[] triangles;

	Color[] colors;
	Vector2[] uvs;
	public Gradient gradient;

	private ObjGenerate gen;
	private Chunk_Loader Chnk_World;
	public GameObject Player;
	public GameObject World;

	float rendDist;

	bool objLoad = true;
	bool far = false;

	public int xSize = 100;
	public int zSize = 100;
	public int xLocation;
	public int zLocation;
	public int octaves = 1;

	public float seed;

	float currentX = 0;
	float currentZ = 0;
	float distX = 0;
	float distZ = 0;

	public float ymult;
	public AnimationCurve ycurv;

	//[Range(0.1f, 0.9f)]
	public float frequency = 1f;
	//[Range(0.1f, 0.9f)]
	public float amplitude = 1f;

	public float minTerrainHeight = 0f;
	public float maxTerrianHeight = 0f;

	public Material WaterMat;
	public Material CloudMat;



	void Start()
	{
		Init();
		CreateShape();
		UpdateMesh();
		CreateCollider();
		MakeWater();
		MakeClouds(100f);


		ObjGenerate gen = this.GetComponent<ObjGenerate> ();
		gen.enabled = true;

	}

	void Update()
	{
		//CreateShape();
		//UpdateMesh();
		//CreateCollider();

		calcDistance();
		

	}

	void CreateShape()
	{

		vertices = new Vector3[(xSize + 1) * (zSize + 1)];  //3 squares has 4 verts

		for(int i = 0, z = 0; z <= zSize; z++){        //Assign verts to position on grid
			for (int x = 0; x <= xSize; x++) {

				float y = GetNoiseSample(x + xLocation,z + zLocation);

				// ymult = 3 
				vertices [i] = new Vector3 (x, ycurv.Evaluate(y) * ymult, z);      //each vert gets it's location, xyz

				//vertices [i] = new Vector3 (x, y * ymult, z);   

				//vertices [i] = new Vector3 (x, y, z);

				i++;
			}
		}

		triangles = new int[xSize * zSize * 6];   //each quad * amount of points in two tris   

		int vert = 0;          //linking verts with triangles
		int tris = 0;
		for (int z = 0; z < zSize; z++)
		{
			for(int x = 0; x < xSize; x++)
			{
			
				triangles[tris + 0] = vert + 0;
				triangles[tris + 1] = vert + xSize + 1;
				triangles[tris + 2] = vert + 1;
				triangles[tris + 3] = vert + 1;
				triangles[tris + 4] = vert + xSize + 1;
				triangles[tris + 5] = vert + xSize + 2;

				vert++;
				tris += 6;
			}
			vert++;  //stops the end of rows from linking
		}
		

		uvs = new Vector2[vertices.Length];
		for(int i = 0, z = 0; z <= zSize; z++)
		{
			for (int x = 0; x <= xSize; x++){

				uvs[i] = new Vector2((float)x / xSize, (float)z / zSize);
				i++;

			}
		}

		colors = new Color[vertices.Length];
		for(int i = 0, z = 0; z <= zSize; z++)
		{
			for (int x = 0; x <= xSize; x++) 
			{
				float height = Mathf.InverseLerp(minTerrainHeight, maxTerrianHeight * ymult, vertices[i].y);
				colors[i] = gradient.Evaluate(height);
				i++;
			}
		}

	}

	void UpdateMesh()
	{
		mesh.Clear ();
		mesh.vertices = vertices;
		mesh.triangles = triangles;
		mesh.uv = uvs;
		mesh.colors = colors;
		mesh.RecalculateNormals();
		mesh.RecalculateBounds();
	}
		
	float GetNoiseSample(int x,int z)
	{
		float freq = frequency;
		float amp = amplitude;
		float p = 0;

		freq *= 0.05f;
		amp *= 2f;


		//freq = 1f;
		//amp = 1f;

		//float Noise = Mathf.PerlinNoise ((seed + x) * freq, (seed + z) * freq) * 2 - 1;
		//p = Noise * amp;

		//p = Mathf.PerlinNoise ((seed + x) * freq, (seed + z) * freq);

		//float Noise = Mathf.PerlinNoise ((seed + x) * freq, (seed + z) * freq) * 2 - 1;
		//p = Noise * amp;

		float Noise = Mathf.PerlinNoise ((seed + x) * freq, (seed + z) * freq);
		p = Noise * amp; 

		freq *= 0.05f;
		amp *= 2f;

		Noise = Mathf.PerlinNoise ((seed + x) * freq, (seed + z) * freq);
		p += Noise * amp; 

		Noise = Mathf.PerlinNoise ((seed + x) * 0.5f, (seed + z) * 0.5f);
		p += (Noise * 0.1f) / ymult; 

		//freq *= 0.05f;
		//amp *= 2f;
		//Noise = Mathf.PerlinNoise ((seed + x) * freq, (seed + z) * freq) * 2 - 1;
		//p += Noise * amp;
	
		/*

		if (p > maxTerrianHeight) {
			maxTerrianHeight = p;
		}

		if (p < minTerrainHeight) {
			minTerrainHeight = p;
		}

		*/

		p = Mathf.InverseLerp(0, 1 * amp, p);


	    //if (p > 0.999) {
		//	Debug.Log (p);
		//}



		return p;
	}

	void CreateCollider()
	{
		MeshCollider myMC = GetComponent<MeshCollider>();
		Mesh newMesh = new Mesh();
		newMesh  = new Mesh();
		newMesh.vertices  = vertices;
		newMesh.triangles = triangles;
		newMesh.RecalculateBounds();
		myMC.sharedMesh = newMesh;
	}

	void MakeWater()
	{/* 
		for(int x = 0;x < 100;x++)
			for(int y = 0;y < 100;y++)
			{
			GameObject water = GameObject.CreatePrimitive(PrimitiveType.Quad);

			water.transform.position = new Vector3(gameObject.transform.position.x + (x),8,gameObject.transform.position.z + (y));
			water.transform.Rotate(90,0,0);
			water.transform.localScale += new Vector3(.9F, .9f, 0f);
			water.transform.SetParent(gameObject.transform);
			water.name = "Water";
			Renderer w = water.GetComponent<Renderer> ();
			w.material = WaterMat;
			MeshCollider WaterCol = water.GetComponent<MeshCollider>();
			Destroy(WaterCol);
			}
		

		*/
		GameObject water = GameObject.CreatePrimitive(PrimitiveType.Quad);

		water.transform.position = new Vector3(gameObject.transform.position.x + 50,8,gameObject.transform.position.z + 50);
		water.transform.Rotate(90,0,0);
		water.transform.localScale += new Vector3(99F, 99f, 0f);
		water.transform.SetParent(gameObject.transform);
		water.name = "Water";
		Renderer w = water.GetComponent<Renderer> ();
		w.material = WaterMat;
		MeshCollider WaterCol = water.GetComponent<MeshCollider>();
		Destroy(WaterCol);
		
	}

	void MakeClouds(float y)
	{
		GameObject Clouds = GameObject.CreatePrimitive(PrimitiveType.Quad);

		Clouds.transform.position = new Vector3(gameObject.transform.position.x + 50,y,gameObject.transform.position.z + 50);
		Clouds.transform.Rotate(-90,0,0);
		Clouds.transform.localScale += new Vector3(99F, 99f, 0f);
		Clouds.transform.SetParent(gameObject.transform);
		Clouds.name = "Clouds";
		Renderer c = Clouds.GetComponent<Renderer> ();
		c.material = CloudMat;
		MeshCollider CloudCol = Clouds.GetComponent<MeshCollider>();
		Destroy(CloudCol);
	}

	void toggleObjects()
	{
		if(far && objLoad)
		{
			for(int i = 0; i < gameObject.transform.childCount; i++)
			{
    			GameObject child = this.transform.GetChild(i).gameObject;
   				if(child != null)
  		    	child.SetActive(false);
			}
		objLoad = false;
		}else if(!far && !objLoad)
		{
			for(int i = 0; i < gameObject.transform.childCount; i++)
			{
    			GameObject child = this.transform.GetChild(i).gameObject;
   				if(child != null)
  		    	child.SetActive(true);
			}
		objLoad = true;
		}
	}

	void calcDistance()
	{
        toggleObjects();
		currentX = Player.transform.position.x;
		currentZ = Player.transform.position.z;
		distX = gameObject.transform.position.x - currentX;
		distZ = gameObject.transform.position.z - currentZ;
		if((Mathf.Abs(distX) >  rendDist) || (Mathf.Abs(distZ) > rendDist)){
			far = true;}else
			far = false;
	}

	void Init()
	{
		loadData ();

		Chunk_Loader Chnk_World = World.GetComponent<Chunk_Loader> ();
		//rendDist = Chnk_World.distance;
		rendDist++;
		rendDist = rendDist * 100f; //making the render distance the same as the chunk loader 1f = 1 chunk distance
		seed = Chnk_World.seed;
		mesh = new Mesh ();
		GetComponent<MeshFilter> ().mesh = mesh;
		transform.localScale += new Vector3(9F, 9f, 9f);
	}

	void loadData()
	{
		GameObject loaderobj = GameObject.FindWithTag ("Loader");
		Loader = loaderobj.GetComponent<MenuLoader> ();
		amplitude = Loader.Amplitude;
		frequency = Loader.Frequency;
		ymult = Loader.YMult;
		rendDist = Loader.ViewDistance;
	}

}