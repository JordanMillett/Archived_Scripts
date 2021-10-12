using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Manager : MonoBehaviour
{
    public GameObject Planet;

    public int Count = 500;
    public GameObject Prefab;

    void Start()
    {
        Application.targetFrameRate = 60;

        for(int i = 0; i < Count; i++)
            SpawnPrefab(i.ToString());
    }

    void SpawnPrefab(string N)
    {
        RaycastHit hit;

        Vector3 RayPos = Random.onUnitSphere * 300f;

        if(Physics.Raycast(RayPos, -Vector3.Normalize(RayPos), out hit, 400f))
        {
            GameObject G = hit.collider.transform.gameObject;

            if(G == Planet)
            {
                GameObject Instance = Instantiate(Prefab, hit.point, Quaternion.identity);
                Instance.name = "AI_" + N;
            }
        }
        
    }
}
