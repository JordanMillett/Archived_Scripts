using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjGenerate : MonoBehaviour 
{

	MenuLoader Loader;
	Map_Gen map;

	public GameObject[] tree;
	public int trees = 25;

	public GameObject grass;
	int grasses = 0;

	public GameObject small_rock;
	public int small_rocks = 25;

	public GameObject flint;
	public int flints = 25;

	public GameObject log;
	public int logs = 10;

	public GameObject bushes;
	public int busheses = 10;

	public GameObject flower;
	int flowers = 10;

    public GameObject BigRock;
    int bigRocks = 10;
    public GameObject BigFlint;
    int bigFlints = 10;
    public GameObject BigOre;
    int bigOres = 10;

	Vector3 loc = new Vector3(0,0,0);

	void Start () 
	{
		LoadInts ();

		map = gameObject.GetComponent<Map_Gen>();

        createObj(BigRock, bigRocks, 1, true);
        createObj(BigFlint, bigFlints, 1, true);
        createObj(BigOre, bigOres, 1, true);

		createObj(tree[0], (trees + 1) / 2, 0);
		createObj(tree[1], (trees + 1) / 2, 0);

		createObj(log, logs, 2);
		createObj(small_rock, small_rocks, 1);
		createObj(grass, grasses, 2);
		createObj(flint, flints, 1);
		createObj(bushes, busheses, 2);

		createObj(flower, flowers, 2);

	}

	void LoadInts ()
	{
		GameObject loaderobj = GameObject.FindWithTag ("Loader");
		Loader = loaderobj.GetComponent<MenuLoader> ();
		trees = Loader.Trees;
		small_rocks = Loader.Rocks;
		flints = Loader.Flints;
		logs = Loader.Logs;
		busheses = Loader.Bushes;


	}

	void createObj(GameObject obj, int amount , int rotation, bool outOf = false) //Pass Prefab, Amount, and Rotation Type
	{
		GameObject empt;
		empt = new GameObject("_" + obj.name);
		empt.transform.SetParent(map.transform);
		
        if(outOf)
        {
            int ranNum = Random.Range(1,101);

            if(ranNum < amount)
                amount = 1;
            else
                amount = 0;
            

            
        }

		for(int x = 0; x < amount; x++){

			GameObject tr;

			int i = Random.Range(1,map.vertices.Length);
			float xOff = Random.Range(0f,10f);
			float zOff = Random.Range(0f,10f);

			loc = transform.TransformPoint(map.vertices[i]);


			
			if(loc.y > 8.5)
			{
			switch (rotation){
			case 2:															//2 = Random Y Rotation
				tr = Instantiate(obj, loc, Quaternion.identity);
				tr.transform.eulerAngles += new Vector3 (0f,Random.Range(0, 360),0f);
				break;
			case 1:															//1 = Completely Random Rotation
				tr = Instantiate(obj, loc, Random.rotation);
				break;
			default:														//0 = Default Prefab Rotation
				tr = Instantiate(obj, loc, Quaternion.identity);
				break;
			}

			tr.name = obj.name;
			tr.transform.SetParent(empt.transform);
			tr.transform.position += new Vector3(xOff,5,zOff);

			RaycastHit hit;
			if(Physics.Raycast(tr.transform.position,new Vector3(0,-90,0),out hit,8))
			{
				if(hit.transform.gameObject.name == "Model")
					Destroy(tr);

				tr.transform.position = hit.point;
			}else
				Destroy(tr);

			if(tr.transform.position.y < 8.5f)
				Destroy(tr);
			}
		}
	}
}
