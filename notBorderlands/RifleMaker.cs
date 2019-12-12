using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RifleMaker : MonoBehaviour
{
    public List<GameObject> Barrels;
    public List<GameObject> Grips;
    public List<GameObject> Magazines;
    public List<GameObject> Scopes;
    public List<GameObject> Stocks;

    void Start()
    {
        SpawnRandomPart(Barrels);
        SpawnRandomPart(Grips);
        SpawnRandomPart(Magazines);
        SpawnRandomPart(Scopes);
        SpawnRandomPart(Stocks);
    }

    void SpawnRandomPart(List<GameObject> Parts)
    {

        int Index = Random.Range(0, Parts.Count);
        GameObject Part = Instantiate(Parts[Index], Vector3.zero, Quaternion.identity);
        Part.transform.SetParent(this.transform);
        Part.transform.localPosition = Vector3.zero;
        Part.transform.localEulerAngles = new Vector3(-90f,0f,0f);
        Part.name = Parts[Index].name;
    

    }
}
