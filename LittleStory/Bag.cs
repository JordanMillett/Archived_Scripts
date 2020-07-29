using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bag : MonoBehaviour
{
    public void Pickup()
    {
        if(GameObject.FindWithTag("Hands").GetComponent<Hands>().Full)
        {
            
        }else
        {
            GameObject.FindWithTag("Hands").GetComponent<Hands>().PickupBag();
        }
        

    }
}
