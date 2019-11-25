using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorBarricade : MonoBehaviour
{

    public int Amount;
    Rigidbody[] Pieces;

    int Total = 0;
    int Broken = 0;

    void Start()
    {
        Pieces = GetComponentsInChildren<Rigidbody>();
        Total = Pieces.Length;
    }

    void Update()
    {

        Broken = 0;

        foreach (Rigidbody R in Pieces)
            if(R == null || R.isKinematic == false)
            {   

                Broken++;

            }            

        if(Broken >= Amount)
            Collapse();
            
    }

    void Collapse()
    {

        foreach (Rigidbody R in Pieces)
            if(R != null)
                R.isKinematic = false;

        Destroy(this);

    }
}
