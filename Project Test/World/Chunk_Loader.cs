using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Chunk_Loader : MonoBehaviour {

	//public GameObject Origin;
	//Map_Gen map;
	public Map_Gen map;
	Map_Gen loc;
	public GameObject terrain;
	public GameObject Player;
	MenuLoader Loader;

	public LoadingScreen LodScreen;

	//public GameObject[,] grid = new GameObject[100, 100];

	int size = 100;

	public float seed = 0;

	public int distance = 10;

	public float currentX = 0;
	public float currentZ = 0;

	public float lastX = 0;
	public float lastZ = 0;

	public float distX = 0;
	public float distZ = 0;

	int MapSize = 10;

	void Start () 
	{
		seed = MakeSeed();
		GameObject loaderobj = GameObject.FindWithTag ("Loader");
		Loader = loaderobj.GetComponent<MenuLoader> ();

		MapSize = Loader.MapSize;
		distance = Loader.ViewDistance;

		//Map_Gen map = Origin.GetComponent<Map_Gen>();

		currentX = 0; //player cords
		currentZ = 0;

		//grid[0,0] = Origin;

		//makeGround(0,0); //generate starting zone

		LodScreen.OutOf = (MapSize + MapSize) * (MapSize + MapSize);

		/* 
		for (int z = -MapSize; z <= MapSize; z++)
		{
			for (int x = -MapSize; x <= MapSize; x++) 
			{
				chonk(x,z);
				LodScreen.Current++;
				//StartCoroutine(Example(x,z));
			}
		}
		*/

		StartCoroutine(StartingChunks());


		lastX = 0; //when ground was last generated
		lastZ = 0;



		currentX = Player.transform.position.x / 100f; 
		currentZ = Player.transform.position.z / 100f;

		distX = lastX - currentX; 
		distZ = lastZ - currentZ;

		/*for(int i = 1; i < 25;i++){
			chonk(-1,i);
			chonk(0,i);
			chonk(1,i);
		}*/

		//makeGround(Mathf.RoundToInt(currentX),Mathf.RoundToInt(currentZ));

	}

	private void Update()
	{
		/* 
		currentX = Player.transform.position.x / 100f;  //set player cords in same size of chunk cords
		currentZ = Player.transform.position.z / 100f;

		distX = lastX - currentX;  //find distance in 0 - 1 = 1 chunk
		distZ = lastZ - currentZ;

		StartCoroutine(makeGround(Mathf.RoundToInt(currentX),Mathf.RoundToInt(currentZ))); //feed current location rounded to make area nearby
		*/
	}

	IEnumerator makeGround(int offX, int offZ)   //Generate chunks within radius of player location
	{
		for (int z = -distance + offZ; z <= distance + offZ; z++)
		{
			for (int x = -distance + offX; x <= distance + offX; x++) 
			{
				chonk(x,z);
				yield return new WaitForSeconds(0.1f);
			}
		}
	}

	void chonk(int xLoc, int zLoc)  //feed cords like 0,0 5,2    transform.cord / chunk size
	{
		bool skip = false;

			try  //find if chunk has already been generated, to not re make current existing chunks
			{
				GameObject.Find(xLoc + " " + zLoc).SetActive(true);
				skip = true;
	
			}
			catch(NullReferenceException){}



			//create new chunk

			if(skip == false){
			/*
			grid[xLoc,zLoc] = Instantiate (terrain, new Vector3 (0, 0, 0), Quaternion.identity);

			grid[xLoc,zLoc].transform.SetParent(this.transform);
			grid[xLoc,zLoc].name = xLoc + " " + zLoc;



			grid[xLoc,zLoc].transform.position = new Vector3 (size * xLoc, 0, size * zLoc);

			Map_Gen loc = grid[xLoc,zLoc].GetComponent<Map_Gen> ();

			loc.xLocation = map.xSize * xLoc;
			loc.zLocation = map.zSize * zLoc;

			loc.Player = Player;

			*/


			GameObject tr = Instantiate (terrain, new Vector3 (0, 0, 0), Quaternion.identity);
			tr.transform.SetParent (this.transform);
			tr.name = xLoc + " " + zLoc;
			Map_Gen loc = tr.GetComponent<Map_Gen> ();

			tr.transform.position = new Vector3 (size * xLoc, 0, size * zLoc);

			loc.xLocation = map.xSize * xLoc;
			loc.zLocation = map.zSize * zLoc;

			loc.Player = Player;
			loc.World = this.gameObject;



			}

	}

	public float MakeSeed()
	{
		//seed = seed * seed * 12345;

		float x = UnityEngine.Random.Range(550f,5555f);

		return x;
	}

	IEnumerator StartingChunks()
    {
		LodScreen.OutOf = 1;
		for (int z = -MapSize; z <= MapSize; z++)
		{
			for (int x = -MapSize; x <= MapSize; x++) 
			{
				LodScreen.OutOf++;			
			}
		}

		LodScreen.OutOf--;
		
		int count = 0;

		for (int z = -MapSize; z <= MapSize; z++)
		{
			for (int x = -MapSize; x <= MapSize; x++) 
			{
				chonk(x,z);
				LodScreen.Current++;
				count++;
				if(count > LodScreen.OutOf/25f){
					yield return new WaitForSeconds(0.0001f); //untiy rounds up to one frame
					count = 0;
				}
					
			}
		}

    }

}
