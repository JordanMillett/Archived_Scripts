using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomObjecTSpawnerTest : MonoBehaviour
{
    Vector3 loc = new Vector3(0,0,0);
    public int Scale;

    public GameObject Sun;
    public float delay;
    public int amount;
    public float deletetime;
    //public GameObject Rock;

    void Start()
    {
        //createObj(Sun, 1);

        InvokeRepeating("createObj",delay,delay);
    }

    void Update()
    {
        
    }

    //void createObj(GameObject obj, int amount)
    void createObj()
	{
        GameObject obj = Sun;
        //GameObject obj = Rock;

		//GameObject empt;
		//empt = new GameObject("_" + obj.name);
		//empt.transform.SetParent(this.transform);

		for(int x = 0; x < amount; x++)
        {
			GameObject tr;

			float ranX = Random.Range(-150,150) * Scale;
            //float ranY = Random.Range(-1000,1000);
            float ranZ = Random.Range(100,1000) * Scale;

			loc = new Vector3(ranX,Random.Range(-100f,100f),ranZ);


			
			tr = Instantiate(obj, loc, Quaternion.identity);
			tr.transform.eulerAngles += new Vector3 (0f,Random.Range(0, 360),0f);

			tr.name = obj.name;
			tr.transform.SetParent(this.transform);
            StartCoroutine(Delete(tr));

            //if((ranX < 250 && ranX > -250) || (ranY < 250 && ranY > -250) || (ranZ < 250 && ranZ > -250)) 
                //Destroy(tr);
        }
	}

    IEnumerator Delete(GameObject Gam)
    {
        yield return new WaitForSeconds(deletetime);
        try{
            Destroy(Gam);
        }catch {}

    }
}
