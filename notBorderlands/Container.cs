using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Container : MonoBehaviour
{
    public List<GameObject> Spawnables;

    public List<Transform> Locations;

    bool Activated = false;

    public void Activate()
    {
        if(!Activated)
        {
            Activated = true;
            Destroy(GetComponent<BoxCollider>());

            for(int loc = 0; loc < Locations.Count; loc++)
            {
                int i = Random.Range(0, Spawnables.Count); 
                Instantiate(Spawnables[i], Locations[loc].position, Locations[loc].rotation);
            }
        }
    }
}
