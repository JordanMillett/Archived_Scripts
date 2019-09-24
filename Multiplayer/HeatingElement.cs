using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeatingElement : MonoBehaviour
{

    public bool On;
    public float Temperature;

    public List<Cookable> Items;

    //List of all items on burner/object

    void FixedUpdate()
    {

        Cook();

    }

    void OnCollisionEnter(Collision collision)
    {

        try{

            Cookable C = collision.transform.gameObject.GetComponent<Cookable>();

            if(C != null)
            {

                Items.Add(C);

            }

        }catch{}
    }

    void OnCollisionExit(Collision collision)
    {

        try{

            Cookable C = collision.transform.gameObject.GetComponent<Cookable>();

            if(C != null)
            {

                Items.Remove(C);


            }

        }catch{}

    }

    void Cook()
    {
        if(On)
        {
            foreach(Cookable x in Items)
            {

                x.CookedAmount += Temperature;

            }
        }
    }
}
