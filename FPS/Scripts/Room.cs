using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour
{
    public List<Transform> Connectors;
    //public List<Vector3> Connectors;
    public List<bool> Used;
    //public List<int> Rotations;
    Transform Colliders;

    int Children = 0;

    void Start()
    {

        Colliders = transform.GetChild(0);
        Destroy(Colliders.gameObject);

    }

    void OnEnable()
    {/*
        Colliders = transform.GetChild(0);
        int totalChildren = 0;

        foreach(Transform child in Colliders)
        {

            BoxCollider BC = child.GetComponent<BoxCollider>();

            Vector3 Center = BC.center + this.transform.position;
            Vector3 Size = BC.size;

            Collider[] hitColliders = Physics.OverlapBox(Center, Size/2f, Quaternion.identity);

            totalChildren += hitColliders.Length;

            

        }

        Debug.Log(this.gameObject.name + " : " + totalChildren);*/

    }


    public bool CollidersHit()
    {
        //return false;
        
        //God fucking damnit.
        //Fuck everything

        //Destroy(transform.GetChild(1).gameObject);

        //transform.GetChild(1).gameObject.SetActive(false);
        Colliders = transform.GetChild(0);
        int totalChildren = 0;

        foreach(Transform child in Colliders)
        {

            BoxCollider BC = child.GetComponent<BoxCollider>();

            Vector3 Center = BC.center + this.transform.position;
            Vector3 Size = BC.size;

            

            Collider[] hitColliders = Physics.OverlapBox(Center, Size/2f, Quaternion.identity);

            Debug.Log(hitColliders.Length);

            totalChildren += hitColliders.Length;

            

        }

        Destroy(Colliders.gameObject);

        //Debug.Log(totalChildren);

        if(totalChildren > Children + 2)
            return true;
        else
            return false;


        //transform.GetChild(1).gameObject.SetActive(true);

        
    }

}
