using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Property : MonoBehaviour
{
    public GameObject InteriorPrefab;
    GameObject InteriorReference;
    public GameObject Door;
    public bool InteriorSpawned = false;
    

    public int OneIn = 100;
    public Transform BuyLocation;
    public GameObject BuySignObject;
    GameObject SignReference;

    public void SpawnSign()
    {
        if(SignReference != null)
            Destroy(SignReference);

        if(!InteriorSpawned)
        {
            int Guess = Random.Range(1, OneIn + 1);
            if(Guess == Random.Range(1, OneIn + 1))
            {
                SignReference = Instantiate(BuySignObject, BuyLocation.position, BuyLocation.rotation);
                SignReference.GetComponent<BuySign>().Connected = this;
            }
        }
    }

    public void SpawnInterior()
    {
        if(!InteriorSpawned)
        {
            InteriorReference = Instantiate(InteriorPrefab, this.transform.position, Quaternion.identity);
            InteriorReference.transform.SetParent(this.transform);
            InteriorReference.transform.localPosition = Vector3.zero;
            InteriorReference.transform.localEulerAngles = new Vector3(-90f, 0f, 0f);

            Door.SetActive(false);

            InteriorSpawned = true;
        }else
        {
            Destroy(InteriorReference);

            Door.SetActive(true);
            InteriorSpawned = false;
        }
    }
    
}
