using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuFallingItems : MonoBehaviour
{
    public int AmountPerSpawn;
    public float TimeBetweenSpawns;
    public float SpawnRadius;
    public float Drag;
    public float DespawnTime;
    public GameObject[] Items;
    
    //find rigidbody , set speed, set random location in circle on y on gameobject, set random 360 rotation

    void Start()
    {

        InvokeRepeating("Spawn", TimeBetweenSpawns, TimeBetweenSpawns);

    }

    void Spawn()
    {

        for(int i = 0;i < AmountPerSpawn; i++)
        {
            int Index = Random.Range(0,Items.Length);

            GameObject Item = Instantiate(Items[Index], GetPointOnCircle(), Random.rotation);
            Rigidbody r = Item.GetComponent<Rigidbody>();
            r.drag = Drag;
            r.angularDrag = Drag;
            StartCoroutine(Delete(Item));

        }

    }

    IEnumerator Delete(GameObject G)
    {

        yield return new WaitForSeconds(DespawnTime);
        Destroy(G);

    }

    Vector3 GetPointOnCircle()
    {
        Vector3 OffsetVector = new Vector3(Random.Range(-SpawnRadius,SpawnRadius),0f,Random.Range(-SpawnRadius,SpawnRadius));

        //Vector3 DirVector = new Vector3(Random.Range(-1f,1f),0f,0f);
        //float Distance = Random.Range(0f,SpawnRadius);

        //return this.transform.position + (DirVector * Distance);
        return this.transform.position + OffsetVector;

    }
}
