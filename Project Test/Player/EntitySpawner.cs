using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntitySpawner : MonoBehaviour 
{
	public GameObject[] hostileMobs;
	public GameObject[] passiveMobs;
	List<Stats> Info = new List<Stats>();

	public int maxEntities;
	public float spawnRate;
	public float spawnRadius;

	int currentEntities;
	float minDist;

	Transform Player;

	void Start () 
	{
		Init ();

		InvokeRepeating ("Spawner", spawnRate, spawnRate);
		InvokeRepeating ("CheckDead", 5f, 5f);
	}

	void Init()
	{
		Player = this.transform.parent;
		minDist = 150f;
	}

	void Spawner()
	{

		if (currentEntities < maxEntities) 
		{
			bool passive = (Random.value > 0.20f);
			currentEntities++;


			if (passive) 
			{
				int ranPass = Random.Range (0, passiveMobs.Length);

				Spawn (passiveMobs[ranPass]);
				


			} else 
			{
				int ranHost = Random.Range (0, hostileMobs.Length);

				Spawn (hostileMobs [ranHost]);

			}
		}
	}

	void Spawn(GameObject Entity)
	{
		GameObject Mob = Instantiate(Entity, RanPos(), Quaternion.identity);
		Mob.transform.position += new Vector3(0f,50,0f);

			RaycastHit hit;
			if(Physics.Raycast(Mob.transform.position,new Vector3(0,-90,0),out hit,80))
			{
				Mob.transform.position = hit.point;
				Info.Add(Mob.GetComponent<Stats>());
			}else
			{
				Destroy(Mob);
				currentEntities--;
			}
			
			if(Mob.transform.position.y < 8.5f)
			{
				Destroy(Mob);
				currentEntities--;
			}
	}

	Vector3 RanPos() //player offset positon
	{
		float x = 0f;
		float y = 0f;
		float z = 0f;

		y = 3f;

		do {

			x = Random.Range (-spawnRadius, spawnRadius);
			z = Random.Range (-spawnRadius, spawnRadius);

		} while(((x < -minDist) && (x < minDist)) || ((z < -minDist) && (z < minDist)));

		//return new Vector3(0,0,0);
		//WAHT E H HECKIETY


		return new Vector3(x,y,z) + Player.position;
	}

	void CheckDead()
	{

		int Length = Info.Count;

		for(int i = 0; i < Length;i++)
		{
			if(Info[i].Health <= 0)
			{
				Info.Remove(Info[i]);
				Length--;
			}

		}

		currentEntities = Info.Count;

	}
		
}
